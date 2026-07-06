using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace MyStartUpCompany.Api.Features.Buildings.Models;

/// <summary>
/// Search/filter and pagination parameters for building queries
/// Accepted by GET /api/buildings/search endpoint
/// Excluded from code coverage as it is a data transfer object with no business logic.
/// </summary>
[DisplayName("BuildingSearch")]
[ExcludeFromCodeCoverage]
public record SearchBuildingRequest(
    string? SearchTerm = null,
    int? LocationId = null,
    string? BuildingCode = null,
    bool? IsActive = null,
    [FromQuery(Name = "officeCodes")] IEnumerable<string>? OfficeCodes = null,
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
    /// Send multiple values using: ?officeCodes=CODE1&officeCodes=CODE2&officeCodes=CODE3
    /// </summary>
    [FromQuery(Name = "officeCodes")]
    public IEnumerable<string>? OfficeCodes { get; init; } = OfficeCodes;

    /// <summary>
    /// Page number (1-based). Validated by SearchBuildingRequestValidator.
    /// </summary>
    public int PageNumber { get; init; } = PageNumber;

    /// <summary>
    /// Number of items per page. Validated by SearchBuildingRequestValidator.
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
}

