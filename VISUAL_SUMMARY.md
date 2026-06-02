# API Naming Convention - Visual Summary

## Current State vs. Recommended State

### 🔴 CURRENT (Inconsistent)

```
MyStartUpCompany.Api
├── Features
│   ├── CompanyDetails
│   │   └── Models
│   │       ├── Company              ❌ (ambiguous - request or response?)
│   │       └── CompanyRequest       ⚠️  (clear request, but no Response counterpart)
│   │
│   ├── Locations
│   │   └── Models
│   │       └── Location             ❌ (ambiguous)
│   │
│   ├── Buildings
│   │   └── Models
│   │       └── Building             ❌ (ambiguous)
│   │
│   ├── Offices
│   │   └── Models
│   │       └── Office               ❌ (ambiguous)
│   │
│   └── Projects
│       └── Models
│           ├── ProjectResponse      ✅ (correct!)
│           ├── CreateProjectRequest ✅ (correct!)
│           ├── ProjectFilterRequest ✅ (correct!)
│           └── ProjectTypeReferenceDto ✅ (correct!)
```

---

### 🟢 RECOMMENDED (Consistent)

```
MyStartUpCompany.Api
├── Features
│   ├── CompanyDetails
│   │   └── Models
│   │       ├── CompanyResponse           ✅ (clear - response DTO)
│   │       ├── CreateCompanyRequest      ✅ (clear - create request)
│   │       ├── UpdateCompanyRequest      ✅ (clear - update request)
│   │       └── SearchCompanyRequest      ✅ (clear - search request)
│   │
│   ├── Locations
│   │   └── Models
│   │       ├── LocationResponse          ✅ (clear - response DTO)
│   │       ├── CreateLocationRequest     ✅ (clear - create request)
│   │       ├── UpdateLocationRequest     ✅ (clear - update request)
│   │       └── SearchLocationRequest     ✅ (clear - search request)
│   │
│   ├── Buildings
│   │   └── Models
│   │       ├── BuildingResponse          ✅ (clear - response DTO)
│   │       ├── CreateBuildingRequest     ✅ (clear - create request)
│   │       ├── UpdateBuildingRequest     ✅ (clear - update request)
│   │       └── SearchBuildingRequest     ✅ (clear - search request)
│   │
│   ├── Offices
│   │   └── Models
│   │       ├── OfficeResponse            ✅ (clear - response DTO)
│   │       ├── CreateOfficeRequest       ✅ (clear - create request)
│   │       ├── UpdateOfficeRequest       ✅ (clear - update request)
│   │       └── SearchOfficeRequest       ✅ (clear - search request)
│   │
│   └── Projects
│       └── Models
│           ├── ProjectResponse           ✅ (already correct!)
│           ├── CreateProjectRequest      ✅ (already correct!)
│           ├── ProjectFilterRequest      ✅ (already correct!)
│           └── ProjectTypeReferenceDto   ✅ (already correct!)
```

---

## Pattern Comparison

### Naming Pattern Summary

```
REQUEST/RESPONSE FLOW IN API
════════════════════════════

GET /api/companies
	│
	├─→ Response: CompanyResponse          ✅ Clear!
	│   (List of all companies)
	│

GET /api/companies/{id}
	│
	├─→ Response: CompanyResponse          ✅ Clear!
	│   (Single company)
	│

POST /api/companies
	│
	├─← Request: CreateCompanyRequest      ✅ Clear!
	│   (Create a new company)
	│
	└─→ Response: CompanyResponse          ✅ Clear!
		(Newly created company)

GET /api/companies/search?region=CA
	│
	├─← Request: SearchCompanyRequest      ✅ Clear!
	│   (Filter parameters)
	│
	└─→ Response: PagedResult<CompanyResponse>  ✅ Clear!
		(Filtered companies)

PUT /api/companies/{id}
	│
	├─← Request: UpdateCompanyRequest      ✅ Clear!
	│   (Update company data)
	│
	└─→ Response: CompanyResponse          ✅ Clear!
		(Updated company)

DELETE /api/companies/{id}
	│
	├─← Request: int id (from route)       ✅ Clear!
	│   (Just the ID needed)
	│
	└─→ Response: 204 No Content           ✅ Clear!
		(Success, no data returned)

GET /api/projects/types
	│
	└─→ Response: List<ProjectTypeReferenceDto>  ✅ Clear!
		(Reference lookup data)
```

