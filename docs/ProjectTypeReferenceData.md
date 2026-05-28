# ProjectType Reference Data - External System Integration Guide

## Overview

The **ProjectType Reference Data** provides external applications (reports, BI tools, third-party systems) with a standardized way to understand and work with valid project types without needing application-level knowledge of the .NET enum.

This is an industry-standard solution using the **Reference Data Table Pattern**, where enum definitions are stored in a queryable database table alongside metadata.

## Why Reference Data?

### Problem
- External systems need to know what ProjectType values are valid
- Hardcoding enum values in external applications creates maintenance issues
- When new project types are added, external systems don't automatically know about them
- Different systems may use different representations (string, int, display name)

### Solution
The reference data approach provides:
- ✅ Single source of truth for all project types
- ✅ Accessible via database or API
- ✅ Includes display names, descriptions, icons, colors for UI
- ✅ Supports deprecation (IsActive flag)
- ✅ Easy to update without code changes

## API Endpoints

### 1. Get All Project Types
**Purpose:** Retrieve all available project types (most common usage)

```
GET /api/projecttypes
```

**Response:**
```json
{
  "totalCount": 13,
  "projectTypes": [
	{
	  "id": 1,
	  "enumName": "GameDevelopment",
	  "displayName": "Game Development",
	  "description": "Game design, engine development, art, audio, and game programming projects",
	  "iconIdentifier": "🎮",
	  "displayOrder": 1,
	  "isActive": true,
	  "colorCode": "#FF6B6B"
	},
	{
	  "id": 2,
	  "enumName": "CloudService",
	  "displayName": "Cloud Service",
	  "description": "Cloud infrastructure, migration, deployment, and cloud-native application projects",
	  "iconIdentifier": "☁️",
	  "displayOrder": 2,
	  "isActive": true,
	  "colorCode": "#4ECDC4"
	},
	// ... 11 more types
  ],
  "lastUpdated": "2025-01-15T10:30:00Z",
  "apiVersion": "1.0"
}
```

**Use Cases:**
- Populate UI dropdown menus
- Display available options in reports
- Build filter options in external systems

---

### 2. Get Specific Project Type
**Purpose:** Retrieve details for a single project type by name

```
GET /api/projecttypes/CloudService
```

**Response:**
```json
{
  "id": 2,
  "enumName": "CloudService",
  "displayName": "Cloud Service",
  "description": "Cloud infrastructure, migration, deployment, and cloud-native application projects",
  "iconIdentifier": "☁️",
  "displayOrder": 2,
  "isActive": true,
  "colorCode": "#4ECDC4"
}
```

**Use Cases:**
- Get details for a specific project type
- Display comprehensive information about a project type
- Validate and display type information in reports

---

### 3. Validate Project Type
**Purpose:** Validate if a project type value is valid before using it

```
GET /api/projecttypes/validate?type=CloudService
```

**Response:**
```json
{
  "isValid": true,
  "errorMessage": null
}
```

**Use Cases:**
- Client-side validation before API calls
- Validation in reporting filters
- Pre-check before creating/updating projects

---

### 4. Get Grouped Project Types
**Purpose:** Get project types organized by active/inactive status

```
GET /api/projecttypes/grouped
```

**Response:**
```json
{
  "activeTypes": [
	// All 13 active project types...
  ],
  "inactiveTypes": [],
  "totalActive": 13,
  "totalInactive": 0
}
```

**Use Cases:**
- Advanced filtering scenarios
- Show deprecated types separately
- Analytics on type availability

---

## Database Reference

### Direct Database Access
External systems can also query the database directly:

```sql
-- Get all active project types
SELECT * 
FROM ProjectTypeReferences 
WHERE IsActive = 1 
ORDER BY DisplayOrder

-- Get specific type
SELECT * 
FROM ProjectTypeReferences 
WHERE EnumName = 'CloudService'

-- Count of project types by status
SELECT COUNT(*) 
FROM Projects 
GROUP BY Type
```

### Table Schema
```sql
CREATE TABLE ProjectTypeReferences (
	Id INT PRIMARY KEY,
	EnumName NVARCHAR(50) UNIQUE NOT NULL,
	DisplayName NVARCHAR(100) NOT NULL,
	Description NVARCHAR(500) NOT NULL,
	IconIdentifier NVARCHAR(10),
	DisplayOrder INT NOT NULL,
	IsActive BIT NOT NULL DEFAULT 1,
	ColorCode NVARCHAR(7),
	CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
	UpdatedAt DATETIME2
)
```

---

## Integration Examples

### Example 1: Reporting System (SQL Server)

**Query to show projects with type information:**
```sql
SELECT 
	p.ProjectIdentifier,
	p.Name,
	ptr.DisplayName AS ProjectType,
	ptr.Description,
	ptr.IconIdentifier,
	p.Location,
	p.CreatedAt
FROM Projects p
INNER JOIN ProjectTypeReferences ptr ON ptr.Id = p.Type
ORDER BY p.CreatedAt DESC
```

### Example 2: Python External Application

```python
import requests
import json

# Get all project types
response = requests.get('https://api.company.com/api/projecttypes')
project_types = response.json()['projectTypes']

# Create mapping for filtering
type_mapping = {pt['enumName']: pt['displayName'] for pt in project_types}

# Get projects and enrich with type names
projects_response = requests.get('https://api.company.com/api/projects')
projects = projects_response.json()['items']

# Add readable type names
for project in projects:
	project['typeName'] = type_mapping.get(project['type'], 'Unknown')

print(json.dumps(projects, indent=2))
```

