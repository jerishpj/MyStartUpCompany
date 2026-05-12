using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries;

public record GetCompanyQuery(int CompanyId);

public class GetCompanyQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetCompanyQueryHandler> _logger;

    public GetCompanyQueryHandler(AppDbContext dbContext, ILogger<GetCompanyQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<CompanyDto> HandleAsync(int companyId, CancellationToken cancellationToken = default)
    {
        if (companyId <= 0)
        {
            throw new BadRequestException("Company ID must be greater than 0");
        }

        _logger.LogInformation("Handling GetCompanyQuery for CompanyId: {CompanyId}", companyId);

        var company = await _dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken);

        if (company == null)
        {
            _logger.LogWarning("Company with Id {CompanyId} not found", companyId);
            throw new NotFoundException(nameof(Company), companyId);
        }

        return MapToDto(company);
    }

    private static CompanyDto MapToDto(Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Description = company.Description,
            Address = company.Address,
            City = company.City,
            Region = company.Region,
            PostalCode = company.PostalCode,
            Country = company.Country,
            Phone = company.Phone,
        };
    }
}
