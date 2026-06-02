# MyStartUpCompany.Api - Naming Convention Standard (v1.0)

> **Official Naming Convention Guide for all API DTOs and Models**
> 
> This document establishes the standard naming convention for all Data Transfer Objects (DTOs) and model classes in MyStartUpCompany.Api.
> 
> **Effective Date:** June 2025  
> **Last Updated:** June 2, 2025  
> **Version:** 1.0 - DRAFT

---

## Table of Contents

1. [Overview](#overview)
2. [Naming Rules](#naming-rules)
3. [Pattern by Use Case](#pattern-by-use-case)
4. [Examples](#examples)
5. [When to Use What](#when-to-use-what)
6. [Anti-Patterns](#anti-patterns)
7. [Code Review Checklist](#code-review-checklist)
8. [FAQ](#faq)

---

## Overview

### Purpose
This standard ensures:
- ✅ Consistency across the codebase
- ✅ Clarity about model intent (request vs response)
- ✅ Alignment with industry best practices
- ✅ Easier API documentation
- ✅ Better developer experience

### Scope
Applies to:
- All public record/class DTOs in `Features/*/Models/` folders
- All request binding models
- All response models
- Reference/lookup data models

Does NOT apply to:
- Persistence layer entities (MyStartUpCompany.Persistence)
- Worker/background service models
- Internal helper classes

### Reference Standards
Based on:
- Microsoft ASP.NET Core documentation
- REST API best practices
- Google API Design Guide
- Industry-standard DTO patterns

---

## Naming Rules

### Rule 1: Response Models (GET endpoints return data)

**Pattern:**
```
[EntityName]Response
```

**Definition:**
- Represents data returned FROM the API
- Used by GET endpoints
- Contains all readable fields
- May include calculated/derived properties
- Typically a `public record`

**Examples:**
```csharp
public record CompanyResponse { }
public record LocationResponse { }
public record BuildingResponse { }
public record OfficeResponse { }
public record ProjectResponse { }
```

**When to Use:**
- GET /api/companies/{id} → Returns `CompanyResponse`
- GET /api/companies → Returns `List<CompanyResponse>`
- GET /api/locations?companyId=1 → Returns `List<LocationResponse>`

---

### Rule 2: Create Request Models (POST endpoint creates data)

**Pattern:**
```
Create[EntityName]Request
```

**Definition:**
- Represents data sent TO the API (POST)
- Used by POST endpoints only
- Does NOT contain an `Id` field (auto-generated)
- Does NOT contain audit fields (`CreatedAt`, `UpdatedAt`, `CreatedBy`, etc.)
- Only contains fields accepted during creation
- All required fields should be marked `required`

**Examples:**
```csharp
public record CreateCompanyRequest
{
	public required string Name { get; init; }
	public required string Address { get; init; }
	// No Id field
	// No audit fields
}

public record CreateLocationRequest
{
	public int CompanyId { get; init; }
	public required string Name { get; init; }
	public required string Address { get; init; }
	// No Id field
}
```

**When to Use:**
- POST /api/companies → Accepts `CreateCompanyRequest`
- POST /api/locations → Accepts `CreateLocationRequest`

---

### Rule 3: Update Request Models (PUT/PATCH endpoint modifies data)

**Pattern:**
```
Update[EntityName]Request
```

**Definition:**
- Represents data sent TO the API (PUT/PATCH)
- Used by PUT/PATCH endpoints only
- Does NOT contain an `Id` field (taken from URL route)
- Does NOT contain audit fields
- Only contains fields that can be updated
- Fields should typically be nullable for partial updates

**Examples:**
```csharp
public record UpdateCompanyRequest
{
	public string? Name { get; init; }
	public string? Address { get; init; }
	public string? City { get; init; }
	// No Id field
	// No audit fields
}

public record UpdateLocationRequest
{
	public string? Name { get; init; }
	public string? Description { get; init; }
	// Only updatable fields
}
```

**When to Use:**
- PUT /api/companies/{id} → Accepts `UpdateCompanyRequest`
- PATCH /api/locations/{id} → Accepts `UpdateLocationRequest`

---

### Rule 4: Search/Filter Request Models (GET /search with parameters)

**Pattern (Option A - Recommended):**
```
Search[EntityName]Request
```

**Pattern (Option B - Alternative):**
```
[EntityName]FilterRequest
```

**Definition:**
- Represents search/filter parameters sent TO the API
- Used by GET /search endpoints
- All fields should be OPTIONAL (nullable)
- Includes pagination parameters (PageNumber, PageSize)
- Include filtering criteria specific to the entity

**Examples:**
```csharp
// Option A: Search prefix (RECOMMENDED)
public record SearchCompanyRequest
{
	public string? Region { get; init; }
	public string? Country { get; init; }
	public int PageNumber { get; init; } = 1;
	public int PageSize { get; init; } = 10;
}

// Option B: Filter suffix (ACCEPTABLE - be consistent)
public record CompanyFilterRequest
{
	public string? Region { get; init; }
	public string? Country { get; init; }
	public int PageNumber { get; init; } = 1;
	public int PageSize { get; init; } = 10;
}
```

**IMPORTANT: Consistency**
- ✅ DO: Use Search prefix OR Filter suffix consistently across ALL features
- ❌ DON'T: Mix SearchXxxRequest and XxxFilterRequest in the same project

**When to Use:**
- GET /api/companies/search?region=CA&pageNumber=1 → Accepts `SearchCompanyRequest`
- GET /api/locations/search?city=NYC → Accepts `SearchLocationRequest`

---

### Rule 5: Reference/Lookup Data (Static reference data)

**Pattern:**
```
[EntityName]ReferenceDto
```

OR (if simpler/shorter):

```
[EntityName]Dto
```

**Definition:**
- Represents static reference/lookup data (enums, categories, types)
- Used by GET endpoints returning lists of reference values
- Typically immutable and rarely updated
- Used by dropdowns, selectors in UI

**Examples:**
```csharp
public record ProjectTypeReferenceDto
{
	public int Id { get; init; }
	public required string EnumName { get; init; }
	public required string DisplayName { get; init; }
	public string? Description { get; init; }
}

public record CompanyStatusDto
{
	public int Id { get; init; }
	public required string Name { get; init; }
}
```

**When to Use:**
- GET /api/projects/types → Returns `List<ProjectTypeReferenceDto>`
- GET /api/company-statuses → Returns `List<CompanyStatusDto>`

---

## Pattern by Use Case

### Use Case 1: Simple CRUD Read

```csharp
// GET /api/companies/{id}
// GET /api/companies

[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
public async Task<IActionResult> GetCompany(int id)
{
	var company = await _handler.HandleAsync(id);
	return Ok(company);  // Returns CompanyResponse
}
```

---

### Use Case 2: Search/Filter with Pagination

```csharp
// GET /api/companies/search?region=CA&pageNumber=1&pageSize=10

[HttpGet("search")]
[ProducesResponseType(typeof(PagedResult<CompanyResponse>), StatusCodes.Status200OK)]
public async Task<IActionResult> Search(
	[FromQuery] SearchCompanyRequest request)
{
	var result = await _handler.HandleAsync(request);
	return Ok(result);  // Returns PagedResult<CompanyResponse>
}
```

---

### Use Case 3: Create (POST)

```csharp
// POST /api/companies
// Body: { "name": "Acme Corp", "address": "123 Main St", ... }

[HttpPost]
[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status201Created)]
public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request)
{
	var company = await _handler.HandleAsync(request);
	return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
}
```

---

### Use Case 4: Update (PUT)

```csharp
// PUT /api/companies/{id}
// Body: { "name": "Updated Corp", ... }

[HttpPut("{id}")]
[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
public async Task<IActionResult> Update(
	[FromRoute] int id,
	[FromBody] UpdateCompanyRequest request)
{
	var company = await _handler.HandleAsync(id, request);
	return Ok(company);  // Returns CompanyResponse
}
```

---

### Use Case 5: Reference Data

```csharp
// GET /api/project-types

[HttpGet("types")]
[ProducesResponseType(typeof(List<ProjectTypeReferenceDto>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetTypes()
{
	var types = await _handler.HandleAsync();
	return Ok(types);
}
```

---

## Examples

### Company Feature (Complete Example)

**Models Folder Structure:**
```
Features/CompanyDetails/Models/
├── CompanyResponse.cs
├── CreateCompanyRequest.cs
├── UpdateCompanyRequest.cs
└── SearchCompanyRequest.cs
```

**CompanyResponse.cs:**
```csharp
namespace MyStartUpCompany.Api.Features.CompanyDetails.Models;

/// <summary>
/// Response DTO for company details
/// Returned by GET /api/companies/{id} and GET /api/companies
/// </summary>
[DisplayName("Company")]
public record CompanyResponse
{
	/// <summary>
	/// Unique company identifier
	/// </summary>
	public int Id { get; init; }

	/// <summary>
	/// Company official name
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// Company description
	/// </summary>
	public string? Description { get; init; }

	// ... other read properties
}
```

**CreateCompanyRequest.cs:**
```csharp
namespace MyStartUpCompany.Api.Features.CompanyDetails.Models;

/// <summary>
/// Request DTO for creating a company
/// Accepted by POST /api/companies
/// </summary>
public record CreateCompanyRequest
{
	/// <summary>
	/// Company name (required)
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// Company description (optional)
	/// </summary>
	public string? Description { get; init; }

	// ... only creation fields, no Id, no audit fields
}
```

**UpdateCompanyRequest.cs:**
```csharp
namespace MyStartUpCompany.Api.Features.CompanyDetails.Models;

/// <summary>
/// Request DTO for updating a company
/// Accepted by PUT /api/companies/{id}
/// </summary>
public record UpdateCompanyRequest
{
	/// <summary>
	/// Updated company name (optional)
	/// </summary>
	public string? Name { get; init; }

	/// <summary>
	/// Updated company description (optional)
	/// </summary>
	public string? Description { get; init; }

	// ... only updatable fields, no Id, no audit fields
}
```

**SearchCompanyRequest.cs:**
```csharp
namespace MyStartUpCompany.Api.Features.CompanyDetails.Models;

/// <summary>
/// Request DTO for searching/filtering companies
/// Accepted by GET /api/companies/search
/// </summary>
public record SearchCompanyRequest
{
	/// <summary>
	/// Filter by region (optional)
	/// </summary>
	[FromQuery(Name = "region")]
	public string? Region { get; init; }

	/// <summary>
	/// Filter by country (optional)
	/// </summary>
	[FromQuery(Name = "country")]
	public string? Country { get; init; }

	/// <summary>
	/// Page number for pagination (default: 1)
	/// </summary>
	[FromQuery(Name = "pageNumber")]
	public int PageNumber { get; init; } = 1;

	/// <summary>
	/// Items per page (default: 10)
	/// </summary>
	[FromQuery(Name = "pageSize")]
	public int PageSize { get; init; } = 10;
}
```

**CompanyController.cs (Usage):**
```csharp
[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetCompany(int id, CancellationToken ct)
	{
		var company = await _handler.HandleAsync(id, ct);
		return Ok(company);  // ✅ Returns CompanyResponse
	}

	[HttpPost]
	[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status201Created)]
	public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request, CancellationToken ct)
	{
		var company = await _handler.HandleAsync(request, ct);
		return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
	}

	[HttpPut("{id}")]
	[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
	public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCompanyRequest request, CancellationToken ct)
	{
		var company = await _handler.HandleAsync(id, request, ct);
		return Ok(company);
	}

	[HttpGet("search")]
	[ProducesResponseType(typeof(PagedResult<CompanyResponse>), StatusCodes.Status200OK)]
	public async Task<IActionResult> Search([FromQuery] SearchCompanyRequest request, CancellationToken ct)
	{
		var result = await _handler.HandleAsync(request, ct);
		return Ok(result);
	}
}
```

---

## When to Use What

### Decision Tree

```
Is this data being RETURNED from the API?
  └─ YES → Use [EntityName]Response
  └─ NO  → Is this for creating an entity?
			└─ YES → Use Create[EntityName]Request
			└─ NO  → Is this for updating an entity?
					  └─ YES → Use Update[EntityName]Request
					  └─ NO  → Is this for searching/filtering?
								└─ YES → Use Search[EntityName]Request
								└─ NO  → Is this reference/lookup data?
										  └─ YES → Use [EntityName]ReferenceDto
										  └─ NO  → Use [EntityName]Dto (or consult team lead)
```

### Quick Lookup Table

| Scenario | Pattern | Example |
|----------|---------|---------|
| GET endpoint returns this | `[Entity]Response` | `CompanyResponse` |
| POST endpoint accepts this | `Create[Entity]Request` | `CreateCompanyRequest` |
| PUT endpoint accepts this | `Update[Entity]Request` | `UpdateCompanyRequest` |
| GET /search accepts this | `Search[Entity]Request` | `SearchCompanyRequest` |
| GET returns reference data | `[Entity]ReferenceDto` | `ProjectTypeReferenceDto` |

---

## Anti-Patterns

### ❌ Don't Do This

**Anti-Pattern 1: Ambiguous Names**
```csharp
// ❌ BAD - Is this request or response?
public record Company { }
public record Location { }

// ✅ GOOD
public record CompanyResponse { }
public record LocationResponse { }
```

**Anti-Pattern 2: Inconsistent Suffixes**
```csharp
// ❌ BAD - Mixing Response and Dto for same purpose
public record CompanyResponse { }
public record LocationDto { }
public record Building { }

// ✅ GOOD
public record CompanyResponse { }
public record LocationResponse { }
public record BuildingResponse { }
```

**Anti-Pattern 3: Response Models with Id Field**
```csharp
// ❌ BAD - Confuses request vs response
public record CreateCompanyRequest
{
	public int Id { get; init; }  // ❌ No ID in create request!
	public required string Name { get; init; }
}

// ✅ GOOD
public record CreateCompanyRequest
{
	public required string Name { get; init; }
	// Id is generated by server, not provided
}
```

**Anti-Pattern 4: Overly Long Names**
```csharp
// ❌ BAD - Too verbose
public record CompanyDataResponseModel { }
public record GetCompanyResponseDto { }

// ✅ GOOD
public record CompanyResponse { }
```

**Anti-Pattern 5: Mixing Dto and Response**
```csharp
// ❌ BAD - Inconsistent across features
public record CompanyResponse { }
public record LocationDto { }
public record BuildingResponse { }

// ✅ GOOD - Choose one pattern and stick with it
public record CompanyResponse { }
public record LocationResponse { }
public record BuildingResponse { }
```

---

## Code Review Checklist

When reviewing pull requests with new models, verify:

- [ ] **Response Models**
  - [ ] Named with `Response` suffix
  - [ ] Used only for GET endpoints
  - [ ] Contains readable properties
  - [ ] Has XML documentation
  - [ ] Listed in `[ProducesResponseType]`

- [ ] **Create Request Models**
  - [ ] Named with `Create[Entity]Request` pattern
  - [ ] Used only for POST endpoints
  - [ ] Does NOT contain Id field
  - [ ] Does NOT contain audit fields
  - [ ] All required fields marked `required`
  - [ ] Has XML documentation

- [ ] **Update Request Models**
  - [ ] Named with `Update[Entity]Request` pattern
  - [ ] Used only for PUT/PATCH endpoints
  - [ ] Does NOT contain Id field
  - [ ] Only contains updatable fields
  - [ ] Properties are optional (nullable)
  - [ ] Has XML documentation

- [ ] **Search/Filter Models**
  - [ ] Named with `Search[Entity]Request` pattern
  - [ ] All filter fields are optional
  - [ ] Includes pagination (PageNumber, PageSize)
  - [ ] Uses `[FromQuery]` attributes
  - [ ] Has XML documentation

- [ ] **General Checks**
  - [ ] Model is `public record` (for DTOs)
  - [ ] Namespace follows convention
  - [ ] No business logic in models
  - [ ] Immutable properties (`{ get; init; }`)
  - [ ] No Persistence entities mixed in

---

## FAQ

### Q: What about inheritance/base classes?
**A:** Use a common base record for shared properties:
```csharp
public abstract record BaseResponse
{
	public int Id { get; init; }
	public DateTime CreatedAt { get; init; }
}

public record CompanyResponse : BaseResponse
{
	public required string Name { get; init; }
}
```

### Q: Should I use class or record?
**A:** **Always use `record`** for DTOs because:
- Immutable by default
- Lightweight syntax
- Built-in equality
- Modern C# best practice
- Aligns with functional programming

### Q: What about nested/complex objects?
**A:** Create separate Response models:
```csharp
public record CompanyResponse
{
	public int Id { get; init; }
	public required string Name { get; init; }
	public List<LocationResponse> Locations { get; init; } = [];  // ✅ Nested response
}

public record LocationResponse
{
	public int Id { get; init; }
	public required string Name { get; init; }
}
```

### Q: Can I use [DisplayName] with Response?
**A:** Yes! For OpenAPI schema clarity:
```csharp
[DisplayName("Company")]          // Shows in OpenAPI schema
public record CompanyResponse     // C# class name
{
	// ...
}
```

### Q: What about versioned models?
**A:** Use explicit versioning:
```csharp
// V1 (deprecated)
public record CompanyResponseV1 { }

// V2 (current)
public record CompanyResponse { }

// Alternative: namespace-based
namespace MyStartUpCompany.Api.V1.Features.Companies.Models;
public record CompanyResponse { }
```

### Q: Should search request have paging?
**A:** Yes! Paging is part of search:
```csharp
public record SearchCompanyRequest
{
	public string? Name { get; init; }              // Filter
	public int PageNumber { get; init; } = 1;      // Paging
	public int PageSize { get; init; } = 10;       // Paging
}
```

### Q: How do I handle optional fields in updates?
**A:** Make properties nullable:
```csharp
public record UpdateCompanyRequest
{
	public string? Name { get; init; }             // Can be null = don't update
	public string? Address { get; init; }          // Can be null = don't update
}

// Handler logic:
if (request.Name is not null)
	company.Name = request.Name;
```

---

## Approval & Signatures

| Role | Name | Date | Signature |
|------|------|------|-----------|
| Architect/Lead | [TBD] | [TBD] | [TBD] |
| Team | [TBD] | [TBD] | [TBD] |

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 2025-06-02 | Initial standard document | AI Review |

---

## Related Documents

- [API_NAMING_CONVENTION_ANALYSIS.md](./API_NAMING_CONVENTION_ANALYSIS.md) - Detailed analysis and rationale
- [DTO_NAMING_QUICK_REFERENCE.md](./DTO_NAMING_QUICK_REFERENCE.md) - Quick reference guide
- [REFACTORING_GUIDE.md](./REFACTORING_GUIDE.md) - Step-by-step refactoring instructions

---

**Last Updated:** June 2, 2025  
**Status:** DRAFT - Awaiting Team Approval