---

## Code Patterns - Before & After

### GET Endpoint

#### ❌ BEFORE (Ambiguous)
```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
public async Task<IActionResult> GetCompany(int id)
{
	var company = await _handler.HandleAsync(id);
	return Ok(company);  // Is "Company" a request or response? 🤔
}
```

#### ✅ AFTER (Clear)
```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]  // 🔴 → 🟢
public async Task<IActionResult> GetCompany(int id)
{
	var company = await _handler.HandleAsync(id);
	return Ok(company);  // "CompanyResponse" = definitely a response! ✅
}
```

---

### POST Endpoint (Future Enhancement)

#### ❌ BEFORE (Ambiguous)
```csharp
[HttpPost]
[ProducesResponseType(typeof(Company), StatusCodes.Status201Created)]
public async Task<IActionResult> Create([FromBody] Company company)
{
	// Is "Company" for input or output? 🤔
	var result = await _handler.HandleAsync(company);
	return CreatedAtAction(nameof(GetCompany), new { id = result.Id }, result);
}
```

#### ✅ AFTER (Clear)
```csharp
[HttpPost]
[ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status201Created)]
public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request)  // 🟢
{
	// "CreateCompanyRequest" = input, "CompanyResponse" = output! Crystal clear! ✅
	var result = await _handler.HandleAsync(request);
	return CreatedAtAction(nameof(GetCompany), new { id = result.Id }, result);
}
```

---

### Search/Filter Endpoint

#### ❌ BEFORE (No structured model)
```csharp
[HttpGet("search")]
[ProducesResponseType(typeof(PagedResult<Company>), StatusCodes.Status200OK)]
public async Task<IActionResult> Search(
	[FromQuery] string? region,
	[FromQuery] int pageNumber = 1,
	[FromQuery] int pageSize = 10)
{
	// Parameters are scattered across method signature 🤔
	var result = await _handler.HandleAsync(region, pageNumber, pageSize);
	return Ok(result);
}
```

#### ✅ AFTER (Structured & clear)
```csharp
[HttpGet("search")]
[ProducesResponseType(typeof(PagedResult<CompanyResponse>), StatusCodes.Status200OK)]  // 🟢
public async Task<IActionResult> Search([FromQuery] SearchCompanyRequest request)  // 🟢
{
	// All parameters in one structured model! Validation-ready! ✅
	var result = await _handler.HandleAsync(request);
	return Ok(result);
}
```

---

## Swagger/OpenAPI Impact

### ❌ BEFORE (Confusing Schema)
```json
{
  "components": {
	"schemas": {
	  "Company": {
		"type": "object",
		"properties": {
		  "id": { "type": "integer" },
		  "name": { "type": "string" },
		  ...
		}
		// ❌ "Company" - unclear if this is request or response
	  },
	  "CompanyRequest": {
		"type": "object",
		"properties": {
		  "region": { "type": "string" },
		  "pageNumber": { "type": "integer" },
		  ...
		}
		// ⚠️ "CompanyRequest" - unclear what this is for
	  }
	}
  }
}
```

### ✅ AFTER (Clear Schema)
```json
{
  "components": {
	"schemas": {
	  "CompanyResponse": {
		"type": "object",
		"description": "Response model for Company GET endpoints",
		"properties": {
		  "id": { "type": "integer" },
		  "name": { "type": "string" },
		  ...
		}
		// ✅ "CompanyResponse" - obviously a response!
	  },
	  "SearchCompanyRequest": {
		"type": "object",
		"description": "Search/filter parameters for companies",
		"properties": {
		  "region": { "type": "string" },
		  "pageNumber": { "type": "integer" },
		  ...
		}
		// ✅ "SearchCompanyRequest" - obviously a search filter!
	  },
	  "CreateCompanyRequest": {
		"type": "object",
		"description": "Request model for creating a company",
		"properties": {
		  "name": { "type": "string" },
		  ...
		}
		// ✅ "CreateCompanyRequest" - obviously for creation!
	  }
	}
  }
}
```

---

## Developer Experience Impact

### IDE IntelliSense Comparison

#### ❌ BEFORE
```csharp
var result = new Company();           // 🤔 What does this do?
result.                               // Only properties, no hint about purpose
```

