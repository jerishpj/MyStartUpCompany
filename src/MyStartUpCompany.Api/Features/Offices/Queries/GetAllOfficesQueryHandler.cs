using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Offices.Models;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Offices.Queries;

/// <summary>
/// Query handler for retrieving all offices
/// </summary>
public interface IGetAllOfficesQueryHandler
{
    /// <summary>
    /// Retrieves all offices in the system
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all offices</returns>
    Task<IEnumerable<OfficeResponse>> HandleAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of GetAllOfficesQueryHandler
/// </summary>
public class GetAllOfficesQueryHandler : IGetAllOfficesQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetAllOfficesQueryHandler> _logger;

    public GetAllOfficesQueryHandler(
        AppDbContext dbContext,
        ILogger<GetAllOfficesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<OfficeResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all offices");

        var offices = await _dbContext.Offices
            .AsNoTracking()
            .OrderBy(o => o.Name)
            .Select(o => new OfficeResponse
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
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {OfficeCount} offices", offices.Count);

        return offices;
    }
}
