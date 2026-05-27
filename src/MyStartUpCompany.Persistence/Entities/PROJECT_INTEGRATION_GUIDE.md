# Project Entity Integration Guide

This guide demonstrates how to integrate and use the new `Project` entity with the hybrid normalized + JSON storage pattern across different layers of the application.

## Table of Contents

1. [Database Access Patterns](#database-access-patterns)
2. [API Endpoint Examples](#api-endpoint-examples)
3. [Mapping & Conversion](#mapping--conversion)
4. [Common Queries](#common-queries)
5. [Error Handling](#error-handling)

---

## Database Access Patterns

### Basic Repository Pattern

```csharp
// Repository Interface
public interface IProjectRepository
{
	Task<Project?> GetByIdAsync(int id);
	Task<Project?> GetByIdentifierAsync(string projectIdentifier);
	Task<List<Project>> GetByCompanyAsync(int companyId, int pageNumber = 1, int pageSize = 20);
	Task<List<Project>> GetByLocationAsync(string location);
	Task<List<Project>> GetByCodeAsync(string code);
	Task<Project> CreateAsync(Project project);
	Task<Project> UpdateAsync(Project project);
	Task DeleteAsync(int id);
	Task<int> GetCountAsync();
}

// Repository Implementation
public class ProjectRepository : IProjectRepository
{
	private readonly AppDbContext _context;

	public ProjectRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<Project?> GetByIdAsync(int id)
	{
		return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
	}

	public async Task<Project?> GetByIdentifierAsync(string projectIdentifier)
	{
		// Uses IX_Project_ProjectIdentifier unique index
		return await _context.Projects
			.FirstOrDefaultAsync(p => p.ProjectIdentifier == projectIdentifier);
	}

	public async Task<List<Project>> GetByCompanyAsync(int companyId, int pageNumber = 1, int pageSize = 20)
	{
		// Uses IX_Project_CompanyId index
		return await _context.Projects
			.Where(p => p.CompanyId == companyId)
			.OrderBy(p => p.Name)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
	}

	public async Task<List<Project>> GetByLocationAsync(string location)
	{
		// Uses IX_Project_Location index
		return await _context.Projects
			.Where(p => p.Location == location)
			.OrderBy(p => p.Name)
			.ToListAsync();
	}

	public async Task<List<Project>> GetByCodeAsync(string code)
	{
		// Uses IX_Project_Code index
		return await _context.Projects
			.Where(p => p.Code == code)
			.OrderBy(p => p.Location)
			.ThenBy(p => p.Name)
			.ToListAsync();
	}

	public async Task<Project> CreateAsync(Project project)
	{
		project.CreatedAt = DateTime.UtcNow;
		_context.Projects.Add(project);
		await _context.SaveChangesAsync();
		return project;
	}

	public async Task<Project> UpdateAsync(Project project)
	{
		project.UpdatedAt = DateTime.UtcNow;
		_context.Projects.Update(project);
		await _context.SaveChangesAsync();
		return project;
	}

	public async Task DeleteAsync(int id)
	{
		var project = await GetByIdAsync(id);
		if (project != null)
		{
			_context.Projects.Remove(project);
			await _context.SaveChangesAsync();
		}
	}

	public async Task<int> GetCountAsync()
	{
		return await _context.Projects.CountAsync();
	}
}
```

### Complex Query Examples

```csharp
// Find all projects for a specific company in a specific location
public async Task<List<Project>> GetProjectsByCompanyAndLocationAsync(
	int companyId, 
	string location)
{
	// Uses IX_Project_CompanyId_Location_Code composite index
	return await _context.Projects
		.Where(p => p.CompanyId == companyId && p.Location == location)
		.OrderBy(p => p.Name)
		.ToListAsync();
}

// Find active projects by code (filtering on JSON Details field)
// Note: JSON field filtering is done in-memory after database retrieval
public async Task<List<Project>> GetActiveProjectsByCodeAsync(string code)
{
	var projects = await _context.Projects
		.Where(p => p.Code == code)
		.ToListAsync();

	return projects
		.Where(p => p.Details.Status == "Active")
		.OrderBy(p => p.Details.ProgressPercentage descending)
		.ToList();
}

// Find projects with high budget or high risk
public async Task<List<Project>> GetHighValueOrHighRiskProjectsAsync(int companyId)
{
	var projects = await _context.Projects
		.Where(p => p.CompanyId == companyId)
		.ToListAsync();

	return projects
		.Where(p => p.Details.Budget > 100000 || p.Details.RiskLevel == "Critical")
		.OrderBy(p => p.Details.Budget descending)
		.ToList();
}

// Pagination with sorting
public async Task<(List<Project> Projects, int Total)> GetProjectsPaginatedAsync(
	int pageNumber, 
	int pageSize, 
	string sortBy = "Name",
	string sortOrder = "asc")
{
	var query = _context.Projects.AsQueryable();

	// Apply sorting
	query = sortBy switch
	{
		"Code" => sortOrder == "desc" ? query.OrderByDescending(p => p.Code) : query.OrderBy(p => p.Code),
		"Location" => sortOrder == "desc" ? query.OrderByDescending(p => p.Location) : query.OrderBy(p => p.Location),
		"CreatedAt" => sortOrder == "desc" ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
		_ => sortOrder == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
	};

	var total = await query.CountAsync();

	var projects = await query
		.Skip((pageNumber - 1) * pageSize)
		.Take(pageSize)
		.ToListAsync();

	return (projects, total);
}
```

---

## API Endpoint Examples

### Controller Pattern

```csharp
using Microsoft.AspNetCore.Mvc;
using MyStartUpCompany.Api.Features.Projects.Models;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Features.Projects;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
	private readonly IProjectService _projectService;

	public ProjectsController(IProjectService projectService)
	{
		_projectService = projectService;
	}

	/// <summary>
	/// Get a project by ID
	/// </summary>
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProjectResponse>> GetProject(int id)
	{
		var project = await _projectService.GetProjectByIdAsync(id);
		if (project == null)
		{
			return NotFound(new { message = $"Project with ID {id} not found" });
		}
		return Ok(project);
	}

	/// <summary>
	/// Get a project by unique identifier
	/// </summary>
	[HttpGet("identifier/{projectIdentifier}")]
	[ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ProjectResponse>> GetProjectByIdentifier(string projectIdentifier)
	{
		var project = await _projectService.GetProjectByIdentifierAsync(projectIdentifier);
		if (project == null)
		{
			return NotFound(new { message = $"Project '{projectIdentifier}' not found" });
		}
		return Ok(project);
	}

	/// <summary>
	/// List projects with filtering and pagination
	/// </summary>
	[HttpGet]
	[ProducesResponseType(typeof(ProjectListResponse), StatusCodes.Status200OK)]
	public async Task<ActionResult<ProjectListResponse>> ListProjects([FromQuery] ProjectFilterRequest filter)
	{
		var result = await _projectService.ListProjectsAsync(filter);
		return Ok(result);
	}

	/// <summary>
	/// Create a new project
	/// </summary>
	[HttpPost]
	[ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<ProjectResponse>> CreateProject([FromBody] CreateProjectRequest request)
	{
		var project = await _projectService.CreateProjectAsync(request);
		return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
	}

	/// <summary>
	/// Update an existing project
	/// </summary>
	[HttpPut("{id}")]
	[ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<ProjectResponse>> UpdateProject(int id, [FromBody] CreateProjectRequest request)
	{
		var project = await _projectService.UpdateProjectAsync(id, request);
		if (project == null)
		{
			return NotFound(new { message = $"Project with ID {id} not found" });
		}
		return Ok(project);
	}

	/// <summary>
	/// Delete a project
	/// </summary>
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteProject(int id)
	{
		var success = await _projectService.DeleteProjectAsync(id);
		if (!success)
		{
			return NotFound(new { message = $"Project with ID {id} not found" });
		}
		return NoContent();
	}
}
```

### Example API Requests

```bash
# Get all projects with filtering
GET /api/projects?code=AIPL&location=SanFrancisco&pageNumber=1&pageSize=10

# Get all projects for a company
GET /api/projects?companyId=1&sortBy=Name&sortOrder=asc

# Get a project by ID
GET /api/projects/1

# Get a project by identifier
GET /api/projects/identifier/PROJ-2024-001

# Create a new project
POST /api/projects
Content-Type: application/json

{
  "projectIdentifier": "PROJ-2024-099",
  "name": "New Advanced Platform",
  "code": "NAP",
  "location": "San Francisco",
  "companyId": 1,
  "details": {
	"budget": 500000,
	"status": "Planning",
	"startDate": "2024-06-01T00:00:00Z",
	"endDate": "2024-12-31T00:00:00Z",
	"description": "Next-generation platform development",
	"projectManager": "Alice Johnson",
	"teamMembers": ["Alice Johnson", "Bob Dev", "Carol QA"],
	"priority": "High",
	"tags": ["Platform", "Core", "Strategic"],
	"progressPercentage": 0,
	"riskLevel": "Medium",
	"deliverables": ["Architecture", "MVP", "Testing", "Docs"]
  }
}

# Update a project
PUT /api/projects/1
Content-Type: application/json

{
  "projectIdentifier": "PROJ-2024-001",
  "name": "AI-Powered Analytics Platform - Updated",
  "code": "AIPL",
  "location": "Palo Alto",
  "companyId": 1,
  "details": {
	"budget": 300000,
	"status": "Active",
	"startDate": "2024-05-01T00:00:00Z",
	"endDate": "2024-11-01T00:00:00Z",
	"description": "Enhanced machine learning analytics",
	"projectManager": "Sarah Chen",
	"teamMembers": ["Sarah Chen", "John Dev", "Alice ML", "Bob DevOps"],
	"priority": "Critical",
	"tags": ["AI/ML", "Analytics", "Enterprise"],
	"progressPercentage": 75,
	"riskLevel": "Low",
	"deliverables": ["MVP", "Pipeline", "Models", "Dashboard"]
  }
}

# Delete a project
DELETE /api/projects/1
```

---

## Mapping & Conversion

### AutoMapper Configuration

```csharp
using AutoMapper;
using MyStartUpCompany.Api.Features.Projects.Models;
using MyStartUpCompany.Persistence.Entities;
using MyStartUpCompany.Persistence.Entities.ValueObjects;

namespace MyStartUpCompany.Api.Features.Projects.Mappers;

public class ProjectMappingProfile : Profile
{
	public ProjectMappingProfile()
	{
		// Project entity to response DTO
		CreateMap<Project, ProjectResponse>();

		// ProjectDetails value object to DTO
		CreateMap<ProjectDetails, ProjectDetailsDto>().ReverseMap();

		// Create request to entity
		CreateMap<CreateProjectRequest, Project>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
			.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
			.ForMember(dest => dest.Company, opt => opt.Ignore());

		// Create request details to value object
		CreateMap<ProjectDetailsDto, ProjectDetails>().ReverseMap();
	}
}

// Register in Program.cs
builder.Services.AddAutoMapper(typeof(ProjectMappingProfile));
```

### Manual Conversion Example (if not using AutoMapper)

```csharp
// Entity to Response DTO
private ProjectResponse ProjectToResponse(Project project)
{
	return new ProjectResponse
	{
		Id = project.Id,
		ProjectIdentifier = project.ProjectIdentifier,
		Name = project.Name,
		Code = project.Code,
		Location = project.Location,
		CompanyId = project.CompanyId,
		Details = new ProjectDetailsDto
		{
			Budget = project.Details.Budget,
			Status = project.Details.Status,
			StartDate = project.Details.StartDate,
			EndDate = project.Details.EndDate,
			Description = project.Details.Description,
			ProjectManager = project.Details.ProjectManager,
			TeamMembers = project.Details.TeamMembers,
			Priority = project.Details.Priority,
			Tags = project.Details.Tags,
			Metrics = project.Details.Metrics,
			Metadata = project.Details.Metadata,
			ProgressPercentage = project.Details.ProgressPercentage,
			Notes = project.Details.Notes,
			BudgetSpent = project.Details.BudgetSpent,
			Outcome = project.Details.Outcome,
			RiskLevel = project.Details.RiskLevel,
			Deliverables = project.Details.Deliverables,
			Dependencies = project.Details.Dependencies
		},
		CreatedAt = project.CreatedAt,
		UpdatedAt = project.UpdatedAt
	};
}

// Response DTO to Entity
private Project ResponseToProject(CreateProjectRequest request)
{
	return new Project
	{
		ProjectIdentifier = request.ProjectIdentifier,
		Name = request.Name,
		Code = request.Code,
		Location = request.Location,
		CompanyId = request.CompanyId,
		Details = new ProjectDetails
		{
			Budget = request.Details.Budget,
			Status = request.Details.Status ?? "Planning",
			StartDate = request.Details.StartDate,
			EndDate = request.Details.EndDate,
			Description = request.Details.Description,
			ProjectManager = request.Details.ProjectManager,
			TeamMembers = request.Details.TeamMembers,
			Priority = request.Details.Priority,
			Tags = request.Details.Tags,
			Metrics = request.Details.Metrics,
			Metadata = request.Details.Metadata,
			ProgressPercentage = request.Details.ProgressPercentage,
			Notes = request.Details.Notes,
			BudgetSpent = request.Details.BudgetSpent,
			Outcome = request.Details.Outcome,
			RiskLevel = request.Details.RiskLevel,
			Deliverables = request.Details.Deliverables,
			Dependencies = request.Details.Dependencies
		}
	};
}
```

---

## Common Queries

### Search by Multiple Criteria

```csharp
public async Task<List<Project>> SearchProjectsAsync(
	string? searchTerm,
	string? location,
	int? companyId,
	int pageNumber = 1,
	int pageSize = 20)
{
	var query = _context.Projects.AsQueryable();

	// Filter by location (indexed column)
	if (!string.IsNullOrWhiteSpace(location))
	{
		query = query.Where(p => p.Location == location);
	}

	// Filter by company (indexed column)
	if (companyId.HasValue && companyId > 0)
	{
		query = query.Where(p => p.CompanyId == companyId);
	}

	// Search in name or identifier (indexed columns)
	if (!string.IsNullOrWhiteSpace(searchTerm))
	{
		var term = searchTerm.ToLower();
		query = query.Where(p => 
			p.Name.ToLower().Contains(term) || 
			p.ProjectIdentifier.ToLower().Contains(term));
	}

	return await query
		.OrderBy(p => p.Name)
		.Skip((pageNumber - 1) * pageSize)
		.Take(pageSize)
		.ToListAsync();
}
```

### Aggregations

```csharp
// Total project count by company
public async Task<Dictionary<int, int>> GetProjectCountByCompanyAsync()
{
	return await _context.Projects
		.GroupBy(p => p.CompanyId)
		.ToDictionaryAsync(g => g.Key, g => g.Count());
}

// Projects grouped by location
public async Task<Dictionary<string, int>> GetProjectCountByLocationAsync()
{
	return await _context.Projects
		.GroupBy(p => p.Location)
		.ToDictionaryAsync(g => g.Key, g => g.Count());
}

// Projects grouped by code
public async Task<Dictionary<string, int>> GetProjectCountByCodeAsync()
{
	return await _context.Projects
		.GroupBy(p => p.Code)
		.ToDictionaryAsync(g => g.Key, g => g.Count());
}
```

### Budget Analytics (in-memory filtering on JSON)

```csharp
public async Task<(decimal Total, decimal Average, decimal Max)> GetBudgetStatisticsAsync(int companyId)
{
	var projects = await _context.Projects
		.Where(p => p.CompanyId == companyId)
		.ToListAsync();

	var budgets = projects.Select(p => p.Details.Budget).ToList();

	return (
		Total: budgets.Sum(),
		Average: budgets.Count > 0 ? budgets.Average() : 0,
		Max: budgets.Count > 0 ? budgets.Max() : 0
	);
}

// Find over-budget projects (JSON field filtering)
public async Task<List<Project>> GetOverBudgetProjectsAsync(int companyId)
{
	var projects = await _context.Projects
		.Where(p => p.CompanyId == companyId)
		.ToListAsync();

	return projects
		.Where(p => p.Details.BudgetSpent > p.Details.Budget)
		.OrderByDescending(p => (p.Details.BudgetSpent - p.Details.Budget) / p.Details.Budget)
		.ToList();
}
```

---

## Error Handling

### Validation Errors

```csharp
// ValidationException is thrown by FluentValidationFilter
// It's caught and returns a ProblemDetails response

public class ProjectService
{
	private readonly IValidator<CreateProjectRequest> _validator;

	public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request)
	{
		// Validation is handled by the FluentValidationFilter in the API
		// But can also be called manually:
		var validationResult = await _validator.ValidateAsync(request);
		if (!validationResult.IsValid)
		{
			throw new ValidationException(validationResult.Errors);
		}

		// Create project...
	}
}

// Example error response:
/*
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
	"ProjectIdentifier": ["Project identifier is required"],
	"Details.Status": ["Project status must be one of: Planning, Active, OnHold, Completed, Archived"],
	"Details.ProgressPercentage": ["Progress percentage must be between 0 and 100"]
  }
}
*/
```

### Database Errors

```csharp
public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request)
{
	try
	{
		var project = _mapper.Map<Project>(request);
		await _projectRepository.CreateAsync(project);
		return _mapper.Map<ProjectResponse>(project);
	}
	catch (DbUpdateException ex)
	{
		// Unique constraint violation on ProjectIdentifier
		if (ex.InnerException?.Message.Contains("IX_Project_ProjectIdentifier") == true)
		{
			throw new ValidationException(new[] 
			{ 
				new ValidationFailure("ProjectIdentifier", 
					$"Project identifier '{request.ProjectIdentifier}' already exists") 
			});
		}

		// Foreign key violation
		if (ex.InnerException?.Message.Contains("FK_Projects_Companies") == true)
		{
			throw new ValidationException(new[] 
			{ 
				new ValidationFailure("CompanyId", 
					$"Company with ID {request.CompanyId} does not exist") 
			});
		}

		throw;
	}
}
```

---

## Additional Resources

- See [PROJECT_HYBRID_STORAGE_DESIGN.md](PROJECT_HYBRID_STORAGE_DESIGN.md) for detailed architecture documentation
- Review [ProjectConfiguration.cs](Configurations/ProjectConfiguration.cs) for EF Core mapping details
- Check [ProjectRequestValidator.cs](../Api/Features/Projects/Validators/ProjectRequestValidator.cs) for validation rules
