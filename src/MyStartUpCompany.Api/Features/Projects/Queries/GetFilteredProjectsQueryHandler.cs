using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Common.Utilities;
using MyStartUpCompany.Api.Features.Projects.Models;
using MyStartUpCompany.Api.Shared.Models;
using MyStartUpCompany.Observability;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities.Enums;
using System.Diagnostics;

namespace MyStartUpCompany.Api.Features.Projects.Queries;

/// <summary>
/// Implementation of query handler for retrieving filtered and paginated projects
/// </summary>
public class GetFilteredProjectsQueryHandler : IGetFilteredProjectsQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetFilteredProjectsQueryHandler> _logger;

    public GetFilteredProjectsQueryHandler(
        AppDbContext dbContext,
        ILogger<GetFilteredProjectsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves filtered and paginated projects
    /// </summary>
    public async Task<PagedResult<ProjectResponse>> HandleAsync(
        ProjectFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogDebug(
                "Retrieving filtered projects - PageNumber: {PageNumber}, PageSize: {PageSize}",
                request.PageNumber, request.PageSize);

            // Build optimized query with database-level filters
            var query = BuildQuery(request);

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting and pagination with database-level projection for efficiency
            var responses = await ApplySorting(query, request.SortBy, request.SortOrder)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ProjectToProjectResponse()
                .ToListAsync(cancellationToken);

            stopwatch.Stop();

            // Record metrics
            BusinessMetrics.RecordDbQuery("GetFilteredProjects", responses.Count, stopwatch.ElapsedMilliseconds);
            BusinessMetrics.RecordPagingMetrics("Projects", request.PageNumber, request.PageSize, totalCount);

            _logger.LogDebug(
                "Retrieved {Count} projects (Total: {TotalCount}, Page: {PageNumber}/{TotalPages}) in {ElapsedMs}ms",
                responses.Count, totalCount, request.PageNumber,
                (int)Math.Ceiling(totalCount / (double)request.PageSize), stopwatch.ElapsedMilliseconds);

            return new PagedResult<ProjectResponse>
            {
                Items = responses,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            BusinessMetrics.RecordDbQueryError("GetFilteredProjects", stopwatch.ElapsedMilliseconds);
            _logger.LogError(ex, "Error retrieving filtered projects");
            throw;
        }
    }

    /// <summary>
    /// Builds the base query with database-level filters
    /// Only filters on indexed/searchable columns for optimal performance
    /// </summary>
    private IQueryable<Persistence.Entities.Project> BuildQuery(ProjectFilterRequest request)
    {
        var query = _dbContext.Projects.AsNoTracking();

        // PERFORMANCE: Apply most selective filters first
        // Order: CompanyId (foreign key) > ProjectIdentifier > Type > Code > Location > Name (substring search)

        // Filter by company ID (indexed)
        if (request.CompanyId.HasValue && request.CompanyId > 0)
        {
            query = query.Where(p => p.CompanyId == request.CompanyId);
        }

        // Filter by project identifier (unique index, most selective)
        if (!string.IsNullOrWhiteSpace(request.ProjectIdentifier))
        {
            var pattern = SqlLikeHelper.CreateLikePattern(request.ProjectIdentifier);
            query = query.Where(p => EF.Functions.Like(p.ProjectIdentifier, pattern));
        }

        // Filter by project type (indexed enum)
        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            if (Enum.TryParse<ProjectType>(request.Type, ignoreCase: true, out var projectType))
            {
                query = query.Where(p => p.Type == projectType);
            }
            // If parsing fails, silently skip this filter to prevent errors
        }

        // Filter by code (indexed)
        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var pattern = SqlLikeHelper.CreateExactPattern(request.Code);
            query = query.Where(p => EF.Functions.Like(p.Code, pattern));
        }

        // Filter by location (indexed)
        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            var pattern = SqlLikeHelper.CreateExactPattern(request.Location);
            query = query.Where(p => EF.Functions.Like(p.Location, pattern));
        }

        // Filter by name (substring search - indexed but less selective)
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var pattern = SqlLikeHelper.CreateLikePattern(request.Name);
            query = query.Where(p => EF.Functions.Like(p.Name, pattern));
        }

        return query;
    }

    /// <summary>
    /// Applies sorting based on the requested sort field and order
    /// </summary>
    private static IQueryable<Persistence.Entities.Project> ApplySorting(
        IQueryable<Persistence.Entities.Project> query,
        string sortBy,
        string sortOrder)
    {
        var isDescending = sortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

        return (sortBy?.ToLower()) switch
        {
            "code" => isDescending
                ? query.OrderByDescending(p => p.Code).ThenBy(p => p.Id)
                : query.OrderBy(p => p.Code).ThenBy(p => p.Id),

            "location" => isDescending
                ? query.OrderByDescending(p => p.Location).ThenBy(p => p.Id)
                : query.OrderBy(p => p.Location).ThenBy(p => p.Id),

            "createdat" => isDescending
                ? query.OrderByDescending(p => p.CreatedAt).ThenBy(p => p.Id)
                : query.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id),

            "projectidentifier" => isDescending
                ? query.OrderByDescending(p => p.ProjectIdentifier).ThenBy(p => p.Id)
                : query.OrderBy(p => p.ProjectIdentifier).ThenBy(p => p.Id),

            // Default: sort by name
            _ => isDescending
                ? query.OrderByDescending(p => p.Name).ThenBy(p => p.Id)
                : query.OrderBy(p => p.Name).ThenBy(p => p.Id)
        };
    }
}
