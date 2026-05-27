# Project Entity: Hybrid Normalized + JSON Storage Pattern

## Overview

The `Project` entity implements a **hybrid storage pattern** that combines:
1. **Normalized relational columns** for frequently searched/indexed fields
2. **JSON column** for flexible, non-searchable project metadata

This design optimizes database performance for typical query patterns while maintaining flexibility for evolving project details without schema migrations.

## Architecture

### Entity Structure

```
┌─────────────────────────────────────────────────────────────┐
│                    Project Table                              │
├─────────────────────────────────────────────────────────────┤
│ NORMALIZED COLUMNS (Indexed, Searchable)                     │
│  • Id (int, PK)                                               │
│  • ProjectIdentifier (nvarchar(50), Unique Index)            │
│  • Name (nvarchar(500), Index)                               │
│  • Code (nvarchar(50), Index)                                │
│  • Location (nvarchar(200), Index)                           │
│  • CompanyId (int, FK to Companies, Index)                   │
│  • CreatedAt (datetime2, Default: GETUTCDATE())             │
│  • UpdatedAt (datetime2, Nullable)                           │
├─────────────────────────────────────────────────────────────┤
│ JSON COLUMN (Non-Searchable)                                 │
│  • Details (nvarchar(max), JSON)                             │
│    ├─ Budget (decimal)                                       │
│    ├─ Status (string: Planning|Active|OnHold|Completed|...) │
│    ├─ StartDate (datetime)                                   │
│    ├─ EndDate (datetime, nullable)                           │
│    ├─ Description (string)                                   │
│    ├─ ProjectManager (string)                                │
│    ├─ TeamMembers (List<string>)                             │
│    ├─ Priority (string: Critical|High|Medium|Low)           │
│    ├─ Tags (List<string>)                                    │
│    ├─ Metrics (Dictionary<string, string>)                   │
│    ├─ Metadata (Dictionary<string, object>)                  │
│    ├─ ProgressPercentage (int: 0-100)                       │
│    ├─ Notes (string)                                         │
│    ├─ BudgetSpent (decimal, nullable)                       │
│    ├─ Outcome (string)                                       │
│    ├─ RiskLevel (string: Low|Medium|High|Critical)          │
│    ├─ Deliverables (List<string>)                           │
│    └─ Dependencies (List<string>)                            │
└─────────────────────────────────────────────────────────────┘
```

## Design Rationale

### Why Hybrid Storage?

**Problem**: Traditional fully-normalized schema would require:
- A separate column for each project attribute (50+ columns)
- Schema migrations for any new attributes
- Complex joins for filtering on multiple attributes
- Reduced flexibility for future extensibility

**Solution**: Hybrid pattern provides:
- ✅ Fast indexed searches on common fields (ProjectIdentifier, Code, Location)
- ✅ Flexible storage for evolving details without migrations
- ✅ Clean API responses (auto-deserialized DTOs)
- ✅ Reduced column bloat in the main table
- ✅ Clear separation of concerns (searchable vs. metadata)

### Searchable Fields (Normalized Columns)

The following fields are stored in dedicated columns and indexed for optimal query performance:

| Field | Type | Index | Use Case |
|-------|------|-------|----------|
| **ProjectIdentifier** | string(50) | Unique | External system integration, direct lookups |
| **Name** | string(500) | Yes | Full-text search, sorting, filtering |
| **Code** | string(50) | Yes | Project categorization, quick filtering |
| **Location** | string(200) | Yes | Geographic filtering, resource allocation |
| **CompanyId** | int (FK) | Yes | Multi-tenant queries, company-scoped reports |
| **CreatedAt** | datetime2 | No | Sorting, filtering by date |

### Non-Searchable Fields (JSON Column)

The `Details` JSON column contains project-specific metadata that typically:
- Are not used in WHERE clauses
- May vary significantly between projects
- Change frequently (requiring flexibility)
- Include complex data structures (lists, dictionaries)

## Index Strategy

### Individual Indexes
```sql
IX_Project_ProjectIdentifier (UNIQUE)  -- Fast external system lookups
IX_Project_Code                        -- Project type/category filtering
IX_Project_Location                    -- Location-based queries
IX_Project_Name                        -- Project search/sorting
IX_Project_CompanyId                   -- Company-scoped queries
```

