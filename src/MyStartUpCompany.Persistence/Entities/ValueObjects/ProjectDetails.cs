namespace MyStartUpCompany.Persistence.Entities.ValueObjects;

/// <summary>
/// Value object representing project details stored as JSON in the database.
/// Contains non-searchable details that don't require indexing.
/// </summary>
public class ProjectDetails
{
    /// <summary>
    /// Project budget in the organization's default currency
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// Current status of the project (e.g., Planning, Active, OnHold, Completed, Archived)
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// Project start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project end date (estimated or actual)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Project description/summary
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Project manager name or team lead
    /// </summary>
    public string? ProjectManager { get; set; }

    /// <summary>
    /// List of team members involved in the project
    /// </summary>
    public List<string>? TeamMembers { get; set; } = new();

    /// <summary>
    /// Project priority level (e.g., Critical, High, Medium, Low)
    /// </summary>
    public string? Priority { get; set; }

    /// <summary>
    /// Tags for categorization and filtering
    /// </summary>
    public List<string>? Tags { get; set; } = new();

    /// <summary>
    /// Key performance indicators or metrics
    /// </summary>
    public Dictionary<string, string>? Metrics { get; set; } = new();

    /// <summary>
    /// Custom metadata for extensibility
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; } = new();

    /// <summary>
    /// Completion percentage (0-100)
    /// </summary>
    public int ProgressPercentage { get; set; }

    /// <summary>
    /// Notes or comments about the project
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Budget spent so far
    /// </summary>
    public decimal? BudgetSpent { get; set; }

    /// <summary>
    /// Outcome or result of the project (for completed projects)
    /// </summary>
    public string? Outcome { get; set; }

    /// <summary>
    /// Risk level assessment (Low, Medium, High, Critical)
    /// </summary>
    public string? RiskLevel { get; set; }

    /// <summary>
    /// Key deliverables
    /// </summary>
    public List<string>? Deliverables { get; set; } = new();

    /// <summary>
    /// Dependencies on other projects
    /// </summary>
    public List<string>? Dependencies { get; set; } = new();
}
