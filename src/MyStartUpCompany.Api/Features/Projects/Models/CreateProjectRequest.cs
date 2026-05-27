namespace MyStartUpCompany.Api.Features.Projects.Models;

/// <summary>
/// Request DTO for creating or updating a Project
/// Maps to the normalized searchable fields; ProjectDetails are constructed internally
/// </summary>
public class CreateProjectRequest
{
    /// <summary>
    /// Unique project identifier (e.g., "PROJ-2024-001")
    /// </summary>
    public required string ProjectIdentifier { get; set; }

    /// <summary>
    /// Project name/title
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Project code/abbreviation (e.g., "DVP", "MKT")
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Project location/site
    /// </summary>
    public required string Location { get; set; }

    /// <summary>
    /// Company ID this project belongs to
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Project details (non-searchable, stored as JSON internally)
    /// </summary>
    public ProjectDetailsDto Details { get; set; } = new();
}

/// <summary>
/// Data Transfer Object for Project details
/// Mirrors ProjectDetails value object for JSON serialization
/// </summary>
public class ProjectDetailsDto
{
    public decimal Budget { get; set; }
    public string? Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public string? ProjectManager { get; set; }
    public List<string>? TeamMembers { get; set; }
    public string? Priority { get; set; }
    public List<string>? Tags { get; set; }
    public Dictionary<string, string>? Metrics { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public int ProgressPercentage { get; set; }
    public string? Notes { get; set; }
    public decimal? BudgetSpent { get; set; }
    public string? Outcome { get; set; }
    public string? RiskLevel { get; set; }
    public List<string>? Deliverables { get; set; }
    public List<string>? Dependencies { get; set; }
}
