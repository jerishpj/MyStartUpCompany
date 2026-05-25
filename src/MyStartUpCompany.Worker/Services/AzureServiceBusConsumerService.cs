using Azure.Messaging.ServiceBus;
using MyStartUpCompany.Worker.Configuration;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Mappers;
using System.Text.Json;

namespace MyStartUpCompany.Worker.Services
{
    /// <summary>
    /// Background service that listens to Azure Service Bus Topic and processes company messages.
    /// This service runs continuously and handles messages asynchronously.
    /// </summary>
    public class AzureServiceBusConsumerService : BackgroundService
    {
        private readonly ILogger<AzureServiceBusConsumerService> _logger;
        private readonly AzureServiceBusSettings _settings;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ServiceBusClient? _serviceBusClient;
        private ServiceBusProcessor? _processor;

        public AzureServiceBusConsumerService(
            ILogger<AzureServiceBusConsumerService> logger,
            AzureServiceBusSettings settings,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _settings = settings;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Starts the background service and initializes the Azure Service Bus processor.
        /// </summary>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting Azure Service Bus Consumer Service");

                // Initialize Service Bus Client
                _serviceBusClient = new ServiceBusClient(_settings.ConnectionString);

                // Create a processor for the topic subscription
                _processor = _serviceBusClient.CreateProcessor(
                    _settings.TopicName,
                    _settings.SubscriptionName,
                    new ServiceBusProcessorOptions
                    {
                        AutoCompleteMessages = _settings.AutoCompleteMessages,
                        MaxConcurrentCalls = _settings.MaxConcurrentCalls,
                        MaxAutoLockRenewalDuration = TimeSpan.FromSeconds(_settings.MaxAutoLockRenewalDuration)
                    });

                // Register message and error handlers
                _processor.ProcessMessageAsync += ProcessMessageAsync;
                _processor.ProcessErrorAsync += ProcessErrorAsync;

                // Start processing
                await _processor.StartProcessingAsync(cancellationToken);

                _logger.LogInformation(
                    "Azure Service Bus Consumer Service started successfully. " +
                    "Listening to Topic: {TopicName}, Subscription: {SubscriptionName}",
                    _settings.TopicName, _settings.SubscriptionName);

                await base.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting Azure Service Bus Consumer Service");
                throw;
            }
        }

        /// <summary>
        /// Executes the background service (keeps the service running).
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Keep the service running until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        /// <summary>
        /// Handles incoming messages from the Service Bus Topic.
        /// Extracts the source identifier from message properties and passes it to the processor.
        /// </summary>
        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var messageBody = args.Message.Body.ToString();
                var messageId = args.Message.MessageId;

                _logger.LogInformation(
                    "Received message from Service Bus. MessageId: {MessageId}, Size: {Size} bytes",
                    messageId, args.Message.Body.Length);

                // Extract source identifier from message properties or headers
                var source = ExtractMessageSource(args.Message);

                _logger.LogInformation(
                    "Message {MessageId} identified as from source: {Source}",
                    messageId, source);

                // Deserialize and map the message based on source
                var companyDto = DeserializeAndMapMessage(messageBody, source, messageId);

                if (companyDto == null)
                {
                    // Invalid message - complete it anyway to avoid reprocessing
                    await args.CompleteMessageAsync(args.Message);
                    return;
                }

                // Process the company data using the message processor
                using var scope = _serviceScopeFactory.CreateScope();
                var messageProcessor = scope.ServiceProvider.GetRequiredService<CompanyMessageProcessor>();
                var result = await messageProcessor.ProcessCompanyAsync(
                    companyDto,
                    $"ServiceBusMessage-{messageId}",
                    args.CancellationToken);

                // Complete the message only if processed successfully
                if (result.ProcessingStatus == CompanyMessageProcessor.CompanyProcessingResult.Status.Success ||
                    result.ProcessingStatus == CompanyMessageProcessor.CompanyProcessingResult.Status.Duplicate ||
                    result.ProcessingStatus == CompanyMessageProcessor.CompanyProcessingResult.Status.Invalid)
                {
                    await args.CompleteMessageAsync(args.Message);
                    _logger.LogInformation("Message {MessageId} completed successfully", messageId);
                }
                else
                {
                    // On error, abandon the message so it can be retried
                    await args.AbandonMessageAsync(args.Message);
                    _logger.LogWarning("Message {MessageId} abandoned due to processing error", messageId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from Service Bus");
                // Abandon the message on unexpected error to allow retry
                await args.AbandonMessageAsync(args.Message);
            }
        }

