using System.ComponentModel;

namespace MyStartUpCompany.Api.Features.Offices.Models;

/// <summary>
/// Search/filter and pagination parameters for office queries
/// Accepted by GET /api/offices/search endpoint
/// </summary>
[DisplayName("OfficeSearch")]
public record SearchOfficeRequest
{
    /// <summary>
    /// Search term for office name or description
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by building ID
    /// </summary>
    public int? BuildingId { get; init; }

    /// <summary>
    /// Filter by department
    /// </summary>
    public string? Department { get; init; }

    /// <summary>
    /// Filter by office type
    /// </summary>
    public string? OfficeType { get; init; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    /// Filter by building name (denormalized field for fast search without joins).
    /// Supports partial/wildcard matching.
    /// </summary>
    public string? BuildingName { get; init; }

    /// <summary>
    /// Filter by location city (denormalized field for geographic filtering).
    /// Exact match search.
    /// </summary>
    public string? LocationCity { get; init; }

    /// <summary>
    /// Filter by location region/state (denormalized field for geographic filtering).
    /// Exact match search.
    /// </summary>
    public string? LocationRegion { get; init; }

    /// <summary>
    /// Filter by location country (denormalized field for geographic filtering).
    /// Exact match search.
    /// </summary>
    public string? LocationCountry { get; init; }

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
