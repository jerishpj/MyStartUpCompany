using MyStartUpCompany.Api.Features.CompanyDetails.Queries;
using MyStartUpCompany.Api.Tests.Shared.Helpers;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Tests.Features.CompanyDetails.Queries;

public class GetAllCompaniesQueryHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly GetAllCompaniesQueryHandler _handler;

    public GetAllCompaniesQueryHandlerTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext(Guid.NewGuid().ToString());
        TestDbContextFactory.SeedTestData(_context);
        _handler = new GetAllCompaniesQueryHandler(_context, LoggerMock.Create<GetAllCompaniesQueryHandler>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsAllCompanies()
    {
        // Act
        var result = await _handler.HandleAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task HandleAsync_ReturnsCompaniesWithCorrectProperties()
    {
        // Act
        var result = (await _handler.HandleAsync()).ToList();

        // Assert
        result.Should().Contain(c => c.Name == "Acme Corporation");
        result.Should().Contain(c => c.Name == "TechVision Inc");
        result.Should().Contain(c => c.Name == "Global Systems Ltd");

        var acme = result.First(c => c.Id == 1);
        acme.City.Should().Be("San Francisco");
        acme.Phone.Should().Be("+1-555-123-4567");
    }

    [Fact]
    public async Task HandleAsync_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var emptyContext = TestDbContextFactory.CreateInMemoryContext(Guid.NewGuid().ToString());
        var handler = new GetAllCompaniesQueryHandler(emptyContext, LoggerMock.Create<GetAllCompaniesQueryHandler>());

        // Act
        var result = await handler.HandleAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        emptyContext.Dispose();
    }

    [Fact]
    public async Task HandleAsync_WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = async () => await _handler.HandleAsync(cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}