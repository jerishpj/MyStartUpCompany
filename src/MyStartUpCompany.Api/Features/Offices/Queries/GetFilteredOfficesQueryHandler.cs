using System;
using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Offices.Models;
using MyStartUpCompany.Api.Shared.Models;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Offices.Queries;

/// <summary>
/// Query handler for retrieving filtered and paginated offices
/// </summary>
public interface IGetFilteredOfficesQueryHandler
{
    /// <summary>
    /// Retrieves filtered and paginated offices
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated result of offices</returns>
    Task<PagedResult<OfficeResponse>> HandleAsync(SearchOfficeRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of GetFilteredOfficesQueryHandler
/// </summary>
public class GetFilteredOfficesQueryHandler : IGetFilteredOfficesQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetFilteredOfficesQueryHandler> _logger;

    public GetFilteredOfficesQueryHandler(
        AppDbContext dbContext,
        ILogger<GetFilteredOfficesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<OfficeResponse>> HandleAsync(
        SearchOfficeRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving filtered offices. SearchTerm: {SearchTerm}, BuildingId: {BuildingId}, " +
            "BuildingName: {BuildingName}, LocationCity: {LocationCity}, LocationCountry: {LocationCountry}, " +
            "Department: {Department}, PageNumber: {PageNumber}, PageSize: {PageSize}",
            request.SearchTerm, request.BuildingId, request.BuildingName, request.LocationCity, 
            request.LocationCountry, request.Department, request.PageNumber, request.PageSize);

        var query = _dbContext.Offices.AsNoTracking();

        // ========== OFFICE-LEVEL FILTERS ==========

        // Search term filter (office name or description)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(o =>
                o.Name.ToLower().Contains(searchTerm) ||
                (o.Description != null && o.Description.ToLower().Contains(searchTerm)));
        }

        // Filter by specific building ID (direct FK lookup - fastest)
        if (request.BuildingId.HasValue)
        {
            query = query.Where(o => o.BuildingId == request.BuildingId);
        }

        // Filter by department
        if (!string.IsNullOrWhiteSpace(request.Department))
        {
            query = query.Where(o => o.Department.ToLower() == request.Department.ToLower());
        }

        // Filter by office type
        if (!string.IsNullOrWhiteSpace(request.OfficeType))
        {
            query = query.Where(o => o.OfficeType.ToLower() == request.OfficeType.ToLower());
        }

        // Filter by active status
        if (request.IsActive.HasValue)
        {
            query = query.Where(o => o.IsActive == request.IsActive);
        }

        // ========== DENORMALIZED FIELDS FILTERS (Performance-Optimized - No Joins) ==========
        // These filters use composite indexes and do NOT require table joins

        // Filter by building name (denormalized field)
        // Uses IX_Office_BuildingName or composite indexes containing BuildingName
        if (!string.IsNullOrWhiteSpace(request.BuildingName))
        {
            var buildingName = request.BuildingName.ToLower();
            query = query.Where(o => 
                o.BuildingName != null && o.BuildingName.ToLower().Contains(buildingName));
        }

        // Filter by location city (denormalized field)
        // Uses IX_Office_Location_Geographic composite index
        if (!string.IsNullOrWhiteSpace(request.LocationCity))
        {
            query = query.Where(o => 
                o.LocationCity != null && o.LocationCity.ToLower() == request.LocationCity.ToLower());
        }

        // Filter by location region (denormalized field)
        // Uses IX_Office_Location_Geographic composite index
        if (!string.IsNullOrWhiteSpace(request.LocationRegion))
        {
            query = query.Where(o => 
                o.LocationRegion != null && o.LocationRegion.ToLower() == request.LocationRegion.ToLower());
        }

        // Filter by location country (denormalized field)
        // Uses IX_Office_Location_Geographic composite index
        if (!string.IsNullOrWhiteSpace(request.LocationCountry))
        {
            query = query.Where(o => 
                o.LocationCountry != null && o.LocationCountry.ToLower() == request.LocationCountry.ToLower());
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        // Apply pagination
        var skipCount = (request.PageNumber - 1) * request.PageSize;
        var offices = await query
            .Skip(skipCount)
            .Take(request.PageSize)
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
                BuildingName = o.BuildingName,
                LocationCity = o.LocationCity,
                LocationRegion = o.LocationRegion,
                LocationCountry = o.LocationCountry,
                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved {OfficeCount} offices out of {TotalCount} matching criteria",
            offices.Count, totalCount);

        return new PagedResult<OfficeResponse>
        {
            Items = offices,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private IQueryable<Persistence.Entities.Office> ApplySorting(
        IQueryable<Persistence.Entities.Office> query,
        string? sortBy,
        string? sortOrder)
    {
        var isDescending = sortOrder?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "name" => isDescending ? query.OrderByDescending(o => o.Name) : query.OrderBy(o => o.Name),
            "department" => isDescending ? query.OrderByDescending(o => o.Department) : query.OrderBy(o => o.Department),
            "floor" => isDescending ? query.OrderByDescending(o => o.FloorNumber) : query.OrderBy(o => o.FloorNumber),
            "createdat" => isDescending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt),
            "updatedat" => isDescending ? query.OrderByDescending(o => o.UpdatedAt) : query.OrderBy(o => o.UpdatedAt),
            _ => query.OrderBy(o => o.Name) // Default sort
        };
    }
}
