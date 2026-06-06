using MyStartUpCompany.Api.Features.Locations.Models;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Features.Locations.Queries;

/// <summary>
/// Mapping extensions for location entities within the Locations feature.
/// These mappings are scoped to this feature for proper vertical slice encapsulation.
/// 
/// Architecture Note:
/// - These extensions use database-level LINQ projections for efficiency
/// - They are marked 'internal' to prevent cross-feature dependencies
/// - Maintains feature independence while preserving performance benefits
/// </summary>
internal static class LocationMappingExtensions
{
    /// <summary>
    /// Projects a queryable collection of locations to LocationResponse DTOs at the database level.
    /// </summary>
    public static IQueryable<LocationResponse> ProjectToLocationResponse(
        this IQueryable<Location> query)
    {
        return query.Select(l => new LocationResponse
        {
            Id = l.Id,
            CompanyId = l.CompanyId,
            Name = l.Name,
            Description = l.Description,
            Address = l.Address,
            City = l.City,
            Region = l.Region,
            PostalCode = l.PostalCode,
            Country = l.Country,
            Phone = l.Phone,
            Email = l.Email,
            ManagerName = l.ManagerName,
            IsActive = l.IsActive,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt
        });
    }

    /// <summary>
    /// Maps a single location entity to a LocationResponse DTO.
    /// </summary>
    public static LocationResponse MapToLocationResponse(this Location location)
    {
        ArgumentNullException.ThrowIfNull(location);

        return new LocationResponse
        {
            Id = location.Id,
            CompanyId = location.CompanyId,
            Name = location.Name,
            Description = location.Description,
            Address = location.Address,
            City = location.City,
            Region = location.Region,
            PostalCode = location.PostalCode,
            Country = location.Country,
            Phone = location.Phone,
            Email = location.Email,
            ManagerName = location.ManagerName,
            IsActive = location.IsActive,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }
}
