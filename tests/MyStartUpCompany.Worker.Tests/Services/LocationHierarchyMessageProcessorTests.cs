using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Worker.Handlers.AddLocationHierarchy;
using MyStartUpCompany.Worker.Services;
using Xunit;

namespace MyStartUpCompany.Worker.Tests.Services
{
    /// <summary>
    /// Tests for LocationHierarchyMessageProcessor verifying validation and routing logic.
    /// </summary>
    public class LocationHierarchyMessageProcessorTests : IAsyncLifetime
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<LocationHierarchyMessageProcessor> _logger;
        private readonly ILogger<AddLocationHierarchyEventHandler> _handlerLogger;
        private readonly AddLocationHierarchyEventHandler _handler;
        private readonly LocationHierarchyMessageProcessor _processor;

        public LocationHierarchyMessageProcessorTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"LocationProcessorTest_{Guid.NewGuid()}")
                .Options;

            _dbContext = new AppDbContext(options);

            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<LocationHierarchyMessageProcessor>();
            _handlerLogger = loggerFactory.CreateLogger<AddLocationHierarchyEventHandler>();

            _handler = new AddLocationHierarchyEventHandler(_dbContext, _handlerLogger);
            _processor = new LocationHierarchyMessageProcessor(_handler, _logger);
        }

        public async Task InitializeAsync()
        {
            await _dbContext.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task ProcessLocationHierarchyAsync_WithValidMessage_ReturnsSuccess()
        {
            // Arrange
            var message = CreateValidMessage();

            // Act
            var result = await _processor.ProcessLocationHierarchyAsync(
                message, "test-source", CancellationToken.None);

            // Assert
            Assert.Equal(
                LocationHierarchyMessageProcessor.LocationProcessingResult.Status.Success, 
                result.ProcessingStatus);
        }

        [Fact]
        public async Task ProcessLocationHierarchyAsync_WithNullMessage_ReturnsInvalid()
        {
            // Act
            var result = await _processor.ProcessLocationHierarchyAsync(
                null!, "test-source", CancellationToken.None);

            // Assert
            Assert.Equal(
                LocationHierarchyMessageProcessor.LocationProcessingResult.Status.Invalid, 
                result.ProcessingStatus);
            Assert.Contains("null", result.Message?.ToLower() ?? "");
        }

        [Fact]
        public async Task ProcessLocationHierarchyAsync_WithNullLocation_ReturnsInvalid()
        {
            // Arrange
            var message = new LocationHierarchyMessage
            {
                CorrelationId = "test-id",
                Location = null! // Null location
            };

            // Act
            var result = await _processor.ProcessLocationHierarchyAsync(
                message, "test-source", CancellationToken.None);

            // Assert
            Assert.Equal(
                LocationHierarchyMessageProcessor.LocationProcessingResult.Status.Invalid, 
                result.ProcessingStatus);
        }

        [Fact]
        public async Task ProcessLocationHierarchyAsync_WithMissingLocationName_ReturnsInvalid()
        {
            // Arrange
            var message = new LocationHierarchyMessage
            {
                Location = new LocationInputDto
                {
                    Name = "", // Missing name
                    Address = "123 Main St",
                    City = "Test City",
                    PostalCode = "12345",
                    Country = "Test Country"
                }
            };

            // Act
            var result = await _processor.ProcessLocationHierarchyAsync(
                message, "test-source", CancellationToken.None);

            // Assert
            Assert.Equal(
                LocationHierarchyMessageProcessor.LocationProcessingResult.Status.Invalid, 
                result.ProcessingStatus);
        }

        [Fact]
        public async Task ProcessLocationHierarchyAsync_WithMissingBuildingName_ReturnsInvalid()
        {
            // Arrange
            var message = new LocationHierarchyMessage
            {
                Location = new LocationInputDto
                {
                    Name = "Valid Location",
                    Address = "123 Main St",
                    City = "Test City",
                    PostalCode = "12345",
                    Country = "Test Country",
                    Buildings = new List<BuildingInputDto>
                    {
                        new BuildingInputDto
                        {
                            Name = "", // Missing building name
                            Address = "123 Main St"
                        }
                    }
                }
            };

            // Act
            var result = await _processor.ProcessLocationHierarchyAsync(
                message, "test-source", CancellationToken.None);

            // Assert
            Assert.Equal(
                LocationHierarchyMessageProcessor.LocationProcessingResult.Status.Invalid, 
                result.ProcessingStatus);
        }

        [Fact]
        public async Task ProcessLocationHierarchyAsync_WithMissingOfficeName_ReturnsInvalid()
        {
            // Arrange
            var message = new LocationHierarchyMessage
            {
                Location = new LocationInputDto
                {
                    Name = "Valid Location",
                    Address = "123 Main St",
                    City = "Test City",
                    PostalCode = "12345",
                    Country = "Test Country",
                    Buildings = new List<BuildingInputDto>
                    {
                        new BuildingInputDto
                        {
                            Name = "Valid Building",
                            Address = "123 Main St",
                            Offices = new List<OfficeInputDto>
                            {
                                new OfficeInputDto
                                {
                                    Name = "" // Missing office name
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var result = await _processor.ProcessLocationHierarchyAsync(
                message, "test-source", CancellationToken.None);

            // Assert
            Assert.Equal(
                LocationHierarchyMessageProcessor.LocationProcessingResult.Status.Invalid, 
                result.ProcessingStatus);
        }

        [Fact]
        public async Task ProcessLocationHierarchyAsync_WithDuplicateLocation_ReturnsDuplicate()
        {
            // Arrange
            var message = CreateValidMessage();

            // Act - Process twice
            await _processor.ProcessLocationHierarchyAsync(
                message, "test-source", CancellationToken.None);
            var result2 = await _processor.ProcessLocationHierarchyAsync(
                message, "test-source", CancellationToken.None);

            // Assert
            Assert.Equal(
                LocationHierarchyMessageProcessor.LocationProcessingResult.Status.Duplicate, 
                result2.ProcessingStatus);
        }

        private LocationHierarchyMessage CreateValidMessage()
        {
            return new LocationHierarchyMessage
            {
                CorrelationId = "test-correlation-id",
                CreatedAt = DateTime.UtcNow.ToString("O"),
                Source = "Test",
                Location = new LocationInputDto
                {
                    Name = $"Test Location {Guid.NewGuid()}",
                    Address = "123 Test Street",
                    City = "Test City",
                    Region = "Test Region",
                    PostalCode = "12345",
                    Country = "Test Country",
                    Buildings = new List<BuildingInputDto>
                    {
                        new BuildingInputDto
                        {
                            Name = "Test Building",
                            Address = "123 Test Street",
                            Offices = new List<OfficeInputDto>
                            {
                                new OfficeInputDto
                                {
                                    Name = "Test Office",
                                    FloorNumber = 1
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
