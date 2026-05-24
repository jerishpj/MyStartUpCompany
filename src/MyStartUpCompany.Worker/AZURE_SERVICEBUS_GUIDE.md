# Azure Service Bus Integration - Implementation Guide

## Overview

The MyStartUpCompany.Worker now supports receiving company data from two sources:
1. **Input Files** - JSON files in the `Input` folder (existing functionality)
2. **Azure Service Bus Topic** - Messages published to an Azure Service Bus Topic (new functionality)

Both sources feed into the same processing pipeline and use the same database persistence layer.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    MyStartUpCompany.Worker                  │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────────────────┐       ┌──────────────────────┐ │
│  │  Worker Service         │       │ AzureServiceBus      │ │
│  │ (File Processor)        │       │ ConsumerService      │ │
│  │                         │       │                      │ │
│  │ - Polls Input folder    │       │ - Listens to Topic   │ │
│  │ - Reads JSON files      │       │ - Receives messages  │ │
│  └────────┬────────────────┘       └──────────┬───────────┘ │
│           │                                   │              │
│           └───────────────┬───────────────────┘              │
│                           │                                  │
│                    ┌──────▼──────────┐                       │
│                    │ CompanyMessage   │                       │
│                    │ Processor        │                       │	
│                    │                  │                       │
│                    │ - Validates data │                       │
│                    │ - Processes      │                       │
│                    │   companies      │                       │
│                    │ - Logs results   │                       │
│                    └──────┬───────────┘                       │
│                           │                                  │
│                    ┌──────▼──────────────┐                   │
│                    │ AddCompanyEvent     │                   │
│                    │ Handler             │                   │
│                    │                     │                   │
│                    │ - Duplicate check   │                   │
│                    │ - Database insert   │                   │
│                    └──────┬──────────────┘                   │
│                           │                                  │
│                    ┌──────▼──────────┐                       │
│                    │   Database      │                       │
│                    │   (AppDbContext)│                       │
│                    └─────────────────┘                       │
└─────────────────────────────────────────────────────────────┘
```

## Configuration

### appsettings.json

```json
{
  "AzureServiceBus": {
	"ConnectionString": "Endpoint=sb://[your-namespace].servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[your-access-key]",
	"TopicName": "company-events",
	"SubscriptionName": "company-processor-subscription",
	"MaxConcurrentCalls": 1,
	"MaxAutoLockRenewalDuration": 300,
	"AutoCompleteMessages": false
  }
}
```

### Configuration Parameters

- **ConnectionString**: Connection string to your Azure Service Bus namespace
  - Format: `Endpoint=sb://[namespace].servicebus.windows.net/;SharedAccessKeyName=[policy-name];SharedAccessKey=[key]`
  - Get from: Azure Portal → Service Bus Namespace → Shared access policies

- **TopicName**: Name of the topic to subscribe to
  - Must be pre-created in Azure Service Bus
  - Example: `company-events`

- **SubscriptionName**: Name of the subscription for this service
  - Should be pre-created or auto-created by the service if using the right permissions
  - Example: `company-processor-subscription`

- **MaxConcurrentCalls**: Maximum number of messages processed concurrently
  - Default: 1 (process one message at a time)
  - Increase for higher throughput if your database can handle it

- **MaxAutoLockRenewalDuration**: Maximum time (in seconds) to automatically renew message lock
  - Default: 300 seconds (5 minutes)
  - Increase if processing takes longer

- **AutoCompleteMessages**: Whether to automatically complete messages after processing
  - Default: false (recommended)
  - Messages are completed only when successfully processed
  - Failed messages are abandoned and retried

## Message Format

### Expected Message Structure

The Azure Service Bus messages should contain JSON in one of these formats:

#### Option 1: Direct Company Data (Recommended for Simple Cases)
```json
{
  "name": "Acme Corporation",
  "description": "A leading company",
  "address": "123 Main St",
  "city": "New York",
  "region": "NY",
  "postalCode": "10001",
  "country": "USA",
  "phone": "555-0100"
}
```

#### Option 2: Message with Metadata (Recommended for Production)
```json
{
  "correlationId": "abc-123-def",
  "createdAt": "2024-05-22T10:30:00Z",
  "source": "LegacySystem",
  "company": {
	"name": "Acme Corporation",
	"description": "A leading company",
	"address": "123 Main St",
	"city": "New York",
	"region": "NY",
	"postalCode": "10001",
	"country": "USA",
	"phone": "555-0100"
  }
}
```

## How to Send Messages to Service Bus

### Using Azure Portal

1. Navigate to your Service Bus Topic
2. Click "Send test message"
3. Enter the message body in JSON format
4. Send

### Using C# SDK

```csharp
var client = new ServiceBusClient("Endpoint=sb://[namespace].servicebus.windows.net/;...");
var sender = client.CreateSender("company-events");

var companyData = new
{
	name = "Acme Corporation",
	address = "123 Main St",
	city = "New York",
	region = "NY",
	postalCode = "10001",
	country = "USA",
	phone = "555-0100"
};

var message = new ServiceBusMessage(JsonSerializer.Serialize(companyData));
await sender.SendMessageAsync(message);
```

### Using Azure CLI

```bash
az servicebus topic subscription rule create \
  --resource-group <rg-name> \
  --namespace-name <namespace> \
  --topic-name company-events \
  --subscription-name company-processor-subscription \
  --name test-rule

# Send message
az servicebus topic send \
  --resource-group <rg-name> \
  --namespace-name <namespace> \
  --topic-name company-events \
  --message '{"name":"Acme","address":"123 Main","city":"New York","postalCode":"10001","country":"USA","phone":"555-0100"}'
```

