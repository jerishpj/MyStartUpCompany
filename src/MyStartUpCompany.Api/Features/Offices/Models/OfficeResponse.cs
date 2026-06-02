using System.ComponentModel;

namespace MyStartUpCompany.Api.Features.Offices.Models;

/// <summary>
/// Response DTO for office information
/// Returned by GET /api/offices/{id} and GET /api/offices endpoints
/// </summary>
[DisplayName("Office")]
public record OfficeResponse
{
    /// <summary>
    /// Unique identifier for the office
    /// </summary>
    /// <example>1</example>
    public int Id { get; init; }

    /// <summary>
    /// The building ID this office belongs to
    /// </summary>
    /// <example>1</example>
    public int BuildingId { get; init; }

    /// <summary>
    /// Office name or identifier
    /// </summary>
    /// <example>Floor 3 - Sales Department</example>
    public required string Name { get; init; }

    /// <summary>
    /// Office code for reference
    /// </summary>
    /// <example>F3-SALES-01</example>
    public string? OfficeCode { get; init; }

    /// <summary>
    /// Office description
    /// </summary>
    /// <example>Main sales office with 20 workstations</example>
    public string? Description { get; init; }

    /// <summary>
    /// Floor number
    /// </summary>
    /// <example>3</example>
    public int? FloorNumber { get; init; }

    /// <summary>
    /// Section or wing identifier
    /// </summary>
    /// <example>Wing A</example>
    public string? Section { get; init; }

    /// <summary>
    /// Office capacity (number of workstations)
    /// </summary>
    /// <example>20</example>
    public int? Capacity { get; init; }

    /// <summary>
    /// Office type
    /// </summary>
    /// <example>Open Office</example>
    public string? OfficeType { get; init; }

    /// <summary>
    /// Office area in square meters
    /// </summary>
    /// <example>500.00</example>
    public decimal? SquareMeters { get; init; }

    /// <summary>
    /// Department or team name
    /// </summary>
    /// <example>Sales</example>
    public string? Department { get; init; }

    /// <summary>
    /// Office manager name
    /// </summary>
    /// <example>John Doe</example>
    public string? Manager { get; init; }

    /// <summary>
    /// Office contact phone
    /// </summary>
    /// <example>+1-212-555-0102</example>
    public string? Phone { get; init; }

    /// <summary>
    /// Office contact email
    /// </summary>
    /// <example>sales@company.com</example>
    public string? Email { get; init; }

    /// <summary>
    /// Indicates if this office is operational
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; init; }

    /// <summary>
    /// Timestamp when the office record was created
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Timestamp of the last update
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}
