using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;

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

    public async Task<IEnumerable<CompanyDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all companies");

        var companies = await _dbContext.Companies
            .AsNoTracking()
            .Select(c => new CompanyDto
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

        _logger.LogInformation("Retrieved {Count} companies", companies.Count);

        return companies;
    }
}