using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries
{
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

        //public async Task<CompanyDto?> HandleAsync(GetCompanyQuery query, CancellationToken cancellationToken = default)
        public async Task<CompanyDto?> HandleAsync(int query, CancellationToken cancellationToken = default)
        {
            //_logger.LogInformation("Handling GetCompanyQuery for CompanyId: {CompanyId}", query.CompanyId);
            _logger.LogInformation("Handling GetCompanyQuery for CompanyId: {CompanyId}", query);

            //var company = await _dbContext.Companies.FindAsync(new object[] { query.CompanyId }, cancellationToken);
            var company = await _dbContext.Companies.FindAsync(new object[] { query }, cancellationToken);

            if (company == null)
            {
                // _logger.LogWarning("Company with Id {CompanyId} not found", query.CompanyId);
                _logger.LogWarning("Company with Id {CompanyId} not found", query);
                return null;
            }
            
            return MapToDto(company);
        }

        public async Task<IEnumerable<CompanyDto>> HandleAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving all companies");

            var companies = await _dbContext.Companies
                .AsNoTracking()
                .Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    // Map other properties as needed
                    Description = c.Description,
                    Address = c.Address,
                    City = c.City,
                    Region = c.Region,
                    PostalCode = c.PostalCode,
                    Country = c.Country,
                    Phone = c.Phone
                })
                .ToListAsync(cancellationToken);

            return companies;
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
}
