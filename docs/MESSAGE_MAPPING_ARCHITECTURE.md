# Message Mapping Architecture - Complete Design Guide

## 📋 Overview

This document explains the **industry-standard, scalable, decoupled message mapping architecture** implemented in MyStartUpCompany.Worker for handling multiple Azure Service Bus message sources with different field names and schemas.

## 🎯 Problem Statement

When messages arrive from multiple sources (Source A, Source B, Source C, etc.) via Azure Service Bus, each source may use different property names:

| Source | Property Name | Maps To | Notes |
|--------|---------------|---------|-------|
| SourceA | CompanyName | Name | Direct mapping |
| SourceB | OrganizationName | Name | Different terminology |
| SourceC | BizName | Name | Business terminology |
| Database | Name | - | Target schema |

**Challenge:** The database schema is fixed with properties like `Name`, `Address`, `City`, etc. We need a way to translate source-specific field names to the fixed database schema without modifying core persistence logic when new sources are added.

## ✨ Solution Architecture

### Design Pattern: **Strategy + Factory + Dependency Injection**

```
┌─────────────────────────────────────────────────────────────────┐
│                    Azure Service Bus Topic                       │
│          (Messages from multiple sources arrive here)            │
└────────────────────────┬────────────────────────────────────────┘
						 │
						 ▼
		┌────────────────────────────────────┐
		│  AzureServiceBusConsumerService    │
		│  - Extracts message source         │
		│  - Deserializes message payload    │
		└──────────────┬─────────────────────┘
					   │
					   ▼
		┌────────────────────────────────────┐
		│      MapperFactory                 │
		│ (Resolves correct mapper by source)│
		└──────────────┬─────────────────────┘
					   │
		 ┌─────────────┼─────────────┐
		 ▼             ▼             ▼
	┌────────┐    ┌────────┐    ┌────────┐
	│SourceA │    │SourceB │    │SourceC │
	│ Mapper  │    │ Mapper  │    │ Mapper  │
	│ (IMsg.. │    │ (IMsg.. │    │ (IMsg.. │
	│ Mapper) │    │ Mapper) │    │ Mapper) │
	└────┬───┘    └────┬───┘    └────┬───┘
		 │             │             │
		 └─────────────┼─────────────┘
					   ▼
		┌────────────────────────────────────┐
		│    CompanyInputDto (normalized)    │
		│  - Name, Address, City, etc.       │
		└──────────────┬─────────────────────┘
					   │
					   ▼
		┌────────────────────────────────────┐
		│   CompanyMessageProcessor          │
		│  - Validates normalized data       │
		│  - Persists to database (no changes)
		└────────────────────────────────────┘
```

## 🏗️ Architecture Components

### 1. **Message Source Constants** (`MessageSources.cs`)

Centralized registry of all supported message sources:

```csharp
public static class MessageSources
{
	public const string SourceA = "SourceA";
	public const string SourceB = "SourceB";
	public const string SourceC = "SourceC";
	public const string Direct = "Direct";  // Already in target schema

	public enum MessageSource
	{
		SourceA,
		SourceB,
		SourceC,
		Direct,
		Unknown
	}

	public static IEnumerable<string> GetAllSources() { ... }
	public static MessageSource FromIdentifier(string identifier) { ... }
	public static string ToIdentifier(MessageSource source) { ... }
}
```

**Benefits:**
- Single source of truth for supported sources
- Easy to discover all sources
- Type-safe source handling with enums
- Bidirectional conversion (string ↔ enum)

### 2. **Message Mapper Interface** (`IMessageMapper<T>`)

Strategy pattern interface for each source:

```csharp
public interface IMessageMapper<T>
{
	/// <summary>
	/// Maps a source-specific message to the normalized CompanyInputDto.
	/// </summary>
	/// <param name="sourceMessage">Raw message from source system</param>
	/// <returns>Normalized DTO or null if mapping fails</returns>
	CompanyInputDto? Map(T sourceMessage);
}
```

**Why this works:**
- Each source implements the same interface
- Factory can resolve mappers polymorphically
- New sources just add a new implementation
- No changes to consumer or processor logic

### 3. **Source-Specific Mappers** (e.g., `SourceAMapper.cs`)

Each source gets its own mapper class:

