# FluentValidation Quick Reference

## Files Modified/Created

| File | Type | Purpose |
|------|------|---------|
| `src\MyStartUpCompany.Api\MyStartUpCompany.Api.csproj` | Modified | Added FluentValidation.AspNetCore NuGet package |
| `src\MyStartUpCompany.Api\Features\CompanyDetails\Validators\CompanyRequestValidator.cs` | Created | Validation rules for CompanyRequest |
| `src\MyStartUpCompany.Api\Shared\Filters\FluentValidationFilter.cs` | Created | Action filter for automatic validation |
| `src\MyStartUpCompany.Api\Program.cs` | Modified | Registered validators and validation filter |

## Key Validation Rules

### Pagination
- `PageNumber`: 1-10,000
- `PageSize`: 1-100

### Location Fields (Optional)
- `Region`, `Country`, `City`: Max 100 chars, pattern `[a-zA-Z\s\-]+`
- `PostalCode`: Max 20 chars, pattern `[a-zA-Z0-9\s\-]+`

### Search
- `SearchTerm`: Max 100 chars

## How It Works

1. **Request Arrives** → Request parameters bound to `CompanyRequest`
2. **Filter Executes** → `FluentValidationFilter` validates the model
3. **Validation Fails** → Throws `ValidationException` with error details
4. **Exception Handled** → `GlobalExceptionHandler` converts to HTTP 422 response
5. **Error Response** → Client receives detailed validation errors in JSON

## Example Valid Request

```
GET /api/company/search?region=California&city=San%20Francisco&pageNumber=1&pageSize=10
```

## Example Invalid Request

```
GET /api/company/search?pageSize=150&region=California@#$
```

**Response (422 Unprocessable Entity):**
```json
{
  "status": 422,
  "title": "Validation Error",
  "errors": {
	"PageSize": ["Page size cannot exceed 100."],
	"Region": ["Region can only contain letters, spaces, and hyphens."]
  }
}
```

## Adding New Validators

Create a new class in `Features/{Feature}/Validators/`:

```csharp
using FluentValidation;

namespace MyStartUpCompany.Api.Features.YourFeature.Validators;

public class YourRequestValidator : AbstractValidator<YourRequest>
{
	public YourRequestValidator()
	{
		RuleFor(x => x.PropertyName)
			.NotEmpty().WithMessage("Property is required.")
			.MaximumLength(50).WithMessage("Max 50 characters.");
	}
}
```

The filter will automatically discover and apply it! ✨

## Testing Validation

To test validation in integration tests:

```csharp
var response = await _client.GetAsync("/api/company/search?pageSize=150");

Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
var content = await response.Content.ReadAsStringAsync();
Assert.Contains("Page size cannot exceed 100", content);
```

## Benefits

✅ Industry-standard validation library
✅ Declarative, easy-to-read rules
✅ Centralized validation logic
✅ Automatic error formatting
✅ Reusable across application layers
✅ Excellent documentation & community support

---

For detailed information, see: `docs/FLUENT_VALIDATION.md`
