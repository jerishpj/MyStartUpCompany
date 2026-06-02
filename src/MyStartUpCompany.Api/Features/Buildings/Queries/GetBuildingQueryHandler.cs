using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Buildings.Models;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Buildings.Queries;

/// <summary>
/// Query handler for retrieving a single building by ID
/// </summary>
public interface IGetBuildingQueryHandler
{
    /// <summary>
    /// Retrieves a building by its unique identifier
    /// </summary>
    /// <param name="id">The building ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The building if found</returns>
    /// <exception cref="EntityNotFoundException">Thrown if building is not found</exception>
    Task<Building> HandleAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of GetBuildingQueryHandler
/// </summary>
public class GetBuildingQueryHandler : IGetBuildingQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetBuildingQueryHandler> _logger;

    public GetBuildingQueryHandler(
        AppDbContext dbContext,
        ILogger<GetBuildingQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Building> HandleAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving building with ID: {BuildingId}", id);

        var building = await _dbContext.Buildings
            .AsNoTracking()
            .Where(b => b.Id == id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (building == null)
        {
            _logger.LogWarning("Building not found with ID: {BuildingId}", id);
            throw new NotFoundException(nameof(Building), id);
        }

        _logger.LogInformation("Successfully retrieved building: {BuildingName} (ID: {BuildingId})",
            building.Name, building.Id);

        return building;
    }
}
