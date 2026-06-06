using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MyStartUpCompany.Api.Features.Buildings.Queries;
using MyStartUpCompany.Api.Features.Buildings.Models;
using MyStartUpCompany.Api.Tests.Shared.TestData.Builders;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Api.Shared.Models;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.Features.Buildings.Queries;

/// <summary>
/// Unit tests for GetBuildingQueryHandler
/// </summary>
public class GetBuildingQueryHandlerTests
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<ILogger<GetBuildingQueryHandler>> _loggerMock;
    private readonly GetBuildingQueryHandler _handler;

    public GetBuildingQueryHandlerTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"BuildingTest_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppDbContext(options);
        _loggerMock = new Mock<ILogger<GetBuildingQueryHandler>>();
        _handler = new GetBuildingQueryHandler(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidBuildingId_ReturnsBuilding()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var building = new BuildingBuilder()
            .WithId(1)
            .WithLocationId(location.Id)
            .AsTestBuilding()
            .Build();
        _dbContext.Buildings.Add(building);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync(building.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(building.Id);
        result.Name.Should().Be(building.Name);
        result.Address.Should().Be(building.Address);
        result.LocationId.Should().Be(location.Id);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentBuildingId_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentId = 9999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.HandleAsync(nonExistentId));
    }

    [Fact]
    public async Task HandleAsync_WithInactiveBuildingId_ReturnsBuildingRegardless()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var inactiveBuilding = new BuildingBuilder()
            .WithId(1)
            .WithLocationId(location.Id)
            .AsInactiveBuilding()
            .Build();
        _dbContext.Buildings.Add(inactiveBuilding);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync(inactiveBuilding.Id);

        // Assert
        result.Should().NotBeNull();
        result.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_WithMultipleBuildings_ReturnsCorrectBuilding()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var building1 = new BuildingBuilder().WithId(1).WithLocationId(location.Id).AsModernOfficeTower().Build();
        var building2 = new BuildingBuilder().WithId(2).WithLocationId(location.Id).AsHistoricBuilding().Build();
        var building3 = new BuildingBuilder().WithId(3).WithLocationId(location.Id).AsIndustrialComplex().Build();

        _dbContext.Buildings.AddRange(building1, building2, building3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync(building2.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(building2.Id);
        result.Name.Should().Be(building2.Name);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var building = new BuildingBuilder()
            .WithId(1)
            .WithLocationId(location.Id)
            .AsTestBuilding()
            .Build();
        _dbContext.Buildings.Add(building);
        await _dbContext.SaveChangesAsync();

        var cts = new CancellationTokenSource();

        // Act
        var result = await _handler.HandleAsync(building.Id, cts.Token);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(building.Id);
    }

    [Fact]
    public async Task HandleAsync_WithZeroId_ThrowsNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.HandleAsync(0));
    }

    [Fact]
    public async Task HandleAsync_WithNegativeId_ThrowsNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.HandleAsync(-1));
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}