```csharp
/// <example>
/// SourceA Property          →  Database Property
/// ─────────────────────────────────────────────
/// CompanyName               →  Name
/// CompanyDescription        →  Description
/// StreetAddress             →  Address
/// CityName                  →  City
/// StateCode                 →  Region
/// ZipCode                   →  PostalCode
/// CountryCode               →  Country
/// PhoneNumber               →  Phone
/// </example>
public class SourceAMapper : IMessageMapper<object>
{
	private readonly ILogger<SourceAMapper> _logger;

	public SourceAMapper(ILogger<SourceAMapper> logger)
	{
		_logger = logger;
	}

	public CompanyInputDto? Map(object sourceMessage)
	{
		try
		{
			var sourceA = sourceMessage as SourceAMessage;
			if (sourceA == null)
			{
				_logger.LogWarning("Failed to cast to SourceAMessage");
				return null;
			}

			// Validate required fields
			if (string.IsNullOrWhiteSpace(sourceA.CompanyName))
			{
				_logger.LogWarning("Missing required field: CompanyName");
				return null;
			}

			// Map properties (with trimming for consistency)
			var dto = new CompanyInputDto
			{
				Name = sourceA.CompanyName!.Trim(),
				Description = sourceA.CompanyDescription?.Trim(),
				Address = sourceA.StreetAddress!.Trim(),
				City = sourceA.CityName!.Trim(),
				Region = sourceA.StateCode?.Trim(),
				PostalCode = sourceA.ZipCode!.Trim(),
				Country = sourceA.CountryCode!.Trim(),
				Phone = sourceA.PhoneNumber!.Trim()
			};

			_logger.LogDebug("Mapped SourceA message: {CompanyName}", dto.Name);
			return dto;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error mapping SourceA message");
			return null;
		}
	}
}
```

**Key responsibilities:**
- Define source-specific DTO structure (SourceAMessage)
- Cast and validate source data
- Map each source property to database property
- Log validation failures
- Return null if mapping fails (graceful degradation)

### 4. **Mapper Factory** (`MapperFactory.cs`)

Central resolver for mappers by source:

```csharp
public class MapperFactory : IMapperFactory
{
	private readonly Dictionary<string, IMessageMapper<object>> _mappers;
	private readonly ILogger<MapperFactory> _logger;

	public MapperFactory(IServiceProvider serviceProvider, ILogger<MapperFactory> logger)
	{
		_logger = logger;
		_mappers = new Dictionary<string, IMessageMapper<object>>(StringComparer.OrdinalIgnoreCase);
		InitializeMappers(serviceProvider);
	}

	private void InitializeMappers(IServiceProvider serviceProvider)
	{
		// Dynamically resolve all registered mappers from DI
		// Register SourceA
		var sourceAMapper = serviceProvider.GetService(typeof(SourceAMapper)) as SourceAMapper;
		if (sourceAMapper != null)
		{
			_mappers[MessageSources.SourceA] = sourceAMapper;
			_logger.LogInformation("Registered mapper for: {Source}", MessageSources.SourceA);
		}

		// Register SourceB
		var sourceBMapper = serviceProvider.GetService(typeof(SourceBMapper)) as SourceBMapper;
		if (sourceBMapper != null)
		{
			_mappers[MessageSources.SourceB] = sourceBMapper;
			_logger.LogInformation("Registered mapper for: {Source}", MessageSources.SourceB);
		}

		// Register SourceC
		var sourceCMapper = serviceProvider.GetService(typeof(SourceCMapper)) as SourceCMapper;
		if (sourceCMapper != null)
		{
			_mappers[MessageSources.SourceC] = sourceCMapper;
			_logger.LogInformation("Registered mapper for: {Source}", MessageSources.SourceC);
		}
	}

	public IMessageMapper<object>? GetMapper(string source, string? messageJson = null)
	{
		if (_mappers.TryGetValue(source, out var mapper))
		{
			return mapper;
		}

		_logger.LogWarning("No mapper found for source: {Source}", source);
		return null;
	}

	public CompanyInputDto? MapMessage(string source, object rawMessage)
	{
		var mapper = GetMapper(source);
		return mapper?.Map(rawMessage);
	}
}
```

**Benefits:**
- O(1) mapper lookup by source
- Dynamically discovers mappers from DI
- Provides fallback null for unknown sources
- Centralized logging of registration issues

### 5. **Dependency Injection Registration** (`MapperExtensions.cs`)

Clean extension method to wire everything:

```csharp
public static IServiceCollection AddMessageMappers(this IServiceCollection services)
{
	// Register concrete mappers
	services.AddSingleton<SourceAMapper>();
	services.AddSingleton<SourceBMapper>();
	services.AddSingleton<SourceCMapper>();

	// Register factory
	services.AddSingleton<IMapperFactory, MapperFactory>();

	return services;
}
```

Called from `Program.cs`:

```csharp
builder.Services.AddMessageMappers();
```

### 6. **Service Bus Consumer Integration** (`AzureServiceBusConsumerService.cs`)

