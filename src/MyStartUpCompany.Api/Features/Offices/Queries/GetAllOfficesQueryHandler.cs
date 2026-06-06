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
            .ProjectToOfficeResponse()
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {OfficeCount} offices", offices.Count);

        return offices;
    }
}
