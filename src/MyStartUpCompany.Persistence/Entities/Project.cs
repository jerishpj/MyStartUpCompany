using MyStartUpCompany.Persistence.Entities.ValueObjects;

namespace MyStartUpCompany.Persistence.Entities;

/// <summary>
/// Project entity with hybrid storage strategy:
/// - Normalized searchable columns: ProjectIdentifier, Name, Code, Location (indexed)
/// - JSON column: ProjectDetails containing non-searchable fields
/// This approach optimizes query performance while maintaining flexibility for evolving details.
/// </summary>
public class Project
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique project identifier (e.g., "PROJ-2024-001")
    /// Indexed for fast lookups by external systems
    /// </summary>
    public required string ProjectIdentifier { get; set; }

    /// <summary>
    /// Project name/title
    /// Indexed for search and sorting
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Project code/abbreviation (e.g., "DVP", "MKT")
    /// Indexed for filtering by project type/category
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Project location/site (e.g., "San Francisco", "Remote", "New York")
    /// Indexed for location-based queries
    /// </summary>
    public required string Location { get; set; }

    /// <summary>
    /// Foreign key to Company - each project belongs to a company
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Navigation property to Company
    /// </summary>
    public virtual Company? Company { get; set; }

    /// <summary>
    /// Project details stored as JSON
    /// Includes: Budget, Status, Dates, Team, Tags, Metrics, Metadata, etc.
    /// Serialized/deserialized automatically by EF Core
    /// </summary>
    public ProjectDetails Details { get; set; } = new() { Status = "Planning" };

    /// <summary>
    /// When the project record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the project record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
