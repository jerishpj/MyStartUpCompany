using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Building entity with upsert functionality.
    /// Business key: Name + LocationId (building name unique per location)
    /// Implements the Industry-standard Upsert Pattern:
    /// - If a record matching the unique key (Name, LocationId) exists, update it
    /// - If no matching record exists, insert a new one
    /// </summary>
    public class BuildingRepository : Repository<Building>, IBuildingRepository
    {
        public BuildingRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Building?> FindByNameAndLocationIdAsync(string name, int locationId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(b => b.Name == name && b.LocationId == locationId, cancellationToken);
        }

        public async Task<Building> UpsertAsync(Building building, CancellationToken cancellationToken = default)
        {
            // Find existing building by name and location ID (business key)
            var existingBuilding = await FindByNameAndLocationIdAsync(building.Name, building.LocationId, cancellationToken);

            if (existingBuilding != null)
            {
                // UPDATE scenario
                existingBuilding.BuildingCode = building.BuildingCode;
                existingBuilding.Description = building.Description;
                existingBuilding.Address = building.Address;
                existingBuilding.NumberOfFloors = building.NumberOfFloors;
                existingBuilding.YearConstructed = building.YearConstructed;
                existingBuilding.TotalFloorArea = building.TotalFloorArea;
                existingBuilding.ContactPerson = building.ContactPerson;
                existingBuilding.Phone = building.Phone;
                existingBuilding.IsActive = building.IsActive;
                existingBuilding.UpdatedAt = DateTime.UtcNow;

                Update(existingBuilding);
                await SaveChangesAsync(cancellationToken);

                return existingBuilding;
            }

            // INSERT scenario
            await AddAsync(building, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            return building;
        }
    }
}
