using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Worker.Handlers.AddLocationHierarchy;
using MyStartUpCompany.Worker.Services;
using Xunit;

namespace MyStartUpCompany.Worker.Tests.Handlers.AddLocationHierarchy
{
    /// <summary>
    /// Tests for AddLocationHierarchyEventHandler verifying proper hierarchical processing
    /// and denormalization of Office table with location/building data.
    /// </summary>
    public class AddLocationHierarchyEventHandlerTests : IAsyncLifetime
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AddLocationHierarchyEventHandler> _logger;
        private readonly AddLocationHierarchyEventHandler _handler;

        public AddLocationHierarchyEventHandlerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"LocationHierarchyTest_{Guid.NewGuid()}")
                .Options;

            _dbContext = new AppDbContext(options);

            // Create a mock logger
            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<AddLocationHierarchyEventHandler>();

            _handler = new AddLocationHierarchyEventHandler(_dbContext, _logger);
        }

        public async Task InitializeAsync()
        {
            // Create database and schema
            await _dbContext.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task HandleAsync_WithValidHierarchy_CreatesLocationBuildingAndOfficesWithDenormalization()
        {
            // Arrange
            var locationMessage = CreateValidLocationHierarchyMessage();

            // Act
            var result = await _handler.HandleAsync(locationMessage, CancellationToken.None);

            // Assert
            Assert.True(result);

            // Verify Location was created
            var location = await _dbContext.Locations
                .FirstOrDefaultAsync(l => l.Name == "Test Location");
            Assert.NotNull(location);
            Assert.Equal("Test Location", location.Name);
            Assert.Equal("Test City", location.City);
            Assert.Equal("Test Region", location.Region);
            Assert.Equal("Test Country", location.Country);

            // Verify Building was created
            var building = await _dbContext.Buildings
                .FirstOrDefaultAsync(b => b.Name == "Test Building");
            Assert.NotNull(building);
            Assert.Equal(location.Id, building.LocationId);

            // Verify Office was created with denormalized fields
            var office = await _dbContext.Offices
                .FirstOrDefaultAsync(o => o.Name == "Test Office");
            Assert.NotNull(office);
            Assert.Equal(building.Id, office.BuildingId);

            // ========== CRITICAL: Verify Denormalization ==========
            Assert.Equal("Test Building", office.BuildingName);
            Assert.Equal("Test City", office.LocationCity);
            Assert.Equal("Test Region", office.LocationRegion);
            Assert.Equal("Test Country", office.LocationCountry);
        }

        [Fact]
        public async Task HandleAsync_WithDuplicateLocation_ReturnsFalseAndSkips()
        {
            // Arrange
            var locationMessage1 = CreateValidLocationHierarchyMessage();
            var locationMessage2 = CreateValidLocationHierarchyMessage(); // Same location name and city

            // Act
            var result1 = await _handler.HandleAsync(locationMessage1, CancellationToken.None);
            var result2 = await _handler.HandleAsync(locationMessage2, CancellationToken.None);

            // Assert
            Assert.True(result1);
            Assert.False(result2); // Duplicate should return false

            // Verify only one location exists
            var locationCount = await _dbContext.Locations.CountAsync();
            Assert.Equal(1, locationCount);
        }

        [Fact]
        public async Task HandleAsync_WithNullMessage_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.HandleAsync(null!, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_WithMultipleBuildings_CreatesBuildingsAndOfficesWithCorrectDenormalization()
        {
            // Arrange
            var locationMessage = new LocationHierarchyMessage
            {
                CorrelationId = "test-correlation-id",
                CreatedAt = DateTime.UtcNow.ToString("O"),
                Source = "Test",
                Location = new LocationInputDto
                {
                    Name = "Multi-Building Location",
                    Description = "A location with multiple buildings",
                    Address = "123 Main St",
                    City = "Main City",
                    Region = "Main Region",
                    PostalCode = "12345",
                    Country = "Main Country",
                    Phone = "555-0000",
                    Buildings = new List<BuildingInputDto>
                    {
                        new BuildingInputDto
                        {
                            Name = "Building 1",
                            Address = "123 Main St",
                            Offices = new List<OfficeInputDto>
                            {
                                new OfficeInputDto
                                {
                                    Name = "Office 1-1",
                                    FloorNumber = 1
                                },
                                new OfficeInputDto
                                {
                                    Name = "Office 1-2",
                                    FloorNumber = 2
                                }
                            }
                        },
                        new BuildingInputDto
                        {
                            Name = "Building 2",
                            Address = "456 Oak Ave",
                            Offices = new List<OfficeInputDto>
                            {
                                new OfficeInputDto
                                {
                                    Name = "Office 2-1",
                                    FloorNumber = 1
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var result = await _handler.HandleAsync(locationMessage, CancellationToken.None);

            // Assert
            Assert.True(result);

            var offices = await _dbContext.Offices.ToListAsync();
            Assert.Equal(3, offices.Count);

            // Verify each office has correct denormalized values
            foreach (var office in offices)
            {
                Assert.Equal("Main City", office.LocationCity);
                Assert.Equal("Main Region", office.LocationRegion);
                Assert.Equal("Main Country", office.LocationCountry);

                // BuildingName should match the parent building
                if (office.Name.StartsWith("Office 1"))
                {
                    Assert.Equal("Building 1", office.BuildingName);
                }
                else if (office.Name.StartsWith("Office 2"))
                {
                    Assert.Equal("Building 2", office.BuildingName);
                }
            }
        }

        [Fact]
        public async Task HandleAsync_WithoutOffices_CreatesLocationAndBuildingsOnly()
        {
            // Arrange
            var locationMessage = new LocationHierarchyMessage
            {
                CorrelationId = "test-correlation-id",
                Location = new LocationInputDto
                {
                    Name = "Location Without Offices",
                    Address = "123 Main St",
                    City = "Test City",
                    Region = "Test Region",
                    PostalCode = "12345",
                    Country = "Test Country",
                    Buildings = new List<BuildingInputDto>
                    {
                        new BuildingInputDto
                        {
                            Name = "Empty Building",
                            Address = "123 Main St",
                            Offices = new List<OfficeInputDto>() // Empty offices
                        }
                    }
                }
            };

            // Act
            var result = await _handler.HandleAsync(locationMessage, CancellationToken.None);

            // Assert
            Assert.True(result);

            var location = await _dbContext.Locations.FirstOrDefaultAsync();
            Assert.NotNull(location);

            var building = await _dbContext.Buildings.FirstOrDefaultAsync();
            Assert.NotNull(building);

            var officeCount = await _dbContext.Offices.CountAsync();
            Assert.Equal(0, officeCount);
        }

        private LocationHierarchyMessage CreateValidLocationHierarchyMessage()
        {
            return new LocationHierarchyMessage
            {
                CorrelationId = "test-correlation-id",
                CreatedAt = DateTime.UtcNow.ToString("O"),
                Source = "Test",
                Version = "1.0",
                Location = new LocationInputDto
                {
                    Name = "Test Location",
                    Description = "A test location",
                    Address = "123 Test Street",
                    City = "Test City",
                    Region = "Test Region",
                    PostalCode = "12345",
                    Country = "Test Country",
                    Phone = "555-0001",
                    Email = "test@location.com",
                    ManagerName = "John Doe",
                    Buildings = new List<BuildingInputDto>
                    {
                        new BuildingInputDto
                        {
                            Name = "Test Building",
                            BuildingCode = "B001",
                            Description = "A test building",
                            Address = "123 Test Street",
                            NumberOfFloors = 10,
                            YearConstructed = 2020,
                            TotalFloorArea = 50000m,
                            ContactPerson = "Jane Smith",
                            Phone = "555-0002",
                            Offices = new List<OfficeInputDto>
                            {
                                new OfficeInputDto
                                {
                                    Name = "Test Office",
                                    OfficeCode = "O001",
                                    Description = "A test office",
                                    FloorNumber = 1,
                                    Section = "A",
                                    Capacity = 10,
                                    OfficeType = "Open Office",
                                    SquareMeters = 100m,
                                    Department = "IT",
                                    Manager = "Bob Johnson",
                                    Phone = "555-0003",
                                    Email = "test@office.com"
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
