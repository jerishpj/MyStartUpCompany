# API Model Naming Convention Analysis & Industry Standards Review

## Executive Summary
Your API currently uses **inconsistent naming conventions** for DTOs (Data Transfer Objects) across different features. While the code is well-structured, standardizing the naming will improve clarity and maintainability.

---

## Current State Analysis

### 1. **Company Feature** - INCONSISTENT APPROACH
```
✓ Company.cs              → Simple name (Response DTO - for GET endpoints)
✓ CompanyRequest.cs       → Suffixed with "Request" (Search/Filter DTO)
✗ No explicit "Response" suffix
```
**Current Usage:**
- `GET /api/companies/{id}` returns `Company` record
- `GET /api/companies` returns `Company` record
- `GET /api/companies/search` accepts `CompanyRequest` for filtering

**Issues:**
- `Company` name is ambiguous - is it a request or response?
- Mixing `CompanyRequest` (explicit) with `Company` (implicit) creates confusion
- Inconsistent with other features

---

### 2. **Location Feature** - INCONSISTENT APPROACH
```
✓ Location.cs            → Simple name (Response DTO)
✗ No explicit "Request" or "Response" suffix
✗ No separate request model for filtering
```
**Current Usage:**
- `GET /api/locations/{id}` returns `Location` record
- `GET /api/locations` returns `Location` record
- `GET /api/locations/search` accepts parameters directly (not wrapped in DTO)

**Issues:**
- Similar to Company - implicit naming creates ambiguity
- No structured request model for search parameters
- Inconsistency with Company feature

---

### 3. **Building Feature** - INCONSISTENT APPROACH
```
✓ Building.cs            → Simple name (Response DTO)
✗ No explicit "Request" or "Response" suffix
✗ No separate request model for filtering
```
**Current Usage:**
- Returns `Building` record
- No search/filter request model visible

**Issues:**
- Same problems as Location
- Should have consistency across similar features

---

### 4. **Office Feature** - INCONSISTENT APPROACH
```
✓ Office.cs              → Simple name (Response DTO)
✗ No explicit "Request" or "Response" suffix
✗ No separate request model for filtering
```
**Current Usage:**
- Returns `Office` record
- Likely missing search request model

---

### 5. **Projects Feature** - GOOD BUT VERBOSE
```
✓ ProjectResponse.cs             → Explicit suffix (Read/GET)
✓ CreateProjectRequest.cs        → Explicit suffix (Write/CREATE)
✓ ProjectFilterRequest.cs        → Explicit suffix (Search/Filter)
✓ ProjectTypeReferenceDto.cs     → DTO suffix (Reference data)
```
**Current Usage:**
- `GET /api/projects/{id}` returns `ProjectResponse`
- `POST /api/projects` accepts `CreateProjectRequest`
- `GET /api/projects/search` accepts `ProjectFilterRequest`
- Reference data uses `ProjectTypeReferenceDto`

**Observations:**
- ✅ **MOST CONSISTENT** feature for naming
- ✅ Clear distinction between read and write models
- ✅ Explicit purpose suffixes
- ⚠️ Slightly verbose but very clear

---

## Summary of Current Patterns

| Feature | Response Model | Request Model | Suffix Usage | Rating |
|---------|---|---|---|---|
| **Company** | `Company` | `CompanyRequest` | Inconsistent | ⚠️ Mixed |
| **Location** | `Location` | None visible | None | ❌ Implicit |
| **Building** | `Building` | None visible | None | ❌ Implicit |
| **Office** | `Office` | None visible | None | ❌ Implicit |
| **Project** | `ProjectResponse` | `CreateProjectRequest` | Explicit | ✅ Best Practice |

---

## Industry Standard Naming Conventions

### Industry Best Practices for API DTOs

#### **Approach 1: Microsoft/ASP.NET Core Standards**
Most commonly used in enterprise .NET applications:
```csharp
// Read model (GET response)
public class CompanyResponse { }       // or CompanyDto

// Write model (POST/PUT request)
public class CreateCompanyRequest { }
public class UpdateCompanyRequest { }

// Search/Filter model
public class CompanyFilterRequest { }  // or SearchCompanyRequest
```

**Pros:** Very explicit, Microsoft-recommended, widely understood  
**Cons:** More verbose, longer names

---

