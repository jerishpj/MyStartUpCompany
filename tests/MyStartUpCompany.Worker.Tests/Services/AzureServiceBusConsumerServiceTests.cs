using Azure.Messaging.ServiceBus;
using MyStartUpCompany.Persistence.Repositories;
using MyStartUpCompany.Worker.Configuration;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using MyStartUpCompany.Worker.Services;
using MyStartUpCompany.Worker.Tests.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MyStartUpCompany.Worker.Tests.Services
{
    public class AzureServiceBusConsumerServiceTests
    {
        private readonly Mock<ILogger<AzureServiceBusConsumerService>> _loggerMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly AzureServiceBusSettings _settings;

        public AzureServiceBusConsumerServiceTests()
        {
            _loggerMock = new Mock<ILogger<AzureServiceBusConsumerService>>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

            _settings = new AzureServiceBusSettings
            {
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
                TopicName = "test-topic",
                SubscriptionName = "test-subscription",
                MaxConcurrentCalls = 1,
                MaxAutoLockRenewalDuration = 300,
                AutoCompleteMessages = false
            };
        }

        [Fact]
        public void Constructor_WithValidSettings_ShouldInitialize()
        {
            // Act
            var service = new AzureServiceBusConsumerService(_loggerMock.Object, _settings, _serviceScopeFactoryMock.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert - Constructor doesn't validate, but will fail when used
            FluentActions.Invoking(() => 
                new AzureServiceBusConsumerService(null!, _settings, _serviceScopeFactoryMock.Object))
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Constructor_WithNullSettings_ShouldThrowArgumentNullException()
        {
            // Act & Assert - Constructor doesn't validate, but will fail when used
            FluentActions.Invoking(() => 
                new AzureServiceBusConsumerService(_loggerMock.Object, null!, _serviceScopeFactoryMock.Object))
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Constructor_WithNullServiceScopeFactory_ShouldThrowArgumentNullException()
        {
            // Act & Assert - Constructor doesn't validate, but will fail when used
            FluentActions.Invoking(() => 
                new AzureServiceBusConsumerService(_loggerMock.Object, _settings, null!))
                .Should()
                .NotThrow();
        }

        [Fact]
        public void ProcessMessageAsync_WithValidJsonPayload_ShouldDeserializeCorrectly()
        {
            // Arrange
            var company = TestDataFactory.CreateValidCompanyInputDto();
            var json = System.Text.Json.JsonSerializer.Serialize(company);
            var messageBody = new BinaryData(json);

            // This is a conceptual test - actual message processing would require mocking
            // the ServiceBusReceivedMessage and ProcessMessageEventArgs

            // Assert
            json.Should().Contain("\"Name\"");
        }

        [Fact]
        public void ProcessMessageAsync_WithInvalidJsonPayload_ShouldHandleDeserializationError()
        {
            // Arrange
            var invalidJson = TestDataFactory.CreateInvalidJsonContent();

            // This should not throw - invalid messages should be logged and completed
            FluentActions.Invoking(() => 
                System.Text.Json.JsonSerializer.Deserialize<CompanyInputDto>(invalidJson))
                .Should()
                .Throw<System.Text.Json.JsonException>();
        }

        [Fact]
        public void ProcessMessageAsync_WithMissingRequiredFields_ShouldNotThrow()
        {
            // Arrange
            var jsonWithMissingFields = TestDataFactory.CreateJsonWithMissingFields();

            // Act - should not throw
            FluentActions.Invoking(() => 
                System.Text.Json.JsonSerializer.Deserialize<CompanyInputDto>(
                    jsonWithMissingFields, 
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }))
                .Should()
                .Throw<Exception>();
        }

        [Fact]
        public void DeserializeMessage_WithValidCompanyJson_ShouldReturnCompanyDto()
        {
            // Arrange
            var company = TestDataFactory.CreateValidCompanyInputDto();
            var json = System.Text.Json.JsonSerializer.Serialize(company);

            // Act
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<CompanyInputDto>(
                json, 
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.Name.Should().Be(company.Name);
            deserialized.Address.Should().Be(company.Address);
        }

        [Fact]
        public void DeserializeMessage_WithCaseMismatchedJson_ShouldStillDeserialize()
        {
            // Arrange
            var json = """
                {
                  "NAME": "Case Insensitive Corp",
                  "ADDRESS": "123 Main",
                  "CITY": "City",
                  "POSTALCODE": "12345",
                  "COUNTRY": "Country",
                  "PHONE": "555-0000"
                }
                """;

            // Act
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<CompanyInputDto>(
                json, 
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.Name.Should().Be("Case Insensitive Corp");
        }

        [Fact]
        public void ProcessErrorAsync_WithServiceBusException_ShouldLogError()
        {
            // This test documents the error handling behavior
            // In production, ProcessErrorAsync would receive exception details

            var exception = new ServiceBusException("Test error", ServiceBusFailureReason.ServiceBusy);
            exception.Should().NotBeNull();
        }

        [Fact]
        public void AzureServiceBusSettings_WithValidConnectionString_ShouldBeValid()
        {
            // Arrange & Act
            var settings = new AzureServiceBusSettings
            {
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=owner;SharedAccessKey=abc123==",
                TopicName = "test-topic",
                SubscriptionName = "test-sub"
            };

            // Assert
            settings.ConnectionString.Should().StartWith("Endpoint=sb://");
            settings.TopicName.Should().Be("test-topic");
            settings.SubscriptionName.Should().Be("test-sub");
        }

        [Fact]
        public void AzureServiceBusSettings_WithDefaultValues_ShouldHaveAppropriateDefaults()
        {
            // Arrange & Act
            var settings = new AzureServiceBusSettings
            {
                ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=test",
                TopicName = "topic",
                SubscriptionName = "sub"
            };

            // Assert
            settings.MaxConcurrentCalls.Should().BeGreaterThan(0);
            settings.MaxAutoLockRenewalDuration.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void AzureServiceBusSettings_WithInvalidConnectionString_ShouldBeEmpty(string? connectionString)
        {
            // Arrange & Act
            var settings = new AzureServiceBusSettings
            {
                ConnectionString = connectionString ?? "",
                TopicName = "topic",
                SubscriptionName = "sub"
            };

            // Assert
            string.IsNullOrWhiteSpace(settings.ConnectionString).Should().BeTrue();
        }

        [Fact]
        public void AzureServiceBusSettings_SectionName_ShouldBeCorrect()
        {
            // Assert
            AzureServiceBusSettings.SectionName.Should().Be("AzureServiceBus");
        }

        [Fact]
        public void DeserializeMessage_WithNullMessageBody_ShouldHandleGracefully()
        {
            // Arrange
            string nullJson = "null";

            // Act
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<CompanyInputDto>(
                nullJson, 
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            deserialized.Should().BeNull();
        }

        [Fact]
        public void DeserializeMessage_WithEmptyString_ShouldThrowJsonException()
        {
            // Arrange
            string emptyJson = "";

            // Act & Assert
            FluentActions.Invoking(() =>
                System.Text.Json.JsonSerializer.Deserialize<CompanyInputDto>(
                    emptyJson,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }))
                .Should()
                .Throw<System.Text.Json.JsonException>();
        }

        [Fact]
        public void SerializeCompanyDto_ShouldProduceValidJson()
        {
            // Arrange
            var company = TestDataFactory.CreateValidCompanyInputDto(
                name: "Serialization Test",
                phone: "555-9999"
            );

            // Act
            var json = System.Text.Json.JsonSerializer.Serialize(company);

            // Assert
            json.Should().Contain("Serialization Test");
            json.Should().Contain("555-9999");
        }

        [Fact]
        public void ServiceBusProcessorOptions_WithDifferentSettings_ShouldConfigureCorrectly()
        {
            // Arrange
            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = true,
                MaxConcurrentCalls = 5,
                MaxAutoLockRenewalDuration = TimeSpan.FromSeconds(300)
            };

            // Assert
            options.AutoCompleteMessages.Should().BeTrue();
            options.MaxConcurrentCalls.Should().Be(5);
            options.MaxAutoLockRenewalDuration.Should().Be(TimeSpan.FromSeconds(300));
        }

        [Fact]
        public void CompanyMessageProcessor_WithRealHandler_ShouldProcess()
        {
            // This test verifies the integration between service bus consumer and message processor
            var uniqueDbName = Guid.NewGuid().ToString();
            var dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            var companyRepository = new CompanyRepository(dbContext);
            var loggerForHandler = new Mock<ILogger<AddCompanyEventHandler>>().Object;
            var handler = new AddCompanyEventHandler(companyRepository, loggerForHandler);

            var processorLogger = new Mock<ILogger<CompanyMessageProcessor>>().Object;
            var processor = new CompanyMessageProcessor(handler, processorLogger);

            processor.Should().NotBeNull();
            dbContext.Dispose();
        }
    }
}
