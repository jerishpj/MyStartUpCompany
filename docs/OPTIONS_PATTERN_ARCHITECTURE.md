# Options Pattern Architecture

## Component Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    appsettings.json                             │
│  (Production Configuration)                                     │
├─────────────────────────────────────────────────────────────────┤
│ {                                                               │
│   "Api": { "BaseUrl": "https://api.mystartupcorp.com", ... },  │
│   "Database": { "ConnectionString": "...", ... },              │
│   "Cache": { "Provider": "Memory", ... },                       │
│   "FeatureFlags": { "EnableExport": false, ... }               │
│ }                                                               │
└─────────────────────────────────────────────────────────────────┘
							│
							↓ Binds
┌─────────────────────────────────────────────────────────────────┐
│            OptionsExtensions.AddApplicationOptions()            │
│                                                                 │
│  Registers:                                                     │
│  • ApiOptions                 (ValidateOnStart)               │
│  • DatabaseOptions            (ValidateOnStart)               │
│  • CacheOptions               (ValidateOnStart)               │
│  • FeatureFlags               (No ValidateOnStart)            │
└─────────────────────────────────────────────────────────────────┘
							│
							↓ Dependency Injection
┌─────────────────────────────────────────────────────────────────┐
│                  Application Services                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  CompanyService                 ExportService                   │
│  ├─ IOptions<ApiOptions>        ├─ IOptionsMonitor<Flags>      │
│  └─ ConfigurationExampleService └─ IOptions<CacheOptions>      │
│     ├─ IOptions<ApiOptions>                                    │
│     ├─ IOptionsMonitor<Flags>                                  │
│     ├─ IOptions<CacheOptions>                                  │
│     └─ ILogger<T>                                              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Sequence Diagram: Startup Validation

```
Application Start
	│
	├─→ builder.CreateBuilder()
	│       │
	│       └─→ Load appsettings.json
	│
	├─→ builder.Services.AddApplicationOptions()
	│       │
	│       ├─→ AddOptions<ApiOptions>()
	│       │       ├─ BindConfiguration("Api")
	│       │       ├─ ValidateDataAnnotations()
	│       │       └─ ValidateOnStart()
	│       │           │
	│       │           ├─ Check [Required] ✓
	│       │           ├─ Check [Url] ✓
	│       │           ├─ Check [Range] ✓
	│       │           └─ ALL VALID ✓
	│       │
	│       ├─→ AddOptions<DatabaseOptions>()
	│       │       └─ Same validation process ✓
	│       │
	│       ├─→ AddOptions<CacheOptions>()
	│       │       └─ Same validation process ✓
	│       │
	│       └─→ AddOptions<FeatureFlags>()
	│               └─ ValidateDataAnnotations() only
	│                   (No ValidateOnStart - dynamic)
	│
	├─→ app.Build()
	│       │
	│       └─ All services ready with validated options
	│
	└─→ app.Run() ✓ Application starts successfully
```

## Data Flow: From Configuration to Service

```
appsettings.json
{
  "Api": {
	"DefaultPageSize": 20,
	"MaxPageSize": 100
  }
}
	│
	↓ (BindConfiguration)

ApiOptions
{
	SectionName = "Api"
	DefaultPageSize = 20
	MaxPageSize = 100
}
	│
	↓ (Registered in DI Container)

IOptions<ApiOptions>
	│
	↓ (Injected into service constructor)

public class CompanyService
{
	public CompanyService(IOptions<ApiOptions> options)
	{
		_options = options;
	}

	public void GetCompanies()
	{
		var defaultSize = _options.Value.DefaultPageSize;  // 20
		var maxSize = _options.Value.MaxPageSize;          // 100
	}
}
```

## Service Registration Flow

```
┌─────────────────────────────────────────────────┐
│ Program.cs (Startup)                            │
├─────────────────────────────────────────────────┤
│                                                 │
│ var builder = WebApplication.CreateBuilder()   │
│                                                 │
│ // Register options with validation             │
│ builder.Services.AddApplicationOptions(         │
│     builder.Configuration);                     │
│                                                 │
│ // At this point:                               │
│ // - ApiOptions.Enabled = true ✓               │
│ // - DatabaseOptions.ConnectionString = "..." ✓│
│ // - CacheOptions.Provider = "Memory" ✓         │
│ // - FeatureFlags.EnableExport = false ✓       │
│                                                 │
│ var app = builder.Build();  ← Validation runs! │
│                                                 │
│ if (Config is INVALID)                          │
│     throw OptionsValidationException ✗          │
│                                                 │
│ app.Run();  ← App only reaches here if valid ✓ │
│                                                 │
└─────────────────────────────────────────────────┘
```

