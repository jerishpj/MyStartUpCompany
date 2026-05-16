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

        // Start with base query
        var query = _dbContext.Companies.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Region))
        {
            query = query.Where(c => c.Region != null && c.Region.Contains(request.Region));
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(c => c.Country.Contains(request.Country));
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(c => c.City.Contains(request.City));
        }

        if (!string.IsNullOrWhiteSpace(request.PostalCode))
        {
            query = query.Where(c => c.PostalCode.Contains(request.PostalCode));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(request.SearchTerm) || 
                                    (c.Description != null && c.Description.Contains(request.SearchTerm)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var companies = await query
            .OrderBy(c => c.Name) // Add default ordering
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
                Phone = c.Phone,
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
}