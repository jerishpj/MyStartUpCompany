# Quick Reference: Adding a New Message Source

## TL;DR - 5 Files to Modify/Create

### 1️⃣ Create Source DTO
**File:** `src/MyStartUpCompany.Worker/Mappers/Sources/Source[X]Message.cs`

```csharp
public class Source[X]Message
{
	public string? SourceSpecificPropertyName { get; set; }
	// ... all other source-specific properties
}
```

### 2️⃣ Create Mapper
**File:** `src/MyStartUpCompany.Worker/Mappers/Sources/Source[X]Mapper.cs`

```csharp
using MyStartUpCompany.Worker.Handlers.AddCompany;

namespace MyStartUpCompany.Worker.Mappers.Sources
{
	public class Source[X]Mapper : IMessageMapper<object>
	{
		private readonly ILogger<Source[X]Mapper> _logger;

		public Source[X]Mapper(ILogger<Source[X]Mapper> logger)
		{
			_logger = logger;
		}

		public CompanyInputDto? Map(object sourceMessage)
		{
			try
			{
				var source[X] = sourceMessage as Source[X]Message;
				if (source[X] == null)
				{
					_logger.LogWarning("Failed to cast to Source[X]Message");
					return null;
				}

				// Validate all required fields
				if (string.IsNullOrWhiteSpace(source[X].SourceSpecificPropertyName))
				{
					_logger.LogWarning("Missing required field: SourceSpecificPropertyName");
					return null;
				}

				// Map to CompanyInputDto
				var dto = new CompanyInputDto
				{
					Name = source[X].SourceSpecificPropertyName!.Trim(),
					// ... map all properties using Source[X] property names
				};

				_logger.LogDebug("Mapped Source[X] message: {CompanyName}", dto.Name);
				return dto;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error mapping Source[X] message");
				return null;
			}
		}
	}
}
```

### 3️⃣ Register Source Constant
**File:** `src/MyStartUpCompany.Worker/Mappers/MessageSources.cs`

Add to `MessageSources` class:
```csharp
public const string Source[X] = "Source[X]";
```

Add to `MessageSource` enum:
```csharp
public enum MessageSource
{
	// ... existing values
	Source[X],  // ← Add here
}
```

Add to `FromIdentifier` method:
```csharp
Source[X] => MessageSource.Source[X],  // ← Add this case
```

Add to `ToIdentifier` method:
```csharp
MessageSource.Source[X] => Source[X],  // ← Add this case
```

Add to `GetAllSources` method:
```csharp
Source[X],  // ← Add to array
```

### 4️⃣ Register in DI
**File:** `src/MyStartUpCompany.Worker/Extensions/MapperExtensions.cs`

```csharp
public static IServiceCollection AddMessageMappers(this IServiceCollection services)
{
	// ... existing registrations
	services.AddSingleton<Source[X]Mapper>();  // ← Add this

	return services;
}
```

### 5️⃣ Register in Factory
**File:** `src/MyStartUpCompany.Worker/Mappers/MapperFactory.cs`

In `InitializeMappers` method, add:
```csharp
// Register Source[X] mapper
try
{
	var source[X]Mapper = serviceProvider.GetService(typeof(Source[X]Mapper)) as Source[X]Mapper;
	if (source[X]Mapper != null)
	{
		_mappers[MessageSources.Source[X]] = source[X]Mapper;
		_logger.LogInformation("Registered mapper for source: {Source}", MessageSources.Source[X]);
	}
}
catch (Exception ex)
{
	_logger.LogWarning(ex, "Failed to register Source[X] mapper");
}
```

---

## Property Mapping Reference

### Mapping Template

Use this as a template when creating your mapper:

| Source Property | Database Property | Required | Notes |
|-----------------|-------------------|----------|-------|
| SourceSpecificName | Name | ✅ | Trim whitespace |
| SourceSpecificDesc | Description | ❌ | Optional |
| SourceSpecificAddr | Address | ✅ | Trim whitespace |
| SourceSpecificCity | City | ✅ | Trim whitespace |
| SourceSpecificRegion | Region | ❌ | Optional |
| SourceSpecificZip | PostalCode | ✅ | Trim whitespace |
| SourceSpecificCountry | Country | ✅ | Trim whitespace |
| SourceSpecificPhone | Phone | ✅ | Trim whitespace |

### Database Schema (Target)

```csharp
public class CompanyInputDto
{
	public string? Name { get; set; }                // Required
	public string? Description { get; set; }        // Optional
	public string? Address { get; set; }            // Required
	public string? City { get; set; }               // Required
	public string? Region { get; set; }             // Optional
	public string? PostalCode { get; set; }         // Required
	public string? Country { get; set; }            // Required
	public string? Phone { get; set; }              // Required
}
```

