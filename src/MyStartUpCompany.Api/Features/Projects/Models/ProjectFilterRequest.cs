namespace MyStartUpCompany.Api.Features.Projects.Models;

/// <summary>
/// Query/filter parameters for project list endpoints
/// Maps to searchable columns: ProjectIdentifier, Code, Location, CompanyId, Type
/// </summary>
public class ProjectFilterRequest
{
    /// <summary>
    /// Filter by project identifier (partial match supported)
    /// </summary>
    public string? ProjectIdentifier { get; set; }

    /// <summary>
    /// Filter by project name (partial match supported)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filter by project code (exact match)
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Filter by location (exact match)
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Filter by company ID
    /// </summary>
    public int? CompanyId { get; set; }

    /// <summary>
    /// Filter by project type (exact match)
    /// If not specified, all project types are included
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Sort order: "asc" or "desc" (default: "asc")
    /// </summary>
    public string SortOrder { get; set; } = "asc";

    /// <summary>
    /// Sort by field: "Name", "Code", "Location", "CreatedAt" (default: "Name")
    /// </summary>
    public string SortBy { get; set; } = "Name";

    /// <summary>
    /// Page number (1-based, default: 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size (default: 20, max: 100)
    /// </summary>
    public int PageSize { get; set; } = 20;
}
