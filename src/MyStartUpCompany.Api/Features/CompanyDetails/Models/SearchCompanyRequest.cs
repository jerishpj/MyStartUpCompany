using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Models;

/// <summary>
/// Search/filter and pagination parameters for company queries
/// Accepted by GET /api/companies/search endpoint
/// Excluded from code coverage as it is a data transfer object with no business logic.
/// </summary>
[DisplayName("CompanySearch")]
[ExcludeFromCodeCoverage]
public record SearchCompanyRequest(
    string? Region = null,
    string? Country = null,
    string? City = null,
    string? PostalCode = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null)
{
    /// <summary>
    /// Filter by region
    /// </summary>
    /// <example>CA</example>
    public string? Region { get; init; } = Region;

    /// <summary>
    /// Filter by country
    /// </summary>
    /// <example>United States</example>
    public string? Country { get; init; } = Country;

    /// <summary>
    /// Filter by city
    /// </summary>
    /// <example>San Francisco</example>
    public string? City { get; init; } = City;

    /// <summary>
    /// Filter by postal code
    /// </summary>
    /// <example>94105</example>
    public string? PostalCode { get; init; } = PostalCode;

    /// <summary>
    /// Page number (1-based). Defaults to 1 if not provided, blank, or invalid.
    /// </summary>
    /// <example>1</example>
    public int PageNumber { get; init; } = PageNumber;

    /// <summary>
    /// Number of items per page. Defaults to 10 if not provided, blank, or invalid.
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; init; } = PageSize;

    /// <summary>
    /// Search term for name (optional)
    /// </summary>
    /// <example>Acme</example>
    public string? SearchTerm { get; init; } = SearchTerm;
}