#### ✅ AFTER
```csharp
// Creating a response
var result = new CompanyResponse();   // ✅ "Response" = GET data from API

// Creating a request
var filter = new SearchCompanyRequest();  // ✅ "Search" = filtering
var create = new CreateCompanyRequest();  // ✅ "Create" = POST data to API

// Each class name tells the story! 📖
```

---

## Naming Decision Matrix

```
				  ┌─ Is this a return value? ──→ [Entity]Response
				  │
	Starting here ┤
				  │
				  └─ Is this input data?
					  │
					  ├─ Creating entity? ────→ Create[Entity]Request
					  │
					  ├─ Updating entity? ────→ Update[Entity]Request
					  │
					  ├─ Searching/filtering? ──→ Search[Entity]Request
					  │
					  └─ Reference lookup data? → [Entity]ReferenceDto
```

---

## File Organization - Before & After

### ❌ BEFORE
```
Models/
├── Company.cs
├── CompanyRequest.cs
├── Location.cs
├── Building.cs
└── Office.cs
```

### ✅ AFTER
```
Models/
├── CompanyResponse.cs
├── CreateCompanyRequest.cs
├── UpdateCompanyRequest.cs
├── SearchCompanyRequest.cs
├── LocationResponse.cs
├── CreateLocationRequest.cs
├── UpdateLocationRequest.cs
├── SearchLocationRequest.cs
├── BuildingResponse.cs
├── CreateBuildingRequest.cs
├── UpdateBuildingRequest.cs
├── SearchBuildingRequest.cs
├── OfficeResponse.cs
├── CreateOfficeRequest.cs
├── UpdateOfficeRequest.cs
├── SearchOfficeRequest.cs
└── ProjectTypeReferenceDto.cs
```

**Note:** More files = Better organization = Clearer intent!

---

## Migration Path - Timeline

```
Week 1 - QUICK WINS
├─ Rename Company → CompanyResponse
├─ Rename CompanyRequest → SearchCompanyRequest
├─ Rename Location → LocationResponse
├─ Rename Building → BuildingResponse
├─ Rename Office → OfficeResponse
└─ ✅ 75-minute implementation

Week 2+ - CREATE MISSING MODELS (Future)
├─ Create CreateLocationRequest (when POST added)
├─ Create CreateBuildingRequest (when POST added)
├─ Create CreateOfficeRequest (when POST added)
├─ Create UpdateXxxRequest classes (when PUT added)
└─ ✅ Incremental as features added

Ongoing - MAINTAIN STANDARD
└─ Use naming standard for all new features
```

---

## Impact Summary

| Aspect | Current ❌ | Recommended ✅ | Impact |
|--------|----------|----------------|--------|
| **Clarity** | Ambiguous | Crystal clear | HIGH |
| **Standards** | Inconsistent | Industry standard | HIGH |
| **Documentation** | Unclear | Self-documenting | HIGH |
| **Maintenance** | Confusing | Easy | MEDIUM |
| **Onboarding** | Steep curve | Quick learning | MEDIUM |
| **Breaking changes** | N/A | NONE | LOW |
| **Implementation effort** | N/A | 75 minutes | LOW |

---

## Bottom Line

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  Your code is WELL-STRUCTURED.                             │
│                                                             │
│  Naming standardization is the POLISH that makes it        │
│  PRODUCTION-READY.                                          │
│                                                             │
│  ✅ Minimal effort                                          │
│  ✅ Maximum clarity                                         │
│  ✅ Industry standard                                       │
│  ✅ Zero breaking changes                                   │
│  ✅ Long-term maintainability                              │
│                                                             │
│              HIGHLY RECOMMENDED ⭐⭐⭐⭐⭐              │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Quick Reference Cheat Sheet

```
🎯 Response from API?        Use: [Entity]Response
🎯 Create request to API?    Use: Create[Entity]Request
🎯 Update request to API?    Use: Update[Entity]Request
🎯 Search/filter to API?     Use: Search[Entity]Request
🎯 Lookup/reference data?    Use: [Entity]ReferenceDto

✅ Always use "record" for DTOs
✅ All fields immutable (init only)
✅ Use "required" keyword for mandatory fields
✅ Add XML documentation
✅ Use [DisplayName] for OpenAPI if needed
```

---

**Version:** 1.0  
**Date:** June 2, 2025  
**Status:** Ready for Team Review

See full analysis in: NAMING_CONVENTION_STANDARD.md
