# Options Pattern - Quick Reference

## TL;DR - Why Options Pattern?

| Without Options | With Options Pattern |
|---|---|
| ❌ Magic strings `config["Api:BaseUrl"]` | ✅ Type-safe `options.Value.BaseUrl` |
| ❌ Errors at runtime | ✅ Errors at startup with `.ValidateOnStart()` |
| ❌ No IntelliSense | ✅ Full IntelliSense support |
| ❌ Scattered throughout code | ✅ Centralized in typed classes |
| ❌ Hard to test | ✅ Easy to mock `Options.Create(...)` |
| ❌ No validation | ✅ Data annotations + custom validators |

---

## Quick Implementation Checklist

- ✅ Create typed option classes in `Configuration/` folder
- ✅ Add `SectionName` constant to each class
- ✅ Add validation attributes (`[Required]`, `[Range]`, etc.)
- ✅ Add default values for all properties
- ✅ Create extension method for registration
- ✅ Call extension in `Program.cs`
- ✅ Add configuration sections to `appsettings.json`
- ✅ Add environment overrides to `appsettings.Development.json`
- ✅ Inject `IOptions<T>` or `IOptionsMonitor<T>` in services
- ✅ Test with `Options.Create()`

---

## Usage Patterns

### Pattern 1: Immutable Config (Most Common)
```csharp
public class MyService
{
	private readonly IOptions<ApiOptions> _options;

	public MyService(IOptions<ApiOptions> options)
	{
		_options = options;
	}

	public void DoWork()
	{
		var baseUrl = _options.Value.BaseUrl;  // ← Access via .Value
	}
}
```

### Pattern 2: Dynamic Config (Feature Flags)
```csharp
public class FeatureService
{
	private readonly IOptionsMonitor<FeatureFlags> _flags;

	public FeatureService(IOptionsMonitor<FeatureFlags> flags)
	{
		_flags = flags;
	}

	public void DoWork()
	{
		var enabled = _flags.CurrentValue.EnableExport;  // ← Always current
	}
}
```

### Pattern 3: Testing
```csharp
[Fact]
public void MyTest()
{
	// Mock the options
	var options = Options.Create(new ApiOptions { BaseUrl = "https://test" });

	// Inject into service
	var service = new MyService(options);

	// Test
	service.DoWork();
}
```

---

## Configuration Files

### Typical appsettings.json Structure
```json
{
  "Api": {
	"BaseUrl": "https://api.example.com",
	"Version": "v1",
	"MaxPageSize": 100
  },
  "Database": {
	"ConnectionString": "...",
	"Provider": "SqlServer",
	"RetryPolicy": {
	  "Enabled": true,
	  "MaxRetries": 3
	}
  },
  "Cache": {
	"Enabled": true,
	"Provider": "Memory",
	"DefaultDurationSeconds": 300
  },
  "FeatureFlags": {
	"EnableExport": false,
	"EnableAdvancedSearch": false
  }
}
```

---

## Validation

### At Startup (Catch Errors Early)
```csharp
services.AddOptions<ApiOptions>()
	.BindConfiguration(ApiOptions.SectionName)
	.ValidateDataAnnotations()
	.ValidateOnStart();  // ← Throws if invalid!
```

### On Demand (For Feature Flags)
```csharp
services.AddOptions<FeatureFlags>()
	.BindConfiguration(FeatureFlags.SectionName);
	// ← No .ValidateOnStart() - features can be flexible
```

---

## Common Mistakes to Avoid

### ❌ Mistake 1: Injecting IConfiguration instead
```csharp
// DON'T do this:
public MyService(IConfiguration config)
{
	_value = config["Api:BaseUrl"];  // ← No type safety, no validation
}

// ✅ DO this:
public MyService(IOptions<ApiOptions> options)
{
	_value = options.Value.BaseUrl;  // ← Type safe, validated
}
```

### ❌ Mistake 2: Forgetting ValidateOnStart()
```csharp
// DON'T do this:
services.AddOptions<ApiOptions>()
	.BindConfiguration(ApiOptions.SectionName)
	.ValidateDataAnnotations();
	// ← Error might occur hours later when option is accessed!

// ✅ DO this:
services.AddOptions<ApiOptions>()
	.BindConfiguration(ApiOptions.SectionName)
	.ValidateDataAnnotations()
	.ValidateOnStart();  // ← Fail immediately at startup
```

