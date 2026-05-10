using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Queries
{
    public class GetAllCompanyQueryHandler
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetCompanyQueryHandler> _logger;

        public GetAllCompanyQueryHandler(AppDbContext dbContext, ILogger<GetCompanyQueryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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
    }
}
