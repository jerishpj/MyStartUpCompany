using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Observability;
using MyStartUpCompany.Persistence;
using System.Diagnostics;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public class GetAllCompaniesQueryHandler : IGetAllCompaniesQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetAllCompaniesQueryHandler> _logger;

    public GetAllCompaniesQueryHandler(
        AppDbContext dbContext,
        ILogger<GetAllCompaniesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<CompanyResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Retrieving all companies");

            var companies = await _dbContext.Companies
                .AsNoTracking()
                .Select(c => new CompanyResponse
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

            stopwatch.Stop();

            // Record metrics
            BusinessMetrics.RecordDbQuery("GetAllCompanies", companies.Count, stopwatch.ElapsedMilliseconds);

            _logger.LogInformation("Retrieved {Count} companies in {ElapsedMs}ms", 
                companies.Count, stopwatch.ElapsedMilliseconds);

            return companies;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            BusinessMetrics.RecordDbQueryError("GetAllCompanies", stopwatch.ElapsedMilliseconds);
            _logger.LogError(ex, "Error retrieving all companies");
            throw;
        }
    }
}