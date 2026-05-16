using Microsoft.EntityFrameworkCore;
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

    public async Task<PagedResult<Company>> HandleAsync(
        CompanyRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving filtered companies - Region: {Region}, Country: {Country}, City: {City}, " +
            "PostalCode: {PostalCode}, SearchTerm: {SearchTerm}, PageNumber: {PageNumber}, PageSize: {PageSize}",
            request.Region, request.Country, request.City, request.PostalCode,
            request.SearchTerm, request.PageNumber, request.PageSize);

        // Build optimized query
        var query = BuildQuery(request);

        // Get total count (consider caching this for subsequent pages)
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination with optimized ordering
        var companies = await query
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Id) // Stable sort for consistent pagination
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new Company
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Address = c.Address,
                City = c.City,
                Region = c.Region,
                PostalCode = c.PostalCode,
                Country = c.Country,
                Phone = c.Phone
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} companies (Total: {TotalCount}, Page: {PageNumber}/{TotalPages})",
            companies.Count, totalCount, request.PageNumber,
            (int)Math.Ceiling(totalCount / (double)request.PageSize));

        return new PagedResult<Company>
        {
            Items = companies,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private IQueryable<Persistence.Entities.Company> BuildQuery(CompanyRequest request)
    {
        var query = _dbContext.Companies.AsNoTracking();

        // PERFORMANCE TIP: Apply most selective filters first
        // Order: PostalCode (most selective) > City > Region > Country > SearchTerm

        // Exact match for postal code (fastest)
        if (!string.IsNullOrWhiteSpace(request.PostalCode))
        {
            query = query.Where(c => c.PostalCode == request.PostalCode);
        }

        // Use EF.Functions.Like for better index utilization
        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(c => EF.Functions.Like(c.City, $"%{EscapeLikeParameter(request.City)}%"));
        }

        if (!string.IsNullOrWhiteSpace(request.Region))
        {
            query = query.Where(c => c.Region != null &&
                EF.Functions.Like(c.Region, $"%{EscapeLikeParameter(request.Region)}%"));
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(c => EF.Functions.Like(c.Country, $"%{EscapeLikeParameter(request.Country)}%"));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var escapedTerm = EscapeLikeParameter(request.SearchTerm);
            var searchPattern = $"%{escapedTerm}%";
            query = query.Where(c =>
                EF.Functions.Like(c.Name, searchPattern) ||
                (c.Description != null && EF.Functions.Like(c.Description, searchPattern)));
        }

        return query;
    }

    /// <summary>
    /// Escapes special characters in LIKE patterns to prevent SQL injection
    /// </summary>
    private static string EscapeLikeParameter(string parameter)
    {
        return parameter
            .Replace("[", "[[]")
            .Replace("%", "[%]")
            .Replace("_", "[_]");
    }
}