        /// <summary>
        /// Handles errors that occur during message processing.
        /// </summary>
        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(
                args.Exception,
                "Error occurred in Service Bus processor. Exception source: {Source}",
                args.ErrorSource);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Extracts the source identifier from the Service Bus message.
        /// Checks for source in multiple places: custom property, user property, or defaults to "Direct".
        /// </summary>
        /// <remarks>
        /// Source resolution order:
        /// 1. Custom property "Source" on the message
        /// 2. User property "Source" in the message properties
        /// 3. Subject line if it contains source identifier
        /// 4. Defaults to "Direct" if not found
        /// </remarks>
        private string ExtractMessageSource(ServiceBusReceivedMessage message)
        {
            try
            {
                // Check for custom property "Source"
                if (message.ApplicationProperties.TryGetValue("Source", out var sourceObj))
                {
                    var sourceValue = sourceObj?.ToString();
                    if (!string.IsNullOrWhiteSpace(sourceValue))
                    {
                        _logger.LogDebug("Message source found in ApplicationProperties: {Source}", sourceValue);
                        return sourceValue;
                    }
                }

                // Check for source in subject
                if (!string.IsNullOrWhiteSpace(message.Subject))
                {
                    _logger.LogDebug("Message source found in Subject: {Subject}", message.Subject);
                    return message.Subject;
                }

                // Default to "Direct" if no source is found
                _logger.LogDebug("No source identifier found in message properties, defaulting to: Direct");
                return MessageSources.Direct;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error extracting message source, defaulting to: Direct");
                return MessageSources.Direct;
            }
        }

        /// <summary>
        /// Deserializes a message body and applies source-specific mapping.
        /// </summary>
        /// <remarks>
        /// For "Direct" source, deserializes directly to CompanyInputDto.
        /// For other sources, deserializes to object and applies mapper factory transformation.
        /// </remarks>
        private CompanyInputDto? DeserializeAndMapMessage(string messageBody, string source, string messageId)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // For Direct source, deserialize directly to CompanyInputDto
                if (source.Equals(MessageSources.Direct, StringComparison.OrdinalIgnoreCase))
                {
                    var companyDto = JsonSerializer.Deserialize<CompanyInputDto>(messageBody, options);

                    if (companyDto == null)
                    {
                        _logger.LogWarning(
                            "Failed to deserialize Direct message {MessageId}: deserialization returned null",
                            messageId);
                        return null;
                    }

                    _logger.LogDebug("Successfully deserialized Direct message {MessageId}", messageId);
                    return companyDto;
                }

                // For other sources, deserialize to object and use mapper factory
                using var scope = _serviceScopeFactory.CreateScope();
                var mapperFactory = scope.ServiceProvider.GetRequiredService<IMapperFactory>();

                // Deserialize to dynamic object for mapper inspection
                var rawMessage = JsonSerializer.Deserialize<object>(messageBody, options);

                if (rawMessage == null)
                {
                    _logger.LogWarning(
                        "Failed to deserialize message {MessageId} from source {Source}: deserialization returned null",
                        messageId, source);
                    return null;
                }

                _logger.LogDebug(
                    "Attempting to map message {MessageId} from source {Source}",
                    messageId, source);

                var mappedDto = mapperFactory.MapMessage(source, rawMessage);

                if (mappedDto == null)
                {
                    _logger.LogWarning(
                        "Mapper returned null for message {MessageId} from source {Source}",
                        messageId, source);
                    return null;
                }

                _logger.LogDebug(
                    "Successfully mapped message {MessageId} from source {Source} to CompanyInputDto",
                    messageId, source);

                return mappedDto;
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "JSON deserialization failed for message {MessageId} from source {Source}. Body: {MessageBody}",
                    messageId, source, messageBody);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error deserializing/mapping message {MessageId} from source {Source}",
                    messageId, source);
                return null;
            }
        }

        /// <summary>
        /// Deserializes a message body to CompanyInputDto.
        /// DEPRECATED: Use DeserializeAndMapMessage instead. Kept for backward compatibility.
        /// </summary>
        [Obsolete("Use DeserializeAndMapMessage instead")]
        private CompanyInputDto? DeserializeMessage(string messageBody, string messageId)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var companyDto = JsonSerializer.Deserialize<CompanyInputDto>(messageBody, options);

                if (companyDto == null)
                {
                    _logger.LogWarning("Failed to deserialize message {MessageId}: deserialization returned null", messageId);
                    return null;
                }

                return companyDto;
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "JSON deserialization failed for message {MessageId}. Body: {MessageBody}",
                    messageId, messageBody);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deserializing message {MessageId}", messageId);
                return null;
            }
        }

        /// <summary>
        /// Stops the background service and cleans up resources.
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Stopping Azure Service Bus Consumer Service");

                if (_processor != null)
                {
                    await _processor.StopProcessingAsync(cancellationToken);
                    await _processor.DisposeAsync();
                }

                if (_serviceBusClient != null)
                {
                    await _serviceBusClient.DisposeAsync();
                }

                _logger.LogInformation("Azure Service Bus Consumer Service stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping Azure Service Bus Consumer Service");
            }

            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Disposes resources when the service is disposed.
        /// </summary>
        public override void Dispose()
        {
            _processor?.DisposeAsync().GetAwaiter().GetResult();
            _serviceBusClient?.DisposeAsync().GetAwaiter().GetResult();
            base.Dispose();
        }
    }
}
