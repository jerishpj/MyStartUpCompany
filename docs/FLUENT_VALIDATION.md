# FluentValidation Implementation Guide

## Overview

This document outlines the FluentValidation implementation for the MyStartUpCompany.Api project, providing industry-standard input validation for API request models.

## Implementation Details

### Components

#### 1. **CompanyRequestValidator** (`Features/CompanyDetails/Validators/CompanyRequestValidator.cs`)

The main validator class that defines all validation rules for the `CompanyRequest` query model used in the `/api/company/search` endpoint.

**Validation Rules:**

| Field | Rules | Constraints |
|-------|-------|-------------|
| **PageNumber** | Required | Min: 1, Max: 10,000 |
| **PageSize** | Required | Min: 1, Max: 100 |
| **Region** | Optional | Max Length: 100 chars, Pattern: `[a-zA-Z\s\-]+` |
| **Country** | Optional | Max Length: 100 chars, Pattern: `[a-zA-Z\s\-]+` |
| **City** | Optional | Max Length: 100 chars, Pattern: `[a-zA-Z\s\-]+` |
| **PostalCode** | Optional | Max Length: 20 chars, Pattern: `[a-zA-Z0-9\s\-]+` |
| **SearchTerm** | Optional | Max Length: 100 chars |

**Key Features:**
- Uses `DependentRules()` for conditional validation logic
- Validates optional fields only when they contain non-whitespace values
- Uses regex patterns to ensure data integrity (e.g., no SQL injection characters in location fields)
- Clear, descriptive error messages for each validation failure

#### 2. **FluentValidationFilter** (`Shared/Filters/FluentValidationFilter.cs`)

A custom ASP.NET Core action filter that automatically validates incoming request models using registered FluentValidation validators.

**Functionality:**
- Intercepts all controller actions before execution
- Validates model state from ASP.NET Core's ModelState
- Applies FluentValidation validators to request models
- Throws `ValidationException` if validation fails
- Exceptions are automatically handled by `GlobalExceptionHandler` and converted to ProblemDetails responses

#### 3. **Program.cs Configuration**

The composition root registers all validators and the validation filter:

```csharp
// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<CompanyRequestValidator>();
builder.Services.AddScoped<FluentValidationFilter>();

// Register the filter globally
builder.Services.AddControllers(options =>
{
	options.Filters.Add<FluentValidationFilter>();
});
```

#### 4. **Integration with Exception Handling**

The existing `GlobalExceptionHandler` in `Shared/Exceptions/GlobalExceptionHandler.cs` handles `ValidationException` and returns a proper HTTP 422 (Unprocessable Entity) response with detailed error information:

```json
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "Validation Error",
  "status": 422,
  "detail": "One or more validation failures have occurred.",
  "instance": "/api/company/search",
  "traceId": "0HMVII9NFA8TG:00000001",
  "timestamp": "2024-01-15T10:30:45.123Z",
  "errors": {
	"PageSize": ["Page size cannot exceed 100."],
	"Region": ["Region can only contain letters, spaces, and hyphens."]
  }
}
```

## Usage Examples

### Valid Requests

```http
GET /api/company/search?region=California&pageNumber=1&pageSize=10
GET /api/company/search?country=United%20States&city=San%20Francisco&searchTerm=tech
GET /api/company/search?pageNumber=2&pageSize=20
```

### Invalid Requests

```http
// Page size exceeds maximum (100)
GET /api/company/search?pageSize=150
// Response: 422 Unprocessable Entity with error details

// Invalid region format (contains special characters)
GET /api/company/search?region=California@#$
// Response: 422 Unprocessable Entity

// Page number is zero (must be >= 1)
GET /api/company/search?pageNumber=0
// Response: 422 Unprocessable Entity
```

## Architecture Benefits

1. **Separation of Concerns**: Validation logic is isolated in dedicated validator classes
2. **Reusability**: Validators can be used in multiple contexts (API, background services, etc.)
3. **Maintainability**: Centralized validation rules make updates easy
4. **Testability**: Validators can be unit tested independently
5. **Industry Standard**: FluentValidation is the most popular .NET validation library
6. **Declarative Syntax**: Fluent API makes validation rules easy to understand
7. **Comprehensive Error Handling**: Integration with existing exception handling pipeline

## Adding New Validations

To add validations for new request models:

1. Create a new validator class inheriting from `AbstractValidator<T>`
2. Define rules in the constructor using the fluent API
3. Place it in the `Features/{Feature}/Validators` directory
4. The `FluentValidationFilter` will automatically discover and apply it

Example:
```csharp
public class MyNewRequestValidator : AbstractValidator<MyNewRequest>
{
	public MyNewRequestValidator()
	{
		RuleFor(x => x.SomeField)
			.NotEmpty()
			.MaximumLength(100);
	}
}
```

## NuGet Package

- **FluentValidation.AspNetCore** v11.3.0 - Provides FluentValidation integration with ASP.NET Core

## Performance Considerations

- Validators are registered as scoped services and instantiated once per request
- Validation runs early in the request pipeline, before handlers are invoked
- Regex patterns are compiled by .NET's regex engine for optimal performance
- Optional field validation short-circuits on null/whitespace values

## References

- [FluentValidation Official Documentation](https://docs.fluentvalidation.net/)
- [ASP.NET Core Model Validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)