Orchestrates source extraction and mapping:

```csharp
private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
{
	var messageId = args.Message.MessageId ?? Guid.NewGuid().ToString();

	try
	{
		// Extract source metadata from Service Bus message
		var source = ExtractMessageSource(args.Message);
		_logger.LogInformation("Processing message from source: {Source}", source);

		// Deserialize and map message
		var companyDto = DeserializeAndMapMessage(
			args.Message.Body.ToString(),
			source,
			messageId
		);

		if (companyDto == null)
		{
			_logger.LogWarning("Failed to map message from source: {Source}", source);
			await args.CompleteMessageAsync(args.CancellationToken);
			return;
		}

		// Process normalized DTO
		await _companyMessageProcessor.ProcessCompanyAsync(companyDto, messageId, CancellationToken.None);
		await args.CompleteMessageAsync(args.CancellationToken);
	}
	catch (Exception ex)
	{
		_logger.LogError(ex, "Error processing message {MessageId}", messageId);
	}
}

private string ExtractMessageSource(ServiceBusReceivedMessage message)
{
	// Try multiple metadata locations
	if (message.ApplicationProperties.TryGetValue("Source", out var source))
		return source?.ToString() ?? MessageSources.Direct;

	if (!string.IsNullOrWhiteSpace(message.Subject))
		return message.Subject;

	return MessageSources.Direct;
}

private CompanyInputDto? DeserializeAndMapMessage(string messageBody, string source, string messageId)
{
	try
	{
		if (source == MessageSources.Direct)
		{
			// Direct format is already CompanyInputDto
			return JsonConvert.DeserializeObject<CompanyInputDto>(messageBody);
		}

		// For other sources, deserialize to object then map
		var rawMessage = JsonConvert.DeserializeObject(messageBody);
		return _mapperFactory?.MapMessage(source, rawMessage);
	}
	catch (Exception ex)
	{
		_logger.LogError(ex, "Deserialization error for message {MessageId}", messageId);
		return null;
	}
}
```

## 📚 How to Add a New Source

When a **new Source D** is added, follow these minimal steps:

### Step 1: Create Source DTO

File: `src/MyStartUpCompany.Worker/Mappers/Sources/SourceDMessage.cs`

```csharp
public class SourceDMessage
{
	public string? DataSourceCompanyName { get; set; }
	public string? DataSourceAddress { get; set; }
	public string? DataSourceCity { get; set; }
	// ... other source-specific properties
}
```

### Step 2: Create Mapper

File: `src/MyStartUpCompany.Worker/Mappers/Sources/SourceDMapper.cs`

```csharp
public class SourceDMapper : IMessageMapper<object>
{
	private readonly ILogger<SourceDMapper> _logger;

	public SourceDMapper(ILogger<SourceDMapper> logger)
	{
		_logger = logger;
	}

	public CompanyInputDto? Map(object sourceMessage)
	{
		try
		{
			var sourceD = sourceMessage as SourceDMessage;
			if (sourceD == null)
			{
				_logger.LogWarning("Failed to cast to SourceDMessage");
				return null;
			}

			// Validate and map
			if (string.IsNullOrWhiteSpace(sourceD.DataSourceCompanyName))
				return null;

			var dto = new CompanyInputDto
			{
				Name = sourceD.DataSourceCompanyName!.Trim(),
				Address = sourceD.DataSourceAddress!.Trim(),
				City = sourceD.DataSourceCity!.Trim(),
				// ... map all properties
			};

			_logger.LogDebug("Mapped SourceD: {Name}", dto.Name);
			return dto;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error mapping SourceD");
			return null;
		}
	}
}
```

### Step 3: Register Source Constant

File: `src/MyStartUpCompany.Worker/Mappers/MessageSources.cs`

```csharp
public static class MessageSources
{
	// ... existing constants
	public const string SourceD = "SourceD";

	public enum MessageSource
	{
		// ... existing values
		SourceD,  // Add here
	}

	// Update conversion methods to handle SourceD
	public static MessageSource FromIdentifier(string identifier)
	{
		return identifier switch
		{
			// ... existing cases
			SourceD => MessageSource.SourceD,  // Add here
			_ => MessageSource.Unknown
		};
	}
}
```

### Step 4: Register in DI

File: `src/MyStartUpCompany.Worker/Extensions/MapperExtensions.cs`

```csharp
public static IServiceCollection AddMessageMappers(this IServiceCollection services)
{
	// ... existing registrations
	services.AddSingleton<SourceDMapper>();  // Add this line

	return services;
}
```

### Step 5: Register in Factory

