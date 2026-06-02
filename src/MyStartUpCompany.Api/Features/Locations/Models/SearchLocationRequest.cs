using System.ComponentModel;

namespace MyStartUpCompany.Api.Features.Locations.Models;

/// <summary>
/// Search/filter and pagination parameters for location queries
/// Accepted by GET /api/locations/search endpoint
/// </summary>
[DisplayName("LocationSearch")]
public record SearchLocationRequest
{
    /// <summary>
    /// Search term for location name or description
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by company ID
    /// </summary>
    public int? CompanyId { get; init; }

    /// <summary>
    /// Filter by country
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// Filter by city
    /// </summary>
    public string? City { get; init; }

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
