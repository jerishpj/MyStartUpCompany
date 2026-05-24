using Azure.Messaging.ServiceBus;
using MyStartUpCompany.Worker.Configuration;
using MyStartUpCompany.Worker.Handlers.AddCompany;
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

                // Deserialize the message content to CompanyInputDto
                var companyDto = DeserializeMessage(messageBody, messageId);

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
        /// Deserializes a message body to CompanyInputDto.
        /// </summary>
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
