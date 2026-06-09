using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository interface for Location entity operations including upsert functionality.
    /// Business key: Name + CompanyId (location name unique per company)
    /// </summary>
    public interface ILocationRepository : IRepository<Location>
    {
        /// <summary>
        /// Upserts a location record. If a location with the same name and company exists, it updates the record.
        /// Otherwise, it inserts a new record.
        /// </summary>
        /// <param name="location">The location to upsert</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The upserted location entity with its Id populated</returns>
        Task<Location> UpsertAsync(Location location, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a location by name and company ID.
        /// </summary>
        /// <param name="name">The location name</param>
        /// <param name="companyId">The parent company ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The location if found, otherwise null</returns>
        Task<Location?> FindByNameAndCompanyIdAsync(string name, int companyId, CancellationToken cancellationToken = default);
    }
}