### Example 3: JavaScript/React UI

```javascript
// Fetch project types for dropdown
async function loadProjectTypes() {
  const response = await fetch('/api/projecttypes');
  const data = await response.json();

  // Sort by displayOrder and filter active only
  return data.projectTypes
	.filter(pt => pt.isActive)
	.sort((a, b) => a.displayOrder - b.displayOrder);
}

// Populate dropdown
async function populateTypeDropdown() {
  const types = await loadProjectTypes();
  const select = document.getElementById('projectType');

  types.forEach(type => {
	const option = document.createElement('option');
	option.value = type.enumName;
	option.textContent = `${type.iconIdentifier} ${type.displayName}`;
	option.title = type.description;
	select.appendChild(option);
  });
}

// Usage in component
<select id="projectType">
  <option value="">-- Select Project Type --</option>
</select>
```

### Example 4: C# .NET External Client

```csharp
public class ProjectTypeClient
{
	private readonly HttpClient _httpClient;

	public ProjectTypeClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<List<ProjectTypeReferenceDto>> GetAllProjectTypesAsync()
	{
		var response = await _httpClient.GetAsync("https://api/projecttypes");
		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsStringAsync();
		var result = JsonSerializer.Deserialize<ProjectTypeReferenceListResponse>(content);

		return result?.ProjectTypes ?? new();
	}

	public async Task<bool> ValidateProjectTypeAsync(string enumName)
	{
		var response = await _httpClient.GetAsync($"https://api/projecttypes/validate?type={enumName}");
		var content = await response.Content.ReadAsStringAsync();
		var result = JsonSerializer.Deserialize<ProjectTypeValidationResponse>(content);

		return result?.IsValid ?? false;
	}
}

// Usage
var client = new ProjectTypeClient(httpClient);
var types = await client.GetAllProjectTypesAsync();
var isValid = await client.ValidateProjectTypeAsync("CloudService");
```

---

## Key Properties Explained

| Property | Purpose | Example |
|----------|---------|---------|
| **EnumName** | Internal enum identifier, use when filtering projects or setting types | "CloudService" |
| **DisplayName** | Human-readable name for UI and reports | "Cloud Service" |
| **Description** | Detailed explanation of the project type | "Cloud infrastructure..." |
| **IconIdentifier** | Emoji/icon for visual identification in UI | "☁️" |
| **DisplayOrder** | Sort order in UI dropdowns (1-13) | 2 |
| **IsActive** | Whether this type is available for new projects | true/false |
| **ColorCode** | Hex color for dashboards/reports | "#4ECDC4" |

---

## Best Practices

### ✅ DO

1. **Cache reference data locally** - Reference data changes infrequently
   ```python
   # Cache for 24 hours
   @cache.cached(timeout=86400)
   def get_project_types():
	   response = requests.get('/api/projecttypes')
	   return response.json()
   ```

2. **Use EnumName for filtering and API calls**
   ```
   GET /api/projects?type=CloudService
   ```

3. **Use DisplayName for UI display**
   ```html
   <span>{{ project.type.displayName }}</span>
   ```

4. **Validate user input against reference data**
   ```python
   if request.form['type'] not in valid_types:
	   raise ValidationError("Invalid project type")
   ```

5. **Subscribe to API version changes**
   ```python
   # Check version before processing
   if response['apiVersion'] != "1.0":
	   log_warning("API version mismatch")
   ```

### ❌ DON'T

1. **Don't hardcode enum values**
   ```python
   # BAD ❌
   TYPES = ['GameDevelopment', 'CloudService', ...]

   # GOOD ✅
   types = requests.get('/api/projecttypes').json()['projectTypes']
   ```

2. **Don't rely on enum integer values**
   ```
   # The integer value (0-12) might change between versions
   # Always use EnumName instead
   ```

3. **Don't make API calls on every request**
   ```
   # Cache the reference data locally
   ```

---

## Handling Changes

### When a new ProjectType is added:
1. New row is added to `ProjectTypeReferences` table
2. Enum is updated in the .NET application
3. Reference data API automatically returns the new type
4. External systems fetch updated data (no code changes needed)

### When a ProjectType is deprecated:
1. Set `IsActive = false` in the database
2. Existing projects keep their type (backward compatible)
3. External systems respect the `IsActive` flag
4. New projects cannot use deprecated types

---

## Caching Strategy

### Local Caching (Recommended)
- Cache for 24 hours (or your preference)
- Reduces API calls
- Handles brief network outages

### Real-time Updates
- Poll API every 5 minutes for critical systems
- Subscribe to webhook notifications (if implemented)
- Implement version checking

---

## Support and Troubleshooting

### Question: Why do IDs start at 1 instead of 0?
**Answer:** EF Core requires seed entities to have non-zero IDs. The database still represents all 13 types correctly.

### Question: Can I add my own ProjectTypes?
**Answer:** ProjectTypes are managed by the core system. Submit change requests through proper channels. New types require enum updates and migration deployment.

### Question: What if the API is down?
**Answer:** External systems should cache reference data locally. Use cached data as fallback, with appropriate warnings logged.

### Question: How often do ProjectTypes change?
**Answer:** Rarely. ProjectTypes are stable. Reference data is suitable for long-term caching.

---

## Summary

The ProjectType Reference Data pattern provides external systems with:
- 📊 A single source of truth for project types
- 🔄 Automatic updates without code changes
- 🎨 UI metadata (icons, colors, descriptions)
- ✅ Easy validation and filtering
- 📈 Support for future deprecation

External applications should use the API endpoints to fetch this data rather than hardcoding enum values.