### Composite Indexes
```sql
IX_Project_CompanyId_Code              -- Find projects by company and type
IX_Project_Location_Code               -- Filter by location and category
IX_Project_Location_Name_Id            -- Sorted location-based queries
IX_Project_CompanyId_Location_Code     -- Comprehensive company/location/type filtering
```

**Rationale**: Composite indexes optimize the most common query patterns:
- "Get all active projects for Company X by Code"
- "Get projects in Location Y with Code Z"
- "List projects for Company X sorted by Name"

## EF Core Implementation

### Entity Configuration (ProjectConfiguration.cs)

```csharp
// JSON Conversion
builder.Property(p => p.Details)
	.HasConversion(
		v => JsonSerializer.Serialize(v, JsonSerializerOptions),
		v => JsonSerializer.Deserialize<ProjectDetails>(v, JsonSerializerOptions) 
			 ?? new ProjectDetails { Status = "Planning" })
	.HasColumnType("nvarchar(max)");
```

**Key Points**:
- `HasConversion()` automates serialization/deserialization
- `JsonSerializer` handles complex nested types (Lists, Dictionaries)
- Case-insensitive deserialization for flexibility
- Default `Status = "Planning"` for null JSON

### Entity Model (Project.cs)

```csharp
public class Project
{
	// Searchable columns (indexed)
	public int Id { get; set; }
	public required string ProjectIdentifier { get; set; }
	public required string Name { get; set; }
	public required string Code { get; set; }
	public required string Location { get; set; }
	public int CompanyId { get; set; }

	// Relationship
	public virtual Company? Company { get; set; }

	// JSON details (deserialized automatically)
	public ProjectDetails Details { get; set; } = new() { Status = "Planning" };

	// Timestamps
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
```

### Value Object (ProjectDetails.cs)

```csharp
public class ProjectDetails
{
	public decimal Budget { get; set; }
	public required string Status { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime? EndDate { get; set; }
	// ... additional properties
	public List<string>? TeamMembers { get; set; } = new();
	public Dictionary<string, string>? Metrics { get; set; } = new();
}
```

**Design Principles**:
- Encapsulates all non-searchable project data
- Supports complex types (Lists, Dictionaries) via JSON serialization
- `required string Status` enforces data consistency
- Nullable properties allow partial data entry

## API Contract

### Request DTO (CreateProjectRequest)

```csharp
public class CreateProjectRequest
{
	// Maps to normalized columns
	public required string ProjectIdentifier { get; set; }
	public required string Name { get; set; }
	public required string Code { get; set; }
	public required string Location { get; set; }
	public int CompanyId { get; set; }

	// Includes all details in normal object format
	public ProjectDetailsDto Details { get; set; } = new();
}
```

### Response DTO (ProjectResponse)

```csharp
public class ProjectResponse
{
	public int Id { get; set; }
	public required string ProjectIdentifier { get; set; }
	public required string Name { get; set; }
	public required string Code { get; set; }
	public required string Location { get; set; }
	public int CompanyId { get; set; }

	// Details are fully deserialized for the API consumer
	public ProjectDetailsDto Details { get; set; } = new();

	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
```

### Filter Request (ProjectFilterRequest)

```csharp
public class ProjectFilterRequest
{
	// Only searchable fields can be used for filtering
	public string? ProjectIdentifier { get; set; }
	public string? Name { get; set; }
	public string? Code { get; set; }
	public string? Location { get; set; }
	public int? CompanyId { get; set; }

	// Pagination & sorting
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 20;
	public string SortBy { get; set; } = "Name";
}
```

**Key Insight**: The API contract shows all details normally, but filtering is restricted to searchable columns only. This guides developers to query patterns that benefit from indexes.

## Data Flow

### Creating a Project

```
API Request (CreateProjectRequest)
	↓
[FluentValidation]
	↓
Mapper: DTO → Entity
	↓
Project entity with Details value object
	↓
DbContext.SaveChanges()
	↓
EF Core Conversion:
	ProjectDetails object → JSON string (System.Text.Json.JsonSerializer)
	↓
INSERT INTO Projects (ProjectIdentifier, Name, Code, Location, CompanyId, Details, CreatedAt)
```

### Querying Projects

