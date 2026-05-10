using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Worker.Handlers.AddCompany
{
    public class AddCompanyEventHandler
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AddCompanyEventHandler> _logger;

        public AddCompanyEventHandler(AppDbContext dbContext, ILogger<AddCompanyEventHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> HandleAsync(CompanyInputDto companyDto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if company already exists by name
                var existingCompany = await _dbContext.Companies
                    .FirstOrDefaultAsync(c => c.Name == companyDto.Name, cancellationToken);

                if (existingCompany != null)
                {
                    _logger.LogWarning("Company '{CompanyName}' already exists with Id {Id}",
                        companyDto.Name, existingCompany.Id);
                    return false;
                }

                // Create new company entity
                var company = new Company
                {
                    Name = companyDto.Name,
                    Description = companyDto.Description,
                    Address = companyDto.Address,
                    City = companyDto.City,
                    Region = companyDto.Region,
                    PostalCode = companyDto.PostalCode,
                    Country = companyDto.Country,
                    Phone = companyDto.Phone
                };

                await _dbContext.Companies.AddAsync(company, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully added company '{CompanyName}' with Id {Id}",
                    company.Name, company.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding company '{CompanyName}'", companyDto.Name);
                throw;
            }

        }

    }
}
