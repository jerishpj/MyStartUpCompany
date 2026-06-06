using MyStartUpCompany.Api.Features.Buildings.Models;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Features.Buildings.Queries;

/// <summary>
/// Mapping extensions for building entities within the Buildings feature.
/// These mappings are scoped to this feature for proper vertical slice encapsulation.
/// 
/// Architecture Note:
/// - These extensions use database-level LINQ projections for efficiency
/// - They are marked 'internal' to prevent cross-feature dependencies
/// - Maintains feature independence while preserving performance benefits
/// </summary>
internal static class BuildingMappingExtensions
{
    /// <summary>
    /// Projects a queryable collection of buildings to BuildingResponse DTOs at the database level.
    /// </summary>
    public static IQueryable<BuildingResponse> ProjectToBuildingResponse(
        this IQueryable<Building> query)
    {
        return query.Select(b => new BuildingResponse
        {
            Id = b.Id,
            LocationId = b.LocationId,
            Name = b.Name,
            BuildingCode = b.BuildingCode,
            Description = b.Description,
            Address = b.Address,
            NumberOfFloors = b.NumberOfFloors,
            YearConstructed = b.YearConstructed,
            TotalFloorArea = b.TotalFloorArea,
            ContactPerson = b.ContactPerson,
            Phone = b.Phone,
            IsActive = b.IsActive,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        });
    }

    /// <summary>
    /// Maps a single building entity to a BuildingResponse DTO.
    /// </summary>
    public static BuildingResponse MapToBuildingResponse(this Building building)
    {
        ArgumentNullException.ThrowIfNull(building);

        return new BuildingResponse
        {
            Id = building.Id,
            LocationId = building.LocationId,
            Name = building.Name,
            BuildingCode = building.BuildingCode,
            Description = building.Description,
            Address = building.Address,
            NumberOfFloors = building.NumberOfFloors,
            YearConstructed = building.YearConstructed,
            TotalFloorArea = building.TotalFloorArea,
            ContactPerson = building.ContactPerson,
            Phone = building.Phone,
            IsActive = building.IsActive,
            CreatedAt = building.CreatedAt,
            UpdatedAt = building.UpdatedAt
        };
    }
}
