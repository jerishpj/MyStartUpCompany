# Message Source Mapping Pattern - Implementation Guide

## Overview

This document describes the clean, industry-standard design pattern implemented for handling messages from multiple sources in the MyStartUpCompany.Worker project. The solution uses **Strategy Pattern** combined with **Factory Pattern** and **Dependency Injection** to create a flexible, extensible architecture.

## Architecture

### Design Patterns Used

1. **Strategy Pattern**: Each message source has its own mapping strategy (mapper) that knows how to transform that specific source's message format into the fixed `CompanyInputDto` schema.

2. **Factory Pattern**: A `MapperFactory` centralizes the logic for selecting and instantiating the appropriate mapper based on the message source identifier.

3. **Dependency Injection**: All mappers and the factory are registered in the DI container, making the system loosely coupled and easily testable.

### Key Components

```
┌─────────────────────────────────────────────────────────────┐
│                    Azure Service Bus                        │
│                   (Message Source: X)                       │
└──────────────────────────┬──────────────────────────────────┘
						   │
						   ▼
┌──────────────────────────────────────────────────────────────┐
│        AzureServiceBusConsumerService                       │
│   - Extracts message source identifier                      │
│   - Calls mapper factory for transformation                 │
└──────────────────────────┬──────────────────────────────────┘
						   │
						   ▼
┌──────────────────────────────────────────────────────────────┐
│              IMapperFactory (Factory Pattern)                │
│   - Resolves appropriate mapper based on source            │
│   - Returns source-specific mapper strategy                 │
└──────────────────────────┬──────────────────────────────────┘
						   │
				┌──────────┼──────────┐
				│          │          │
				▼          ▼          ▼
		┌────────────┐ ┌───────────┐ ┌────────────┐
		│ SourceA    │ │ SourceB   │ │  Direct    │
		│ Mapper     │ │ Mapper    │ │  Mapper    │
		│ (Strategy) │ │ (Strategy)│ │ (Strategy) │
		└─────┬──────┘ └────┬──────┘ └────┬───────┘
			  │             │             │
			  └─────────────┼─────────────┘
							│
							▼
		┌────────────────────────────────────┐
		│    CompanyInputDto (Unified)       │
		│   (Fixed Database Schema)          │
		└────────────────────────────────────┘
							│
							▼
		┌────────────────────────────────────┐
		│     AddCompanyEventHandler         │
		│   (Save to Database)               │
		└────────────────────────────────────┘
```

## File Structure

```
src/MyStartUpCompany.Worker/
├── Mappers/
│   ├── IMessageMapper.cs              # Generic mapper interface (Strategy contract)
│   ├── IMapperFactory.cs              # Factory interface
│   ├── MapperFactory.cs               # Factory implementation
│   ├── MessageSources.cs              # Source identifiers and constants
│   └── Sources/
│       ├── SourceAMapper.cs           # Strategy: SourceA → CompanyInputDto
│       └── SourceBMapper.cs           # Strategy: SourceB → CompanyInputDto
├── Extensions/
│   └── MapperExtensions.cs            # DI registration extension
├── Services/
│   ├── AzureServiceBusConsumerService.cs  # Modified: extracts source, calls mapper
│   └── CompanyMessageProcessor.cs         # Modified: added optional mapper support
└── ...
```

## How It Works

### 1. Message Arrives from Service Bus

```csharp
// Message with "Source" property in ApplicationProperties
{
	"ApplicationProperties": {
		"Source": "SourceA"  // or "SourceB", "Direct"
	},
	"Body": { /* company data in SourceA format */ }
}
```

### 2. Source Extraction

`AzureServiceBusConsumerService.ExtractMessageSource()` looks for the source identifier in:
1. `message.ApplicationProperties["Source"]` (recommended)
2. `message.Subject` (alternative)
3. Defaults to `"Direct"` if not found

### 3. Mapper Selection

`MapperFactory.GetMapper(source)` returns the appropriate mapper:
- **SourceA** → `SourceAMapper`
- **SourceB** → `SourceBMapper`
- **Direct** → Deserializes directly to `CompanyInputDto`

### 4. Message Transformation