## Processing Results

### Success Cases

- **Success**: Company data is valid and inserted into database successfully
  - Message is **completed** (acknowledged)
  - Company is now available in the database

- **Duplicate**: Company with same name already exists
  - Message is **completed** (acknowledged)
  - No duplicate entry created (idempotent)

- **Invalid**: Required fields are missing or data fails validation
  - Message is **completed** (acknowledged)
  - Logged with details of validation failure
  - Message is not retried to avoid infinite loops

### Error Cases

- **Processing Error**: Unexpected error during database operations
  - Message is **abandoned** (not acknowledged)
  - Message returns to the queue for retry
  - Check logs for error details

## Logging

All operations are logged with appropriate levels:

- **Information**: Normal operations (service start/stop, successful processing)
- **Warning**: Data issues (validation failures, duplicates)
- **Error**: Exceptions and processing failures

Check logs for:
- `src/MyStartUpCompany.Worker/Worker.cs` - File processing logs
- `src/MyStartUpCompany.Worker/Services/AzureServiceBusConsumerService.cs` - Service Bus processing logs
- `src/MyStartUpCompany.Worker/Services/CompanyMessageProcessor.cs` - Business logic logs

## Deployment Considerations

### Environment-Specific Configuration

Use different configurations per environment:

**appsettings.Development.json** (local/dev)
```json
{
  "AzureServiceBus": {
	"ConnectionString": "[dev-connection-string]",
	"TopicName": "company-events-dev",
	"SubscriptionName": "company-processor-dev"
  }
}
```

**appsettings.Production.json** (prod - DON'T commit secrets!)
```json
{
  "AzureServiceBus": {
	"ConnectionString": "[prod-connection-string-from-secrets]",
	"TopicName": "company-events-prod",
	"SubscriptionName": "company-processor-prod"
  }
}
```

### Using Azure Key Vault

For production, store secrets in Azure Key Vault:

```csharp
// In Program.cs
if (builder.Environment.IsProduction())
{
	var keyVaultEndpoint = new Uri(builder.Configuration["KeyVault:Endpoint"]);
	builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
}
```

## Monitoring & Troubleshooting

### Common Issues

1. **Service Bus Consumer Not Starting**
   - Check: Connection string is configured in appsettings
   - Check: Topic and subscription exist in Azure
   - Check: Service has permissions to read from subscription

2. **Messages Not Being Processed**
   - Check: Subscription name is correct
   - Check: Service is running (check logs for startup messages)
   - Check: Messages are in the correct JSON format
   - Check: Check Application Insights or logs for errors

3. **Processing Takes Too Long**
   - Adjust: `MaxConcurrentCalls` to process multiple messages in parallel
   - Adjust: `MaxAutoLockRenewalDuration` if processing exceeds 5 minutes

### Performance Tuning

- **Higher Throughput**: Increase `MaxConcurrentCalls` gradually (e.g., 5, 10, 20)
- **Higher Reliability**: Keep `MaxConcurrentCalls` at 1 for guaranteed sequential processing
- **Batch Efficiency**: Keep messages small (< 1 MB)

## Testing

### Unit Testing

Services are designed to be testable:

```csharp
[TestClass]
public class CompanyMessageProcessorTests
{
	private Mock<AddCompanyEventHandler> _mockHandler;
	private Mock<ILogger<CompanyMessageProcessor>> _mockLogger;
	private CompanyMessageProcessor _processor;

	[TestInitialize]
	public void Setup()
	{
		_mockHandler = new Mock<AddCompanyEventHandler>();
		_mockLogger = new Mock<ILogger<CompanyMessageProcessor>>();
		_processor = new CompanyMessageProcessor(_mockHandler.Object, _mockLogger.Object);
	}

	[TestMethod]
	public async Task ProcessCompanyAsync_WithValidData_ReturnsSuccess()
	{
		// Arrange
		var company = new CompanyInputDto
		{
			Name = "Test Corp",
			Address = "123 Main",
			City = "Test City",
			PostalCode = "12345",
			Country = "USA",
			Phone = "555-0100"
		};

		_mockHandler.Setup(h => h.HandleAsync(It.IsAny<CompanyInputDto>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		// Act
		var result = await _processor.ProcessCompanyAsync(company, "test-source");

		// Assert
		Assert.AreEqual(CompanyMessageProcessor.CompanyProcessingResult.Status.Success, result.ProcessingStatus);
	}
}
```

### Integration Testing

Test against a real or emulated Service Bus locally using Azure Storage Emulator.

## Next Steps

1. **Create Azure Resources**:
   - Service Bus Namespace
   - Topic: `company-events`
   - Subscription: `company-processor-subscription`

2. **Update Configuration**:
   - Replace placeholders in `appsettings.json`
   - Add connection string from Azure Portal

3. **Deploy Worker**:
   - Deploy to Azure App Service, Container, or On-Premises
   - Monitor logs for successful startup

4. **Send Test Messages**:
   - Use Azure Portal or CLI to send test messages
   - Verify processing in logs and database

5. **Monitor & Scale**:
   - Monitor throughput and latency
   - Adjust `MaxConcurrentCalls` if needed
   - Consider implementing dead-letter handling

## References

- [Azure Service Bus Documentation](https://learn.microsoft.com/en-us/azure/service-bus-messaging/)
- [Azure SDK for .NET - Service Bus](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme)
- [Service Bus Pricing & SLA](https://azure.microsoft.com/en-us/pricing/details/service-bus/)
