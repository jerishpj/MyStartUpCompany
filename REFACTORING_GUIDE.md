# MyStartUpCompany.Api - Naming Convention Refactoring Guide

## RECOMMENDATION: Adopt Microsoft/ASP.NET Core Standard

✅ **Use explicit `Response`/`Request` suffixes**

This aligns with:
- Microsoft official documentation
- Your existing Projects feature
- Industry best practices
- Enterprise .NET standards

---

## Current State Summary

### ✅ Already Correct (Do Not Change)
```
✓ ProjectResponse              - Correct naming
✓ CreateProjectRequest         - Correct naming
✓ ProjectFilterRequest         - Correct naming
✓ ProjectTypeReferenceDto      - Correct naming (reference data)
```

### ❌ Needs Standardization
```
✗ Company               → Should be CompanyResponse
✗ CompanyRequest        → Should be SearchCompanyRequest
✗ Location              → Should be LocationResponse
✗ Building              → Should be BuildingResponse
✗ Office                → Should be OfficeResponse
```

---

## Feature-by-Feature Refactoring Plan

### FEATURE 1: Company Details

#### Current Files
```
src/MyStartUpCompany.Api/Features/CompanyDetails/Models/
├── Company.cs
└── CompanyRequest.cs
```

#### Refactoring

**Step 1.1: Rename Company.cs → CompanyResponse.cs**

**Current:**
```csharp
// Company.cs
namespace MyStartUpCompany.Api.Features.CompanyDetails.Models;

[DisplayName("Company")]
public record Company
{
	public int Id { get; init; }
	public required string Name { get; init; }
	// ... properties
}
```

**Recommended:**
```csharp
// CompanyResponse.cs
namespace MyStartUpCompany.Api.Features.CompanyDetails.Models;

/// <summary>
/// Response DTO for company details - returned from GET endpoints
/// </summary>
[DisplayName("Company")]  // Keep this for OpenAPI schema display
public record CompanyResponse
{
	public int Id { get; init; }
	public required string Name { get; init; }
	// ... properties
}
```

**Step 1.2: Rename CompanyRequest.cs → SearchCompanyRequest.cs**

**Current:**
```csharp
// CompanyRequest.cs
[DisplayName("CompanyRequest")]
public record CompanyRequest
{
	public string? Region { get; init; }
	public string? Country { get; init; }
	public int PageNumber { get; init; } = 1;
	public int PageSize { get; init; } = 10;
}
```

**Recommended:**
```csharp
// SearchCompanyRequest.cs
/// <summary>
/// Search/filter request DTO for company queries
/// Used by GET /api/companies/search endpoint
/// </summary>
[DisplayName("CompanySearch")]  // For OpenAPI schema
public record SearchCompanyRequest
{
	public string? Region { get; init; }
	public string? Country { get; init; }
	public int PageNumber { get; init; } = 1;
	public int PageSize { get; init; } = 10;
}
```

**Step 1.3: Update CompanyController.cs**

**Current:**
```csharp
public class CompanyController : ControllerBase
{
	// ...

	[HttpGet("{id:int}")]
	[ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetCompany(...)
	{
		var company = await _getCompanyHandler.HandleAsync(id, cancellationToken);
		return Ok(company);
	}

	[HttpGet("search")]
	[ProducesResponseType(typeof(PagedResult<Company>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetFilteredCompanies(
		[FromQuery] CompanyRequest request, ...)
	{
		var result = await _getFilteredCompaniesHandler.HandleAsync(request, ...);
		return Ok(result);
	}
}
```

**Recommended:**
```csharp
public class CompanyController : ControllerBase
{
	// ...

	[HttpGet("{id:int}")]
	[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]  // ✅ Updated
	public async Task<IActionResult> GetCompany(...)
	{
		var company = await _getCompanyHandler.HandleAsync(id, cancellationToken);
		return Ok(company);
	}

	[HttpGet("search")]
	[ProducesResponseType(typeof(PagedResult<CompanyResponse>), StatusCodes.Status200OK)]  // ✅ Updated
	public async Task<IActionResult> GetFilteredCompanies(
		[FromQuery] SearchCompanyRequest request, ...)  // ✅ Updated
	{
		var result = await _getFilteredCompaniesHandler.HandleAsync(request, ...);
		return Ok(result);
	}
}
```

**Step 1.4: Update CompanyApiIntegrationTests.cs**

```csharp
// Update return type assertions
var response = await client.GetAsync($"/api/companies/{companyId}");
var content = await response.Content.ReadAsAsync<CompanyResponse>();  // ✅ Updated

// Update test data builders
var searchRequest = new SearchCompanyRequest  // ✅ Updated
{
	Region = "CA",
	PageNumber = 1,
	PageSize = 10
};
```

