# DTO Naming Convention Quick Reference Guide

## Visual Comparison of Approaches

### Your Current Approach
```
❌ INCONSISTENT
├── Company             (ambiguous - is this request or response?)
├── CompanyRequest      (clear - but no "Response" counterpart)
├── Location            (ambiguous)
├── Building            (ambiguous)
├── Office              (ambiguous)
├── ProjectResponse     ✅ (clear)
├── CreateProjectRequest ✅ (clear)
└── ProjectTypeReferenceDto ✅ (clear)
```

---

### Recommended Approach (Microsoft Standard)
```
✅ CONSISTENT & INDUSTRY-STANDARD

READ MODELS (GET responses)
├── CompanyResponse
├── LocationResponse
├── BuildingResponse
├── OfficeResponse
├── ProjectResponse          ✅ Already done!
└── ProjectTypeReferenceDto  ✅ Already done!

CREATE MODELS (POST requests)
├── CreateCompanyRequest
├── CreateLocationRequest
├── CreateBuildingRequest
├── CreateOfficeRequest
└── CreateProjectRequest     ✅ Already done!

UPDATE MODELS (PUT/PATCH requests)
├── UpdateCompanyRequest
├── UpdateLocationRequest
├── UpdateBuildingRequest
└── UpdateOfficeRequest

SEARCH/FILTER MODELS (GET /search)
├── SearchCompanyRequest
├── SearchLocationRequest
├── SearchBuildingRequest
└── SearchOfficeRequest
```

---

## Industry Approach Comparison Matrix

| Criteria | Approach 1: Response/Request | Approach 2: Dto | Approach 3: Context | Approach 4: CQRS |
|----------|---|---|---|---|
| **Clarity** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Industry Adoption** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Verbosity** | ⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐ |
| **Learning Curve** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐ |
| **Microsoft Recommended** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐ |
| **Recommended for this project** | ✅ YES | No | No | No |

---

## Code Examples: Side-by-Side Comparison

### Scenario 1: GET /api/companies/{id}

#### ❌ Current (Ambiguous)
```csharp
[HttpGet("{id:int}")]
public async Task<IActionResult> GetCompany(int id, CancellationToken ct)
{
	var company = await _handler.HandleAsync(id, ct);
	return Ok(company);  // Returns "Company" - unclear if this is request or response
}
```

#### ✅ Recommended (Clear)
```csharp
[HttpGet("{id:int}")]
[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
public async Task<IActionResult> GetCompany(int id, CancellationToken ct)
{
	var company = await _handler.HandleAsync(id, ct);
	return Ok(company);  // Returns "CompanyResponse" - crystal clear!
}
```

**Benefits:**
- OpenAPI schema explicitly shows `CompanyResponse` as return type
- Swagger/Scalar UI is unambiguous
- IDE IntelliSense helps developers
- Code documentation improves

---

### Scenario 2: GET /api/companies/search?region=CA

#### ❌ Current (Incomplete)
```csharp
[HttpGet("search")]
public async Task<IActionResult> Search(
	[FromQuery] string? region,
	[FromQuery] int pageNumber = 1,
	[FromQuery] int pageSize = 10)
{
	// No structured model - parameters scattered
	var result = await _handler.HandleAsync(region, pageNumber, pageSize);
	return Ok(result);
}
```

#### ✅ Recommended (Structured)
```csharp
[HttpGet("search")]
[ProducesResponseType(typeof(PagedResult<CompanyResponse>), StatusCodes.Status200OK)]
public async Task<IActionResult> Search(
	[FromQuery] SearchCompanyRequest request,
	CancellationToken ct)
{
	var result = await _handler.HandleAsync(request, ct);
	return Ok(result);
}

// Models
public record SearchCompanyRequest
{
	[FromQuery(Name = "region")]
	public string? Region { get; init; }

	[FromQuery(Name = "pageNumber")]
	public int PageNumber { get; init; } = 1;

	[FromQuery(Name = "pageSize")]
	public int PageSize { get; init; } = 10;
}
```

**Benefits:**
- Query parameters are documented in a model
- Validation can be applied to the entire request
- Easier to extend with new filters
- Clear API contract

---

### Scenario 3: POST /api/companies (future)

#### ❌ Ambiguous
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] Company company)
{
	// Is "Company" a request or response? Unclear!
}
```

#### ✅ Recommended (Clear)
```csharp
[HttpPost]
[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ValidationException), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Create(
	[FromBody] CreateCompanyRequest request,
	CancellationToken ct)
{
	var company = await _handler.HandleAsync(request, ct);
	return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
}

