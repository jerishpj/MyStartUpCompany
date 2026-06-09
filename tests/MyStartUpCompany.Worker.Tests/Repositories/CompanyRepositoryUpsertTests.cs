using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;
using MyStartUpCompany.Persistence.Repositories;
using MyStartUpCompany.Worker.Tests.Utilities;

namespace MyStartUpCompany.Worker.Tests.Repositories
{
    /// <summary>
    /// Tests for the CompanyRepository upsert functionality.
    /// Verifies that the upsert pattern works correctly:
    /// - Inserts new records when they don't exist
    /// - Updates existing records when they match by Name and Address
    /// </summary>
    public class CompanyRepositoryUpsertTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly ICompanyRepository _repository;

        public CompanyRepositoryUpsertTests()
        {
            var uniqueDbName = Guid.NewGuid().ToString();
            _dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            _repository = new CompanyRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task UpsertAsync_WithNewCompany_ShouldInsertRecord()
        {
            // Arrange
            var company = new Company
            {
                Name = "TechCorp",
                Description = "A tech company",
                Address = "123 Tech Street",
                City = "San Francisco",
                Region = "CA",
                PostalCode = "94105",
                Country = "USA",
                Phone = "555-0101"
            };

            // Act
            var result = await _repository.UpsertAsync(company);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("TechCorp");
            result.Description.Should().Be("A tech company");

            var savedCompany = await _repository.FindByNameAndAddressAsync("TechCorp", "123 Tech Street");
            savedCompany.Should().NotBeNull();
            savedCompany.Id.Should().Be(result.Id);
        }

        [Fact]
        public async Task UpsertAsync_WithExistingCompanyByNameAndAddress_ShouldUpdateRecord()
        {
            // Arrange
            var originalCompany = new Company
            {
                Name = "TechCorp",
                Description = "Original description",
                Address = "123 Tech Street",
                City = "San Francisco",
                Region = "CA",
                PostalCode = "94105",
                Country = "USA",
                Phone = "555-0101"
            };

            // Insert original company
            await _repository.UpsertAsync(originalCompany);
            var originalId = originalCompany.Id;

            // Create updated company with same name and address
            var updatedCompany = new Company
            {
                Name = "TechCorp",
                Description = "Updated description",
                Address = "123 Tech Street",
                City = "New York",
                Region = "NY",
                PostalCode = "10001",
                Country = "USA",
                Phone = "555-0102"
            };

            // Act
            var result = await _repository.UpsertAsync(updatedCompany);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(originalId);
            result.Description.Should().Be("Updated description");
            result.City.Should().Be("New York");
            result.Region.Should().Be("NY");
            result.PostalCode.Should().Be("10001");
            result.Phone.Should().Be("555-0102");

            var companyCount = (await _repository.GetAllAsync()).Count();
            companyCount.Should().Be(1, "Should still have only one company record");
        }

        [Fact]
        public async Task UpsertAsync_WithDifferentAddressSameName_ShouldInsertNewRecord()
        {
            // Arrange
            var company1 = new Company
            {
                Name = "TechCorp",
                Description = "Headquarters",
                Address = "123 Tech Street",
                City = "San Francisco",
                Region = "CA",
                PostalCode = "94105",
                Country = "USA",
                Phone = "555-0101"
            };

            var company2 = new Company
            {
                Name = "TechCorp",
                Description = "Branch Office",
                Address = "456 Innovation Drive",
                City = "New York",
                Region = "NY",
                PostalCode = "10001",
                Country = "USA",
                Phone = "555-0102"
            };

            // Act
            await _repository.UpsertAsync(company1);
            var result2 = await _repository.UpsertAsync(company2);

            // Assert
            company1.Id.Should().BeGreaterThan(0);
            result2.Id.Should().BeGreaterThan(0);
            result2.Id.Should().NotBe(company1.Id);

            var allCompanies = await _repository.GetAllAsync();
            allCompanies.Count().Should().Be(2, "Should have two distinct company records");
        }

        [Fact]
        public async Task UpsertAsync_WithMultipleUpdates_ShouldPersistLatestValues()
        {
            // Arrange
            var company = new Company
            {
                Name = "TechCorp",
                Description = "Version 1",
                Address = "123 Tech Street",
                City = "San Francisco",
                Region = "CA",
                PostalCode = "94105",
                Country = "USA",
                Phone = "555-0101"
            };

            // Act - First upsert (insert)
            var result1 = await _repository.UpsertAsync(company);

            // Update and upsert again
            company.Description = "Version 2";
            company.Phone = "555-0102";
            var result2 = await _repository.UpsertAsync(company);

            // Update and upsert one more time
            company.Description = "Version 3";
            company.City = "Los Angeles";
            var result3 = await _repository.UpsertAsync(company);

            // Assert
            result1.Id.Should().Be(result2.Id);
            result2.Id.Should().Be(result3.Id);

            var savedCompany = await _repository.FindByNameAndAddressAsync("TechCorp", "123 Tech Street");
            savedCompany.Should().NotBeNull();
            savedCompany.Description.Should().Be("Version 3");
            savedCompany.City.Should().Be("Los Angeles");
            savedCompany.Phone.Should().Be("555-0102");

            var allCompanies = await _repository.GetAllAsync();
            allCompanies.Count().Should().Be(1, "Should have only one company record after multiple upserts");
        }

        [Fact]
        public async Task FindByNameAndAddressAsync_WithNonExistentCompany_ShouldReturnNull()
        {
            // Act
            var result = await _repository.FindByNameAndAddressAsync("NonExistent", "NonExistent Address");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task FindByNameAndAddressAsync_WithExistingCompany_ShouldReturnCompany()
        {
            // Arrange
            var company = new Company
            {
                Name = "TechCorp",
                Description = "A tech company",
                Address = "123 Tech Street",
                City = "San Francisco",
                Region = "CA",
                PostalCode = "94105",
                Country = "USA",
                Phone = "555-0101"
            };

            await _repository.UpsertAsync(company);

            // Act
            var result = await _repository.FindByNameAndAddressAsync("TechCorp", "123 Tech Street");

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("TechCorp");
            result.Address.Should().Be("123 Tech Street");
        }
    }
}