The selected mapper's `Map()` method transforms the source message:
- Handles null/missing fields gracefully
- Validates required fields
- Maps field names (e.g., `CompanyTitle` → `Name`)
- Transforms field values (e.g., concatenate phone + extension)
- Returns `CompanyInputDto` or `null` on failure

### 5. Database Save

`AddCompanyEventHandler.HandleAsync()` persists the mapped `CompanyInputDto` to the database.

## Adding a New Message Source

### Step 1: Create Source Message Model

```csharp
// src/MyStartUpCompany.Worker/Mappers/Sources/SourceCMapper.cs
namespace MyStartUpCompany.Worker.Mappers.Sources
{
	public class SourceCMessage
	{
		public string? CompanyId { get; set; }
		public string? CompanyName { get; set; }
		// ... other SourceC-specific fields
	}
```

### Step 2: Create Mapper Implementation

```csharp
	public class SourceCMapper : IMessageMapper<object>
	{
		private readonly ILogger<SourceCMapper> _logger;

		public SourceCMapper(ILogger<SourceCMapper> logger)
		{
			_logger = logger;
		}

		public CompanyInputDto? Map(object sourceMessage)
		{
			try
			{
				var sourceC = sourceMessage as SourceCMessage;
				if (sourceC == null)
				{
					_logger.LogWarning("Failed to cast message to SourceCMessage");
					return null;
				}

				// Validation logic
				if (string.IsNullOrWhiteSpace(sourceC.CompanyName))
				{
					_logger.LogWarning("Source C message missing required field: CompanyName");
					return null;
				}

				// Mapping logic
				var companyDto = new CompanyInputDto
				{
					Name = sourceC.CompanyName!.Trim(),
					// ... map other fields
				};

				_logger.LogDebug("Successfully mapped Source C message");
				return companyDto;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error mapping Source C message");
				return null;
			}
		}
	}
}
```

### Step 3: Add Source Constant

```csharp
// src/MyStartUpCompany.Worker/Mappers/MessageSources.cs
public static class MessageSources
{
	public const string SourceA = "SourceA";
	public const string SourceB = "SourceB";
	public const string SourceC = "SourceC";  // NEW
	// ...

	public static IEnumerable<string> GetAllSources()
	{
		return new[]
		{
			SourceA,
			SourceB,
			SourceC,  // NEW
			Direct
		};
	}
}

public enum MessageSource
{
	SourceA,
	SourceB,
	SourceC,  // NEW
	Direct,
	Unknown
}
```

### Step 4: Register in DI Container

```csharp
// src/MyStartUpCompany.Worker/Extensions/MapperExtensions.cs
public static IServiceCollection AddMessageMappers(this IServiceCollection services)
{
	// ... existing registrations ...

	// Register SourceC mapper
	services.AddSingleton<SourceCMapper>();

	// The factory will automatically discover it in InitializeMappers()
	// But we need to add it to the factory's mapper resolution
}
```

### Step 5: Update MapperFactory Initialization

```csharp
// src/MyStartUpCompany.Worker/Mappers/MapperFactory.cs
private void InitializeMappers(IServiceProvider serviceProvider)
{
	// ... existing mappers ...

	// Register SourceC mapper
	try
	{
		var sourceCMapper = serviceProvider.GetService(typeof(SourceCMapper)) as SourceCMapper;
		if (sourceCMapper != null)
		{
			_mappers[MessageSources.SourceC] = sourceCMapper;
			_logger.LogInformation("Registered mapper for source: {Source}", MessageSources.SourceC);
		}
	}
	catch (Exception ex)
	{
		_logger.LogWarning(ex, "Failed to register SourceC mapper");
	}
}
```

### Step 6: Send Messages with Source Identifier

When sending messages to Service Bus, include the source identifier:

```csharp
var properties = new Dictionary<string, object>
{
	{ "Source", MessageSources.SourceC }  // Identifies the source
};

await serviceBusClient.SendMessageAsync(
	new ServiceBusMessage(jsonBody) 
	{ 
		ApplicationProperties = properties 
	}
);
```

## Key Benefits

### ✅ Open/Closed Principle
- **Open for extension**: Add new mappers without modifying existing code
- **Closed for modification**: Core business logic unchanged

