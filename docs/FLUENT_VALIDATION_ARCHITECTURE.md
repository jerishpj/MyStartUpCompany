# FluentValidation Architecture & Design

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     HTTP Request                            │
│          GET /api/company/search?pageSize=150               │
└────────────────────────┬────────────────────────────────────┘
						 │
						 ▼
┌─────────────────────────────────────────────────────────────┐
│            ASP.NET Core Model Binding                       │
│         Binds query string to CompanyRequest                │
└────────────────────────┬────────────────────────────────────┘
						 │
						 ▼
┌─────────────────────────────────────────────────────────────┐
│           FluentValidationFilter (Action Filter)            │
│  • Inspects ModelState                                      │
│  • Resolves CompanyRequestValidator from DI                │
│  • Executes validation rules                                │
│  • Throws ValidationException if invalid                    │
└────────────────────────┬────────────────────────────────────┘
						 │
		 ┌───────────────┴───────────────┐
		 │                               │
		 ▼ Valid                         ▼ Invalid
	[Continue]                    [ValidationException]
		 │                               │
		 │                               ▼
		 │                    ┌──────────────────────────┐
		 │                    │ GlobalExceptionHandler   │
		 │                    │ • Catches exception      │
		 │                    │ • Formats ProblemDetails │
		 │                    │ • Returns HTTP 422       │
		 │                    └──────────────────────────┘
		 │                               │
		 ▼                               ▼
	[Controller]               [JSON Error Response]
	[Handler]                  {
								 "status": 422,
								 "title": "Validation Error",
								 "errors": {...}
							   }
```

## Component Interactions

### 1. Request Flow

```csharp
// Request comes in
GET /api/company/search?pageSize=150&region=California@#$

// Model Binder creates CompanyRequest
var request = new CompanyRequest 
{ 
	PageSize = 150,
	Region = "California@#$",
	PageNumber = 1,
	// ... other defaults
};

// FluentValidationFilter intercepts
// It finds CompanyRequestValidator in DI container
var validator = serviceProvider.GetService<IValidator<CompanyRequest>>();

// Validates the request
var result = await validator.ValidateAsync(request);

// Returns errors
{
	PageSize: ["Page size cannot exceed 100."],
	Region: ["Region can only contain letters, spaces, and hyphens."]
}

// Throws ValidationException(errors)
// Caught by GlobalExceptionHandler
// Returns HTTP 422 with formatted error
```

### 2. Dependency Resolution

```
Program.cs Startup
	↓
builder.Services.AddValidatorsFromAssemblyContaining<CompanyRequestValidator>()
	↓
Scans assembly for IValidator<T> implementations
	↓
Discovers: CompanyRequestValidator : AbstractValidator<CompanyRequest>
	↓
Registers: IValidator<CompanyRequest> → CompanyRequestValidator
	↓
builder.Services.AddScoped<FluentValidationFilter>()
	↓
Registers: FluentValidationFilter (with IServiceProvider dependency)
	↓
Registers filter globally in AddControllers(options => options.Filters.Add<FluentValidationFilter>())
```

### 3. Validation Rule Chain

```
CompanyRequestValidator.cs
	├── PageNumber validation
	│   ├── GreaterThanOrEqualTo(1)
	│   └── LessThanOrEqualTo(10000)
	│
	├── PageSize validation
	│   ├── GreaterThanOrEqualTo(1)
	│   └── LessThanOrEqualTo(100)
	│
	├── Region validation (conditional)
	│   ├── MaximumLength(100)
	│   ├── When(!string.IsNullOrWhiteSpace(x.Region))
	│   └── Matches(regex) for pattern validation
	│
	├── Country validation (conditional)
	│   ├── MaximumLength(100)
	│   ├── When(!string.IsNullOrWhiteSpace(x.Country))
	│   └── Matches(regex) for pattern validation
	│
	├── City validation (conditional)
	│   ├── MaximumLength(100)
	│   ├── When(!string.IsNullOrWhiteSpace(x.City))
	│   └── Matches(regex) for pattern validation
	│
	├── PostalCode validation (conditional)
	│   ├── MaximumLength(20)
	│   ├── When(!string.IsNullOrWhiteSpace(x.PostalCode))
	│   └── Matches(regex) for pattern validation
	│
	└── SearchTerm validation (conditional)
		├── MaximumLength(100)
		└── When(!string.IsNullOrWhiteSpace(x.SearchTerm))
```

## Data Flow Example

### Valid Request

```
Request: GET /api/company/search?region=California&city=San%20Francisco
		 ↓
