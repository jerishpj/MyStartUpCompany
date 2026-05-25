# Message Mapping Pattern - Quick Reference

## Architecture at a Glance

```
Message Source (A, B, C...) 
	↓
Extract Source Identifier
	↓
MapperFactory.GetMapper(source)
	↓
Mapper.Map(rawMessage) → CompanyInputDto
	↓
Database Save
```

## File Organization

```
MyStartUpCompany.Worker/
├── Mappers/
│   ├── IMessageMapper.cs         # Generic mapper interface
│   ├── IMapperFactory.cs         # Factory interface
│   ├── MapperFactory.cs          # Factory implementation
│   ├── MessageSources.cs         # Source constants & enums
│   └── Sources/
│       ├── SourceAMapper.cs      # SourceA strategy
│       └── SourceBMapper.cs      # SourceB strategy
├── Extensions/
│   └── MapperExtensions.cs       # DI registration
├── Services/
│   ├── AzureServiceBusConsumerService.cs  # ✏️ Modified
│   └── CompanyMessageProcessor.cs         # ✏️ Modified
└── Program.cs                             # ✏️ Modified
```

## How Messages Flow

1. **Message arrives** from Azure Service Bus with source identifier
2. **Source extracted** from `ApplicationProperties["Source"]` or `Subject`
3. **Mapper selected** via factory: `mapperFactory.GetMapper(source)`
4. **Message mapped** to `CompanyInputDto` by mapper strategy
5. **Data persisted** to database via `AddCompanyEventHandler`

## Adding a New Source - Quick Steps

```csharp
// 1. Create message model
public class SourceCMessage { /* fields */ }

// 2. Create mapper (implement IMessageMapper<object>)
public class SourceCMapper : IMessageMapper<object>
{
	public CompanyInputDto? Map(object sourceMessage) { /* logic */ }
}

// 3. Register in MapperFactory.InitializeMappers()
var sourceCMapper = serviceProvider.GetService(typeof(SourceCMapper)) as SourceCMapper;
if (sourceCMapper != null)
	_mappers[MessageSources.SourceC] = sourceCMapper;

// 4. Add source constant to MessageSources.cs
public const string SourceC = "SourceC";

// 5. Send message with source identifier
var message = new ServiceBusMessage(json)
{
	ApplicationProperties = new Dictionary<string, object>
	{
		{ "Source", "SourceC" }
	}
};
```

## Key Classes & Interfaces

| Class | Purpose | Location |
|-------|---------|----------|
| `IMessageMapper<T>` | Mapper strategy contract | `Mappers/IMessageMapper.cs` |
| `IMapperFactory` | Factory interface | `Mappers/IMapperFactory.cs` |
| `MapperFactory` | Mapper resolution & management | `Mappers/MapperFactory.cs` |
| `SourceAMapper` | Strategy for SourceA format | `Mappers/Sources/SourceAMapper.cs` |
| `SourceBMapper` | Strategy for SourceB format | `Mappers/Sources/SourceBMapper.cs` |
| `MessageSources` | Source identifiers | `Mappers/MessageSources.cs` |

## Common Tasks

### Extract Source from Message
```csharp
// In AzureServiceBusConsumerService
string source = ExtractMessageSource(message);
// Returns: "SourceA", "SourceB", "Direct", or "Unknown"
```

### Get Mapper for Source
```csharp
var mapper = mapperFactory.GetMapper(source);
if (mapper == null)
{
	_logger.LogWarning("No mapper for source: {Source}", source);
	return null;
}
```

### Map Message to DTO
```csharp
var companyDto = mapperFactory.MapMessage(source, rawMessage);
if (companyDto == null)
{
	_logger.LogWarning("Mapping failed for source: {Source}", source);
	return null;
}
```

### Process Mapped Data
```csharp
var result = await messageProcessor.ProcessCompanyAsync(
	companyDto, 
	sourceIdentifier, 
	cancellationToken);

// Result: Success, Duplicate, Invalid, or Error
```

## Patterns Used

| Pattern | Problem Solved | Benefit |
|---------|---|---|
| **Strategy** | Different sources need different mapping logic | Easy to add new transformations |
| **Factory** | Decouple processor from mapper implementations | Mapper selection centralized |
| **Dependency Injection** | Loose coupling and easy testing | Testable, maintainable code |

## Logging

Common log messages to monitor:

```
[Debug]   Found mapper for source: SourceA
[Debug]   Attempting to map message 12345 from source SourceA
[Debug]   Successfully mapped message 12345 from source SourceA to CompanyInputDto
[Warning] No mapper found for source: SourceX
[Warning] Mapper returned null for source: SourceA
[Error]   Error mapping message from source SourceA
```

## Configuration

### DI Registration (Program.cs)
```csharp
builder.Services.AddMessageMappers();  // One-line setup
```

### Runtime Source Identifier
Send messages with:
```csharp
// ApplicationProperties (Recommended)
message.ApplicationProperties["Source"] = "SourceA"

// OR Subject line
message.Subject = "SourceB"

// Defaults to "Direct" if not specified
```

## Testing

### Test a Mapper
```csharp
[Fact]
public void SourceAMapper_WithValidMessage_ReturnsMappedDto()
{
	var mapper = new SourceAMapper(logger);
	var sourceA = new SourceAMessage { /* data */ };

	var result = mapper.Map(sourceA);

	Assert.NotNull(result);
	Assert.Equal("Expected Name", result.Name);
}
```

### Test Factory
```csharp
[Fact]
public void MapperFactory_GetMapper_WithSourceA_ReturnsSourceAMapper()
{
	var factory = new MapperFactory(serviceProvider, logger);

	var mapper = factory.GetMapper("SourceA");

	Assert.NotNull(mapper);
	Assert.IsType<SourceAMapper>(mapper);
}
```

## Performance

- **Lookup Time**: O(1) - Dictionary-based
- **Memory**: Minimal - Singletons
- **Thread Safety**: Yes - Concurrent dictionary
- **Initialization**: Once on startup

## Backward Compatibility

✅ Fully backward compatible:
- Existing "Direct" messages still work
- All 90 existing tests pass
- No breaking changes to existing APIs

## Documentation Files

- **`MAPPER_PATTERN_GUIDE.md`** - Comprehensive guide with examples
- **`IMPLEMENTATION_SUMMARY.md`** - Implementation overview
- **This file** - Quick reference

---

**Quick Links**:
- 📖 Full Guide: `MAPPER_PATTERN_GUIDE.md`
- 📋 Summary: `IMPLEMENTATION_SUMMARY.md`
- 💻 Example Mappers: `Mappers/Sources/SourceAMapper.cs`, `SourceBMapper.cs`
- 🏭 Factory: `Mappers/MapperFactory.cs`

**Status**: ✅ Production Ready | **Tests**: ✅ 90/90 Passing | **Build**: ✅ Successful
