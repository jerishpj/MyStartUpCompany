using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;
using MyStartUpCompany.Persistence.Repositories;
using MyStartUpCompany.Worker.Tests.Utilities;

namespace MyStartUpCompany.Worker.Tests.Repositories
{
    /// <summary>
    /// Tests for the OfficeRepository upsert functionality.
    /// </summary>
    public class OfficeRepositoryUpsertTests : IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly IOfficeRepository _repository;

        public OfficeRepositoryUpsertTests()
        {
            var uniqueDbName = Guid.NewGuid().ToString();
            _dbContext = TestDataFactory.CreateInMemoryAppDbContext(uniqueDbName);
            _repository = new OfficeRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        [Fact]
        public async Task UpsertAsync_WithNewOffice_ShouldInsertRecord()
        {
            // Arrange
            var office = new Office
            {
                BuildingId = 1,
                Name = "Sales Department",
                OfficeCode = "SALES-1",
                FloorNumber = 3,
                Capacity = 20
            };

            // Act
            var result = await _repository.UpsertAsync(office);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Sales Department");
        }

        [Fact]
        public async Task UpsertAsync_WithExistingOfficeByNameAndBuildingId_ShouldUpdateRecord()
        {
            // Arrange
            var originalOffice = new Office
            {
                BuildingId = 1,
                Name = "Engineering",
                OfficeCode = "ENG-1",
                Capacity = 30
            };

            await _repository.UpsertAsync(originalOffice);
            var originalId = originalOffice.Id;

            var updatedOffice = new Office
            {
                BuildingId = 1,
                Name = "Engineering",
                OfficeCode = "ENG-1-UPDATED",
                Capacity = 40
            };

            // Act
            var result = await _repository.UpsertAsync(updatedOffice);

            // Assert
            result.Id.Should().Be(originalId);
            result.Capacity.Should().Be(40);
        }

        [Fact]
        public async Task UpsertAsync_WithSameNameDifferentBuilding_ShouldInsertNewRecord()
        {
            // Arrange
            var office1 = new Office
            {
                BuildingId = 1,
                Name = "Conference Room",
                OfficeCode = "CR-B1"
            };

            var office2 = new Office
            {
                BuildingId = 2,
                Name = "Conference Room",
                OfficeCode = "CR-B2"
            };

            // Act
            await _repository.UpsertAsync(office1);
            var result2 = await _repository.UpsertAsync(office2);

            // Assert
            office1.Id.Should().NotBe(result2.Id);
        }
    }
}
