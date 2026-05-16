using System.ComponentModel;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Models;

/// <summary>
/// Filter and pagination parameters for company search
/// </summary>
[DisplayName("CompanyRequest")]
public record CompanyRequest
{
    /// <summary>
    /// Filter by region
    /// </summary>
    /// <example>CA</example>
    public string? Region { get; init; }

    /// <summary>
    /// Filter by country
    /// </summary>
    /// <example>United States</example>
    public string? Country { get; init; }

    /// <summary>
    /// Filter by city
    /// </summary>
    /// <example>San Francisco</example>
    public string? City { get; init; }

    /// <summary>
    /// Filter by postal code
    /// </summary>
    /// <example>94105</example>
    public string? PostalCode { get; init; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    /// <example>1</example>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Search term for name (optional)
    /// </summary>
    /// <example>Acme</example>
    public string? SearchTerm { get; init; }
}