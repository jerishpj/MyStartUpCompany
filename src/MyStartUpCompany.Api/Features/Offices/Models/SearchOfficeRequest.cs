using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Features.Offices.Models;

/// <summary>
/// Search/filter and pagination parameters for office queries
/// Accepted by GET /api/offices/search endpoint
/// Excluded from code coverage as it is a data transfer object with no business logic.
/// </summary>
[DisplayName("OfficeSearch")]
[ExcludeFromCodeCoverage]
public record SearchOfficeRequest(
    string? SearchTerm = null,
    int? BuildingId = null,
    string? Department = null,
    string? OfficeType = null,
    bool? IsActive = null,
    string? BuildingName = null,
    string? LocationCity = null,
    string? LocationRegion = null,
    string? LocationCountry = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = "Name",
    string? SortOrder = "asc")
{
    /// <summary>
    /// Search term for office name or description
    /// </summary>
    public string? SearchTerm { get; init; } = SearchTerm;

    /// <summary>
    /// Filter by building ID
    /// </summary>
    public int? BuildingId { get; init; } = BuildingId;

    /// <summary>
    /// Filter by department
    /// </summary>
    public string? Department { get; init; } = Department;

    /// <summary>
    /// Filter by office type
    /// </summary>
    public string? OfficeType { get; init; } = OfficeType;

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; init; } = IsActive;

    /// <summary>
    /// Filter by building name (denormalized field for fast search without joins).
    /// Supports partial/wildcard matching.
    /// </summary>
    public string? BuildingName { get; init; } = BuildingName;

    /// <summary>
    /// Filter by location city (denormalized field for geographic filtering).
    /// Exact match search.
    /// </summary>
    public string? LocationCity { get; init; } = LocationCity;

    /// <summary>
    /// Filter by location region/state (denormalized field for geographic filtering).
    /// Exact match search.
    /// </summary>
    public string? LocationRegion { get; init; } = LocationRegion;

    /// <summary>
    /// Filter by location country (denormalized field for geographic filtering).
    /// Exact match search.
    /// </summary>
    public string? LocationCountry { get; init; } = LocationCountry;

    /// <summary>
    /// Page number (1-based). Validated by SearchOfficeRequestValidator.
    /// </summary>
    public int PageNumber { get; init; } = PageNumber;

    /// <summary>
    /// Number of items per page. Validated by SearchOfficeRequestValidator.
    /// </summary>
    public int PageSize { get; init; } = PageSize;

    /// <summary>
    /// Sort field
    /// </summary>
    public string? SortBy { get; init; } = SortBy;

    /// <summary>
    /// Sort order (asc/desc)
    /// </summary>
    public string? SortOrder { get; init; } = SortOrder;

    /// <summary>
    /// Silently defaults PageNumber to 1 if the provided value is invalid or non-positive.
    /// </summary>
    private static int ValidatePageNumber(int pageNumber)
    {
        return pageNumber > 0 ? pageNumber : 1;
    }

    /// <summary>
    /// Silently defaults PageSize to 10 if the provided value is invalid or non-positive.
    /// </summary>
    private static int ValidatePageSize(int pageSize)
    {
        return pageSize > 0 ? pageSize : 10;
    }
}
