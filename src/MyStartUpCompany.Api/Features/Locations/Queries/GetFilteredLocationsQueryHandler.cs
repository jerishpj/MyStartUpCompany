using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Locations.Models;
using MyStartUpCompany.Api.Shared.Models;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Locations.Queries;

/// <summary>
/// Request model for filtering and paginating locations
/// </summary>
public class LocationRequest
{
    /// <summary>
    /// Search term for location name or description
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by company ID
    /// </summary>
    public int? CompanyId { get; set; }

    /// <summary>
    /// Filter by country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Filter by city
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Items per page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Sort field
    /// </summary>
    public string? SortBy { get; set; } = "Name";

    /// <summary>
    /// Sort order (asc/desc)
    /// </summary>
    public string? SortOrder { get; set; } = "asc";
}

/// <summary>
/// Query handler for retrieving filtered and paginated locations
/// </summary>
public interface IGetFilteredLocationsQueryHandler
{
    /// <summary>
    /// Retrieves filtered and paginated locations
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated result of locations</returns>
    Task<PagedResult<Location>> HandleAsync(LocationRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of GetFilteredLocationsQueryHandler
/// </summary>
public class GetFilteredLocationsQueryHandler : IGetFilteredLocationsQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetFilteredLocationsQueryHandler> _logger;

    public GetFilteredLocationsQueryHandler(
        AppDbContext dbContext,
        ILogger<GetFilteredLocationsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<Location>> HandleAsync(
        LocationRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving filtered locations. SearchTerm: {SearchTerm}, CompanyId: {CompanyId}, " +
            "PageNumber: {PageNumber}, PageSize: {PageSize}",
            request.SearchTerm, request.CompanyId, request.PageNumber, request.PageSize);

        var query = _dbContext.Locations.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(l =>
                l.Name.ToLower().Contains(searchTerm) ||
                (l.Description != null && l.Description.ToLower().Contains(searchTerm)));
        }

        if (request.CompanyId.HasValue)
        {
            query = query.Where(l => l.CompanyId == request.CompanyId);
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(l => l.Country.ToLower() == request.Country.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(l => l.City.ToLower() == request.City.ToLower());
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(l => l.IsActive == request.IsActive);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        // Apply pagination
        var skipCount = (request.PageNumber - 1) * request.PageSize;
        var locations = await query
            .Skip(skipCount)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved {LocationCount} locations out of {TotalCount} matching criteria",
            locations.Count, totalCount);

        return new PagedResult<Location>
        {
            Items = locations,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private IQueryable<Persistence.Entities.Location> ApplySorting(
        IQueryable<Persistence.Entities.Location> query,
        string? sortBy,
        string? sortOrder)
    {
        var isDescending = sortOrder?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "name" => isDescending ? query.OrderByDescending(l => l.Name) : query.OrderBy(l => l.Name),
            "city" => isDescending ? query.OrderByDescending(l => l.City) : query.OrderBy(l => l.City),
            "country" => isDescending ? query.OrderByDescending(l => l.Country) : query.OrderBy(l => l.Country),
            "createdat" => isDescending ? query.OrderByDescending(l => l.CreatedAt) : query.OrderBy(l => l.CreatedAt),
            "updatedat" => isDescending ? query.OrderByDescending(l => l.UpdatedAt) : query.OrderBy(l => l.UpdatedAt),
            _ => query.OrderBy(l => l.Name) // Default sort
        };
    }
}