---

### FEATURE 2: Locations

#### Current Files
```
src/MyStartUpCompany.Api/Features/Locations/Models/
└── Location.cs
```

#### Refactoring

**Step 2.1: Rename Location.cs → LocationResponse.cs**

**Current:**
```csharp
// Location.cs
[DisplayName("Location")]
public record Location
{
	public int Id { get; init; }
	public int CompanyId { get; init; }
	public required string Name { get; init; }
	// ... other properties
}
```

**Recommended:**
```csharp
// LocationResponse.cs
/// <summary>
/// Response DTO for location details - returned from GET endpoints
/// </summary>
[DisplayName("Location")]  // Keep for OpenAPI schema
public record LocationResponse
{
	public int Id { get; init; }
	public int CompanyId { get; init; }
	public required string Name { get; init; }
	// ... other properties
}
```

**Step 2.2: Create SearchLocationRequest.cs (if filtering is needed)**

```csharp
// SearchLocationRequest.cs
namespace MyStartUpCompany.Api.Features.Locations.Models;

/// <summary>
/// Search/filter request DTO for location queries
/// Used by GET /api/locations/search endpoint
/// </summary>
public record SearchLocationRequest
{
	[FromQuery(Name = "companyId")]
	public int? CompanyId { get; init; }

	[FromQuery(Name = "city")]
	public string? City { get; init; }

	[FromQuery(Name = "region")]
	public string? Region { get; init; }

	[FromQuery(Name = "pageNumber")]
	public int PageNumber { get; init; } = 1;

	[FromQuery(Name = "pageSize")]
	public int PageSize { get; init; } = 10;
}
```

**Step 2.3: Update LocationsController.cs**

**Current:**
```csharp
[HttpGet("{id:int}")]
[ProducesResponseType(typeof(Location), StatusCodes.Status200OK)]  // ❌
public async Task<IActionResult> GetLocation(int id, CancellationToken ct)
{
	var location = await _getLocationHandler.HandleAsync(id, ct);
	return Ok(location);
}
```

**Recommended:**
```csharp
[HttpGet("{id:int}")]
[ProducesResponseType(typeof(LocationResponse), StatusCodes.Status200OK)]  // ✅
public async Task<IActionResult> GetLocation(int id, CancellationToken ct)
{
	var location = await _getLocationHandler.HandleAsync(id, ct);
	return Ok(location);
}

[HttpGet("search")]
[ProducesResponseType(typeof(PagedResult<LocationResponse>), StatusCodes.Status200OK)]  // ✅
public async Task<IActionResult> GetFilteredLocations(
	[FromQuery] SearchLocationRequest request,  // ✅
	CancellationToken ct)
{
	var result = await _getFilteredLocationsHandler.HandleAsync(request, ct);
	return Ok(result);
}
```

---

### FEATURE 3: Buildings

#### Current Files
```
src/MyStartUpCompany.Api/Features/Buildings/Models/
└── Building.cs
```

#### Refactoring

**Step 3.1: Rename Building.cs → BuildingResponse.cs**

**Current:**
```csharp
// Building.cs
[DisplayName("Building")]
public record Building
{
	public int Id { get; init; }
	public int LocationId { get; init; }
	public required string Name { get; init; }
	// ... properties
}
```

**Recommended:**
```csharp
// BuildingResponse.cs
/// <summary>
/// Response DTO for building details - returned from GET endpoints
/// </summary>
[DisplayName("Building")]
public record BuildingResponse
{
	public int Id { get; init; }
	public int LocationId { get; init; }
	public required string Name { get; init; }
	// ... properties
}
```

**Step 3.2: Update BuildingsController.cs**

```csharp
[HttpGet("{id:int}")]
[ProducesResponseType(typeof(BuildingResponse), StatusCodes.Status200OK)]  // ✅
public async Task<IActionResult> GetBuilding(int id, CancellationToken ct)
{
	var building = await _getBuildingHandler.HandleAsync(id, ct);
	return Ok(building);
}

[HttpGet]
[ProducesResponseType(typeof(PagedResult<BuildingResponse>), StatusCodes.Status200OK)]  // ✅
public async Task<IActionResult> GetAllBuildings(...)
{
	var buildings = await _getAllBuildingsHandler.HandleAsync(...);
	return Ok(buildings);
}
```

---

### FEATURE 4: Offices

#### Current Files
```
src/MyStartUpCompany.Api/Features/Offices/Models/
└── Office.cs
```

#### Refactoring

