using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository interface for Employee entity operations including upsert functionality.
    /// Business key: Name + Email (unique identifier in organization)
    /// </summary>
    public interface IEmployeeRepository : IRepository<Employee>
    {
        /// <summary>
        /// Upserts an employee record. If an employee with the same name and email exists, it updates the record.
        /// Otherwise, it inserts a new record.
        /// </summary>
        /// <param name="employee">The employee to upsert</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The upserted employee entity with its Id populated</returns>
        Task<Employee> UpsertAsync(Employee employee, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds an employee by name and email.
        /// </summary>
        /// <param name="name">The employee name</param>
        /// <param name="email">The employee email</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The employee if found, otherwise null</returns>
        Task<Employee?> FindByNameAndEmailAsync(string name, string email, CancellationToken cancellationToken = default);
    }
}
