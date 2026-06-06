using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MyStartUpCompany.Api.Features.Buildings;
using MyStartUpCompany.Api.Features.Buildings.Models;
using MyStartUpCompany.Api.Features.Buildings.Queries;
using MyStartUpCompany.Api.Tests.Shared.TestData.Builders;
using MyStartUpCompany.Api.Shared.Models;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.Features.Buildings;

/// <summary>
/// Unit tests for BuildingsController
/// </summary>
public class BuildingsControllerTests
{
    private readonly Mock<IGetBuildingQueryHandler> _getBuildingHandlerMock;
    private readonly Mock<IGetAllBuildingsQueryHandler> _getAllBuildingsHandlerMock;
    private readonly Mock<IGetFilteredBuildingsQueryHandler> _getFilteredBuildingsHandlerMock;
    private readonly Mock<ILogger<BuildingsController>> _loggerMock;
    private readonly BuildingsController _controller;

    public BuildingsControllerTests()
    {
        _getBuildingHandlerMock = new Mock<IGetBuildingQueryHandler>();
        _getAllBuildingsHandlerMock = new Mock<IGetAllBuildingsQueryHandler>();
        _getFilteredBuildingsHandlerMock = new Mock<IGetFilteredBuildingsQueryHandler>();
        _loggerMock = new Mock<ILogger<BuildingsController>>();

        _controller = new BuildingsController(
            _getBuildingHandlerMock.Object,
            _getAllBuildingsHandlerMock.Object,
            _getFilteredBuildingsHandlerMock.Object,
            _loggerMock.Object);
    }

    #region GetBuilding Tests