### ✅ Single Responsibility
- Each mapper handles one source's transformation
- Clear, focused responsibilities

### ✅ Loose Coupling
- Message processors are unaware of specific source formats
- Easy to swap implementations

### ✅ Easy Testing
- Mock individual mappers independently
- Test each source transformation in isolation

### ✅ Maintainability
- New team members can understand the pattern quickly
- Adding new sources is straightforward
- Clear separation of concerns

## Testing

### Unit Tests for Mappers

```csharp
[Fact]
public void SourceAMapper_Map_WithValidMessage_ReturnsCompanyInputDto()
{
	// Arrange
	var logger = new Mock<ILogger<SourceAMapper>>();
	var mapper = new SourceAMapper(logger.Object);

	var sourceAMessage = new SourceAMessage
	{
		CompanyTitle = "Test Company",
		CompanyInfo = new SourceACompanyInfo
		{
			Description = "Test",
			StreetAddress = "123 Main St",
			CityName = "Boston",
			State = "MA",
			ZipCode = "02101",
			CountryName = "USA"
		},
		Contact = new SourceAContact { PhoneNumber = "555-1234" }
	};

	// Act
	var result = mapper.Map(sourceAMessage);

	// Assert
	Assert.NotNull(result);
	Assert.Equal("Test Company", result.Name);
	Assert.Equal("123 Main St", result.Address);
}
```

### Integration Tests

Messages flow through the entire pipeline:
1. Message received from Service Bus
2. Source extracted and mapper selected
3. Message mapped to CompanyInputDto
4. Data validated
5. Saved to database

All existing tests continue to pass, ensuring backward compatibility.

## Best Practices

### Message Source Identification

Always include the source identifier in one of these ways (in order of preference):

1. **ApplicationProperties** (Recommended)
   ```csharp
   var properties = new ServiceBusMessage(body)
   {
	   ApplicationProperties = new Dictionary<string, object>
	   {
		   { "Source", "SourceA" }
	   }
   };
   ```

2. **Subject Line**
   ```csharp
   var message = new ServiceBusMessage(body)
   {
	   Subject = "SourceB"
   };
   ```

3. **Default to "Direct"**
   - If no source found, assumes direct `CompanyInputDto` format

### Mapper Implementation Guidelines

- **Validate early**: Check all required fields before mapping
- **Log appropriately**: Use `_logger.LogWarning()` for validation failures
- **Handle nulls gracefully**: Return `null` if mapping fails
- **Trim strings**: Remove leading/trailing whitespace
- **Use consistent naming**: Follow existing mapper patterns

### Error Handling

- Mappers return `null` on failure (not exceptions)
- AzureServiceBusConsumerService catches and logs all exceptions
- Invalid messages are marked as complete (won't retry)
- Processing errors cause message abandonment (retry opportunity)

## Monitoring & Logging

All mapping operations are logged:

```
[Debug] Found mapper for source: SourceA
[Debug] Attempting to map message 12345 from source SourceA
[Debug] Successfully mapped message 12345 from source SourceA to CompanyInputDto
[Warning] No mapper found for source: SourceX. Available sources: SourceA, SourceB, Direct
[Error] Error mapping message from source SourceA: System.NullReferenceException
```

Monitor these log entries to:
- Verify correct mappers are being selected
- Detect unsupported sources
- Troubleshoot mapping failures

## Performance Considerations

- **O(1) mapper lookup**: Dictionary-based factory for instant resolution
- **Singleton lifetime**: Mappers and factory are singletons (created once)
- **Minimal memory overhead**: No unnecessary object creation
- **Thread-safe**: Factory and mappers are thread-safe

## Future Enhancements

Potential improvements for future iterations:

1. **Dynamic Mapper Registration**: Load mappers from configuration or plugins
2. **Mapper Caching**: Cache mapper selection results
3. **Async Mapping**: Support async transformations if needed
4. **Validation Pipeline**: Extract validation logic into reusable validators
5. **Telemetry**: Add instrumentation for performance monitoring

---

**Version**: 1.0  
**Last Updated**: 2025  
**Status**: Production Ready
