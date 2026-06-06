using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

/// <summary>
/// Mapping extensions for company entities within the CompanyDetails feature.
/// These mappings are scoped to this feature for proper vertical slice encapsulation.
/// 
/// Architecture Note:
/// - These extensions use database-level LINQ projections for efficiency
/// - They are marked 'internal' to prevent cross-feature dependencies
/// - Each feature maintains its own DTOs and mappings
/// - Moving mapping logic to feature scope maintains performance while improving architecture
/// </summary>
internal static class CompanyMappingExtensions
{
    /// <summary>
    /// Projects a queryable collection of companies to CompanyResponse DTOs at the database level.
    /// This is more efficient than fetching entities and projecting in memory.
    /// </summary>
    public static IQueryable<CompanyResponse> ProjectToCompanyResponse(
        this IQueryable<Company> query)
    {
        return query.Select(c => new CompanyResponse
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Address = c.Address,
            City = c.City,
            Region = c.Region,
            PostalCode = c.PostalCode,
            Country = c.Country,
            Phone = c.Phone,
        });
    }

    /// <summary>
    /// Maps a single company entity to a CompanyResponse DTO.
    /// Used when the entity is already fetched from the database.
    /// </summary>
    public static CompanyResponse MapToCompanyResponse(this Company company)
    {
        ArgumentNullException.ThrowIfNull(company);

        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Description = company.Description,
            Address = company.Address,
            City = company.City,
            Region = company.Region,
            PostalCode = company.PostalCode,
            Country = company.Country,
            Phone = company.Phone,
        };
    }
}