## Configuration Binding Process

```
Raw JSON Configuration
┌──────────────────────┐
│ "Api": {             │
│   "BaseUrl": "...",  │
│   "Version": "v1"    │
│ }                    │
└──────────────────────┘
		│
		↓ (Configuration Provider)

Intermediate Dict
┌──────────────────────┐
│ "Api:BaseUrl" → "..." │
│ "Api:Version" → "v1"  │
└──────────────────────┘
		│
		↓ (BindConfiguration)

Typed ApiOptions Object
┌──────────────────────┐
│ BaseUrl = "..."      │
│ Version = "v1"       │
└──────────────────────┘
		│
		↓ (ValidateDataAnnotations)

Validation
┌──────────────────────┐
│ [Url] ✓              │
│ [Required] ✓         │
│ [Range] ✓            │
└──────────────────────┘
		│
		↓ (ValidateOnStart)

Result
┌──────────────────────┐
│ ✓ VALID - Proceed    │
│ or                   │
│ ✗ INVALID - Throw    │
└──────────────────────┘
```

## Environment-Specific Override Flow

```
Base Configuration (appsettings.json)
┌────────────────────────────────────┐
│ Api:BaseUrl = "https://api.prod"   │
│ Api:IncludeExceptionDetails = false │
│ FeatureFlags:EnableExport = false   │
└────────────────────────────────────┘
		│
		↓ (if ASPNETCORE_ENVIRONMENT == Development)

Environment Override (appsettings.Development.json)
┌────────────────────────────────────┐
│ Api:BaseUrl = "https://localhost"   │
│ Api:IncludeExceptionDetails = true  │
│ FeatureFlags:EnableExport = true    │
└────────────────────────────────────┘
		│
		↓ (Merge: Override wins)

Final Configuration in Memory
┌────────────────────────────────────┐
│ Api:BaseUrl = "https://localhost"   │ ← Override
│ Api:IncludeExceptionDetails = true  │ ← Override
│ FeatureFlags:EnableExport = true    │ ← Override
└────────────────────────────────────┘
```

## Validation Error Handling

```
Application Start
	│
	├─→ AddApplicationOptions()
	│       │
	│       ├─→ Validate ApiOptions
	│       │       ├─ [Required] BaseUrl
	│       │       │   ├─ Value: null ✗
	│       │       │   └─ ERROR: "BaseUrl is required"
	│       │       │
	│       │       ├─ [Url] BaseUrl
	│       │       │   ├─ Value: "invalid" ✗
	│       │       │   └─ ERROR: "BaseUrl must be a valid URL"
	│       │       │
	│       │       └─ [Range] MaxPageSize
	│       │           ├─ Value: 2000 ✗ (> 1000)
	│       │           └─ ERROR: "Must be between 1 and 1000"
	│       │
	│       └─ Validation FAILS
	│
	├─→ app.Build() 
	│       │
	│       └─ Throws OptionsValidationException
	│           │
	│           └─ Message shows all validation errors
	│
	└─→ Application EXITS with error message
		(Developer sees problems immediately!)
```

## Dependency Injection Pattern

```
Service Consumer:
┌──────────────────────────────────────┐
│ public class MyService               │
│ {                                    │
│   public MyService(                  │
│     IOptions<ApiOptions> options,   │ ← Immutable
│     IOptionsMonitor<Flags> flags    │ ← Dynamic
│   ) { }                              │
│ }                                    │
└──────────────────────────────────────┘
		   │
		   ↑ Resolved from DI Container

┌──────────────────────────────────────┐
│ Service Provider (DI Container)      │
│                                      │
│ IOptions<ApiOptions>                │
│   → Registered by AddOptions()       │
│   → Singleton (one instance)         │
│   → Immutable after startup          │
│                                      │
│ IOptionsMonitor<FeatureFlags>       │
│   → Registered by AddOptions()       │
│   → Singleton scope                  │
│   → Can detect changes               │
│                                      │
│ IOptionsSnapshot<T>                 │
│   → Scoped (one per request)        │
│   → Can have request-specific values │
│                                      │
└──────────────────────────────────────┘
```

