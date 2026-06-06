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
    Task<IEnumerable<BuildingResponse>> HandleAsync(CancellationToken cancellationToken = default);
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
    public async Task<IEnumerable<BuildingResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all buildings");

        var buildings = await _dbContext.Buildings
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ProjectToBuildingResponse()
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {BuildingCount} buildings", buildings.Count);

        return buildings;
    }
}
