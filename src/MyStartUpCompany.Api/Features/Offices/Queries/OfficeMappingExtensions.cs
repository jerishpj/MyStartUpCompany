using MyStartUpCompany.Api.Features.Offices.Models;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Features.Offices.Queries;

/// <summary>
/// Mapping extensions for office entities within the Offices feature.
/// These mappings are scoped to this feature for proper vertical slice encapsulation.
/// 
/// Architecture Note:
/// - These extensions use database-level LINQ projections for efficiency
/// - They are marked 'internal' to prevent cross-feature dependencies
/// - Maintains feature independence while preserving performance benefits
/// </summary>
internal static class OfficeMappingExtensions
{
    /// <summary>
    /// Projects a queryable collection of offices to OfficeResponse DTOs at the database level.
    /// </summary>
    public static IQueryable<OfficeResponse> ProjectToOfficeResponse(
        this IQueryable<Office> query)
    {
        return query.Select(o => new OfficeResponse
        {
            Id = o.Id,
            BuildingId = o.BuildingId,
            Name = o.Name,
            OfficeCode = o.OfficeCode,
            Description = o.Description,
            FloorNumber = o.FloorNumber,
            Section = o.Section,
            Capacity = o.Capacity,
            OfficeType = o.OfficeType,
            SquareMeters = o.SquareMeters,
            Department = o.Department,
            Manager = o.Manager,
            Phone = o.Phone,
            Email = o.Email,
            IsActive = o.IsActive,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        });
    }

    /// <summary>
    /// Maps a single office entity to an OfficeResponse DTO.
    /// </summary>
    public static OfficeResponse MapToOfficeResponse(this Office office)
    {
        ArgumentNullException.ThrowIfNull(office);

        return new OfficeResponse
        {
            Id = office.Id,
            BuildingId = office.BuildingId,
            Name = office.Name,
            OfficeCode = office.OfficeCode,
            Description = office.Description,
            FloorNumber = office.FloorNumber,
            Section = office.Section,
            Capacity = office.Capacity,
            OfficeType = office.OfficeType,
            SquareMeters = office.SquareMeters,
            Department = office.Department,
            Manager = office.Manager,
            Phone = office.Phone,
            Email = office.Email,
            IsActive = office.IsActive,
            CreatedAt = office.CreatedAt,
            UpdatedAt = office.UpdatedAt
        };
    }
}
