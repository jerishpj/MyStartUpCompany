using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Company entity with upsert functionality.
    /// Implements the Industry-standard Upsert Pattern:
    /// - If a record matching the unique key (Name, Address) exists, update it
    /// - If no matching record exists, insert a new one
    /// </summary>
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Company?> FindByNameAndAddressAsync(string name, string address, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(c => c.Name == name && c.Address == address, cancellationToken);
        }

        public async Task<Company> UpsertAsync(Company company, CancellationToken cancellationToken = default)
        {
            // Find existing company by name and address (natural unique key)
            var existingCompany = await FindByNameAndAddressAsync(company.Name, company.Address, cancellationToken);

            if (existingCompany != null)
            {
                // Update existing company with new values
                existingCompany.Description = company.Description;
                existingCompany.City = company.City;
                existingCompany.Region = company.Region;
                existingCompany.PostalCode = company.PostalCode;
                existingCompany.Country = company.Country;
                existingCompany.Phone = company.Phone;

                // Update the entity state
                Update(existingCompany);
                await SaveChangesAsync(cancellationToken);

                return existingCompany;
            }

            // Insert new company
            await AddAsync(company, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            return company;
        }
    }
}