#### **Approach 2: RESTful DTO Convention**
Simpler, popular in smaller projects:
```csharp
// Read model (GET response)
public class CompanyDto { }

// Write models
public class CreateCompanyDto { }
public class UpdateCompanyDto { }

// Search model
public class CompanySearchDto { }
```

**Pros:** Concise, "Dto" makes it clear these are transfer objects  
**Cons:** Less explicit about direction (request vs response)

---

#### **Approach 3: API-First Convention** (Modern REST API)
Popular in API-first design and GraphQL communities:
```csharp
// Read model (implied from usage in GET)
public class Company { }

// Write models (explicitly suffixed)
public class CreateCompanyInput { }
public class UpdateCompanyInput { }
public class CompanyFilter { }
```

**Pros:** Simple, readable, context-driven  
**Cons:** Can be ambiguous without proper documentation

---

#### **Approach 4: CQRS-Inspired Convention**
For advanced scenarios with separate command/query models:
```csharp
// Query result (read)
public class CompanyDetails { }

// Commands (write)
public class CreateCompanyCommand { }
public class UpdateCompanyCommand { }

// Query filters
public class GetCompaniesQuery { }
```

**Pros:** Clear separation of concerns, scalable  
**Cons:** More complex, overkill for simple CRUD APIs

---

## Recommendation: Hybrid Industry Standard

### **For MyStartUpCompany.Api - RECOMMENDED APPROACH**

Adopt **Microsoft/ASP.NET Core Standard** (Approach 1) with **Response/Request suffixes**:

```csharp
// ✅ STANDARDIZED NAMING CONVENTION

// 1. READ MODELS (GET responses)
public record CompanyResponse { }       // Use "Response" suffix for explicit GET
public record LocationResponse { }
public record BuildingResponse { }
public record OfficeResponse { }
public record ProjectResponse { }       // Already following this! ✅

// 2. CREATE MODELS (POST requests)
public record CreateCompanyRequest { }
public record CreateLocationRequest { }
public record CreateBuildingRequest { }
public record CreateOfficeRequest { }
public record CreateProjectRequest { }  // Already following this! ✅

// 3. UPDATE MODELS (PUT/PATCH requests)
public record UpdateCompanyRequest { }
public record UpdateLocationRequest { }
public record UpdateBuildingRequest { }
public record UpdateOfficeRequest { }

// 4. SEARCH/FILTER MODELS (GET /search with filters)
public record SearchCompanyRequest { }
public record SearchLocationRequest { }
public record SearchBuildingRequest { }
public record SearchOfficeRequest { }
public record SearchProjectRequest { }

// 5. REFERENCE DATA (Static lookup data)
public record ProjectTypeReferenceDto { }  // Already following this! ✅
```

---

## Why This Approach?

### ✅ **Advantages:**

1. **Crystal Clear Intent**
   - `CompanyResponse` → "This is data returned FROM the API"
   - `CreateCompanyRequest` → "This is data sent TO the API"
   - No ambiguity for developers consuming your API

2. **ASP.NET Core Official Standard**
   - Aligns with Microsoft documentation
   - Familiar to enterprise .NET developers
   - Recommended in official guides

3. **SOLID Principles Compliance**
   - Single Responsibility: Each class has one clear purpose
   - Open/Closed: Easy to extend without modifying existing models
   - Consistency: Same pattern everywhere

4. **Backward Compatible Option**
   - Can gradually migrate if breaking changes are a concern
   - Can alias old names during transition

5. **Self-Documenting**
   - No need to check code to understand if something is request or response
   - IDE autocomplete is more meaningful
   - OpenAPI/Swagger schema names are clearer

6. **Already Partially Done**
   - Your Projects feature already uses `ProjectResponse` and `CreateProjectRequest`
   - Just need consistency across other features

---

## Current vs Recommended Mapping

| Current | Recommended | Reason |
|---------|-------------|--------|
| `Company` | `CompanyResponse` | Explicit read model |
| `CompanyRequest` | `SearchCompanyRequest` | More explicit purpose |
| `Location` | `LocationResponse` | Consistent with projects |
| *(missing)* | `CreateLocationRequest` | Explicit write model |
| *(missing)* | `UpdateLocationRequest` | Explicit update model |
| `Building` | `BuildingResponse` | Consistent pattern |
| *(missing)* | `CreateBuildingRequest` | For future POST endpoints |
| `Office` | `OfficeResponse` | Consistent pattern |
| *(missing)* | `CreateOfficeRequest` | For future POST endpoints |
| `ProjectResponse` | `ProjectResponse` | ✅ Already correct! |
| `CreateProjectRequest` | `CreateProjectRequest` | ✅ Already correct! |
| `ProjectTypeReferenceDto` | `ProjectTypeReferenceDto` | ✅ Already correct! |