File: `src/MyStartUpCompany.Worker/Mappers/MapperFactory.cs`

```csharp
private void InitializeMappers(IServiceProvider serviceProvider)
{
	// ... existing registrations

	// Register SourceD mapper
	try
	{
		var sourceDMapper = serviceProvider.GetService(typeof(SourceDMapper)) as SourceDMapper;
		if (sourceDMapper != null)
		{
			_mappers[MessageSources.SourceD] = sourceDMapper;
			_logger.LogInformation("Registered mapper for: {Source}", MessageSources.SourceD);
		}
	}
	catch (Exception ex)
	{
		_logger.LogWarning(ex, "Failed to register SourceD mapper");
	}
}
```

### That's It! ✅

**No changes needed to:**
- AzureServiceBusConsumerService
- CompanyMessageProcessor
- AddCompanyEventHandler
- Database persistence logic
- Application configuration

## 🎨 Design Patterns Used

1. **Strategy Pattern**: Each mapper implements `IMessageMapper<object>` strategy
2. **Factory Pattern**: `MapperFactory` resolves correct strategy by source
3. **Dependency Injection**: Spring-style constructor injection via .NET DI container
4. **Adapter Pattern**: Mappers adapt source-specific DTOs to unified CompanyInputDto
5. **Null Object Pattern**: Mappers return null instead of throwing exceptions

## 🔒 Key Design Principles

| Principle | Implementation | Benefit |
|-----------|----------------|---------|
| **Single Responsibility** | One mapper per source | Easy to test, modify, understand |
| **Open/Closed** | Open for extension (new mappers), closed for modification (core logic) | Add sources without changing existing code |
| **Dependency Inversion** | Depend on `IMessageMapper<object>` abstraction, not concrete mappers | Loose coupling, easy mocking in tests |
| **Interface Segregation** | Minimal `IMessageMapper<T>` interface | Mappers implement exactly what they need |
| **Don't Repeat Yourself** | Shared validation logic in each mapper | Consistent error handling |

## 🧪 Testing Strategy

Each mapper should have unit tests:

```csharp
[Fact]
public void Map_WithValidSourceAMessage_ReturnsValidDto()
{
	// Arrange
	var mapper = new SourceAMapper(_mockLogger);
	var sourceMessage = new SourceAMessage { CompanyName = "ACME", ... };

	// Act
	var result = mapper.Map(sourceMessage);

	// Assert
	result.Should().NotBeNull();
	result.Name.Should().Be("ACME");
}

[Fact]
public void Map_WithMissingRequiredField_ReturnsNull()
{
	// Arrange
	var mapper = new SourceAMapper(_mockLogger);
	var sourceMessage = new SourceAMessage { CompanyName = null, ... };

	// Act
	var result = mapper.Map(sourceMessage);

	// Assert
	result.Should().BeNull();
}
```

## 📊 Message Flow Example

```
Azure Service Bus Topic receives:
{
  "CompanyName": "Tech Corp",
  "StreetAddress": "123 Main St",
  "CityName": "New York",
  "ZipCode": "10001",
  "CountryCode": "USA",
  "StateCode": "NY",
  "PhoneNumber": "555-1234"
}

↓ [Extract metadata: Source = "SourceA"]

↓ [SourceAMapper.Map(sourceAMessage)]

↓ [Transform to database schema]

{
  "Name": "Tech Corp",
  "Address": "123 Main St",
  "City": "New York",
  "PostalCode": "10001",
  "Country": "USA",
  "Region": "NY",
  "Phone": "555-1234"
}

↓ [CompanyMessageProcessor validates & persists]

✅ Record saved to database
```

## 🚀 Scalability Considerations

- **Horizontal**: Add sources with zero impact on existing code
- **Performance**: Dictionary lookup for mappers is O(1)
- **Testability**: Each mapper is independently testable
- **Maintainability**: Changes to one source don't affect others
- **Logging**: Each mapper has its own logger instance

## ❌ Common Mistakes to Avoid

1. **Putting source-specific logic in the consumer**: Keep transformation at the boundary
2. **Hardcoding source names in core logic**: Use `MessageSources` constants
3. **Throwing exceptions in mappers**: Return null and log instead (graceful degradation)
4. **Forgetting to register new mappers**: Always update DI, Factory, and MessageSources
5. **Not validating source data**: Always check required fields before mapping

## 📖 References

- **Strategy Pattern**: Gang of Four Design Patterns
- **Factory Pattern**: Gang of Four Design Patterns
- **Dependency Injection**: Martin Fowler's article on DI
- **Clean Architecture**: Robert C. Martin (Uncle Bob)
