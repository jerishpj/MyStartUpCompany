using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;
using MyStartUpCompany.Persistence.Repositories;
using MyStartUpCompany.Worker.Tests.Utilities;

namespace MyStartUpCompany.Worker.Tests.Repositories
{
    /// <summary>
    /// Tests for the EmployeeRepository upsert functionality.
    /// </summary>
    public class EmployeeRepositoryUpsertTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly IEmployeeRepository _repository;

        public EmployeeRepositoryUpsertTests()
        {
            var uniqueDbName = Guid.NewGuid().ToString();
            _dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            _repository = new EmployeeRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task UpsertAsync_WithNewEmployee_ShouldInsertRecord()
        {
            // Arrange
            var employee = new Employee
            {
                Name = "John Doe",
                Title = "Senior Developer",
                Email = "john.doe@company.com",
                City = "San Francisco",
                Region = "CA",
                PostalCode = "94105",
                Country = "USA",
                Phone = "555-0101"
            };

            // Act
            var result = await _repository.UpsertAsync(employee);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("John Doe");
            result.Email.Should().Be("john.doe@company.com");
        }

        [Fact]
        public async Task UpsertAsync_WithExistingEmployeeByNameAndEmail_ShouldUpdateRecord()
        {
            // Arrange
            var originalEmployee = new Employee
            {
                Name = "Jane Smith",
                Title = "Developer",
                Email = "jane@company.com",
                City = "New York",
                Phone = "555-0102"
            };

            await _repository.UpsertAsync(originalEmployee);
            var originalId = originalEmployee.Id;

            var updatedEmployee = new Employee
            {
                Name = "Jane Smith",
                Email = "jane@company.com",
                Title = "Senior Developer",
                City = "Los Angeles",
                Phone = "555-0103"
            };

            // Act
            var result = await _repository.UpsertAsync(updatedEmployee);

            // Assert
            result.Id.Should().Be(originalId);
            result.Title.Should().Be("Senior Developer");
            result.City.Should().Be("Los Angeles");
        }

        [Fact]
        public async Task UpsertAsync_WithSameNameDifferentEmail_ShouldInsertNewRecord()
        {
            // Arrange
            var employee1 = new Employee
            {
                Name = "John Smith",
                Email = "john.smith@company.com",
                Title = "Developer"
            };

            var employee2 = new Employee
            {
                Name = "John Smith",
                Email = "john.smith.alt@company.com",
                Title = "Manager"
            };

            // Act
            await _repository.UpsertAsync(employee1);
            var result2 = await _repository.UpsertAsync(employee2);

            // Assert
            employee1.Id.Should().NotBe(result2.Id);
            var allEmployees = await _repository.GetAllAsync();
            allEmployees.Count().Should().Be(2);
        }
    }
}