## Configuration Validation Timeline

```
┌─────────────────────────────────────────────────────────┐
│                  Application Lifetime                   │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  [1] Startup                                            │
│      ├─ Load appsettings.json        (0-100ms)         │
│      ├─ Merge environment overrides  (0-100ms)         │
│      ├─ Create ApiOptions instance   (0-50ms)          │
│      │                                                 │
│      ├─ Validate with Data Annotations                │
│      │  ├─ Check [Required]           (0-50ms)         │
│      │  ├─ Check [Url]                (0-50ms)         │
│      │  ├─ Check [Range]              (0-50ms)         │
│      │  └─ Check [Custom]             (0-50ms)         │
│      │                                                 │
│      └─ ValidateOnStart() → Throws if invalid ✗       │
│         or Proceeds if valid ✓                         │
│                                                         │
│  [2] Configuration VALID ✓                             │
│      └─ Options registered in DI                       │
│                                                         │
│  [3] Build & Run                                       │
│      ├─ Services instantiated                          │
│      ├─ Options injected into constructors             │
│      └─ Application ready to handle requests           │
│                                                         │
│  [4] Runtime (During Request Processing)              │
│      ├─ IOptions<T>.Value (cached, never changes)     │
│      ├─ IOptionsMonitor<T>.CurrentValue (dynamic)     │
│      └─ IOptionsSnapshot<T>.Value (per-request)       │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

## Key Differences: Options Access Patterns

```
┌─────────────┬──────────────┬──────────────┬────────────────┐
│  Interface  │  Scope       │  Caching     │  Use Case      │
├─────────────┼──────────────┼──────────────┼────────────────┤
│             │              │              │                │
│ IOptions<T> │  Singleton   │  Immutable   │ Most settings  │
│             │  (App-wide)  │  Cached      │ (API, DB)      │
│             │              │              │                │
│ IOptionsMonitor<T>         │ Always       │ Feature flags  │
│             │  Singleton   │  Current     │ Dynamic config │
│             │              │              │                │
│ IOptionsSnapshot<T>        │ Scoped       │ Per-request    │
│             │  Per request │  Snapshot    │ variations     │
│             │              │              │                │
└─────────────┴──────────────┴──────────────┴────────────────┘
```

## Implementation Checklist

```
✓ Created Configuration Classes
  ├─ ApiOptions.cs (12 properties)
  ├─ DatabaseOptions.cs (8 properties + DatabaseRetryPolicy)
  ├─ CacheOptions.cs (7 properties)
  └─ FeatureFlags.cs (11 properties)

✓ Created Registration Extension
  └─ OptionsExtensions.AddApplicationOptions()

✓ Updated Startup
  ├─ Program.cs - added options registration
  └─ Added import statement

✓ Updated Configuration Files
  ├─ appsettings.json - production settings
  └─ appsettings.Development.json - dev overrides

✓ Created Example Usage
  ├─ ConfigurationExampleService.cs (6 example methods)
  └─ ConfigurationExampleServiceTests.cs (10 tests)

✓ Testing
  └─ All 10 unit tests passing ✓

✓ Documentation
  ├─ OPTIONS_PATTERN_GUIDE.md (comprehensive)
  ├─ OPTIONS_PATTERN_IMPLEMENTATION.md (summary)
  └─ OPTIONS_PATTERN_QUICK_REFERENCE.md (quick guide)
```

---

## Summary

The Options Pattern provides a clean, type-safe way to handle configuration in ASP.NET Core:

1. **Configuration Binding** - JSON → Typed Classes
2. **Validation** - Data annotations + custom validators
3. **Dependency Injection** - Services receive typed options
4. **Environment Overrides** - Dev/Prod differences
5. **Dynamic Updates** - Feature flags can change without restart

**Result:** Production-grade configuration management with compile-time safety! ✓