/// <summary>
/// Unit tests for GetAllBuildingsQueryHandler
/// </summary>
public class GetAllBuildingsQueryHandlerTests
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<ILogger<GetAllBuildingsQueryHandler>> _loggerMock;
    private readonly GetAllBuildingsQueryHandler _handler;

    public GetAllBuildingsQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"AllBuildingsTest_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppDbContext(options);
        _loggerMock = new Mock<ILogger<GetAllBuildingsQueryHandler>>();
        _handler = new GetAllBuildingsQueryHandler(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyDatabase_ReturnsEmptyCollection()
    {
        // Act
        var result = await _handler.HandleAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_WithMultipleBuildings_ReturnsAllBuildings()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var building1 = new BuildingBuilder().WithId(1).WithLocationId(location.Id).AsModernOfficeTower().Build();
        var building2 = new BuildingBuilder().WithId(2).WithLocationId(location.Id).AsHistoricBuilding().Build();
        var building3 = new BuildingBuilder().WithId(3).WithLocationId(location.Id).AsIndustrialComplex().Build();

        _dbContext.Buildings.AddRange(building1, building2, building3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Select(b => b.Id).Should().Contain(new[] { 1, 2, 3 });
    }

    [Fact]
    public async Task HandleAsync_ReturnsResultsSortedByName()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var building1 = new BuildingBuilder().WithId(1).WithLocationId(location.Id).WithName("Zebra Building").Build();
        var building2 = new BuildingBuilder().WithId(2).WithLocationId(location.Id).WithName("Apple Building").Build();
        var building3 = new BuildingBuilder().WithId(3).WithLocationId(location.Id).WithName("Middle Building").Build();

        _dbContext.Buildings.AddRange(building1, building2, building3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync();

        // Assert
        result.Should().HaveCount(3);
        var buildingNames = result.Select(b => b.Name).ToList();
        buildingNames.Should().BeInAscendingOrder();
        buildingNames.First().Should().Be("Apple Building");
    }

    [Fact]
    public async Task HandleAsync_IncludesInactiveBuildings()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var activeBuilding = new BuildingBuilder().WithId(1).WithLocationId(location.Id).WithIsActive(true).Build();
        var inactiveBuilding = new BuildingBuilder().WithId(2).WithLocationId(location.Id).WithIsActive(false).Build();

        _dbContext.Buildings.AddRange(activeBuilding, inactiveBuilding);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(b => b.IsActive && b.Id == 1);
        result.Should().Contain(b => !b.IsActive && b.Id == 2);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var building = new BuildingBuilder()
            .WithId(1)
            .WithLocationId(location.Id)
            .AsTestBuilding()
            .Build();
        _dbContext.Buildings.Add(building);
        await _dbContext.SaveChangesAsync();

        var cts = new CancellationTokenSource();

        // Act
        var result = await _handler.HandleAsync(cts.Token);

        // Assert
        result.Should().HaveCount(1);
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}

/// <summary>
/// Unit tests for GetFilteredBuildingsQueryHandler
/// </summary>
public class GetFilteredBuildingsQueryHandlerTests
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<ILogger<GetFilteredBuildingsQueryHandler>> _loggerMock;
    private readonly GetFilteredBuildingsQueryHandler _handler;

    public GetFilteredBuildingsQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"FilteredBuildingsTest_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppDbContext(options);
        _loggerMock = new Mock<ILogger<GetFilteredBuildingsQueryHandler>>();
        _handler = new GetFilteredBuildingsQueryHandler(_dbContext, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithNoFilters_ReturnsAllBuildings()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var building1 = new BuildingBuilder().WithId(1).WithLocationId(location.Id).AsModernOfficeTower().Build();
        var building2 = new BuildingBuilder().WithId(2).WithLocationId(location.Id).AsHistoricBuilding().Build();
        var building3 = new BuildingBuilder().WithId(3).WithLocationId(location.Id).AsIndustrialComplex().Build();

        _dbContext.Buildings.AddRange(building1, building2, building3);
        await _dbContext.SaveChangesAsync();

        var request = new SearchBuildingRequest();

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task HandleAsync_WithSearchTerm_ReturnsFilteredBuildings()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var building1 = new BuildingBuilder()
            .WithId(1)
            .WithLocationId(location.Id)
            .WithName("Modern Office Tower")
            .Build();
        var building2 = new BuildingBuilder()
            .WithId(2)
            .WithLocationId(location.Id)
            .WithName("Historic Building")
            .Build();

        _dbContext.Buildings.AddRange(building1, building2);
        await _dbContext.SaveChangesAsync();

        var request = new SearchBuildingRequest(SearchTerm: "Modern");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Contain("Modern");
    }

    [Fact]
    public async Task HandleAsync_WithLocationIdFilter_ReturnsCorrectBuildings()
    {
        // Arrange
        var location1 = new LocationBuilder().WithId(1).WithName("Location 1").Build();
        var location2 = new LocationBuilder().WithId(2).WithName("Location 2").Build();
        _dbContext.Locations.AddRange(location1, location2);

        var building1 = new BuildingBuilder().WithId(1).WithLocationId(location1.Id).AsModernOfficeTower().Build();
        var building2 = new BuildingBuilder().WithId(2).WithLocationId(location2.Id).AsHistoricBuilding().Build();

        _dbContext.Buildings.AddRange(building1, building2);
        await _dbContext.SaveChangesAsync();

        var request = new SearchBuildingRequest(LocationId: location1.Id);

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().LocationId.Should().Be(location1.Id);
    }

    [Fact]
    public async Task HandleAsync_WithBuildingCodeFilter_ReturnsCorrectBuilding()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var building1 = new BuildingBuilder()
            .WithId(1)
            .WithLocationId(location.Id)
            .WithBuildingCode("MOT-001")
            .Build();
        var building2 = new BuildingBuilder()
            .WithId(2)
            .WithLocationId(location.Id)
            .WithBuildingCode("HIST-001")
            .Build();

        _dbContext.Buildings.AddRange(building1, building2);
        await _dbContext.SaveChangesAsync();

        var request = new SearchBuildingRequest(BuildingCode: "MOT-001");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().BuildingCode.Should().Be("MOT-001");
    }

    [Fact]
    public async Task HandleAsync_WithIsActiveFilter_ReturnsOnlyActiveBuildings()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var activeBuilding = new BuildingBuilder()
            .WithId(1)
            .WithLocationId(location.Id)
            .WithIsActive(true)
            .Build();
        var inactiveBuilding = new BuildingBuilder()
            .WithId(2)
            .WithLocationId(location.Id)
            .WithIsActive(false)
            .Build();

        _dbContext.Buildings.AddRange(activeBuilding, inactiveBuilding);
        await _dbContext.SaveChangesAsync();

        var request = new SearchBuildingRequest(IsActive: true);

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var buildings = Enumerable.Range(1, 15)
            .Select(i => new BuildingBuilder()
                .WithId(i)
                .WithLocationId(location.Id)
                .WithName($"Building {i:D2}")
                .Build())
            .ToList();

        _dbContext.Buildings.AddRange(buildings);
        await _dbContext.SaveChangesAsync();

        var request = new SearchBuildingRequest(PageNumber: 2, PageSize: 5);

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(5);
        result.TotalCount.Should().Be(15);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(5);
    }

    [Fact]
    public async Task HandleAsync_WithMultipleFilters_AppliesAllFilters()
    {
        // Arrange
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        _dbContext.Locations.Add(location);

        var building1 = new BuildingBuilder()
            .WithId(1)
            .WithLocationId(location.Id)
            .WithName("Modern Office Tower")
            .WithBuildingCode("MOT-001")
            .WithIsActive(true)
            .Build();
        var building2 = new BuildingBuilder()
            .WithId(2)
            .WithLocationId(location.Id)
            .WithName("Historic Building")
            .WithBuildingCode("HIST-001")
            .WithIsActive(true)
            .Build();
        var building3 = new BuildingBuilder()
            .WithId(3)
            .WithLocationId(location.Id)
            .WithName("Modern Complex")
            .WithBuildingCode("MOC-001")
            .WithIsActive(false)
            .Build();

        _dbContext.Buildings.AddRange(building1, building2, building3);
        await _dbContext.SaveChangesAsync();

        var request = new SearchBuildingRequest(
            SearchTerm: "Modern",
            LocationId: location.Id,
            IsActive: true);

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Id.Should().Be(1);
        result.Items.First().Name.Should().Contain("Modern");
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}