**Step 4.1: Rename Office.cs → OfficeResponse.cs**

**Current:**
```csharp
// Office.cs
[DisplayName("Office")]
public record Office
{
	public int Id { get; init; }
	public int BuildingId { get; init; }
	public required string Name { get; init; }
	// ... properties
}
```

**Recommended:**
```csharp
// OfficeResponse.cs
/// <summary>
/// Response DTO for office details - returned from GET endpoints
/// </summary>
[DisplayName("Office")]
public record OfficeResponse
{
	public int Id { get; init; }
	public int BuildingId { get; init; }
	public required string Name { get; init; }
	// ... properties
}
```

**Step 4.2: Update OfficesController.cs**

```csharp
[HttpGet("{id:int}")]
[ProducesResponseType(typeof(OfficeResponse), StatusCodes.Status200OK)]  // ✅
public async Task<IActionResult> GetOffice(int id, CancellationToken ct)
{
	var office = await _getOfficeHandler.HandleAsync(id, ct);
	return Ok(office);
}

[HttpGet]
[ProducesResponseType(typeof(PagedResult<OfficeResponse>), StatusCodes.Status200OK)]  // ✅
public async Task<IActionResult> GetAllOffices(...)
{
	var offices = await _getAllOfficesHandler.HandleAsync(...);
	return Ok(offices);
}
```

---

## Query Handlers - Update References

### Example: GetCompanyQueryHandler.cs

**Current:**
```csharp
public class GetCompanyQueryHandler : IGetCompanyQueryHandler
{
	// ...
	public async Task<Company> HandleAsync(int companyId, CancellationToken ct)  // ❌
	{
		var company = await _context.Companies
			.FirstOrDefaultAsync(c => c.Id == companyId, ct);

		return new Company
		{
			// ... mapping
		};
	}
}
```

**Recommended:**
```csharp
public class GetCompanyQueryHandler : IGetCompanyQueryHandler
{
	// ...
	public async Task<CompanyResponse> HandleAsync(int companyId, CancellationToken ct)  // ✅
	{
		var company = await _context.Companies
			.FirstOrDefaultAsync(c => c.Id == companyId, ct);

		return new CompanyResponse  // ✅
		{
			// ... mapping
		};
	}
}
```

### Similar Updates for:
- `IGetLocationQueryHandler` - return `LocationResponse`
- `IGetBuildingQueryHandler` - return `BuildingResponse`
- `IGetOfficeQueryHandler` - return `OfficeResponse`
- `IGetAllLocationsQueryHandler` - return `List<LocationResponse>`
- etc.

---

## Tests - Update All References

### Example: CompanyApiIntegrationTests.cs

**Find and Replace:**
```
Company               → CompanyResponse
CompanyRequest        → SearchCompanyRequest
```

**Before:**
```csharp
[Fact]
public async Task GetCompany_WithValidId_ReturnsCompany()
{
	var response = await _client.GetAsync("/api/companies/1");
	var company = await response.Content.ReadAsAsync<Company>();  // ❌

	Assert.NotNull(company);
	Assert.Equal("Acme Corp", company.Name);
}
```

**After:**
```csharp
[Fact]
public async Task GetCompany_WithValidId_ReturnsCompanyResponse()
{
	var response = await _client.GetAsync("/api/companies/1");
	var company = await response.Content.ReadAsAsync<CompanyResponse>();  // ✅

	Assert.NotNull(company);
	Assert.Equal("Acme Corp", company.Name);
}
```

---

## Affected Files Summary

### Rename These Files
```
Company.cs                          → CompanyResponse.cs
CompanyRequest.cs                   → SearchCompanyRequest.cs
Location.cs                         → LocationResponse.cs
Building.cs                         → BuildingResponse.cs
Office.cs                           → OfficeResponse.cs
```

### Create These New Files
```
SearchLocationRequest.cs            (if filtering not already implemented)
SearchBuildingRequest.cs            (if filtering not already implemented)
SearchOfficeRequest.cs              (if filtering not already implemented)
```

### Update These Files (References Only)
```
CompanyController.cs                - Update parameter/return types
LocationsController.cs              - Update parameter/return types
BuildingsController.cs              - Update parameter/return types
OfficesController.cs                - Update parameter/return types

GetCompanyQueryHandler.cs           - Update return type
GetLocationQueryHandler.cs          - Update return type
GetBuildingQueryHandler.cs          - Update return type
GetOfficeQueryHandler.cs            - Update return type

All GetAllXxxQueryHandler files     - Update return types
All GetFilteredXxxQueryHandler files - Update return types

All test files                      - Update assertions and builders
```

