using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Offices.Models;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Offices.Queries;

/// <summary>
/// Query handler for retrieving a single office by ID
/// </summary>
public interface IGetOfficeQueryHandler
{
    /// <summary>
    /// Retrieves an office by its unique identifier
    /// </summary>
    /// <param name="id">The office ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The office if found</returns>
    /// <exception cref="EntityNotFoundException">Thrown if office is not found</exception>
    Task<OfficeResponse> HandleAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of GetOfficeQueryHandler
/// </summary>
public class GetOfficeQueryHandler : IGetOfficeQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetOfficeQueryHandler> _logger;

    public GetOfficeQueryHandler(
        AppDbContext dbContext,
        ILogger<GetOfficeQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<OfficeResponse> HandleAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving office with ID: {OfficeId}", id);

        var office = await _dbContext.Offices
            .AsNoTracking()
            .Where(o => o.Id == id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (office == null)
        {
            _logger.LogWarning("Office not found with ID: {OfficeId}", id);
            throw new NotFoundException("Office", id);
        }

        _logger.LogInformation("Successfully retrieved office: {OfficeName} (ID: {OfficeId})",
            office.Name, office.Id);

        return office;
    }
}
