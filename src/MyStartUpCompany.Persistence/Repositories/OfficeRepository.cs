using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Office entity with upsert functionality.
    /// Business key: Name + BuildingId (office name unique per building)
    /// Implements the Industry-standard Upsert Pattern:
    /// - If a record matching the unique key (Name, BuildingId) exists, update it
    /// - If no matching record exists, insert a new one
    /// </summary>
    public class OfficeRepository : Repository<Office>, IOfficeRepository
    {
        public OfficeRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Office?> FindByNameAndBuildingIdAsync(string name, int buildingId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(o => o.Name == name && o.BuildingId == buildingId, cancellationToken);
        }

        public async Task<Office> UpsertAsync(Office office, CancellationToken cancellationToken = default)
        {
            // Find existing office by name and building ID (business key)
            var existingOffice = await FindByNameAndBuildingIdAsync(office.Name, office.BuildingId, cancellationToken);

            if (existingOffice != null)
            {
                // UPDATE scenario
                existingOffice.OfficeCode = office.OfficeCode;
                existingOffice.Description = office.Description;
                existingOffice.FloorNumber = office.FloorNumber;
                existingOffice.Section = office.Section;
                existingOffice.Capacity = office.Capacity;
                existingOffice.OfficeType = office.OfficeType;
                existingOffice.SquareMeters = office.SquareMeters;
                existingOffice.Department = office.Department;
                existingOffice.Manager = office.Manager;
                existingOffice.Phone = office.Phone;
                existingOffice.Email = office.Email;
                existingOffice.IsActive = office.IsActive;
                existingOffice.UpdatedAt = DateTime.UtcNow;

                Update(existingOffice);
                await SaveChangesAsync(cancellationToken);

                return existingOffice;
            }

            // INSERT scenario
            await AddAsync(office, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            return office;
        }
    }
}
