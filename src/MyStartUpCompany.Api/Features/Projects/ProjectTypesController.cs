using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Projects.Models;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Projects;

/// <summary>
/// Reference data endpoints for project types
/// Provides information about all valid project types for external systems, reports, and UI
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectTypesController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ProjectTypesController> _logger;

    public ProjectTypesController(
        AppDbContext dbContext,
        ILogger<ProjectTypesController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all available project types (reference data)
    /// This endpoint is designed for external applications, reports, and UI applications to understand valid project types
    /// 
    /// Use this to:
    /// - Populate UI dropdowns for project type selection
    /// - Understand what project types are valid for filtering
    /// - Display project type information in reports
    /// - Configure external system integrations
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all available project types with metadata</returns>
    /// <response code="200">Returns the list of project type reference data</response>
    [HttpGet]
    [ProducesResponseType(typeof(ProjectTypeReferenceListResponse), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetProjectTypes(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all project type reference data");

        var projectTypes = await _dbContext.ProjectTypeReferences
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = projectTypes.Select(MapToDto).ToList();

        var response = new ProjectTypeReferenceListResponse
        {
            TotalCount = dtos.Count,
            ProjectTypes = dtos,
            LastUpdated = DateTime.UtcNow,
            ApiVersion = "1.0"
        };

        _logger.LogInformation("Retrieved {Count} project types", dtos.Count);

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a specific project type by its enum name
    /// Useful for validation and detailed information about a single project type
    /// 
    /// Example: GET /api/projecttypes/CloudService
    /// </summary>
    /// <param name="enumName">The enum name of the project type (e.g., "CloudService", "DataAnalytics")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project type reference data</returns>
    /// <response code="200">Returns the project type details</response>
    /// <response code="404">If the project type is not found</response>
    [HttpGet("{enumName}", Name = "GetProjectTypeByName")]
    [ProducesResponseType(typeof(ProjectTypeReferenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectTypeByName(
        [FromRoute] string enumName,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving project type: {EnumName}", enumName);

        var projectType = await _dbContext.ProjectTypeReferences
            .Where(p => p.EnumName == enumName && p.IsActive)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (projectType is null)
        {
            _logger.LogWarning("Project type '{EnumName}' not found", enumName);
            return NotFound(new ProblemDetails
            {
                Title = "Project Type Not Found",
                Detail = $"Project type '{enumName}' not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        var dto = MapToDto(projectType);
        return Ok(dto);
    }

    /// <summary>
    /// Validates if a given project type value is valid
    /// Useful for external systems to validate user input before making API calls
    /// 
    /// Example: GET /api/projecttypes/validate?type=CloudService
    /// </summary>
    /// <param name="type">The enum name to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result indicating if the type is valid</returns>
    /// <response code="200">Returns validation result</response>
    [HttpGet("validate")]
    [ProducesResponseType(typeof(ProjectTypeValidationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateProjectType(
        [FromQuery] string type,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating project type: {Type}", type);

        if (string.IsNullOrWhiteSpace(type))
        {
            return Ok(new ProjectTypeValidationResponse
            {
                IsValid = false,
                ErrorMessage = "Project type cannot be empty"
            });
        }

        var exists = await _dbContext.ProjectTypeReferences
            .AnyAsync(p => p.EnumName == type && p.IsActive, cancellationToken);

        var response = new ProjectTypeValidationResponse
        {
            IsValid = exists,
            ErrorMessage = exists ? null : $"Project type '{type}' is not valid"
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets project types grouped by category/status
    /// Useful for advanced filtering and reporting scenarios
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project types grouped by active status</returns>
    /// <response code="200">Returns grouped project types</response>
    [HttpGet("grouped")]
    [ProducesResponseType(typeof(ProjectTypeGroupedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectTypesGrouped(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving grouped project type reference data");

        var allTypes = await _dbContext.ProjectTypeReferences
            .OrderBy(p => p.DisplayOrder)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var activeTypes = allTypes.Where(p => p.IsActive).Select(MapToDto).ToList();
        var inactiveTypes = allTypes.Where(p => !p.IsActive).Select(MapToDto).ToList();

        var response = new ProjectTypeGroupedResponse
        {
            ActiveTypes = activeTypes,
            InactiveTypes = inactiveTypes,
            TotalActive = activeTypes.Count,
            TotalInactive = inactiveTypes.Count
        };

        _logger.LogInformation("Retrieved grouped project types: Active={Active}, Inactive={Inactive}", 
            activeTypes.Count, inactiveTypes.Count);

        return Ok(response);
    }

    /// <summary>
    /// Maps a ProjectTypeReference entity to ProjectTypeReferenceDto
    /// </summary>
    private static ProjectTypeReferenceDto MapToDto(Persistence.Entities.ProjectTypeReference entity)
    {
        return new ProjectTypeReferenceDto
        {
            Id = entity.Id,
            EnumName = entity.EnumName,
            DisplayName = entity.DisplayName,
            Description = entity.Description,
            IconIdentifier = entity.IconIdentifier,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive,
            ColorCode = entity.ColorCode
        };
    }
}

/// <summary>
/// Response model for project type validation
/// </summary>
public class ProjectTypeValidationResponse
{
    /// <summary>
    /// Indicates if the provided project type is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Error message if validation failed (null if valid)
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Response model for grouped project types
/// </summary>
public class ProjectTypeGroupedResponse
{
    /// <summary>
    /// List of active project types available for use
    /// </summary>
    public List<ProjectTypeReferenceDto> ActiveTypes { get; set; } = new();

    /// <summary>
    /// List of inactive project types (deprecated or no longer used)
    /// </summary>
    public List<ProjectTypeReferenceDto> InactiveTypes { get; set; } = new();

    /// <summary>
    /// Count of active project types
    /// </summary>
    public int TotalActive { get; set; }

    /// <summary>
    /// Count of inactive project types
    /// </summary>
    public int TotalInactive { get; set; }
}
