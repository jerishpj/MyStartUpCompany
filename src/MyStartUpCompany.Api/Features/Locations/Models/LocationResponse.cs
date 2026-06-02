using System.ComponentModel;

namespace MyStartUpCompany.Api.Features.Locations.Models;

/// <summary>
/// Response DTO for location/branch information
/// Returned by GET /api/locations/{id} and GET /api/locations endpoints
/// </summary>
[DisplayName("Location")]
public record LocationResponse
{
    /// <summary>
    /// Unique identifier for the location
    /// </summary>
    /// <example>1</example>
    public int Id { get; init; }

    /// <summary>
    /// The company ID this location belongs to
    /// </summary>
    /// <example>1</example>
    public int CompanyId { get; init; }

    /// <summary>
    /// Location name or identifier
    /// </summary>
    /// <example>New York Branch</example>
    public required string Name { get; init; }

    /// <summary>
    /// Brief description of the location
    /// </summary>
    /// <example>Main branch office in New York</example>
    public string? Description { get; init; }

    /// <summary>
    /// Street address
    /// </summary>
    /// <example>350 Fifth Avenue</example>
    public required string Address { get; init; }

    /// <summary>
    /// City where location is situated
    /// </summary>
    /// <example>New York</example>
    public required string City { get; init; }

    /// <summary>
    /// State or region
    /// </summary>
    /// <example>NY</example>
    public string? Region { get; init; }

    /// <summary>
    /// Postal code
    /// </summary>
    /// <example>10118</example>
    public required string PostalCode { get; init; }

    /// <summary>
    /// Country
    /// </summary>
    /// <example>United States</example>
    public required string Country { get; init; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    /// <example>+1-212-555-0100</example>
    public string? Phone { get; init; }

    /// <summary>
    /// Contact email address
    /// </summary>
    /// <example>ny@company.com</example>
    public string? Email { get; init; }

    /// <summary>
    /// Location manager name
    /// </summary>
    /// <example>John Smith</example>
    public string? ManagerName { get; init; }

    /// <summary>
    /// Indicates if this location is active
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; init; }

    /// <summary>
    /// Timestamp when the location was created
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Timestamp of the last update
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}
