# Options Pattern Implementation Guide for MyStartUpCompany.Api

## 📚 Table of Contents
1. [Benefits of Options Pattern](#benefits)
2. [Current State Analysis](#current-state)
3. [Step-by-Step Implementation](#implementation)
4. [Best Practices](#best-practices)
5. [Troubleshooting](#troubleshooting)

---

## Benefits of Options Pattern {#benefits}

### 1. **Type Safety** ✅
```csharp
// ❌ WITHOUT Options Pattern (Unsafe)
string dbConnection = configuration["Database:ConnectionString"];
string dbName = configuration["Database:Name"];
int timeout = int.Parse(configuration["Database:Timeout"]); // Exception if invalid

// ✅ WITH Options Pattern (Type Safe)
var dbOptions = options.Value.Database;
string dbConnection = dbOptions.ConnectionString;  // Strongly typed
string dbName = dbOptions.Name;
int timeout = dbOptions.Timeout;  // Already an int
```

### 2. **Validation at Startup** ✅
```csharp
// Catch configuration errors at application startup, not when accessed
// Options are validated when the container is configured
builder.Services.AddOptions<DatabaseOptions>()
	.BindConfiguration(DatabaseOptions.SectionName)
	.ValidateDataAnnotations()
	.ValidateOnStart();  // ← Validates at startup, not runtime
```

### 3. **Centralized Configuration** ✅
- Single source of truth for configuration
- Easy to find what settings are available
- IntelliSense support in code
- Self-documenting through properties

### 4. **Dependency Injection Friendly** ✅
```csharp
// Inject directly into constructors
public class CompanyService
{
	public CompanyService(IOptions<ApiOptions> options)
	{
		var baseUrl = options.Value.BaseUrl;
	}
}
```

### 5. **Configuration Reloading** ✅
```csharp
// Use IOptionsMonitor<T> for dynamic configuration reloading
public class CompanyService
{
	private readonly IOptionsMonitor<ApiOptions> _options;

	public CompanyService(IOptionsMonitor<ApiOptions> options)
	{
		_options = options;  // Can detect config changes
	}

	public void DoWork()
	{
		var baseUrl = _options.CurrentValue.BaseUrl;  // Always current value
	}
}
```

### 6. **Separation of Concerns** ✅
- Configuration logic isolated from business logic
- Easier to test (inject mock options)
- Follows SOLID principles (Single Responsibility)

### 7. **Reduced Magic Strings** ✅
```csharp
// ❌ Magic strings scattered throughout code
var value = configuration["SomeNestedSetting:Deep:Value"];

// ✅ Constants in options class
public const string SectionName = "ApiSettings";
public const string BaseUrlKey = nameof(BaseUrl);
```

### 8. **Environment-Specific Configuration** ✅
```csharp
// appsettings.json (production defaults)
// appsettings.Development.json (dev overrides)
// Seamlessly merged by ASP.NET Core
```

### 9. **Consistency Across the Application** ✅
- All configuration follows the same pattern
- Easier for team members to understand
- Reduced bugs from typos in keys

### 10. **Easy Testing** ✅
```csharp
// Easy to mock in tests
var mockOptions = Options.Create(new ApiOptions { BaseUrl = "http://test" });
var service = new CompanyService(mockOptions);
```

---

## Current State Analysis {#current-state}

### ✅ What's Already Good
Your `ObservabilityOptions` class is an **excellent example** of the Options Pattern:

```csharp
public class ObservabilityOptions
{
	public const string SectionName = "Observability";
	public bool Enabled { get; set; } = true;
	public string ServiceName { get; set; } = "MyStartUpCompany";
	// ... more properties
}
```

And it's used in `appsettings.json`:
```json
{
  "Observability": {
	"Enabled": true,
	"ServiceName": "MyStartUpCompany.Api",
	// ...
  }
}
```

### 📋 Other Configuration Areas in Your API

Looking at `Program.cs` and `appsettings.json`, you have several areas that could benefit from the Options Pattern:

1. **Observability** ✅ Already implemented (good pattern to follow!)
2. **Database** - Currently read directly from configuration
3. **API Settings** - No centralized configuration class
4. **CORS Settings** - Not visible in current config
5. **Authentication/Authorization** - Not visible in current config
6. **Feature Flags** - Not implemented
7. **Cache Settings** - Not implemented
8. **Rate Limiting** - Not implemented

---

## Step-by-Step Implementation {#implementation}

I'll show you how to implement the Options Pattern for several areas of your API.

### STEP 1: Create Options Classes

Create a new folder structure for options:

```
src/MyStartUpCompany.Api/
├── Configuration/
│   ├── ApiOptions.cs
│   ├── DatabaseOptions.cs
│   ├── CacheOptions.cs
│   └── FeatureFlags.cs
```

#### Step 1a: Create ApiOptions.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyStartUpCompany.Api.Configuration
{
	/// <summary>
	/// Configuration options for API-specific settings
	/// </summary>
	public class ApiOptions
	{
		/// <summary>
		/// Configuration section name in appsettings.json
		/// </summary>
		public const string SectionName = "Api";

		/// <summary>
		/// Base URL for the API (used in links, etc.)
		/// </summary>
		[Required(ErrorMessage = "BaseUrl is required")]
		[Url(ErrorMessage = "BaseUrl must be a valid URL")]
		public string BaseUrl { get; set; } = "https://api.mystartupcorp.com";

		/// <summary>
		/// API version
		/// </summary>
		[Required]
		public string Version { get; set; } = "v1";

		/// <summary>
		/// Maximum number of items returned in paginated responses
		/// </summary>
		[Range(1, 1000, ErrorMessage = "MaxPageSize must be between 1 and 1000")]
		public int MaxPageSize { get; set; } = 100;

		/// <summary>
		/// Default page size for paginated responses
		/// </summary>
		[Range(1, 1000, ErrorMessage = "DefaultPageSize must be between 1 and 1000")]
		public int DefaultPageSize { get; set; } = 20;

		/// <summary>
		/// Enable detailed error responses
		/// </summary>
		public bool IncludeExceptionDetails { get; set; } = false;

		/// <summary>
		/// Request timeout in seconds
		/// </summary>
		[Range(1, 300, ErrorMessage = "RequestTimeoutSeconds must be between 1 and 300")]
		public int RequestTimeoutSeconds { get; set; } = 30;

		/// <summary>
		/// Enable API documentation
		/// </summary>
		public bool EnableApiDocumentation { get; set; } = true;
	}
}
```

#### Step 1b: Create DatabaseOptions.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyStartUpCompany.Api.Configuration
{
	/// <summary>
	/// Configuration options for database connectivity
	/// </summary>
	public class DatabaseOptions
	{
		/// <summary>
		/// Configuration section name in appsettings.json
		/// </summary>
		public const string SectionName = "Database";

		/// <summary>
		/// Database connection string
		/// </summary>
		[Required(ErrorMessage = "ConnectionString is required")]
		public string ConnectionString { get; set; } = "Server=localhost;Database=MyStartUpCompany;";

		/// <summary>
		/// Database provider type (SqlServer, PostgreSQL, Sqlite, etc.)
		/// </summary>
		[Required(ErrorMessage = "Provider is required")]
		public string Provider { get; set; } = "SqlServer";

		/// <summary>
		/// Enable detailed SQL logging
		/// </summary>
		public bool EnableDetailedLogging { get; set; } = false;

		/// <summary>
		/// Command timeout in seconds
		/// </summary>
		[Range(1, 300, ErrorMessage = "CommandTimeoutSeconds must be between 1 and 300")]
		public int CommandTimeoutSeconds { get; set; } = 30;

		/// <summary>
		/// Enable query caching
		/// </summary>
		public bool EnableQueryCache { get; set; } = true;

		/// <summary>
		/// Connection pool size
		/// </summary>
		[Range(1, 100, ErrorMessage = "ConnectionPoolSize must be between 1 and 100")]
		public int ConnectionPoolSize { get; set; } = 20;

		/// <summary>
		/// Auto-retry configuration
		/// </summary>
		public RetryPolicy RetryPolicy { get; set; } = new();

		/// <summary>
		/// Nested retry policy configuration
		/// </summary>
		public class RetryPolicy
		{
			/// <summary>
			/// Enable automatic retries on transient failures
			/// </summary>
			public bool Enabled { get; set; } = true;

			/// <summary>
			/// Maximum number of retry attempts
			/// </summary>
			[Range(0, 5, ErrorMessage = "MaxRetries must be between 0 and 5")]
			public int MaxRetries { get; set; } = 3;

			/// <summary>
			/// Delay between retries in milliseconds
			/// </summary>
			[Range(100, 5000, ErrorMessage = "DelayMilliseconds must be between 100 and 5000")]
			public int DelayMilliseconds { get; set; } = 1000;
		}
	}
}
```

#### Step 1c: Create CacheOptions.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyStartUpCompany.Api.Configuration
{
	/// <summary>
	/// Configuration options for caching
	/// </summary>
	public class CacheOptions
	{
		/// <summary>
		/// Configuration section name in appsettings.json
		/// </summary>
		public const string SectionName = "Cache";

		/// <summary>
		/// Enable caching
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// Cache provider type (Memory, Redis, Distributed)
		/// </summary>
		[Required]
		public string Provider { get; set; } = "Memory";

		/// <summary>
		/// Default cache duration in seconds
		/// </summary>
		[Range(0, 3600, ErrorMessage = "DefaultDurationSeconds must be between 0 and 3600")]
		public int DefaultDurationSeconds { get; set; } = 300;

		/// <summary>
		/// Redis connection string (used if Provider is Redis)
		/// </summary>
		public string? RedisConnectionString { get; set; }

		/// <summary>
		/// Cache key prefix
		/// </summary>
		public string KeyPrefix { get; set; } = "api:";

		/// <summary>
		/// Maximum cache size in MB (for memory cache)
		/// </summary>
		[Range(1, 1000, ErrorMessage = "MaxSizeInMegabytes must be between 1 and 1000")]
		public int MaxSizeInMegabytes { get; set; } = 100;
	}
}
```

#### Step 1d: Create FeatureFlags.cs

```csharp
namespace MyStartUpCompany.Api.Configuration
{
	/// <summary>
	/// Feature flags for gradual feature rollout
	/// </summary>
	public class FeatureFlags
	{
		/// <summary>
		/// Configuration section name in appsettings.json
		/// </summary>
		public const string SectionName = "FeatureFlags";

		/// <summary>
		/// Enable advanced search features
		/// </summary>
		public bool EnableAdvancedSearch { get; set; } = false;

		/// <summary>
		/// Enable export functionality
		/// </summary>
		public bool EnableExport { get; set; } = false;

		/// <summary>
		/// Enable batch operations
		/// </summary>
		public bool EnableBatchOperations { get; set; } = false;

		/// <summary>
		/// Enable real-time notifications
		/// </summary>
		public bool EnableRealTimeNotifications { get; set; } = false;

		/// <summary>
		/// Enable rate limiting
		/// </summary>
		public bool EnableRateLimiting { get; set; } = false;

		/// <summary>
		/// Rate limit requests per minute per user
		/// </summary>
		public int RateLimitRequestsPerMinute { get; set; } = 100;
	}
}
```

---

### STEP 2: Update appsettings.json

Add the new configuration sections:

```json
{
  "Observability": {
	// ... existing config
  },
  "Logging": {
	// ... existing config
  },
  "Api": {
	"BaseUrl": "https://api.mystartupcorp.com",
	"Version": "v1",
	"MaxPageSize": 100,
	"DefaultPageSize": 20,
	"IncludeExceptionDetails": false,
	"RequestTimeoutSeconds": 30,
	"EnableApiDocumentation": true
  },
  "Database": {
	"ConnectionString": "Server=sql-server;Database=MyStartUpCompany;Trusted_Connection=true;",
	"Provider": "SqlServer",
	"EnableDetailedLogging": false,
	"CommandTimeoutSeconds": 30,
	"EnableQueryCache": true,
	"ConnectionPoolSize": 20,
	"RetryPolicy": {
	  "Enabled": true,
	  "MaxRetries": 3,
	  "DelayMilliseconds": 1000
	}
  },
  "Cache": {
	"Enabled": true,
	"Provider": "Memory",
	"DefaultDurationSeconds": 300,
	"KeyPrefix": "api:",
	"MaxSizeInMegabytes": 100
  },
  "FeatureFlags": {
	"EnableAdvancedSearch": false,
	"EnableExport": false,
	"EnableBatchOperations": false,
	"EnableRealTimeNotifications": false,
	"EnableRateLimiting": false,
	"RateLimitRequestsPerMinute": 100
  }
}
```

---

### STEP 3: Create an Options Configuration Extension

Create `src/MyStartUpCompany.Api/Extensions/OptionsExtensions.cs`:

```csharp
using MyStartUpCompany.Api.Configuration;

namespace MyStartUpCompany.Api.Extensions
{
	/// <summary>
	/// Extension methods for configuring application options
	/// </summary>
	public static class OptionsExtensions
	{
		/// <summary>
		/// Register all application configuration options with validation
		/// </summary>
		/// <param name="services">Service collection</param>
		/// <param name="configuration">Application configuration</param>
		/// <returns>Service collection for method chaining</returns>
		public static IServiceCollection AddApplicationOptions(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			// Register ApiOptions
			services.AddOptions<ApiOptions>()
				.BindConfiguration(ApiOptions.SectionName)
				.ValidateDataAnnotations()
				.ValidateOnStart();  // ← Validates at startup, catches errors early

			// Register DatabaseOptions
			services.AddOptions<DatabaseOptions>()
				.BindConfiguration(DatabaseOptions.SectionName)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			// Register CacheOptions
			services.AddOptions<CacheOptions>()
				.BindConfiguration(CacheOptions.SectionName)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			// Register FeatureFlags
			services.AddOptions<FeatureFlags>()
				.BindConfiguration(FeatureFlags.SectionName)
				.ValidateDataAnnotations();

			return services;
		}
	}
}
```

---

### STEP 4: Register Options in Program.cs

Update `Program.cs` to use the new options:

```csharp
using FluentValidation;
using MyStartUpCompany.Api.Extensions;
using MyStartUpCompany.Api.Features.Buildings.Queries;
// ... other usings

public partial class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddControllers(options =>
		{
			options.Filters.Add<FluentValidationFilter>();
			options.ModelBinderProviders.Insert(0, new PaginationModelBinderProvider());
		});

		// ✅ NEW: Register application configuration options
		builder.Services.AddApplicationOptions(builder.Configuration);

		// Add OpenTelemetry observability
		builder.Services.AddObservability(builder.Configuration, builder.Environment);
		builder.Services.AddHttpContextAccessor();
		builder.Services.AddScoped<CorrelationIdAccessor>();

		// ... rest of configuration
	}
}
```

---

### STEP 5: Create a Service that Uses Options

Example: `src/MyStartUpCompany.Api/Services/CompanyService.cs`

```csharp
using MyStartUpCompany.Api.Configuration;
using Microsoft.Extensions.Options;

namespace MyStartUpCompany.Api.Services
{
	/// <summary>
	/// Service for company operations that uses typed options
	/// </summary>
	public class CompanyService
	{
		private readonly IOptions<ApiOptions> _apiOptions;
		private readonly IOptionsMonitor<FeatureFlags> _featureFlags;
		private readonly ILogger<CompanyService> _logger;

		public CompanyService(
			IOptions<ApiOptions> apiOptions,
			IOptionsMonitor<FeatureFlags> featureFlags,
			ILogger<CompanyService> logger)
		{
			_apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));
			_featureFlags = featureFlags ?? throw new ArgumentNullException(nameof(featureFlags));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Get paginated companies with defaults from options
		/// </summary>
		public async Task<PagedResult<Company>> GetCompaniesAsync(
			int? pageNumber = null,
			int? pageSize = null)
		{
			var options = _apiOptions.Value;

			// Use configured defaults
			var actualPageNumber = pageNumber ?? 1;
			var actualPageSize = Math.Min(pageSize ?? options.DefaultPageSize, options.MaxPageSize);

			_logger.LogInformation(
				"Fetching companies: Page={PageNumber}, Size={PageSize}",
				actualPageNumber, actualPageSize);

			// Get data...
			return new PagedResult<Company> { /* ... */ };
		}

		/// <summary>
		/// Export companies if feature is enabled
		/// </summary>
		public async Task<Stream?> ExportCompaniesAsync(CancellationToken cancellationToken)
		{
			// ✅ Check feature flag dynamically with IOptionsMonitor
			if (!_featureFlags.CurrentValue.EnableExport)
			{
				_logger.LogWarning("Export feature is disabled");
				return null;
			}

			// Export logic...
			return null;
		}
	}
}
```

---

### STEP 6: Update appsettings.Development.json for Development Environment

```json
{
  "Observability": {
	// ... existing config
  },
  "Api": {
	"BaseUrl": "https://localhost:7001",
	"Version": "v1",
	"MaxPageSize": 100,
	"DefaultPageSize": 10,
	"IncludeExceptionDetails": true,  // ← Show errors in dev
	"RequestTimeoutSeconds": 60,
	"EnableApiDocumentation": true
  },
  "Database": {
	"ConnectionString": "Server=localhost;Database=MyStartUpCompany_Dev;Trusted_Connection=true;",
	"Provider": "SqlServer",
	"EnableDetailedLogging": true,  // ← Verbose logging in dev
	"CommandTimeoutSeconds": 60,
	"EnableQueryCache": true,
	"ConnectionPoolSize": 5,
	"RetryPolicy": {
	  "Enabled": true,
	  "MaxRetries": 3,
	  "DelayMilliseconds": 500
	}
  },
  "Cache": {
	"Enabled": true,
	"Provider": "Memory",
	"DefaultDurationSeconds": 60,  // ← Shorter cache in dev
	"KeyPrefix": "api_dev:",
	"MaxSizeInMegabytes": 50
  },
  "FeatureFlags": {
	"EnableAdvancedSearch": true,  // ← Enable in dev for testing
	"EnableExport": true,
	"EnableBatchOperations": true,
	"EnableRealTimeNotifications": true,
	"EnableRateLimiting": false,  // ← Disabled in dev
	"RateLimitRequestsPerMinute": 1000
  }
}
```

---

## Best Practices {#best-practices}

### 1. **Always Use IOptions<T> for Immutable Configuration**
```csharp
// ✅ GOOD - IOptions for read-only settings
public MyService(IOptions<ApiOptions> options)
{
	var baseUrl = options.Value.BaseUrl;
}

// ✅ ALSO GOOD - IOptionsMonitor for dynamic settings
public MyService(IOptionsMonitor<FeatureFlags> flags)
{
	var enabled = flags.CurrentValue.EnableExport;
}

// ❌ BAD - Direct configuration access (defeats the purpose)
public MyService(IConfiguration configuration)
{
	var baseUrl = configuration["Api:BaseUrl"];  // No type safety!
}
```

### 2. **Always Validate Options at Startup**
```csharp
// ✅ GOOD - Catch errors immediately
services.AddOptions<ApiOptions>()
	.BindConfiguration(ApiOptions.SectionName)
	.ValidateDataAnnotations()
	.ValidateOnStart();  // ← Don't forget this!

// ❌ BAD - Errors only discovered when option is accessed
services.AddOptions<ApiOptions>()
	.BindConfiguration(ApiOptions.SectionName);
```

### 3. **Use Data Annotations for Validation**
```csharp
public class ApiOptions
{
	[Required]
	[Url]
	public string BaseUrl { get; set; }

	[Range(1, 1000)]
	public int MaxPageSize { get; set; }

	[Range(0, double.MaxValue)]
	public double TimeoutSeconds { get; set; }
}
```

### 4. **Provide Meaningful Defaults**
```csharp
public class ApiOptions
{
	// ✅ Good - sensible defaults
	public string BaseUrl { get; set; } = "https://api.example.com";
	public int MaxPageSize { get; set; } = 100;
}
```

### 5. **Group Related Settings**
```csharp
// ✅ Good - nested options for related settings
public class DatabaseOptions
{
	public string ConnectionString { get; set; }
	public RetryPolicy RetryPolicy { get; set; }

	public class RetryPolicy
	{
		public int MaxRetries { get; set; }
		public int DelayMilliseconds { get; set; }
	}
}
```

### 6. **Document Your Options**
```csharp
/// <summary>
/// Configuration for database retry behavior
/// </summary>
public class RetryPolicy
{
	/// <summary>
	/// Maximum number of retry attempts (0-5)
	/// </summary>
	public int MaxRetries { get; set; } = 3;
}
```

### 7. **Use Constants for Section Names**
```csharp
// ✅ Good - consistent, refactorable
public class ApiOptions
{
	public const string SectionName = "Api";
}

services.AddOptions<ApiOptions>()
	.BindConfiguration(ApiOptions.SectionName);

// ❌ Bad - magic strings
services.AddOptions<ApiOptions>()
	.BindConfiguration("Api");  // What if you rename it?
```

### 8. **Test Your Configuration**
```csharp
[Fact]
public void ApiOptions_WithValidSettings_ShouldLoad()
{
	var config = new ConfigurationBuilder()
		.AddInMemoryCollection(new Dictionary<string, string>
		{
			{ "Api:BaseUrl", "https://test.com" },
			{ "Api:MaxPageSize", "50" }
		})
		.Build();

	var options = config.GetSection(ApiOptions.SectionName)
		.Get<ApiOptions>();

	Assert.NotNull(options);
	Assert.Equal("https://test.com", options.BaseUrl);
	Assert.Equal(50, options.MaxPageSize);
}
```

### 9. **Use Named Options for Multiple Instances**
```csharp
// For when you need multiple configurations of the same type
services.Configure<CacheOptions>("primary", options =>
{
	options.Provider = "Redis";
});

services.Configure<CacheOptions>("secondary", options =>
{
	options.Provider = "Memory";
});

// Then inject with IOptionsSnapshot<T>
public class CacheService
{
	private readonly IOptionsSnapshot<CacheOptions> _options;

	public CacheService(IOptionsSnapshot<CacheOptions> options)
	{
		var primary = options.Get("primary");
		var secondary = options.Get("secondary");
	}
}
```

### 10. **Use IOptionsChangeTokenSource for Custom Reloading**
```csharp
// For complex scenarios where you need custom reload logic
public class CustomReloadingService
{
	private readonly IOptionsMonitor<ApiOptions> _options;

	public CustomReloadingService(
		IOptionsMonitor<ApiOptions> options,
		IHostApplicationLifetime lifetime)
	{
		_options = options;

		// Listen for changes
		_options.OnChange((newOptions, name) =>
		{
			Console.WriteLine($"Options changed: {name}");
		});
	}
}
```

---

## Troubleshooting {#troubleshooting}

### Issue 1: "OptionsValidationException: 'ApiOptions' failed one or more validation checks"

**Problem:** Configuration validation failed at startup.

**Solution:**
```csharp
// Check your appsettings.json for missing or invalid values
"Api": {
	"BaseUrl": "https://api.example.com",  // ← Must be valid URL
	"MaxPageSize": 100  // ← Must be within Range(1, 1000)
}
```

### Issue 2: Options are always `null`

**Problem:** Options are not being injected.

**Solution:**
```csharp
// Make sure you registered them:
services.AddOptions<ApiOptions>()
	.BindConfiguration(ApiOptions.SectionName)
	.ValidateDataAnnotations();

// And injected them:
public MyService(IOptions<ApiOptions> options)  // ✅ IOptions<T>
{
	var value = options.Value;  // ✅ Access through .Value
}
```

### Issue 3: Configuration changes not reflected

**Problem:** You changed appsettings.json but service still uses old values.

**Solution:**
```csharp
// Use IOptionsMonitor<T> instead of IOptions<T>
public MyService(IOptionsMonitor<FeatureFlags> flags)
{
	var current = flags.CurrentValue;  // ← Always gets current value
}

// Not this:
public MyService(IOptions<FeatureFlags> flags)
{
	var current = flags.Value;  // ← Cached value, doesn't reload
}
```

### Issue 4: Environment-specific settings not loading

**Problem:** appsettings.Development.json is not overriding production values.

**Solution:**
```powershell
# Make sure ASPNETCORE_ENVIRONMENT is set
$env:ASPNETCORE_ENVIRONMENT = "Development"

# Or in launchSettings.json
{
  "profiles": {
	"MyStartUpCompany.Api": {
	  "environmentVariables": {
		"ASPNETCORE_ENVIRONMENT": "Development"
	  }
	}
  }
}
```

### Issue 5: Complex nested configuration not binding

**Problem:** Nested settings like `RetryPolicy` are not binding properly.

**Solution:**
```csharp
// Make sure class has public parameterless constructor
public class RetryPolicy
{
	public RetryPolicy() { }  // ← Required for binding

	public int MaxRetries { get; set; }
}

// And JSON matches the structure exactly
"Database": {
	"RetryPolicy": {
		"MaxRetries": 3
	}
}
```

---

## Summary

The Options Pattern provides:

| Benefit | Impact |
|---------|--------|
| **Type Safety** | Compile-time checking, IntelliSense |
| **Validation** | Errors at startup, not runtime |
| **Testability** | Easy to mock options in tests |
| **Performance** | Options cached after validation |
| **Maintainability** | Centralized, self-documenting |
| **Flexibility** | Environment-specific configs |
| **Scalability** | Handles complex hierarchies well |

**Next Steps:**
1. Create the options classes (Step 1)
2. Update appsettings.json (Step 2)
3. Create the extension method (Step 3)
4. Update Program.cs (Step 4)
5. Update services to use IOptions<T> (Step 5)
6. Test everything

---

## Reference Implementation Checklist

- [ ] Created `Configuration/` folder
- [ ] Created `ApiOptions.cs`
- [ ] Created `DatabaseOptions.cs`
- [ ] Created `CacheOptions.cs`
- [ ] Created `FeatureFlags.cs`
- [ ] Updated `appsettings.json` with new sections
- [ ] Created `Extensions/OptionsExtensions.cs`
- [ ] Updated `Program.cs` to call `AddApplicationOptions()`
- [ ] Created example service using `IOptions<T>`
- [ ] Updated `appsettings.Development.json`
- [ ] Ran application to verify startup validation
- [ ] Updated existing services to use options
- [ ] Added unit tests for options
- [ ] Documented options in README

*Follow this guide to implement the Options Pattern in your API for production-grade configuration management.*