### ❌ Mistake 3: Using IOptions for dynamic config
```csharp
// DON'T do this for feature flags:
public FeatureService(IOptions<FeatureFlags> flags)
{
	// This cached value won't update if config changes!
	_enabled = flags.Value.EnableExport;
}

// ✅ DO this:
public FeatureService(IOptionsMonitor<FeatureFlags> flags)
{
	// This always gets current value
	_enabled = flags.CurrentValue.EnableExport;
}
```

### ❌ Mistake 4: Missing section name
```csharp
// Configuration:
{
  "Api": { "BaseUrl": "..." }
}

// ✅ Correct section name binding:
public const string SectionName = "Api";
services.AddOptions<ApiOptions>()
	.BindConfiguration(ApiOptions.SectionName);
```

---

## Your Implementation

### Files Created
```
src/MyStartUpCompany.Api/
├── Configuration/
│   ├── ApiOptions.cs          ✅ API settings
│   ├── DatabaseOptions.cs     ✅ DB settings + RetryPolicy
│   ├── CacheOptions.cs        ✅ Cache settings
│   └── FeatureFlags.cs        ✅ Feature toggles
├── Extensions/
│   └── OptionsExtensions.cs   ✅ Registration extension
└── Services/
	└── ConfigurationExampleService.cs  ✅ Usage example

tests/MyStartUpCompany.Api.Tests/
└── Common/
	└── ConfigurationExampleServiceTests.cs  ✅ 10 passing tests

docs/
├── OPTIONS_PATTERN_GUIDE.md           ✅ Detailed guide
└── OPTIONS_PATTERN_IMPLEMENTATION.md  ✅ This implementation
```

### Configuration Sections Available
- **Api** - Base URL, versions, pagination, compression, CORS
- **Database** - Connection strings, timeouts, retry policy
- **Cache** - Provider, TTL, key prefix, size limits
- **FeatureFlags** - All feature toggles for gradual rollout

---

## Testing Pattern

```csharp
public class MyServiceTests
{
	[Fact]
	public void MyTest()
	{
		// Arrange: Create mocked options
		var options = Options.Create(new ApiOptions 
		{ 
			MaxPageSize = 50,
			DefaultPageSize = 10 
		});

		// Act: Inject and test
		var service = new MyService(options);
		var result = service.GetPaginationDefaults();

		// Assert
		Assert.Equal(10, result.PageSize);
	}
}
```

---

## Integration Checklist

- [ ] Services updated to use `IOptions<T>` injection
- [ ] Database configuration reading from `DatabaseOptions`
- [ ] Cache implementation using `CacheOptions`
- [ ] Feature flags checked with `IOptionsMonitor<FeatureFlags>`
- [ ] Rate limiting configured from `FeatureFlags.EnableRateLimiting`
- [ ] CORS configured from `ApiOptions.AllowedCorsOrigins`
- [ ] All configuration errors caught at startup
- [ ] Environment-specific configs working (Dev vs Prod)
- [ ] All services tested with mocked options

---

## Environment Overrides

### Development (appsettings.Development.json)
```json
{
  "Api": {
	"BaseUrl": "https://localhost:7001",
	"IncludeExceptionDetails": true
  },
  "FeatureFlags": {
	"EnableExport": true,
	"EnableAdvancedSearch": true
  }
}
```

### Production (appsettings.json)
```json
{
  "Api": {
	"BaseUrl": "https://api.mystartupcorp.com",
	"IncludeExceptionDetails": false
  },
  "FeatureFlags": {
	"EnableExport": false,
	"EnableAdvancedSearch": false
  }
}
```

---

## Support Interfaces

| Interface | Use Case | Access Pattern |
|---|---|---|
| `IOptions<T>` | Immutable config | `options.Value` |
| `IOptionsMonitor<T>` | Dynamic changes | `optionsMonitor.CurrentValue` |
| `IOptionsSnapshot<T>` | Per-request scope | `optionsSnapshot.Value` |

---

## Key Takeaway

> **Options Pattern = Type Safety + Validation + Dependency Injection**

It's the recommended way to handle configuration in modern ASP.NET Core applications.

---

**Status:** ✅ Implemented and tested  
**Tests:** 10/10 passing  
**Build:** Successful  

For detailed information, see `docs/OPTIONS_PATTERN_GUIDE.md`
