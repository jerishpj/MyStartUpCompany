using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;
using MyStartUpCompany.Persistence.Repositories;
using MyStartUpCompany.Worker.Tests.Utilities;

namespace MyStartUpCompany.Worker.Tests.Repositories
{
    /// <summary>
    /// Tests for the BuildingRepository upsert functionality.
    /// </summary>
    public class BuildingRepositoryUpsertTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly IBuildingRepository _repository;

        public BuildingRepositoryUpsertTests()
        {
            var uniqueDbName = Guid.NewGuid().ToString();
            _dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            _repository = new BuildingRepository(_dbContext);

            // Create required parent entities for foreign key constraints
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Create a test company (required for Location)
            var company = new Company
            {
                Name = "Test Company",
                Address = "123 Test St",
                City = "Test City",
                PostalCode = "12345",
                Country = "USA",
                Phone = "555-0000"
            };
            _dbContext.Companies.Add(company);
            _dbContext.SaveChanges();

            // Create test locations (required for Building)
            var location1 = new Location
            {
                CompanyId = company.Id,
                Name = "Location 1",
                Address = "100 Location St",
                City = "Test City",
                PostalCode = "12345",
                Country = "USA"
            };
            var location2 = new Location
            {
                CompanyId = company.Id,
                Name = "Location 2",
                Address = "200 Location St",
                City = "Test City",
                PostalCode = "12345",
                Country = "USA"
            };
            _dbContext.Locations.Add(location1);
            _dbContext.Locations.Add(location2);
            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task UpsertAsync_WithNewBuilding_ShouldInsertRecord()
        {
            // Arrange
            var building = new Building
            {
                LocationId = 1,
                Name = "Building A",
                Address = "123 Tech Park Drive",
                BuildingCode = "BLD-A",
                NumberOfFloors = 5
            };

            // Act
            var result = await _repository.UpsertAsync(building);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Building A");
        }

        [Fact]
        public async Task UpsertAsync_WithExistingBuildingByNameAndLocationId_ShouldUpdateRecord()
        {
            // Arrange
            var originalBuilding = new Building
            {
                LocationId = 1,
                Name = "Main Building",
                Address = "456 Main Street",
                BuildingCode = "MAIN",
                NumberOfFloors = 3
            };

            await _repository.UpsertAsync(originalBuilding);
            var originalId = originalBuilding.Id;

            var updatedBuilding = new Building
            {
                LocationId = 1,
                Name = "Main Building",
                Address = "789 Main Street",
                BuildingCode = "MAIN-UPDATED",
                NumberOfFloors = 5
            };

            // Act
            var result = await _repository.UpsertAsync(updatedBuilding);

            // Assert
            result.Id.Should().Be(originalId);
            result.NumberOfFloors.Should().Be(5);
            result.Address.Should().Be("789 Main Street");
        }

        [Fact]
        public async Task UpsertAsync_WithSameNameDifferentLocation_ShouldInsertNewRecord()
        {
            // Arrange
            var building1 = new Building
            {
                LocationId = 1,
                Name = "East Tower",
                Address = "100 East Lane",
                BuildingCode = "ET-1"
            };

            var building2 = new Building
            {
                LocationId = 2,
                Name = "East Tower",
                Address = "200 East Lane",
                BuildingCode = "ET-2"
            };

            // Act
            await _repository.UpsertAsync(building1);
            var result2 = await _repository.UpsertAsync(building2);

            // Assert
            building1.Id.Should().NotBe(result2.Id);
        }
    }
}
