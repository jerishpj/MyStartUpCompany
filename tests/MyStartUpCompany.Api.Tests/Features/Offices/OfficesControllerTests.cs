using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using MyStartUpCompany.Api.Features.Offices;
using MyStartUpCompany.Api.Features.Offices.Models;
using MyStartUpCompany.Api.Features.Offices.Queries;
using MyStartUpCompany.Api.Tests.Shared.TestData.Builders;
using MyStartUpCompany.Api.Shared.Models;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.Features.Offices;

/// <summary>
/// Unit tests for OfficesController
/// </summary>
public class OfficesControllerTests
{
    private readonly IGetOfficeQueryHandler _getOfficeHandler;
    private readonly IGetAllOfficesQueryHandler _getAllOfficesHandler;
    private readonly IGetFilteredOfficesQueryHandler _getFilteredOfficesHandler;
    private readonly ILogger<OfficesController> _logger;
    private readonly OfficesController _controller;

    public OfficesControllerTests()
    {
        _getOfficeHandler = Substitute.For<IGetOfficeQueryHandler>();
        _getAllOfficesHandler = Substitute.For<IGetAllOfficesQueryHandler>();
        _getFilteredOfficesHandler = Substitute.For<IGetFilteredOfficesQueryHandler>();
        _logger = Substitute.For<ILogger<OfficesController>>();

        _controller = new OfficesController(
            _getOfficeHandler,
            _getAllOfficesHandler,
            _getFilteredOfficesHandler,
            _logger);
    }

    #region GetOffice Tests

