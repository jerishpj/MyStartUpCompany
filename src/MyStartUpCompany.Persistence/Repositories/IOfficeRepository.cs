using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository interface for Office entity operations including upsert functionality.
    /// Business key: Name + BuildingId (office name unique per building)
    /// </summary>
    public interface IOfficeRepository : IRepository<Office>
    {
        /// <summary>
        /// Upserts an office record. If an office with the same name and building exists, it updates the record.
        /// Otherwise, it inserts a new record.
        /// </summary>
        /// <param name="office">The office to upsert</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The upserted office entity with its Id populated</returns>
        Task<Office> UpsertAsync(Office office, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds an office by name and building ID.
        /// </summary>
        /// <param name="name">The office name</param>
        /// <param name="buildingId">The parent building ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The office if found, otherwise null</returns>
        Task<Office?> FindByNameAndBuildingIdAsync(string name, int buildingId, CancellationToken cancellationToken = default);
    }
}
