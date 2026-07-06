using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Features.Projects.Models;

/// <summary>
/// Response DTO for ProjectType reference data
/// Used by external applications, reports, and UI to understand valid project types
/// Excluded from code coverage as it is a data transfer object with no business logic.
/// </summary>
[ExcludeFromCodeCoverage]
public class ProjectTypeReferenceDto
{
    /// <summary>
    /// Unique identifier (1-based)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Enum member name (e.g., "GameDevelopment", "CloudService")
    /// Use this value when filtering projects or setting project types
    /// </summary>
    public required string EnumName { get; set; }

    /// <summary>
    /// User-friendly display name (e.g., "Game Development", "Cloud Service")
    /// Recommended for UI labels, dropdowns, and reports
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Detailed description of this project type
    /// Explains what kind of projects fit into this category
    /// Useful for UI tooltips and help text
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Icon or emoji identifier (e.g., "🎮", "☁️")
    /// Can be used by UI applications for visual identification
    /// </summary>
    public string? IconIdentifier { get; set; }

    /// <summary>
    /// Display order for sorting in UI dropdowns and lists
    /// Lower values appear first
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Indicates if this project type is currently active/usable
    /// Only active types should be used for new projects
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Color code for UI categorization (hex format, e.g., "#FF5733")
    /// Can be used for visual differentiation in dashboards and reports
    /// </summary>
    public string? ColorCode { get; set; }
}

/// <summary>
/// Response wrapper for project type reference data list
/// </summary>
public class ProjectTypeReferenceListResponse
{
    /// <summary>
    /// Total number of project types available
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// List of all available project types
    /// </summary>
    public List<ProjectTypeReferenceDto> ProjectTypes { get; set; } = new();

    /// <summary>
    /// Timestamp when this reference data was last updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// API version for reference data (for caching and versioning)
    /// </summary>
    public string ApiVersion { get; set; } = "1.0";
}
