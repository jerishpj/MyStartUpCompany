using System.ComponentModel;

namespace MyStartUpCompany.Api.Features.Buildings.Models;

/// <summary>
/// Search/filter and pagination parameters for building queries
/// Accepted by GET /api/buildings/search endpoint
/// </summary>
[DisplayName("BuildingSearch")]
public record SearchBuildingRequest(
    string? SearchTerm = null,
    int? LocationId = null,
    string? BuildingCode = null,
    bool? IsActive = null,
    IEnumerable<string>? OfficeCodes = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = "Name",
    string? SortOrder = "asc")
{
    /// <summary>
    /// Search term for building name or description
    /// </summary>
    public string? SearchTerm { get; init; } = SearchTerm;

    /// <summary>
    /// Filter by location ID
    /// </summary>
    public int? LocationId { get; init; } = LocationId;

    /// <summary>
    /// Filter by building code
    /// </summary>
    public string? BuildingCode { get; init; } = BuildingCode;

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; init; } = IsActive;

    /// <summary>
    /// Filter by office codes (collection). Returns buildings that contain offices with any of these codes.
    /// </summary>
    public IEnumerable<string>? OfficeCodes { get; init; } = OfficeCodes;

    /// <summary>
    /// Page number (1-based). Defaults to 1 if not provided or invalid.
    /// </summary>
    public int PageNumber { get; init; } = ValidatePageNumber(PageNumber);

    /// <summary>
    /// Number of items per page. Defaults to 10 if not provided or invalid.
    /// </summary>
    public int PageSize { get; init; } = ValidatePageSize(PageSize);

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

