using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Features.Buildings.Models;

/// <summary>
/// Response DTO for building information
/// Returned by GET /api/buildings/{id} and GET /api/buildings endpoints
/// Excluded from code coverage as it is a data transfer object with no business logic.
/// </summary>
[DisplayName("Building")]
[ExcludeFromCodeCoverage]
public record BuildingResponse
{
    /// <summary>
    /// Unique identifier for the building
    /// </summary>
    /// <example>1</example>
    public int Id { get; init; }

    /// <summary>
    /// The location ID this building belongs to
    /// </summary>
    /// <example>1</example>
    public int LocationId { get; init; }

    /// <summary>
    /// Building name or identifier
    /// </summary>
    /// <example>Building A</example>
    public required string Name { get; init; }

    /// <summary>
    /// Building code for reference
    /// </summary>
    /// <example>BLD-001</example>
    public string? BuildingCode { get; init; }

    /// <summary>
    /// Building description
    /// </summary>
    /// <example>Main office building with 10 floors</example>
    public string? Description { get; init; }

    /// <summary>
    /// Building address
    /// </summary>
    /// <example>350 Fifth Avenue, Suite 3000</example>
    public required string Address { get; init; }

    /// <summary>
    /// Number of floors
    /// </summary>
    /// <example>10</example>
    public int? NumberOfFloors { get; init; }

    /// <summary>
    /// Year the building was constructed
    /// </summary>
    /// <example>2010</example>
    public int? YearConstructed { get; init; }

    /// <summary>
    /// Total floor area in square meters
    /// </summary>
    /// <example>50000.50</example>
    public decimal? TotalFloorArea { get; init; }

    /// <summary>
    /// Building contact person
    /// </summary>
    /// <example>Jane Doe</example>
    public string? ContactPerson { get; init; }

    /// <summary>
    /// Building contact phone
    /// </summary>
    /// <example>+1-212-555-0101</example>
    public string? Phone { get; init; }

    /// <summary>
    /// Indicates if this building is operational
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; init; }

    /// <summary>
    /// Timestamp when the building record was created
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Timestamp of the last update
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}