---

## Real-World Examples

### Example: Adding Source D (Fintech API)

**Property Mapping:**
- Fintech `bizName` → Database `Name`
- Fintech `bizLocation` → Database `City`
- Fintech `bizZoneCode` → Database `Region`

**File 1: SourceDMessage.cs**
```csharp
public class SourceDMessage
{
	public string? BizName { get; set; }
	public string? BizLocation { get; set; }
	public string? BizZoneCode { get; set; }
	// ... etc
}
```

**File 2: SourceDMapper.cs**
```csharp
public CompanyInputDto? Map(object sourceMessage)
{
	var sourceD = sourceMessage as SourceDMessage;
	if (sourceD?.BizName == null) return null;  // Required validation

	return new CompanyInputDto
	{
		Name = sourceD.BizName.Trim(),              // BizName → Name
		City = sourceD.BizLocation?.Trim(),         // BizLocation → City
		Region = sourceD.BizZoneCode?.Trim(),       // BizZoneCode → Region
		// ... map remaining properties
	};
}
```

---

## Validation Checklist

When adding a new source:

- [ ] Created `Source[X]Message.cs` with all source properties
- [ ] Created `Source[X]Mapper.cs` implementing `IMessageMapper<object>`
- [ ] Mapper validates all required fields before mapping
- [ ] Mapper logs errors and returns null on failure (no exceptions)
- [ ] All properties are trimmed for consistency
- [ ] Added `Source[X]` constant to `MessageSources.cs`
- [ ] Added `Source[X]` enum value to `MessageSources.cs`
- [ ] Updated `FromIdentifier()` method in `MessageSources.cs`
- [ ] Updated `ToIdentifier()` method in `MessageSources.cs`
- [ ] Updated `GetAllSources()` method in `MessageSources.cs`
- [ ] Registered mapper in `MapperExtensions.cs` `AddMessageMappers()` method
- [ ] Registered mapper in `MapperFactory.cs` `InitializeMappers()` method
- [ ] Solution builds successfully
- [ ] All tests pass (90/90 in Worker.Tests)
- [ ] Created unit tests for new mapper (optional but recommended)

---

## Directory Structure

```
src/MyStartUpCompany.Worker/
├── Mappers/
│   ├── IMessageMapper.cs
│   ├── IMapperFactory.cs
│   ├── MapperFactory.cs
│   ├── MessageSources.cs
│   └── Sources/
│       ├── SourceAMessage.cs
│       ├── SourceAMapper.cs
│       ├── SourceBMessage.cs
│       ├── SourceBMapper.cs
│       ├── SourceCMessage.cs
│       ├── SourceCMapper.cs
│       └── Source[X]Message.cs       ← Add here
│       └── Source[X]Mapper.cs        ← Add here
├── Extensions/
│   └── MapperExtensions.cs           ← Update here
└── Services/
	├── AzureServiceBusConsumerService.cs
	└── CompanyMessageProcessor.cs
```

---

## Common Property Name Mappings

### Different Source Property Names for "Company Name"
- SourceA: `CompanyName`
- SourceB: `OrganizationName`
- SourceC: `BizName`
- SourceD: `EnterpriseName`

### Different Source Property Names for "City"
- SourceA: `CityName`
- SourceB: `Municipality`
- SourceC: `BizCity`
- SourceD: `CityLocation`

### Different Source Property Names for "Postal Code"
- SourceA: `ZipCode`
- SourceB: `PostCode`
- SourceC: `BizZip`
- SourceD: `ZoneCode`

---

## Troubleshooting

| Issue | Cause | Solution |
|-------|-------|----------|
| "No mapper found for source" | Mapper not registered in factory | Check MapperFactory.InitializeMappers() |
| Build fails | Mapper type not imported | Add `using MyStartUpCompany.Worker.Mappers.Sources;` |
| Mapper returns null | Required field validation failing | Review mapper validation logic |
| Tests fail | Mapper not registered in DI | Check MapperExtensions.cs AddMessageMappers() |
| Property null in DTO | Source property name mismatch | Verify correct property mapping in mapper |

---

## Related Files

- 📄 **Full Architecture Guide**: `docs/MESSAGE_MAPPING_ARCHITECTURE.md`
- 🔗 **Mapper Interface**: `src/MyStartUpCompany.Worker/Mappers/IMessageMapper.cs`
- 🔗 **Factory Interface**: `src/MyStartUpCompany.Worker/Mappers/IMapperFactory.cs`
- 🔗 **Service Bus Consumer**: `src/MyStartUpCompany.Worker/Services/AzureServiceBusConsumerService.cs`
- 🔗 **Message Processor**: `src/MyStartUpCompany.Worker/Services/CompanyMessageProcessor.cs`
