using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Locations.Models;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Locations.Queries;

/// <summary>
/// Query handler for retrieving a single location by ID
/// </summary>
public interface IGetLocationQueryHandler
{
    /// <summary>
    /// Retrieves a location by its unique identifier
    /// </summary>
    /// <param name="id">The location ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The location if found</returns>
    /// <exception cref="EntityNotFoundException">Thrown if location is not found</exception>
    Task<Location> HandleAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of GetLocationQueryHandler
/// </summary>
public class GetLocationQueryHandler : IGetLocationQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetLocationQueryHandler> _logger;

    public GetLocationQueryHandler(
        AppDbContext dbContext,
        ILogger<GetLocationQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Location> HandleAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving location with ID: {LocationId}", id);

        var location = await _dbContext.Locations
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new Location
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
            .FirstOrDefaultAsync(cancellationToken);

        if (location == null)
        {
            _logger.LogWarning("Location not found with ID: {LocationId}", id);
            throw new NotFoundException(nameof(Location), id);
        }

        _logger.LogInformation("Successfully retrieved location: {LocationName} (ID: {LocationId})",
            location.Name, location.Id);

        return location;
    }
}