    [Fact]
    public async Task GetBuilding_WithValidId_ReturnsOkWithBuilding()
    {
        // Arrange
        var buildingId = 1;
        var buildingResponse = new BuildingBuilder()
            .WithId(buildingId)
            .AsModernOfficeTower()
            .Build();
        var response = MapToResponse(buildingResponse);

        _getBuildingHandlerMock
            .Setup(h => h.HandleAsync(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetBuilding(buildingId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(response);
        _getBuildingHandlerMock.Verify(h => h.HandleAsync(buildingId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBuilding_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var buildingId = 9999;
        _getBuildingHandlerMock
            .Setup(h => h.HandleAsync(buildingId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Building", buildingId));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetBuilding(buildingId, CancellationToken.None));
    }

    [Fact]
    public async Task GetBuilding_WithZeroId_ReturnsNotFound()
    {
        // Arrange
        var buildingId = 0;
        _getBuildingHandlerMock
            .Setup(h => h.HandleAsync(buildingId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Building", buildingId));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetBuilding(buildingId, CancellationToken.None));
    }

    [Fact]
    public async Task GetBuilding_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        var buildingId = 1;
        var buildingResponse = new BuildingBuilder()
            .WithId(buildingId)
            .AsTestBuilding()
            .Build();
        var response = MapToResponse(buildingResponse);

        var cts = new CancellationTokenSource();
        _getBuildingHandlerMock
            .Setup(h => h.HandleAsync(buildingId, cts.Token))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetBuilding(buildingId, cts.Token);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _getBuildingHandlerMock.Verify(h => h.HandleAsync(buildingId, cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetBuilding_WithInactiveBuilding_StillReturnsOk()
    {
        // Arrange
        var buildingId = 1;
        var buildingResponse = new BuildingBuilder()
            .WithId(buildingId)
            .AsInactiveBuilding()
            .Build();
        var response = MapToResponse(buildingResponse);

        _getBuildingHandlerMock
            .Setup(h => h.HandleAsync(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetBuilding(buildingId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        (okResult?.Value as BuildingResponse)?.IsActive.Should().BeFalse();
    }

    #endregion

    #region GetAllBuildings Tests

    [Fact]
    public async Task GetAllBuildings_WithMultipleBuildings_ReturnsOkWithList()
    {
        // Arrange
        var building1 = new BuildingBuilder().WithId(1).AsModernOfficeTower().Build();
        var building2 = new BuildingBuilder().WithId(2).AsHistoricBuilding().Build();
        var buildings = new[] { MapToResponse(building1), MapToResponse(building2) };

        _getAllBuildingsHandlerMock
            .Setup(h => h.HandleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(buildings);

        // Act
        var result = await _controller.GetAllBuildings(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var resultList = okResult?.Value as IEnumerable<BuildingResponse>;
        resultList.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllBuildings_WithEmptyDatabase_ReturnsOkWithEmptyList()
    {
        // Arrange
        var emptyList = Array.Empty<BuildingResponse>();
        _getAllBuildingsHandlerMock
            .Setup(h => h.HandleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetAllBuildings(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var resultList = okResult?.Value as IEnumerable<BuildingResponse>;
        resultList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllBuildings_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        var building = new BuildingBuilder().WithId(1).AsTestBuilding().Build();
        var buildings = new[] { MapToResponse(building) };

        var cts = new CancellationTokenSource();
        _getAllBuildingsHandlerMock
            .Setup(h => h.HandleAsync(cts.Token))
            .ReturnsAsync(buildings);

        // Act
        var result = await _controller.GetAllBuildings(cts.Token);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _getAllBuildingsHandlerMock.Verify(h => h.HandleAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetAllBuildings_ReturnsListInSortedOrder()
    {
        // Arrange
        var building1 = new BuildingBuilder().WithId(1).WithName("Zebra").Build();
        var building2 = new BuildingBuilder().WithId(2).WithName("Apple").Build();
        var building3 = new BuildingBuilder().WithId(3).WithName("Middle").Build();
        var buildings = new[] 
        { 
            MapToResponse(building2),  // Apple first
            MapToResponse(building3),  // Middle second
            MapToResponse(building1)   // Zebra last
        };

        _getAllBuildingsHandlerMock
            .Setup(h => h.HandleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(buildings);

        // Act
        var result = await _controller.GetAllBuildings(CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        var resultList = okResult?.Value as IEnumerable<BuildingResponse>;
        resultList.Select(b => b.Name).Should().BeInAscendingOrder();
    }

    #endregion

    #region GetFilteredBuildings Tests

    [Fact]
    public async Task GetFilteredBuildings_WithNoFilters_ReturnsOkWithPagedResult()
    {
        // Arrange
        var buildings = new[] 
        { 
            MapToResponse(new BuildingBuilder().WithId(1).AsModernOfficeTower().Build()),
            MapToResponse(new BuildingBuilder().WithId(2).AsHistoricBuilding().Build())
        };

        var pagedResult = new PagedResult<BuildingResponse>
        {
            Items = buildings,
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchBuildingRequest();
        _getFilteredBuildingsHandlerMock
            .Setup(h => h.HandleAsync(It.IsAny<SearchBuildingRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetFilteredBuildings(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var resultData = okResult?.Value as PagedResult<BuildingResponse>;
        resultData?.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetFilteredBuildings_WithSearchTerm_PassesSearchTermToHandler()
    {
        // Arrange
        var buildings = new[] 
        { 
            MapToResponse(new BuildingBuilder().WithId(1).WithName("Modern Office Tower").Build())
        };

        var pagedResult = new PagedResult<BuildingResponse>
        {
            Items = buildings,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchBuildingRequest(SearchTerm: "Modern");
        _getFilteredBuildingsHandlerMock
            .Setup(h => h.HandleAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetFilteredBuildings(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _getFilteredBuildingsHandlerMock.Verify(
            h => h.HandleAsync(It.Is<SearchBuildingRequest>(r => r.SearchTerm == "Modern"), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task GetFilteredBuildings_WithLocationIdFilter_PassesLocationIdToHandler()
    {
        // Arrange
        var buildings = new[] 
        { 
            MapToResponse(new BuildingBuilder().WithId(1).WithLocationId(5).Build())
        };

        var pagedResult = new PagedResult<BuildingResponse>
        {
            Items = buildings,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchBuildingRequest(LocationId: 5);
        _getFilteredBuildingsHandlerMock
            .Setup(h => h.HandleAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetFilteredBuildings(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _getFilteredBuildingsHandlerMock.Verify(
            h => h.HandleAsync(It.Is<SearchBuildingRequest>(r => r.LocationId == 5), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task GetFilteredBuildings_WithPagination_ReturnsCorrectPagedResult()
    {
        // Arrange
        var buildings = Enumerable.Range(1, 5)
            .Select(i => MapToResponse(new BuildingBuilder().WithId(i).WithName($"Building {i}").Build()))
            .ToList();

        var pagedResult = new PagedResult<BuildingResponse>
        {
            Items = buildings,
            TotalCount = 25,
            PageNumber = 2,
            PageSize = 5
        };

        var request = new SearchBuildingRequest(PageNumber: 2, PageSize: 5);
        _getFilteredBuildingsHandlerMock
            .Setup(h => h.HandleAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetFilteredBuildings(request, CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        var resultData = okResult?.Value as PagedResult<BuildingResponse>;
        resultData?.PageNumber.Should().Be(2);
        resultData?.PageSize.Should().Be(5);
        resultData?.TotalCount.Should().Be(25);
        resultData?.Items.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetFilteredBuildings_WithEmptyResult_ReturnsOkWithEmptyItems()
    {
        // Arrange
        var pagedResult = new PagedResult<BuildingResponse>
        {
            Items = Array.Empty<BuildingResponse>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchBuildingRequest(SearchTerm: "NonExistent");
        _getFilteredBuildingsHandlerMock
            .Setup(h => h.HandleAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetFilteredBuildings(request, CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        var resultData = okResult?.Value as PagedResult<BuildingResponse>;
        resultData?.Items.Should().BeEmpty();
        resultData?.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetFilteredBuildings_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        var pagedResult = new PagedResult<BuildingResponse>
        {
            Items = Array.Empty<BuildingResponse>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        var cts = new CancellationTokenSource();
        var request = new SearchBuildingRequest();
        _getFilteredBuildingsHandlerMock
            .Setup(h => h.HandleAsync(request, cts.Token))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetFilteredBuildings(request, cts.Token);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _getFilteredBuildingsHandlerMock.Verify(
            h => h.HandleAsync(request, cts.Token), 
            Times.Once);
    }

    [Fact]
    public async Task GetFilteredBuildings_WithMultipleFilters_PassesAllFiltersToHandler()
    {
        // Arrange
        var buildings = new[] 
        { 
            MapToResponse(new BuildingBuilder().WithId(1).WithName("Modern Office Tower").Build())
        };

        var pagedResult = new PagedResult<BuildingResponse>
        {
            Items = buildings,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchBuildingRequest(
            SearchTerm: "Modern",
            LocationId: 5,
            BuildingCode: "MOT-001",
            IsActive: true,
            PageNumber: 1,
            PageSize: 10);

        _getFilteredBuildingsHandlerMock
            .Setup(h => h.HandleAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetFilteredBuildings(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _getFilteredBuildingsHandlerMock.Verify(
            h => h.HandleAsync(It.Is<SearchBuildingRequest>(r =>
                r.SearchTerm == "Modern" &&
                r.LocationId == 5 &&
                r.BuildingCode == "MOT-001" &&
                r.IsActive == true),
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    #endregion

    #region Helper Methods

    private static BuildingResponse MapToResponse(Building building)
    {
        return new BuildingResponse
        {
            Id = building.Id,
            LocationId = building.LocationId,
            Name = building.Name,
            BuildingCode = building.BuildingCode,
            Description = building.Description,
            Address = building.Address,
            NumberOfFloors = building.NumberOfFloors,
            YearConstructed = building.YearConstructed,
            TotalFloorArea = building.TotalFloorArea,
            ContactPerson = building.ContactPerson,
            Phone = building.Phone,
            IsActive = building.IsActive,
            CreatedAt = building.CreatedAt,
            UpdatedAt = building.UpdatedAt
        };
    }

    #endregion
}
