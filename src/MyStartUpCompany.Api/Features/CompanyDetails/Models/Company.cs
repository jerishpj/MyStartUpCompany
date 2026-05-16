using System.ComponentModel;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Models;

/// <summary>
/// Represents a company's detailed information
/// </summary>
[DisplayName("Company")]
public record Company
{
    /// <summary>
    /// The unique identifier for the company
    /// </summary>
    /// <example>1</example>
    public int Id { get; init; }

    /// <summary>
    /// The company's official name
    /// </summary>
    /// <example>Acme Corporation</example>
    public required string Name { get; init; }

    /// <summary>
    /// A brief description of the company's business
    /// </summary>
    /// <example>Leading provider of innovative technology solutions</example>
    public string? Description { get; init; }

    /// <summary>
    /// Street address of the company's primary location
    /// </summary>
    /// <example>123 Main Street</example>
    public required string Address { get; init; }

    /// <summary>
    /// City where the company is located
    /// </summary>
    /// <example>San Francisco</example>
    public required string City { get; init; }

    /// <summary>
    /// State or region (optional)
    /// </summary>
    /// <example>CA</example>
    public string? Region { get; init; }

    /// <summary>
    /// Postal or ZIP code
    /// </summary>
    /// <example>94105</example>
    public required string PostalCode { get; init; }

    /// <summary>
    /// Country where the company operates
    /// </summary>
    /// <example>United States</example>
    public required string Country { get; init; }

    /// <summary>
    /// Primary contact phone number
    /// </summary>
    /// <example>+1-555-123-4567</example>
    public required string Phone { get; init; }

    /// <summary>
    /// Timestamp when the company record was created
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Timestamp of the last update (null if never updated)
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}
