using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Common.Utilities;
using MyStartUpCompany.Api.Features.Locations.Models;
using MyStartUpCompany.Api.Shared.Models;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Locations.Queries;

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
    Task<PagedResult<LocationResponse>> HandleAsync(SearchLocationRequest request, CancellationToken cancellationToken = default);
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
    public async Task<PagedResult<LocationResponse>> HandleAsync(
        SearchLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Retrieving filtered locations - CompanyId: {CompanyId}, PageNumber: {PageNumber}, PageSize: {PageSize}",
            request.CompanyId, request.PageNumber, request.PageSize);

        // Build optimized query with database-level filters
        var query = BuildQuery(request);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting and pagination, then project to DTO
        var locations = await query
            .ApplySorting(request.SortBy, request.SortOrder)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToLocationResponse()
            .ToListAsync(cancellationToken);

        _logger.LogDebug(
            "Retrieved {LocationCount} locations out of {TotalCount}",
            locations.Count, totalCount);

        return new PagedResult<LocationResponse>
        {
            Items = locations,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Builds the optimized query with database-level filters.
    /// Applies most selective filters first for better query optimization.
    /// </summary>
    private IQueryable<Persistence.Entities.Location> BuildQuery(SearchLocationRequest request)
    {
        var query = _dbContext.Locations.AsNoTracking();

        // Apply filters in order of selectivity (most selective first)

        // CompanyId - most selective (indexed foreign key)
        if (request.CompanyId.HasValue)
        {
            query = query.Where(l => l.CompanyId == request.CompanyId);
        }

        // Country - exact match is more selective than substring search
        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            var pattern = SqlLikeHelper.CreateExactPattern(request.Country);
            query = query.Where(l => EF.Functions.Like(l.Country, pattern));
        }

        // City - exact match
        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var pattern = SqlLikeHelper.CreateExactPattern(request.City);
            query = query.Where(l => EF.Functions.Like(l.City, pattern));
        }

        // IsActive - boolean filter (fast)
        if (request.IsActive.HasValue)
        {
            query = query.Where(l => l.IsActive == request.IsActive);
        }

        // SearchTerm - least selective (substring search)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = SqlLikeHelper.CreateLikePattern(request.SearchTerm);
            query = query.Where(l =>
                EF.Functions.Like(l.Name, pattern) ||
                (l.Description != null && EF.Functions.Like(l.Description, pattern)));
        }

        return query;
    }
}

/// <summary>
/// Extension methods for sorting locations queries.
/// </summary>
internal static class LocationSortingExtensions
{
    public static IQueryable<Persistence.Entities.Location> ApplySorting(
        this IQueryable<Persistence.Entities.Location> query,
        string? sortBy,
        string? sortOrder)
    {
        var isDescending = sortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

        return (sortBy?.ToLower()) switch
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
