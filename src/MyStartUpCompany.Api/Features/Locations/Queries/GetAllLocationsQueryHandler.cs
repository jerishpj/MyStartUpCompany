using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Locations.Models;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Locations.Queries;

/// <summary>
/// Query handler for retrieving all locations
/// </summary>
public interface IGetAllLocationsQueryHandler
{
    /// <summary>
    /// Retrieves all locations in the system
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all locations</returns>
    Task<IEnumerable<LocationResponse>> HandleAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of GetAllLocationsQueryHandler
/// </summary>
public class GetAllLocationsQueryHandler : IGetAllLocationsQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetAllLocationsQueryHandler> _logger;

    public GetAllLocationsQueryHandler(
        AppDbContext dbContext,
        ILogger<GetAllLocationsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<LocationResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all locations");

        var locations = await _dbContext.Locations
            .AsNoTracking()
            .OrderBy(l => l.Name)
            .Select(l => new LocationResponse
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
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {LocationCount} locations", locations.Count);

        return locations;
    }
}
