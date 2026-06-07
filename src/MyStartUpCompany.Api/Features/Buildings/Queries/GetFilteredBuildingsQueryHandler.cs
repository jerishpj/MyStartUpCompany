using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Common.Utilities;
using MyStartUpCompany.Api.Features.Buildings.Models;
using MyStartUpCompany.Api.Shared.Models;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Buildings.Queries;

/// <summary>
/// Query handler for retrieving filtered and paginated buildings
/// </summary>
public interface IGetFilteredBuildingsQueryHandler
{
    /// <summary>
    /// Retrieves filtered and paginated buildings
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated result of buildings</returns>
    Task<PagedResult<BuildingResponse>> HandleAsync(SearchBuildingRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of GetFilteredBuildingsQueryHandler
/// </summary>
public class GetFilteredBuildingsQueryHandler : IGetFilteredBuildingsQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetFilteredBuildingsQueryHandler> _logger;

    public GetFilteredBuildingsQueryHandler(
        AppDbContext dbContext,
        ILogger<GetFilteredBuildingsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<BuildingResponse>> HandleAsync(
        SearchBuildingRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving filtered buildings. SearchTerm: {SearchTerm}, LocationId: {LocationId}, " +
            "OfficeCodes: {OfficeCodes}, PageNumber: {PageNumber}, PageSize: {PageSize}",
            request.SearchTerm, request.LocationId, 
            string.Join(",", request.OfficeCodes ?? Enumerable.Empty<string>()),
            request.PageNumber, request.PageSize);

        var query = _dbContext.Buildings.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = SqlLikeHelper.CreateLikePattern(request.SearchTerm);
            query = query.Where(b =>
                EF.Functions.Like(b.Name, pattern) ||
                (b.Description != null && EF.Functions.Like(b.Description, pattern)));
        }

        if (request.LocationId.HasValue)
        {
            query = query.Where(b => b.LocationId == request.LocationId);
        }

        if (!string.IsNullOrWhiteSpace(request.BuildingCode))
        {
            var pattern = SqlLikeHelper.CreateExactPattern(request.BuildingCode);
            query = query.Where(b => EF.Functions.Like(b.BuildingCode, pattern));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(b => b.IsActive == request.IsActive);
        }

        // Filter by office codes if provided
        if (request.OfficeCodes != null && request.OfficeCodes.Any())
        {
            var officeCodes = request.OfficeCodes.ToList();
            query = query.Where(b => b.Offices!.Any(o => 
                o.OfficeCode != null && officeCodes.Contains(o.OfficeCode)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        // Apply pagination
        var skipCount = (request.PageNumber - 1) * request.PageSize;
        var buildings = await query
            .Skip(skipCount)
            .Take(request.PageSize)
            .Select(b => new BuildingResponse
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

        _logger.LogInformation(
            "Retrieved {BuildingCount} buildings out of {TotalCount} matching criteria",
            buildings.Count, totalCount);

        return new PagedResult<BuildingResponse>
        {
            Items = buildings,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private IQueryable<Persistence.Entities.Building> ApplySorting(
        IQueryable<Persistence.Entities.Building> query,
        string? sortBy,
        string? sortOrder)
    {
        var isDescending = sortOrder?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "name" => isDescending ? query.OrderByDescending(b => b.Name) : query.OrderBy(b => b.Name),
            "code" => isDescending ? query.OrderByDescending(b => b.BuildingCode) : query.OrderBy(b => b.BuildingCode),
            "floors" => isDescending ? query.OrderByDescending(b => b.NumberOfFloors) : query.OrderBy(b => b.NumberOfFloors),
            "createdat" => isDescending ? query.OrderByDescending(b => b.CreatedAt) : query.OrderBy(b => b.CreatedAt),
            "updatedat" => isDescending ? query.OrderByDescending(b => b.UpdatedAt) : query.OrderBy(b => b.UpdatedAt),
            _ => query.OrderBy(b => b.Name) // Default sort
        };
    }
}