    [Fact]
    public async Task GetOffice_WithValidId_ReturnsOkWithOffice()
    {
        // Arrange
        var officeId = 1;
        var officeResponse = new OfficeBuilder()
            .WithId(officeId)
            .AsExecutiveSuite()
            .Build();
        var response = MapToResponse(officeResponse);

        _getOfficeHandler
            .HandleAsync(officeId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        // Act
        var result = await _controller.GetOffice(officeId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(response);
        await _getOfficeHandler.Received(1).HandleAsync(officeId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOffice_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var officeId = 9999;
        _getOfficeHandler
            .HandleAsync(officeId, Arg.Any<CancellationToken>())
            .Returns(x => Task.FromException<OfficeResponse>(new NotFoundException("Office", officeId)));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetOffice(officeId, CancellationToken.None));
    }

    [Fact]
    public async Task GetOffice_WithZeroId_ReturnsNotFound()
    {
        // Arrange
        var officeId = 0;
        _getOfficeHandler
            .HandleAsync(officeId, Arg.Any<CancellationToken>())
            .Returns(x => Task.FromException<OfficeResponse>(new NotFoundException("Office", officeId)));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetOffice(officeId, CancellationToken.None));
    }

    [Fact]
    public async Task GetOffice_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        var officeId = 1;
        var officeResponse = new OfficeBuilder()
            .WithId(officeId)
            .AsTestOffice()
            .Build();
        var response = MapToResponse(officeResponse);

        var cts = new CancellationTokenSource();
        _getOfficeHandler
            .HandleAsync(officeId, cts.Token)
            .Returns(Task.FromResult(response));

        // Act
        var result = await _controller.GetOffice(officeId, cts.Token);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetOffice_WithInactiveOffice_StillReturnsOk()
    {
        // Arrange
        var officeId = 1;
        var officeResponse = new OfficeBuilder()
            .WithId(officeId)
            .AsInactiveOffice()
            .Build();
        var response = MapToResponse(officeResponse);

        _getOfficeHandler
            .HandleAsync(officeId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        // Act
        var result = await _controller.GetOffice(officeId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        (okResult?.Value as OfficeResponse)?.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetOffice_WithDenormalizedFields_ReturnsCompleteData()
    {
        // Arrange
        var officeId = 1;
        var officeResponse = new OfficeBuilder()
            .WithId(officeId)
            .AsExecutiveSuite()
            .WithLocationCountry("United States")  // Match what AsExecutiveSuite sets
            .Build();
        var response = MapToResponse(officeResponse);

        _getOfficeHandler
            .HandleAsync(officeId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        // Act
        var result = await _controller.GetOffice(officeId, CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        var officeData = okResult?.Value as OfficeResponse;
        officeData?.BuildingName.Should().Be("Corporate Tower");
        officeData?.LocationCity.Should().Be("San Francisco");
        officeData?.LocationRegion.Should().Be("CA");
        officeData?.LocationCountry.Should().Be("United States");
    }

    #endregion

    #region GetAllOffices Tests

    [Fact]
    public async Task GetAllOffices_WithMultipleOffices_ReturnsOkWithList()
    {
        // Arrange
        var office1 = new OfficeBuilder().WithId(1).AsExecutiveSuite().Build();
        var office2 = new OfficeBuilder().WithId(2).AsSalesDepartmentFloor().Build();
        var offices = new[] { MapToResponse(office1), MapToResponse(office2) };

        _getAllOfficesHandler
            .HandleAsync(Arg.Any<CancellationToken>())
            .Returns(x => Task.FromResult((IEnumerable<OfficeResponse>)offices));

        // Act
        var result = await _controller.GetAllOffices(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var resultList = okResult?.Value as IEnumerable<OfficeResponse>;
        resultList.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllOffices_WithEmptyDatabase_ReturnsOkWithEmptyList()
    {
        // Arrange
        var emptyList = Array.Empty<OfficeResponse>();
        _getAllOfficesHandler
            .HandleAsync(Arg.Any<CancellationToken>())
            .Returns(x => Task.FromResult((IEnumerable<OfficeResponse>)emptyList));

        // Act
        var result = await _controller.GetAllOffices(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var resultList = okResult?.Value as IEnumerable<OfficeResponse>;
        resultList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllOffices_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        var office = new OfficeBuilder().WithId(1).AsTestOffice().Build();
        var offices = new[] { MapToResponse(office) };

        var cts = new CancellationTokenSource();
        _getAllOfficesHandler
            .HandleAsync(cts.Token)
            .Returns(x => Task.FromResult((IEnumerable<OfficeResponse>)offices));

        // Act
        var result = await _controller.GetAllOffices(cts.Token);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetAllOffices_ReturnsListInSortedOrder()
    {
        // Arrange
        var office1 = new OfficeBuilder().WithId(1).WithName("Zebra Office").Build();
        var office2 = new OfficeBuilder().WithId(2).WithName("Apple Office").Build();
        var office3 = new OfficeBuilder().WithId(3).WithName("Middle Office").Build();
        var offices = new[] 
        { 
            MapToResponse(office2),  // Apple first
            MapToResponse(office3),  // Middle second
            MapToResponse(office1)   // Zebra last
        };

        _getAllOfficesHandler
            .HandleAsync(Arg.Any<CancellationToken>())
            .Returns(x => Task.FromResult((IEnumerable<OfficeResponse>)offices));

        // Act
        var result = await _controller.GetAllOffices(CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        var resultList = okResult?.Value as IEnumerable<OfficeResponse>;
        resultList.Select(o => o.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAllOffices_IncludesInactiveOffices()
    {
        // Arrange
        var activeOffice = new OfficeBuilder().WithId(1).WithIsActive(true).Build();
        var inactiveOffice = new OfficeBuilder().WithId(2).WithIsActive(false).Build();
        var offices = new[] { MapToResponse(activeOffice), MapToResponse(inactiveOffice) };

        _getAllOfficesHandler
            .HandleAsync(Arg.Any<CancellationToken>())
            .Returns(x => Task.FromResult((IEnumerable<OfficeResponse>)offices));

        // Act
        var result = await _controller.GetAllOffices(CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        var resultList = okResult?.Value as IEnumerable<OfficeResponse>;
        resultList.Should().HaveCount(2);
        resultList.Should().Contain(o => o.IsActive);
        resultList.Should().Contain(o => !o.IsActive);
    }

    #endregion

    #region GetFilteredOffices Tests

    [Fact]
    public async Task GetFilteredOffices_WithNoFilters_ReturnsOkWithPagedResult()
    {
        // Arrange
        var offices = new[] 
        { 
            MapToResponse(new OfficeBuilder().WithId(1).AsExecutiveSuite().Build()),
            MapToResponse(new OfficeBuilder().WithId(2).AsSalesDepartmentFloor().Build())
        };

        var pagedResult = new PagedResult<OfficeResponse>
        {
            Items = offices,
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchOfficeRequest();
        _getFilteredOfficesHandler
            .HandleAsync(Arg.Any<SearchOfficeRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(pagedResult));

        // Act
        var result = await _controller.GetFilteredOffices(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var resultData = okResult?.Value as PagedResult<OfficeResponse>;
        resultData?.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetFilteredOffices_WithSearchTerm_PassesSearchTermToHandler()
    {
        // Arrange
        var offices = new[] 
        { 
            MapToResponse(new OfficeBuilder().WithId(1).WithName("Executive Suite").Build())
        };

        var pagedResult = new PagedResult<OfficeResponse>
        {
            Items = offices,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchOfficeRequest(SearchTerm: "Executive");
        _getFilteredOfficesHandler
            .HandleAsync(request, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(pagedResult));

        // Act
        var result = await _controller.GetFilteredOffices(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _getFilteredOfficesHandler.Received(1).HandleAsync(
            Arg.Is<SearchOfficeRequest>(r => r.SearchTerm == "Executive"), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFilteredOffices_WithBuildingIdFilter_PassesBuildingIdToHandler()
    {
        // Arrange
        var offices = new[] 
        { 
            MapToResponse(new OfficeBuilder().WithId(1).WithBuildingId(5).Build())
        };

        var pagedResult = new PagedResult<OfficeResponse>
        {
            Items = offices,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchOfficeRequest(BuildingId: 5);
        _getFilteredOfficesHandler
            .HandleAsync(request, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(pagedResult));

        // Act
        var result = await _controller.GetFilteredOffices(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _getFilteredOfficesHandler.Received(1).HandleAsync(
            Arg.Is<SearchOfficeRequest>(r => r.BuildingId == 5), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFilteredOffices_WithDepartmentFilter_PassesDepartmentToHandler()
    {
        // Arrange
        var offices = new[] 
        { 
            MapToResponse(new OfficeBuilder().WithId(1).WithDepartment("Sales").Build())
        };

        var pagedResult = new PagedResult<OfficeResponse>
        {
            Items = offices,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchOfficeRequest(Department: "Sales");
        _getFilteredOfficesHandler
            .HandleAsync(request, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(pagedResult));

        // Act
        var result = await _controller.GetFilteredOffices(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _getFilteredOfficesHandler.Received(1).HandleAsync(
            Arg.Is<SearchOfficeRequest>(r => r.Department == "Sales"), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFilteredOffices_WithLocationCityFilter_PassesLocationCityToHandler()
    {
        // Arrange
        var offices = new[] 
        { 
            MapToResponse(new OfficeBuilder().WithId(1).WithLocationCity("San Francisco").Build())
        };

        var pagedResult = new PagedResult<OfficeResponse>
        {
            Items = offices,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchOfficeRequest(LocationCity: "San Francisco");
        _getFilteredOfficesHandler
            .HandleAsync(request, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(pagedResult));

        // Act
        var result = await _controller.GetFilteredOffices(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _getFilteredOfficesHandler.Received(1).HandleAsync(
            Arg.Is<SearchOfficeRequest>(r => r.LocationCity == "San Francisco"), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFilteredOffices_WithPagination_ReturnsCorrectPagedResult()
    {
        // Arrange
        var offices = Enumerable.Range(1, 5)
            .Select(i => MapToResponse(new OfficeBuilder().WithId(i).WithName($"Office {i}").Build()))
            .ToList();

        var pagedResult = new PagedResult<OfficeResponse>
        {
            Items = offices,
            TotalCount = 25,
            PageNumber = 2,
            PageSize = 5
        };

        var request = new SearchOfficeRequest(PageNumber: 2, PageSize: 5);
        _getFilteredOfficesHandler
            .HandleAsync(request, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(pagedResult));

        // Act
        var result = await _controller.GetFilteredOffices(request, CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        var resultData = okResult?.Value as PagedResult<OfficeResponse>;
        resultData?.PageNumber.Should().Be(2);
        resultData?.PageSize.Should().Be(5);
        resultData?.TotalCount.Should().Be(25);
        resultData?.Items.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetFilteredOffices_WithEmptyResult_ReturnsOkWithEmptyItems()
    {
        // Arrange
        var pagedResult = new PagedResult<OfficeResponse>
        {
            Items = Array.Empty<OfficeResponse>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchOfficeRequest(SearchTerm: "NonExistent");
        _getFilteredOfficesHandler
            .HandleAsync(request, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(pagedResult));

        // Act
        var result = await _controller.GetFilteredOffices(request, CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        var resultData = okResult?.Value as PagedResult<OfficeResponse>;
        resultData?.Items.Should().BeEmpty();
        resultData?.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetFilteredOffices_WithCancellationToken_PropagatesToken()
    {
        // Arrange
        var pagedResult = new PagedResult<OfficeResponse>
        {
            Items = Array.Empty<OfficeResponse>(),
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        var cts = new CancellationTokenSource();
        var request = new SearchOfficeRequest();
        _getFilteredOfficesHandler
            .HandleAsync(request, cts.Token)
            .Returns(Task.FromResult(pagedResult));

        // Act
        var result = await _controller.GetFilteredOffices(request, cts.Token);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _getFilteredOfficesHandler.Received(1).HandleAsync(request, cts.Token);
    }

    [Fact]
    public async Task GetFilteredOffices_WithMultipleFilters_PassesAllFiltersToHandler()
    {
        // Arrange
        var offices = new[] 
        { 
            MapToResponse(new OfficeBuilder().WithId(1).WithName("Executive Suite").Build())
        };

        var pagedResult = new PagedResult<OfficeResponse>
        {
            Items = offices,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var request = new SearchOfficeRequest(
            SearchTerm: "Executive",
            BuildingId: 5,
            Department: "Management",
            LocationCity: "San Francisco",
            OfficeType: "Executive Suite",
            IsActive: true,
            PageNumber: 1,
            PageSize: 10);

        _getFilteredOfficesHandler
            .HandleAsync(request, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(pagedResult));

        // Act
        var result = await _controller.GetFilteredOffices(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _getFilteredOfficesHandler.Received(1).HandleAsync(
            Arg.Is<SearchOfficeRequest>(r =>
                r.SearchTerm == "Executive" &&
                r.BuildingId == 5 &&
                r.Department == "Management" &&
                r.LocationCity == "San Francisco" &&
                r.OfficeType == "Executive Suite" &&
                r.IsActive == true),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region Helper Methods

    private static OfficeResponse MapToResponse(Office office)
    {
        return new OfficeResponse
        {
            Id = office.Id,
            BuildingId = office.BuildingId,
            Name = office.Name,
            OfficeCode = office.OfficeCode,
            Description = office.Description,
            FloorNumber = office.FloorNumber,
            Section = office.Section,
            Capacity = office.Capacity,
            OfficeType = office.OfficeType,
            SquareMeters = office.SquareMeters,
            Department = office.Department,
            Manager = office.Manager,
            Phone = office.Phone,
            Email = office.Email,
            BuildingName = office.BuildingName,
            LocationCity = office.LocationCity,
            LocationRegion = office.LocationRegion,
            LocationCountry = office.LocationCountry,
            IsActive = office.IsActive,
            CreatedAt = office.CreatedAt,
            UpdatedAt = office.UpdatedAt
        };
    }

    #endregion
}

