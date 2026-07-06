using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Features.Projects.Models;

/// <summary>
/// Response DTO for a single Project
/// Presents the complete project data in normal deserialized format
/// Excluded from code coverage as it is a data transfer object with no business logic.
/// </summary>
[ExcludeFromCodeCoverage]
public class ProjectResponse
{
    /// <summary>
    /// Project primary key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique project identifier
    /// </summary>
    public required string ProjectIdentifier { get; set; }

    /// <summary>
    /// Project name/title
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Project code
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Project location
    /// </summary>
    public required string Location { get; set; }

    /// <summary>
    /// Company ID
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Type/category of the project (e.g., "GameDevelopment", "CloudService", "DataAnalytics")
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Project details (deserialized from JSON)
    /// </summary>
    public ProjectDetailsDto Details { get; set; } = new();

    /// <summary>
    /// When the project was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the project was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Response DTO for paginated project list
/// </summary>
public class ProjectListResponse
{
    /// <summary>
    /// Total count of projects matching the query
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// List of projects in the current page
    /// </summary>
    public List<ProjectResponse> Projects { get; set; } = new();
}
