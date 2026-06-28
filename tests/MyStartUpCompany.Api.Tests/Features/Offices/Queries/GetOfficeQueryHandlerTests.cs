using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using MyStartUpCompany.Api.Features.Offices.Queries;
using MyStartUpCompany.Api.Features.Offices.Models;
using MyStartUpCompany.Api.Tests.Shared.TestData.Builders;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.Features.Offices.Queries;

/// <summary>
/// Unit tests for GetOfficeQueryHandler
/// </summary>
public class GetOfficeQueryHandlerTests
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetOfficeQueryHandler> _logger;
    private readonly GetOfficeQueryHandler _handler;

    public GetOfficeQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"OfficeTest_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppDbContext(options);
        _logger = Substitute.For<ILogger<GetOfficeQueryHandler>>();
        _handler = new GetOfficeQueryHandler(_dbContext, _logger);
    }

    [Fact]
    public async Task HandleAsync_WithValidOfficeId_ReturnsOffice()
    {
        // Arrange
        var (office, _, _) = new OfficeBuilder().WithId(1).AsTestOffice().BuildWithBuildingAndLocation();

        _dbContext.Locations.Add(office.Building!.Location!);
        _dbContext.Buildings.Add(office.Building);
        _dbContext.Offices.Add(office);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync(office.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(office.Id);
        result.Name.Should().Be(office.Name);
        result.BuildingId.Should().Be(office.BuildingId);
        result.FloorNumber.Should().Be(office.FloorNumber);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentOfficeId_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentId = 9999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.HandleAsync(nonExistentId));
    }

    [Fact]
    public async Task HandleAsync_WithInactiveOfficeId_ReturnsOfficeRegardless()
    {
        // Arrange
        var office = new OfficeBuilder()
            .WithId(1)
            .WithBuildingId(1)
            .AsInactiveOffice()
            .Build();

        _dbContext.Offices.Add(office);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync(office.Id);

        // Assert
        result.Should().NotBeNull();
        result.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_WithDenormalizedFields_ReturnsDenormalizedData()
    {
        // Arrange
        var office = new OfficeBuilder()
            .WithId(1)
            .WithBuildingId(1)
            .AsExecutiveSuite()
            .Build();

        _dbContext.Offices.Add(office);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync(office.Id);

        // Assert
        result.BuildingName.Should().Be("Corporate Tower");
        result.LocationCity.Should().Be("San Francisco");
        result.LocationRegion.Should().Be("CA");
        result.LocationCountry.Should().Be("United States");
    }

    [Fact]
    public async Task HandleAsync_WithMultipleOffices_ReturnsCorrectOffice()
    {
        // Arrange
        var office1 = new OfficeBuilder().WithId(1).WithBuildingId(1).AsExecutiveSuite().Build();
        var office2 = new OfficeBuilder().WithId(2).WithBuildingId(1).AsSalesDepartmentFloor().Build();
        var office3 = new OfficeBuilder().WithId(3).WithBuildingId(1).AsEngineeringLab().Build();

        _dbContext.Offices.AddRange(office1, office2, office3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync(office2.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(office2.Id);
        result.Name.Should().Be(office2.Name);
        result.Department.Should().Be("Sales");
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        var office = new OfficeBuilder()
            .WithId(1)
            .WithBuildingId(1)
            .AsTestOffice()
            .Build();

        _dbContext.Offices.Add(office);
        await _dbContext.SaveChangesAsync();

        var cts = new CancellationTokenSource();

        // Act
        var result = await _handler.HandleAsync(office.Id, cts.Token);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(office.Id);
    }

    [Fact]
    public async Task HandleAsync_WithZeroId_ThrowsNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.HandleAsync(0));
    }

    [Fact]
    public async Task HandleAsync_WithAllOptionalFields_ReturnsCompleteOffice()
    {
        // Arrange
        var office = new OfficeBuilder()
            .WithId(1)
            .WithBuildingId(1)
            .WithCapacity(50)
            .WithOfficeType("Open Office")
            .WithSquareMeters(1500)
            .WithManager("John Doe")
            .WithEmail("john@company.com")
            .Build();

        _dbContext.Offices.Add(office);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync(office.Id);

        // Assert
        result.Capacity.Should().Be(50);
        result.OfficeType.Should().Be("Open Office");
        result.SquareMeters.Should().Be(1500);
        result.Manager.Should().Be("John Doe");
        result.Email.Should().Be("john@company.com");
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}

/// <summary>
/// Unit tests for GetAllOfficesQueryHandler
/// </summary>
public class GetAllOfficesQueryHandlerTests
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetAllOfficesQueryHandler> _logger;
    private readonly GetAllOfficesQueryHandler _handler;

    public GetAllOfficesQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"AllOfficesTest_{Guid.NewGuid()}")
            .Options;

             _dbContext = new AppDbContext(options);
            _logger = Substitute.For<ILogger<GetAllOfficesQueryHandler>>();
            _handler = new GetAllOfficesQueryHandler(_dbContext, _logger);
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
    public async Task HandleAsync_WithMultipleOffices_ReturnsAllOffices()
    {
        // Arrange
        var office1 = new OfficeBuilder().WithId(1).WithBuildingId(1).AsExecutiveSuite().Build();
        var office2 = new OfficeBuilder().WithId(2).WithBuildingId(1).AsSalesDepartmentFloor().Build();
        var office3 = new OfficeBuilder().WithId(3).WithBuildingId(1).AsEngineeringLab().Build();

        _dbContext.Offices.AddRange(office1, office2, office3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Select(o => o.Id).Should().Contain(new[] { 1, 2, 3 });
    }

    [Fact]
    public async Task HandleAsync_ReturnsResultsSortedByName()
    {
        // Arrange
        var office1 = new OfficeBuilder().WithId(1).WithBuildingId(1).WithName("Zebra Office").Build();
        var office2 = new OfficeBuilder().WithId(2).WithBuildingId(1).WithName("Apple Office").Build();
        var office3 = new OfficeBuilder().WithId(3).WithBuildingId(1).WithName("Middle Office").Build();

        _dbContext.Offices.AddRange(office1, office2, office3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync();

        // Assert
        result.Should().HaveCount(3);
        var officeNames = result.Select(o => o.Name).ToList();
        officeNames.Should().BeInAscendingOrder();
        officeNames.First().Should().Be("Apple Office");
    }

    [Fact]
    public async Task HandleAsync_IncludesInactiveOffices()
    {
        // Arrange
        var activeOffice = new OfficeBuilder().WithId(1).WithBuildingId(1).WithIsActive(true).Build();
        var inactiveOffice = new OfficeBuilder().WithId(2).WithBuildingId(1).WithIsActive(false).Build();

        _dbContext.Offices.AddRange(activeOffice, inactiveOffice);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(o => o.IsActive && o.Id == 1);
        result.Should().Contain(o => !o.IsActive && o.Id == 2);
    }

    [Fact]
    public async Task HandleAsync_FromMultipleBuildings_ReturnsAllOffices()
    {
        // Arrange
        var office1 = new OfficeBuilder().WithId(1).WithBuildingId(1).WithName("Building 1 Office").Build();
        var office2 = new OfficeBuilder().WithId(2).WithBuildingId(2).WithName("Building 2 Office").Build();
        var office3 = new OfficeBuilder().WithId(3).WithBuildingId(1).WithName("Building 1 Office 2").Build();

        _dbContext.Offices.AddRange(office1, office2, office3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _handler.HandleAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(o => o.BuildingId == 1);
        result.Should().Contain(o => o.BuildingId == 2);
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        var office = new OfficeBuilder().WithId(1).WithBuildingId(1).AsTestOffice().Build();
        _dbContext.Offices.Add(office);
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
/// Unit tests for GetFilteredOfficesQueryHandler
/// </summary>
public class GetFilteredOfficesQueryHandlerTests
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetFilteredOfficesQueryHandler> _logger;
    private readonly GetFilteredOfficesQueryHandler _handler;

    public GetFilteredOfficesQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"FilteredOfficesTest_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppDbContext(options);
        _logger = Substitute.For<ILogger<GetFilteredOfficesQueryHandler>>();
        _handler = new GetFilteredOfficesQueryHandler(_dbContext, _logger);
    }

    [Fact]
    public async Task HandleAsync_WithNoFilters_ReturnsAllOffices()
    {
        // Arrange
        var office1 = new OfficeBuilder().WithId(1).WithBuildingId(1).AsExecutiveSuite().Build();
        var office2 = new OfficeBuilder().WithId(2).WithBuildingId(1).AsSalesDepartmentFloor().Build();
        var office3 = new OfficeBuilder().WithId(3).WithBuildingId(1).AsEngineeringLab().Build();

        _dbContext.Offices.AddRange(office1, office2, office3);
        await _dbContext.SaveChangesAsync();

        var request = new SearchOfficeRequest();

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task HandleAsync_WithSearchTerm_ReturnsFilteredOffices()
    {
        // Arrange
        var office1 = new OfficeBuilder()
            .WithId(1)
            .WithBuildingId(1)
            .WithName("Executive Suite")
            .Build();
        var office2 = new OfficeBuilder()
            .WithId(2)
            .WithBuildingId(1)
            .WithName("Sales Department")
            .Build();

        _dbContext.Offices.AddRange(office1, office2);
        await _dbContext.SaveChangesAsync();

        var request = new SearchOfficeRequest(SearchTerm: "Executive");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Contain("Executive");
    }

    [Fact]
    public async Task HandleAsync_WithBuildingIdFilter_ReturnsCorrectOffices()
    {
        // Arrange
        var office1 = new OfficeBuilder().WithId(1).WithBuildingId(1).AsExecutiveSuite().Build();
        var office2 = new OfficeBuilder().WithId(2).WithBuildingId(2).AsSalesDepartmentFloor().Build();
        var office3 = new OfficeBuilder().WithId(3).WithBuildingId(1).AsEngineeringLab().Build();

        _dbContext.Offices.AddRange(office1, office2, office3);
        await _dbContext.SaveChangesAsync();

        var request = new SearchOfficeRequest(BuildingId: 1);

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(o => o.BuildingId.Should().Be(1));
    }

    [Fact]
    public async Task HandleAsync_WithDepartmentFilter_ReturnsCorrectOffices()
    {
        // Arrange
        var office1 = new OfficeBuilder()
            .WithId(1)
            .WithBuildingId(1)
            .WithDepartment("Sales")
            .Build();
        var office2 = new OfficeBuilder()
            .WithId(2)
            .WithBuildingId(1)
            .WithDepartment("Engineering")
            .Build();

        _dbContext.Offices.AddRange(office1, office2);
        await _dbContext.SaveChangesAsync();

        var request = new SearchOfficeRequest(Department: "Sales");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Department.Should().Be("Sales");
    }

    [Fact]
    public async Task HandleAsync_WithLocationCityFilter_ReturnsCorrectOffices()
    {
        // Arrange
        var office1 = new OfficeBuilder()
            .WithId(1)
            .WithBuildingId(1)
            .WithLocationCity("San Francisco")
            .Build();
        var office2 = new OfficeBuilder()
            .WithId(2)
            .WithBuildingId(2)
            .WithLocationCity("New York")
            .Build();

        _dbContext.Offices.AddRange(office1, office2);
        await _dbContext.SaveChangesAsync();

        var request = new SearchOfficeRequest(LocationCity: "San Francisco");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().LocationCity.Should().Be("San Francisco");
    }

    [Fact]
    public async Task HandleAsync_WithOfficeTypeFilter_ReturnsCorrectOffices()
    {
        // Arrange
        var office1 = new OfficeBuilder()
            .WithId(1)
            .WithBuildingId(1)
            .WithOfficeType("Open Office")
            .Build();
        var office2 = new OfficeBuilder()
            .WithId(2)
            .WithBuildingId(1)
            .WithOfficeType("Meeting Room")
            .Build();

        _dbContext.Offices.AddRange(office1, office2);
        await _dbContext.SaveChangesAsync();

        var request = new SearchOfficeRequest(OfficeType: "Open Office");

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().OfficeType.Should().Be("Open Office");
    }

    [Fact]
    public async Task HandleAsync_WithIsActiveFilter_ReturnsOnlyActiveOffices()
    {
        // Arrange
        var activeOffice = new OfficeBuilder()
            .WithId(1)
            .WithBuildingId(1)
            .WithIsActive(true)
            .Build();
        var inactiveOffice = new OfficeBuilder()
            .WithId(2)
            .WithBuildingId(1)
            .WithIsActive(false)
            .Build();

        _dbContext.Offices.AddRange(activeOffice, inactiveOffice);
        await _dbContext.SaveChangesAsync();

        var request = new SearchOfficeRequest(IsActive: true);

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
        var offices = Enumerable.Range(1, 15)
            .Select(i => new OfficeBuilder()
                .WithId(i)
                .WithBuildingId(1)
                .WithName($"Office {i:D2}")
                .Build())
            .ToList();

        _dbContext.Offices.AddRange(offices);
        await _dbContext.SaveChangesAsync();

        var request = new SearchOfficeRequest(PageNumber: 2, PageSize: 5);

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
        var office1 = new OfficeBuilder()
            .WithId(1)
            .WithBuildingId(1)
            .WithName("Executive Suite")
            .WithDepartment("Management")
            .WithLocationCity("San Francisco")
            .WithIsActive(true)
            .Build();
        var office2 = new OfficeBuilder()
            .WithId(2)
            .WithBuildingId(1)
            .WithName("Sales Floor")
            .WithDepartment("Sales")
            .WithLocationCity("San Francisco")
            .WithIsActive(true)
            .Build();
        var office3 = new OfficeBuilder()
            .WithId(3)
            .WithBuildingId(1)
            .WithName("Executive Office")
            .WithDepartment("Management")
            .WithLocationCity("New York")
            .WithIsActive(false)
            .Build();

        _dbContext.Offices.AddRange(office1, office2, office3);
        await _dbContext.SaveChangesAsync();

        var request = new SearchOfficeRequest(
            SearchTerm: "Executive",
            Department: "Management",
            LocationCity: "San Francisco",
            IsActive: true);

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Id.Should().Be(1);
        result.Items.First().Name.Should().Contain("Executive");
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}
