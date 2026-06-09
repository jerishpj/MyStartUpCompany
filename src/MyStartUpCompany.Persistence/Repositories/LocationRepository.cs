using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Location entity with upsert functionality.
    /// Business key: Name + CompanyId (location name unique per company)
    /// Implements the Industry-standard Upsert Pattern:
    /// - If a record matching the unique key (Name, CompanyId) exists, update it
    /// - If no matching record exists, insert a new one
    /// </summary>
    public class LocationRepository : Repository<Location>, ILocationRepository
    {
        public LocationRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Location?> FindByNameAndCompanyIdAsync(string name, int companyId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(l => l.Name == name && l.CompanyId == companyId, cancellationToken);
        }

        public async Task<Location> UpsertAsync(Location location, CancellationToken cancellationToken = default)
        {
            // Find existing location by name and company ID (business key)
            var existingLocation = await FindByNameAndCompanyIdAsync(location.Name, location.CompanyId, cancellationToken);

            if (existingLocation != null)
            {
                // UPDATE scenario
                existingLocation.Description = location.Description;
                existingLocation.Address = location.Address;
                existingLocation.City = location.City;
                existingLocation.Region = location.Region;
                existingLocation.PostalCode = location.PostalCode;
                existingLocation.Country = location.Country;
                existingLocation.Phone = location.Phone;
                existingLocation.Email = location.Email;
                existingLocation.ManagerName = location.ManagerName;
                existingLocation.IsActive = location.IsActive;
                existingLocation.UpdatedAt = DateTime.UtcNow;

                Update(existingLocation);
                await SaveChangesAsync(cancellationToken);

                return existingLocation;
            }

            // INSERT scenario
            await AddAsync(location, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            return location;
        }
    }
}
