using FluentAssertions;
using MyStartUpCompany.Api.Features.CompanyDetails.Queries;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Api.Tests.Helpers;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Tests.Features.CompanyDetails.Queries;

public class GetCompanyQueryHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly GetCompanyQueryHandler _handler;

    public GetCompanyQueryHandlerTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext(Guid.NewGuid().ToString());
        TestDbContextFactory.SeedTestData(_context);
        _handler = new GetCompanyQueryHandler(_context, LoggerMock.Create<GetCompanyQueryHandler>());
    }

    [Fact]
    public async Task HandleAsync_WithValidId_ReturnsCompanyDto()
    {
        // Arrange
        var companyId = 1;

        // Act
        var result = await _handler.HandleAsync(companyId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(companyId);
        result.Name.Should().Be("Acme Corporation");
        result.City.Should().Be("San Francisco");
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentId = 999;

        // Act
        var act = async () => await _handler.HandleAsync(nonExistentId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*{nonExistentId}*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task HandleAsync_WithInvalidId_ThrowsBadRequestException(int invalidId)
    {
        // Arrange & Act
        var act = async () => await _handler.HandleAsync(invalidId);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Company ID must be greater than 0");
    }

    [Fact]
    public async Task HandleAsync_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var companyId = 2;

        // Act
        var result = await _handler.HandleAsync(companyId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(2);
        result.Name.Should().Be("TechVision Inc");
        result.Description.Should().Be("Software development company");
        result.Address.Should().Be("456 Tech Avenue");
        result.City.Should().Be("Seattle");
        result.Region.Should().Be("WA");
        result.PostalCode.Should().Be("98101");
        result.Country.Should().Be("United States");
        result.Phone.Should().Be("+1-555-987-6543");
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        var companyId = 1;
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = async () => await _handler.HandleAsync(companyId, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}