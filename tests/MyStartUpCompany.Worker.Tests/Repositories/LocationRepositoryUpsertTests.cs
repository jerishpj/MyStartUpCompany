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

        public LocationRepositoryUpsertTests()
        {
            var uniqueDbName = Guid.NewGuid().ToString();
            _dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            _repository = new LocationRepository(_dbContext);
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
                CompanyId = 1,
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
                CompanyId = 1,
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
                CompanyId = 1,
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
                CompanyId = 1,
                Name = "Regional Office",
                Address = "111 Boston Lane",
                City = "Boston",
                PostalCode = "02101",
                Country = "USA"
            };

            var location2 = new Location
            {
                CompanyId = 2,
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
