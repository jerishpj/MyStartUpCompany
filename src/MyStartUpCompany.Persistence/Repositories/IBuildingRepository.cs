using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository interface for Building entity operations including upsert functionality.
    /// Business key: Name + LocationId (building name unique per location)
    /// </summary>
    public interface IBuildingRepository : IRepository<Building>
    {
        /// <summary>
        /// Upserts a building record. If a building with the same name and location exists, it updates the record.
        /// Otherwise, it inserts a new record.
        /// </summary>
        /// <param name="building">The building to upsert</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The upserted building entity with its Id populated</returns>
        Task<Building> UpsertAsync(Building building, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a building by name and location ID.
        /// </summary>
        /// <param name="name">The building name</param>
        /// <param name="locationId">The parent location ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The building if found, otherwise null</returns>
        Task<Building?> FindByNameAndLocationIdAsync(string name, int locationId, CancellationToken cancellationToken = default);
    }
}