// Models
public record CreateCompanyRequest
{
	public required string Name { get; init; }
	public required string Address { get; init; }
	public required string City { get; init; }
	// ... other fields
}
```

**Benefits:**
- `CreateCompanyRequest` makes it obvious this is for creation
- `CompanyResponse` makes it obvious what's returned
- OpenAPI documentation is crystal clear
- Validation can be specific to creation logic

---

### Scenario 4: PUT /api/companies/{id} (future)

```csharp
[HttpPut("{id:int}")]
[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
public async Task<IActionResult> Update(
	[FromRoute] int id,
	[FromBody] UpdateCompanyRequest request,
	CancellationToken ct)
{
	var company = await _handler.HandleAsync(id, request, ct);
	return Ok(company);
}

// Model
public record UpdateCompanyRequest
{
	public string? Name { get; init; }
	public string? Address { get; init; }
	public string? City { get; init; }
	// ... only updatable fields
}
```

---

## Real-World Example: Netflix API

Netflix's public API uses a similar pattern:

```
GET /api/shows                    → Returns ShowResponse[]
POST /api/shows                   → Accepts CreateShowRequest
GET /api/shows/{id}               → Returns ShowResponse
PUT /api/shows/{id}               → Accepts UpdateShowRequest
GET /api/shows/search?title=...   → Accepts SearchShowRequest

// Reference data
GET /api/genres                   → Returns GenreReferenceDto[]
GET /api/ratings                  → Returns RatingReferenceDto[]
```

**Key Pattern:** All endpoints explicitly state what they accept/return through model names.

---

## Real-World Example: GitHub API

GitHub uses "Dto" or resource names:

```
GET /repos/{owner}/{repo}                → Returns RepositoryDto/Repository
POST /repos                              → Accepts CreateRepositoryRequest
PUT /repos/{owner}/{repo}                → Accepts UpdateRepositoryRequest

// They also use explicit suffixes for clarity:
GET /repos/{owner}/{repo}/events         → Returns EventResponse
GET /repos/{owner}/{repo}/issues         → Returns IssueResponse
```

---

## Common Naming Patterns in Enterprise .NET

### Pattern 1: Microsoft Docs (Recommended for you)
```csharp
CompanyResponse              // Read model
CreateCompanyRequest         // Write model
UpdateCompanyRequest         // Update model
SearchCompanyRequest         // Filter model
```

### Pattern 2: Google API Design
```csharp
Company                      // Resource (read)
CreateCompanyRequest         // Command
UpdateCompanyRequest         // Command
```

### Pattern 3: GraphQL-Inspired
```csharp
CompanyType                  // Return type
CompanyInput                 // Input type
CompanyFilter                // Filter type
```

### Pattern 4: DDD-Inspired
```csharp
CompanyAggregate             // Domain object
CreateCompanyCommand         // Command
CompanyQueryResult           // Query result
```

**Your Project:** Use Pattern 1 (Microsoft Docs) - it's already partially done with Projects!

---

## Naming Checklist for Each Model

### For Response Models
- [ ] Ends with `Response`
- [ ] Represents data sent FROM the API
- [ ] Used in `[ProducesResponseType(typeof(CompanyResponse))]`
- [ ] Matches GET endpoint return types
- [ ] Contains all readable fields

**Example:**
```csharp
public record CompanyResponse
{
	public int Id { get; init; }
	public required string Name { get; init; }
	public string? Description { get; init; }
	// ... all properties needed for GET response
}
```

### For Create Request Models
- [ ] Starts with `Create` and ends with `Request`
- [ ] Represents data sent TO the API (POST)
- [ ] Should NOT contain an Id field
- [ ] Only contains fields accepted during creation
- [ ] Can have validation rules specific to creation

**Example:**
```csharp
public record CreateCompanyRequest
{
	public required string Name { get; init; }
	public required string Address { get; init; }
	// NO Id field - auto-generated by API
	// No audit fields (CreatedAt, UpdatedAt)
}
```

### For Update Request Models
- [ ] Starts with `Update` and ends with `Request`
- [ ] Represents data sent TO the API (PUT/PATCH)
- [ ] Should NOT contain an Id field (taken from URL)
- [ ] Can make fields optional (partial updates)
- [ ] Only contains updatable fields

**Example:**
```csharp
public record UpdateCompanyRequest
{
	public string? Name { get; init; }           // Optional for partial updates
	public string? Description { get; init; }
	// Can include nullable versions of other fields
}
```

### For Search/Filter Request Models
- [ ] Ends with `Request` or `Filter` (be consistent)
- [ ] Contains all search/filter parameters
- [ ] Represents query string parameters
- [ ] Used with `[FromQuery]` attribute
- [ ] All fields should be optional

**Example:**
```csharp
public record SearchCompanyRequest
{
	[FromQuery(Name = "region")]
	public string? Region { get; init; }

	[FromQuery(Name = "country")]
	public string? Country { get; init; }

	[FromQuery(Name = "pageNumber")]
	public int PageNumber { get; init; } = 1;

	[FromQuery(Name = "pageSize")]
	public int PageSize { get; init; } = 10;
}
```

---

## Migration Path (If Implementing)

### Phase 1: Rename Response Models
```
Company                 → CompanyResponse
Location                → LocationResponse
Building                → BuildingResponse
Office                  → OfficeResponse
```

### Phase 2: Rename/Organize Search Models
```
CompanyRequest          → SearchCompanyRequest
(Create similar for others if needed)
```

### Phase 3: Add Missing Models (if implementing POST/PUT)
```
New: CreateLocationRequest, CreateBuildingRequest, CreateOfficeRequest
New: UpdateLocationRequest, UpdateBuildingRequest, UpdateOfficeRequest
```

### Phase 4: Update Controllers
- Update `[FromBody]` parameters
- Update `ProducesResponseType` attributes
- Update return type documentation

### Phase 5: Update Tests
- Update test fixtures
- Update assertions to use new names
- Update mock data builders

---

## Summary: Why This Matters

| Aspect | Before | After |
|--------|--------|-------|
| **API Documentation** | "What does Company mean?" | "CompanyResponse clearly = GET response" |
| **Developer Experience** | Confusing | Self-documenting |
| **OpenAPI Schema** | Ambiguous names | Clear intent |
| **Code Review** | Hard to spot mistakes | Easy to validate correct model usage |
| **Maintenance** | Confusion over time | Clear patterns for new developers |
| **Team Onboarding** | Steep learning curve | Quick understanding of patterns |

---

## Key Takeaway

> **"Good naming removes the need for comments."** - Robert C. Martin (Clean Code)

Using explicit suffixes (`Response`, `Request`) in your DTOs makes your API:
- ✅ Self-documenting
- ✅ Industry-standard compliant
- ✅ Easier to maintain
- ✅ Better for team collaboration
- ✅ Clearer for API consumers

Your Projects feature already demonstrates this! Just need consistency across other features.