Binding: CompanyRequest {
		   Region = "California",
		   City = "San Francisco",
		   PageNumber = 1,
		   PageSize = 10
		 }
		 ↓
Validation: ✓ All rules pass
		 ↓
Controller: GetFilteredCompanies() executes
		 ↓
Response: HTTP 200 OK with paginated results
```

### Invalid Request

```
Request: GET /api/company/search?pageSize=150&region=California@#$
		 ↓
Binding: CompanyRequest {
		   PageSize = 150,
		   Region = "California@#$",
		   PageNumber = 1
		 }
		 ↓
Validation: ✗ Two failures detected:
			- PageSize > 100
			- Region contains invalid characters
		 ↓
Filter: Creates ValidationException with error dictionary:
		{
		  "PageSize": ["Page size cannot exceed 100."],
		  "Region": ["Region can only contain letters, spaces, and hyphens."]
		}
		 ↓
Handler: Catches ValidationException
		 ↓
Response: HTTP 422 Unprocessable Entity
		 {
		   "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
		   "title": "Validation Error",
		   "status": 422,
		   "errors": {
			 "PageSize": ["Page size cannot exceed 100."],
			 "Region": ["Region can only contain letters, spaces, and hyphens."]
		   }
		 }
```

## Class Structure

### CompanyRequestValidator

```csharp
public class CompanyRequestValidator : AbstractValidator<CompanyRequest>
{
	private const int MinPageNumber = 1;
	private const int MaxPageNumber = 10000;
	private const int MinPageSize = 1;
	private const int MaxPageSize = 100;
	private const int MaxStringLength = 100;

	public CompanyRequestValidator()
	{
		// Pagination rules
		RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(MinPageNumber)...
		RuleFor(x => x.PageSize).GreaterThanOrEqualTo(MinPageSize)...

		// Optional filter rules with conditional validation
		RuleFor(x => x.Region)
			.MaximumLength(MaxStringLength)
			.When(x => !string.IsNullOrWhiteSpace(x.Region))
			.DependentRules(() => {
				RuleFor(x => x.Region)
					.Matches(regex)
					.When(x => !string.IsNullOrWhiteSpace(x.Region))
			});

		// ... similar for Country, City, PostalCode, SearchTerm
	}
}
```

### FluentValidationFilter

```csharp
public class FluentValidationFilter : IAsyncActionFilter
{
	private readonly IServiceProvider _serviceProvider;

	public async Task OnActionExecutionAsync(
		ActionExecutingContext context,
		ActionExecutionDelegate next)
	{
		// 1. Check ASP.NET Core ModelState
		if (!context.ModelState.IsValid)
		{
			throw new ValidationException(errors);
		}

		// 2. Find and apply FluentValidation validators
		foreach (var (key, value) in context.ActionArguments)
		{
			var validatorType = typeof(IValidator<>).MakeGenericType(valueType);

			if (context.HttpContext.RequestServices
				.GetService(validatorType) is IValidator validator)
			{
				var result = await validator.ValidateAsync(
					new ValidationContext<object>(value)
				);

				if (!result.IsValid)
				{
					throw new ValidationException(validationErrors);
				}
			}
		}

		// 3. Continue to controller action
		await next();
	}
}
```

## Integration Points

```
┌──────────────────────────────────────────────────────────┐
│                    Program.cs                            │
│  Services registration and middleware configuration      │
└───────────┬──────────────────────────────────────────────┘
			│
	┌───────┴────────┬──────────────┐
	│                │              │
	▼                ▼              ▼
AddValidators   AddScoped      AddControllers
	│           Filter            │
	│               │             │
	▼               ▼             ▼
IValidator<T>   DI Container   Filters.Add()
	│               │             │
	│               └─────┬───────┘
	│                     │
	▼                     ▼
Discovered          Registered as
AutomatiGlobally
	│
	▼
Available for injection
into FluentValidationFilter
```

## Performance Considerations

1. **Validator Instantiation**: Scoped lifetime = one per request
2. **Rule Evaluation**: Sequential short-circuiting on first failure per field
3. **Regex Compilation**: .NET caches compiled patterns
4. **Early Exit**: Optional fields skip validation if null/whitespace
5. **DependentRules**: Group related conditions for efficiency

## Security Considerations

1. **Pattern Validation**: Prevents special characters in location fields
2. **Length Constraints**: Prevents buffer overflow attacks
3. **Type Safety**: Pagination numbers validated as integers
4. **Error Detail Level**: Returns specific field errors without exposing system internals

---

This architecture provides a clean, scalable, and maintainable validation solution that follows industry best practices.