```
API Request: /projects?code=AIPL&location=SanFrancisco
	↓
[ProjectFilterValidator]
	↓
Database Query:
	SELECT * FROM Projects 
	WHERE Code = 'AIPL' 
	  AND Location = 'San Francisco'
	(Uses indexes: IX_Project_Code, IX_Project_Location)
	↓
EF Core Conversion:
	JSON string → ProjectDetails object (System.Text.Json.JsonSerializer)
	↓
Mapper: Entity → ProjectResponse DTO
	↓
API Response with fully deserialized Project and Details
```

## Validation Strategy

### Searchable Field Validation (ProjectFilterRequest)

- ProjectIdentifier: regex pattern for external system format
- Code: uppercase letters and digits only
- Location: max length, whitespace trimming
- CompanyId: positive integer

These validations prevent invalid index queries and database errors.

### Details Validation (ProjectDetailsDtoValidator)

- Status: enum validation (Planning, Active, OnHold, Completed, Archived)
- Budget: non-negative decimal
- Dates: StartDate < EndDate consistency
- ProgressPercentage: 0-100 range
- RiskLevel: enum validation (Low, Medium, High, Critical)

These validations maintain data integrity for the JSON payload.

## Performance Characteristics

| Operation | Complexity | Notes |
|-----------|-----------|-------|
| Create Project | O(1) | Single INSERT; JSON serialization is fast |
| Read by ProjectIdentifier | O(1) | Unique index lookup; immediate deserialization |
| Read by Code + Location | O(log n) | Composite index (IX_Project_Location_Code) |
| Read by CompanyId | O(log n) | Index lookup; scales with company size |
| Filter by multiple criteria | O(log n) | Uses best matching composite index |
| Update Project | O(1) | Single UPDATE; re-serializes Details JSON |
| Delete Project | O(1) | Single DELETE; cascading FK deletes |

## Migration History

- **20260527133156_AddProjectEntity**: 
  - Creates Projects table with normalized columns
  - Adds all performance indexes
  - Seeds 23 realistic projects across 10 companies

## Future Extensibility

### Adding New Searchable Field

If a frequently-queried field should move from JSON to normalized:

```csharp
// 1. Create migration to add column
migrationBuilder.AddColumn<string>("NewField", ...);

// 2. Update ProjectConfiguration
builder.Property(p => p.NewField).HasMaxLength(100).HasIndex();

// 3. Move data from JSON to column (data migration)
// 4. Remove from ProjectDetails; add as Project property
```

### Adding New JSON Field

To add new project details without schema migration:

```csharp
// 1. Add property to ProjectDetails value object
public string? NewDetail { get; set; }

// 2. No database changes needed (nvarchar(max) accommodates expansion)

// 3. Update DTOs and validators
// 4. Regenerate client code if using OpenAPI
```

## Limitations & Considerations

### Limitations

1. **No direct JSON querying** in LINQ: JSON fields cannot be filtered in WHERE clauses
   - Solution: Always filter by normalized columns or post-fetch filtering

2. **JSON deserialization overhead**: Every read deserialization adds ~1-2ms per record
   - Solution: Use caching for frequently accessed projects

3. **Schema validation complexity**: JSON structure validation relies on code, not database constraints
   - Solution: Comprehensive FluentValidation rules; unit tests for ProjectDetails

### Best Practices

1. **Query Optimization**:
   - Always filter by normalized columns (ProjectIdentifier, Code, Location, CompanyId)
   - Never filter on JSON fields in the database query

2. **Update Patterns**:
   - Use repository pattern to centralize Details handling
   - Avoid direct SQL updates to Projects table (use EF Core)

3. **Data Consistency**:
   - Validate ProjectDetails both client-side (FluentValidation) and server-side
   - Use ProjectDetails value object immutability where possible

4. **Monitoring**:
   - Track project record counts; JSON column size may grow large
   - Monitor index fragmentation on composite indexes
   - Set alerts for projects exceeding reasonable JSON sizes

## References

- **EF Core JSON Columns**: https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0#json-columns
- **SQL Server JSON Support**: https://learn.microsoft.com/en-us/sql/relational-databases/json/json-data-sql-server
- **Index Strategy Best Practices**: https://learn.microsoft.com/en-us/sql/relational-databases/indexes/create-composite-indexes