---

## Common Questions & Answers

### Q: Why not just use "Dto" suffix?
**A:** Because "Dto" (Data Transfer Object) is too generic. It doesn't tell developers if it's a request or response. `CompanyResponse` and `CreateCompanyRequest` are more explicit.

### Q: Won't this break existing clients?
**A:** Yes, if this is a public API. BUT:
- The JSON property names remain unchanged (via JsonPropertyName)
- Only the C# class names change
- Can be migrated gradually
- Consider API versioning for major breaking changes

### Q: Should we use records or classes?
**A:** **Records are better** for DTOs because:
- ✅ Immutable by default
- ✅ Lightweight syntax
- ✅ Built-in equality comparison
- ✅ Modern C# best practice
- Your code already does this! Keep using records.

### Q: What about nullable reference types?
**A:** Already in use with `required` keyword. Continue this pattern.

### Q: For generic response wrappers?
**A:** Use:
```csharp
public record ApiResponse<T>(T Data, string Message = null);
public record PagedResponse<T>(List<T> Items, int PageNumber, int PageSize, int TotalCount);
```

---

## Implementation Priority

### **Phase 1: Quick Wins** (Low risk, high clarity)
1. ✅ Projects feature - Already compliant
2. Rename Company/Location/Building/Office models to include Response suffix
3. Add SearchCompanyRequest to Company feature (may already exist)

### **Phase 2: Completeness** (Medium risk)
1. Add CreateLocationRequest, CreateBuildingRequest, CreateOfficeRequest
2. Add UpdateLocationRequest, UpdateBuildingRequest, UpdateOfficeRequest
3. Update controllers to use new model names
4. Update tests

### **Phase 3: Documentation** (Low risk)
1. Update API documentation/OpenAPI schemas
2. Update Swagger/Scalar UI
3. Document naming convention for team

---

## Naming Convention Guidelines for Future Features

### Rule 1: Response DTOs
```csharp
// For GET endpoints returning entity data
public record [EntityName]Response
```

### Rule 2: Create Request DTOs
```csharp
// For POST endpoints
public record Create[EntityName]Request
```

### Rule 3: Update Request DTOs
```csharp
// For PUT/PATCH endpoints
public record Update[EntityName]Request
```

### Rule 4: Search/Filter Request DTOs
```csharp
// For GET /search with filter parameters
public record Search[EntityName]Request
// OR: Search[EntityName]Filter
// OR: [EntityName]FilterRequest  (if you prefer)
```

### Rule 5: Reference/Lookup Data
```csharp
// For static reference data (enums, lookups)
public record [EntityName]ReferenceDto
// OR: [EntityName]Dto
```

---

## Checklist for Code Review

- [ ] Response models end with `Response`
- [ ] Create request models start with `Create` and end with `Request`
- [ ] Update request models start with `Update` and end with `Request`
- [ ] Search filters end with `Request` or `Filter` (consistent across project)
- [ ] Reference data ends with `Dto` or `Reference`
- [ ] All models are `public record` (for DTOs)
- [ ] Models use `required` keyword for mandatory fields
- [ ] Models have XML documentation comments
- [ ] Namespace follows pattern: `[Namespace].Features.[FeatureName].Models`

---

## References

1. **Microsoft ASP.NET Core Documentation**
   - https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture

2. **DTO Best Practices**
   - https://learn.microsoft.com/en-us/dotnet/architecture/ddd-in-layered-dotnet-apps/

3. **C# Records for DTOs**
   - https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/records

4. **REST API Design Best Practices**
   - https://restfulapi.net/

5. **Google API Design Guide**
   - https://google.aip.dev/

---

## Conclusion

Your codebase is **well-structured** but needs **naming standardization**. The Projects feature already demonstrates best practices with `Response`/`Request` suffixes. 

**Recommendation:** Adopt the **Microsoft/ASP.NET Core Standard** with explicit `Response`/`Request` suffixes for:
- ✅ Maximum clarity
- ✅ Industry alignment
- ✅ Consistency with Projects feature
- ✅ Self-documenting code
- ✅ Better IDE support

This is **production-ready** guidance that balances clarity, maintainability, and industry standards.
