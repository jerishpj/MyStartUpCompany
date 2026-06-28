using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;
using MyStartUpCompany.Persistence.Repositories;
using MyStartUpCompany.Worker.Tests.Utilities;

namespace MyStartUpCompany.Worker.Tests.Repositories
{
    /// <summary>
    /// Tests for the LocationRepository upsert functionality.
    /// </summary>
    public class LocationRepositoryUpsertTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly ILocationRepository _repository;
        private int _company1Id;
        private int _company2Id;

        public LocationRepositoryUpsertTests()
        {
            var uniqueDbName = Guid.NewGuid().ToString();
            _dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            _repository = new LocationRepository(_dbContext);

            // Create required parent entities for foreign key constraints
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Create test companies (required for Location)
            var company1 = new Company
            {
                Name = "Test Company 1",
                Address = "123 Test St",
                City = "Test City",
                PostalCode = "12345",
                Country = "USA",
                Phone = "555-0001"
            };
            var company2 = new Company
            {
                Name = "Test Company 2",
                Address = "456 Test St",
                City = "Test City",
                PostalCode = "12345",
                Country = "USA",
                Phone = "555-0002"
            };
            _dbContext.Companies.Add(company1);
            _dbContext.Companies.Add(company2);
            _dbContext.SaveChanges();

            // Capture the assigned IDs after SaveChanges
            _company1Id = company1.Id;
            _company2Id = company2.Id;
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task UpsertAsync_WithNewLocation_ShouldInsertRecord()
        {
            // Arrange
            var location = new Location
            {
                CompanyId = _company1Id,
                Name = "Headquarters",
                Address = "123 Main Street",
                City = "San Francisco",
                PostalCode = "94105",
                Country = "USA"
            };

            // Act
            var result = await _repository.UpsertAsync(location);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Headquarters");
        }

        [Fact]
        public async Task UpsertAsync_WithExistingLocationByNameAndCompanyId_ShouldUpdateRecord()
        {
            // Arrange
            var originalLocation = new Location
            {
                CompanyId = _company1Id,
                Name = "West Coast Office",
                Address = "456 Market Street",
                City = "San Francisco",
                PostalCode = "94102",
                Country = "USA"
            };

            await _repository.UpsertAsync(originalLocation);
            var originalId = originalLocation.Id;

            var updatedLocation = new Location
            {
                CompanyId = _company1Id,
                Name = "West Coast Office",
                Address = "789 Mission Street",
                City = "Los Angeles",
                PostalCode = "90001",
                Country = "USA"
            };

            // Act
            var result = await _repository.UpsertAsync(updatedLocation);

            // Assert
            result.Id.Should().Be(originalId);
            result.City.Should().Be("Los Angeles");
            result.Address.Should().Be("789 Mission Street");
        }

        [Fact]
        public async Task UpsertAsync_WithSameNameDifferentCompany_ShouldInsertNewRecord()
        {
            // Arrange
            var location1 = new Location
            {
                CompanyId = _company1Id,
                Name = "Regional Office",
                Address = "111 Boston Lane",
                City = "Boston",
                PostalCode = "02101",
                Country = "USA"
            };

            var location2 = new Location
            {
                CompanyId = _company2Id,
                Name = "Regional Office",
                Address = "222 Chicago Avenue",
                City = "Chicago",
                PostalCode = "60601",
                Country = "USA"
            };

            // Act
            await _repository.UpsertAsync(location1);
            var result2 = await _repository.UpsertAsync(location2);

            // Assert
            location1.Id.Should().NotBe(result2.Id);
        }
    }
}
