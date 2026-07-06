using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Features.Locations.Models;

/// <summary>
/// Search/filter and pagination parameters for location queries
/// Accepted by GET /api/locations/search endpoint
/// Excluded from code coverage as it is a data transfer object with no business logic.
/// </summary>
[DisplayName("LocationSearch")]
[ExcludeFromCodeCoverage]
public record SearchLocationRequest(
    string? SearchTerm = null,
    int? CompanyId = null,
    string? Country = null,
    string? City = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = "Name",
    string? SortOrder = "asc")
{
    /// <summary>
    /// Search term for location name or description
    /// </summary>
    public string? SearchTerm { get; init; } = SearchTerm;

    /// <summary>
    /// Filter by company ID
    /// </summary>
    public int? CompanyId { get; init; } = CompanyId;

    /// <summary>
    /// Filter by country
    /// </summary>
    public string? Country { get; init; } = Country;

    /// <summary>
    /// Filter by city
    /// </summary>
    public string? City { get; init; } = City;

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; init; } = IsActive;

    /// <summary>
    /// Page number (1-based). Validated by SearchLocationRequestValidator.
    /// </summary>
    public int PageNumber { get; init; } = PageNumber;

    /// <summary>
    /// Number of items per page. Validated by SearchLocationRequestValidator.
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

