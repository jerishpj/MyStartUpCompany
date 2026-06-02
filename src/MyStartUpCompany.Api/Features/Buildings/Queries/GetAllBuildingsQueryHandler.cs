using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Buildings.Models;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Buildings.Queries;

/// <summary>
/// Query handler for retrieving all buildings
/// </summary>
public interface IGetAllBuildingsQueryHandler
{
    /// <summary>
    /// Retrieves all buildings in the system
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all buildings</returns>
    Task<IEnumerable<Building>> HandleAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of GetAllBuildingsQueryHandler
/// </summary>
public class GetAllBuildingsQueryHandler : IGetAllBuildingsQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetAllBuildingsQueryHandler> _logger;

    public GetAllBuildingsQueryHandler(
        AppDbContext dbContext,
        ILogger<GetAllBuildingsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Building>> HandleAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all buildings");

        var buildings = await _dbContext.Buildings
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new Building
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
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {BuildingCount} buildings", buildings.Count);

        return buildings;
    }
}