---

## Migration Checklist

### Phase 1: File Renames
- [ ] Rename `Company.cs` → `CompanyResponse.cs`
- [ ] Rename `CompanyRequest.cs` → `SearchCompanyRequest.cs`
- [ ] Rename `Location.cs` → `LocationResponse.cs`
- [ ] Rename `Building.cs` → `BuildingResponse.cs`
- [ ] Rename `Office.cs` → `OfficeResponse.cs`

### Phase 2: Create New Files
- [ ] Create `SearchLocationRequest.cs`
- [ ] Create `SearchBuildingRequest.cs`
- [ ] Create `SearchOfficeRequest.cs`

### Phase 3: Update Controllers
- [ ] Update `CompanyController.cs`
- [ ] Update `LocationsController.cs`
- [ ] Update `BuildingsController.cs`
- [ ] Update `OfficesController.cs`

### Phase 4: Update Query Handlers
- [ ] Update all `GetXxxQueryHandler.cs` files
- [ ] Update all `GetAllXxxQueryHandler.cs` files
- [ ] Update all `GetFilteredXxxQueryHandler.cs` files

### Phase 5: Update Tests
- [ ] Update `CompanyApiIntegrationTests.cs`
- [ ] Update any location/building/office tests
- [ ] Update test builders/fixtures

### Phase 6: Verify & Build
- [ ] Full solution rebuild - should compile with 0 errors
- [ ] Run all unit tests
- [ ] Run integration tests
- [ ] Test API endpoints manually
- [ ] Verify OpenAPI/Swagger documentation

---

## Implementation Timeline

| Phase | Effort | Priority | Time |
|-------|--------|----------|------|
| Files Rename | Low | High | 5 min |
| Create New Files | Low | High | 10 min |
| Update Controllers | Low | High | 15 min |
| Update Handlers | Low | High | 15 min |
| Update Tests | Medium | High | 20 min |
| Build & Test | Medium | High | 10 min |
| **Total** | | | **75 min** |

---

## Backward Compatibility Note

### ✅ API Consumers (External)
- **NOT IMPACTED** - No change to JSON response/request structure
- `[DisplayName("Company")]` keeps OpenAPI schema name as "Company"
- Only C# class names change

### ✅ Database/Persistence Layer
- **NOT IMPACTED** - Entity Framework mappings unchanged
- Database queries unaffected

### ⚠️ Internal Code
- **NEEDS UPDATES** - Controllers, tests, and handlers need reference updates
- Compile will catch all breaking changes
- Easy refactor with Find & Replace

---

## Final Recommendations Summary

✅ **DO THIS:**
1. Standardize all response models with `Response` suffix
2. Standardize all request models with appropriate suffix (`Request`, `Dto`)
3. Create search filter models for consistency
4. Update `[DisplayName()]` attributes to keep OpenAPI schemas clean
5. Document the naming convention for future features

❌ **DON'T DO THIS:**
1. Don't mix naming styles (some Response, some Dto)
2. Don't skip test updates
3. Don't forget to update XML documentation
4. Don't break the existing OpenAPI contract without versioning

---

## Questions & Answers

**Q: Won't this break the API?**
A: No. The JSON structure remains identical. Only C# class names change. External API consumers won't notice.

**Q: How do we handle backward compatibility?**
A: You have two options:
1. **Clean break** - If this is internal/new API, just update everything
2. **Gradual migration** - Use old names as aliases, deprecate over time

**Q: What about the `[DisplayName()]` attribute?**
A: Keep using it to control the OpenAPI schema name. Example:
```csharp
[DisplayName("Company")]        // Shows as "Company" in Swagger
public record CompanyResponse   // C# class is CompanyResponse
```

**Q: Do we need to update the database?**
A: No. Database stays the same. Only C# mappings are updated.

**Q: Should we version the API for this change?**
A: Only if breaking API contracts publicly. Since you're at v1 and this is internal, a clean update is fine.

---

## Success Criteria

After implementing these recommendations, verify:

- ✅ All response models end with `Response`
- ✅ All request models end with `Request`
- ✅ All reference data models end with `Dto`
- ✅ Controllers use correct model names
- ✅ Query handlers return correct types
- ✅ All tests pass
- ✅ OpenAPI documentation is clean
- ✅ Code compiles without warnings
- ✅ Projects feature is used as template (already correct!)

---

## Next Steps

1. Review this guide with your team
2. Decide: Implement immediately or gradual migration?
3. Create a Git branch for refactoring
4. Follow the checklist
5. Get code review approval
6. Merge and deploy

This is a **production-ready recommendation** following industry best practices!
