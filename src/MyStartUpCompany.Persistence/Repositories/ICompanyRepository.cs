using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository interface for Company entity operations including upsert functionality.
    /// </summary>
    public interface ICompanyRepository : IRepository<Company>
    {
        /// <summary>
        /// Upserts a company record. If a company with the same name and address exists, it updates the record.
        /// Otherwise, it inserts a new record.
        /// </summary>
        /// <param name="company">The company to upsert</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The upserted company entity with its Id populated</returns>
        Task<Company> UpsertAsync(Company company, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a company by name and address.
        /// </summary>
        /// <param name="name">The company name</param>
        /// <param name="address">The company address</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The company if found, otherwise null</returns>
        Task<Company?> FindByNameAndAddressAsync(string name, string address, CancellationToken cancellationToken = default);
    }
}
