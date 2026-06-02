using System.ComponentModel;

namespace MyStartUpCompany.Api.Features.Buildings.Models;

/// <summary>
/// Search/filter and pagination parameters for building queries
/// Accepted by GET /api/buildings/search endpoint
/// </summary>
[DisplayName("BuildingSearch")]
public record SearchBuildingRequest
{
    /// <summary>
    /// Search term for building name or description
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by location ID
    /// </summary>
    public int? LocationId { get; init; }

    /// <summary>
    /// Filter by building code
    /// </summary>
    public string? BuildingCode { get; init; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    /// Page number (1-based). Defaults to 1 if not provided or invalid.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page. Defaults to 10 if not provided or invalid.
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Sort field
    /// </summary>
    public string? SortBy { get; init; } = "Name";

    /// <summary>
    /// Sort order (asc/desc)
    /// </summary>
    public string? SortOrder { get; init; } = "asc";
}
