using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for Employee entity with upsert functionality.
    /// Business key: Name + Email (unique identifier in organization)
    /// Implements the Industry-standard Upsert Pattern:
    /// - If a record matching the unique key (Name, Email) exists, update it
    /// - If no matching record exists, insert a new one
    /// </summary>
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Employee?> FindByNameAndEmailAsync(string name, string email, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(e => e.Name == name && e.Email == email, cancellationToken);
        }

        public async Task<Employee> UpsertAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            // Find existing employee by name and email (business key)
            var existingEmployee = await FindByNameAndEmailAsync(employee.Name, employee.Email ?? "", cancellationToken);

            if (existingEmployee != null)
            {
                // UPDATE scenario
                existingEmployee.Title = employee.Title;
                existingEmployee.Description = employee.Description;
                existingEmployee.Address = employee.Address;
                existingEmployee.City = employee.City;
                existingEmployee.Region = employee.Region;
                existingEmployee.PostalCode = employee.PostalCode;
                existingEmployee.Country = employee.Country;
                existingEmployee.Phone = employee.Phone;

                Update(existingEmployee);
                await SaveChangesAsync(cancellationToken);

                return existingEmployee;
            }

            // INSERT scenario
            await AddAsync(employee, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            return employee;
        }
    }
}
