using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Common.Utilities;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Shared.Models;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public class GetFilteredCompaniesQueryHandler : IGetFilteredCompaniesQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetFilteredCompaniesQueryHandler> _logger;

    public GetFilteredCompaniesQueryHandler(
        AppDbContext dbContext,
        ILogger<GetFilteredCompaniesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PagedResult<CompanyResponse>> HandleAsync(
        SearchCompanyRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Retrieving filtered companies - PageNumber: {PageNumber}, PageSize: {PageSize}",
            request.PageNumber, request.PageSize);

        // Build optimized query
        var query = BuildQuery(request);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination with database-level projection for efficiency
        var companies = await query
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Id) // Stable sort for consistent pagination
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToCompanyResponse()
            .ToListAsync(cancellationToken);

        _logger.LogDebug(
            "Retrieved {Count} companies (Total: {TotalCount}, Page: {PageNumber}/{TotalPages})",
            companies.Count, totalCount, request.PageNumber,
            (int)Math.Ceiling(totalCount / (double)request.PageSize));

        return new PagedResult<CompanyResponse>
        {
            Items = companies,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Builds the optimized query with database-level filters.
    /// Applies filters in order of selectivity (most selective first) for query optimization.
    /// </summary>
    private IQueryable<Persistence.Entities.Company> BuildQuery(SearchCompanyRequest request)
    {
        var query = _dbContext.Companies.AsNoTracking();

        // PERFORMANCE: Apply most selective filters first
        // Order: PostalCode (most selective) > City > Region > Country > SearchTerm

        // Exact match for postal code (fastest, most selective)
        if (!string.IsNullOrWhiteSpace(request.PostalCode))
        {
            query = query.Where(c => c.PostalCode == request.PostalCode);
        }

        // City - use EF.Functions.Like for better database-level optimization
        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var pattern = SqlLikeHelper.CreateLikePattern(request.City);
            query = query.Where(c => EF.Functions.Like(c.City, pattern));
        }

        // Region - use EF.Functions.Like
        if (!string.IsNullOrWhiteSpace(request.Region))
        {
            var pattern = SqlLikeHelper.CreateLikePattern(request.Region);
            query = query.Where(c => c.Region != null &&
                EF.Functions.Like(c.Region, pattern));
        }

        // Country - use EF.Functions.Like
        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            var pattern = SqlLikeHelper.CreateLikePattern(request.Country);
            query = query.Where(c => EF.Functions.Like(c.Country, pattern));
        }

        // Search term - least selective (substring search on multiple fields)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = SqlLikeHelper.CreateLikePattern(request.SearchTerm);
            query = query.Where(c =>
                EF.Functions.Like(c.Name, pattern) ||
                (c.Description != null && EF.Functions.Like(c.Description, pattern)));
        }

        return query;
    }
}
