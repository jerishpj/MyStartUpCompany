namespace MyStartUpCompany.Persistence.Entities;

/// <summary>
/// Reference data entity for ProjectType enum
/// Provides a centralized definition of all valid project types with descriptions
/// This table is used by:
/// - External applications to discover valid project type values
/// - Reports and BI tools for displaying type information
/// - API documentation and UI dropdowns
/// - Database queries without needing application code knowledge
/// </summary>
public class ProjectTypeReference
{
    /// <summary>
    /// Primary key - matches ProjectType enum value (0-based)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Enum name (e.g., "GameDevelopment", "CloudService")
    /// Matches the ProjectType enum member names exactly
    /// Used by applications to identify the enum value
    /// </summary>
    public required string EnumName { get; set; }

    /// <summary>
    /// Display name for UI/reports (e.g., "Game Development", "Cloud Service")
    /// User-friendly formatted version of EnumName
    /// Used for dropdowns, reports, and UI displays
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Detailed description of this project type
    /// Explains what kind of projects fit into this category
    /// Useful for UI tooltips and API documentation
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Icon or emoji identifier (e.g., "🎮", "☁️", "🎧")
    /// Can be used by UI applications for visual identification
    /// Optional field for future UI enhancements
    /// </summary>
    public string? IconIdentifier { get; set; }

    /// <summary>
    /// Display order for sorting in UI dropdowns and lists
    /// Allows controlling the order projects appear in UI
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Indicates if this project type is currently active/usable
    /// Allows deprecating old project types without deleting data
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Color code for UI categorization (hex format, e.g., "#FF5733")
    /// Can be used for visual differentiation in dashboards and reports
    /// </summary>
    public string? ColorCode { get; set; }

    /// <summary>
    /// When this reference was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this reference was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
