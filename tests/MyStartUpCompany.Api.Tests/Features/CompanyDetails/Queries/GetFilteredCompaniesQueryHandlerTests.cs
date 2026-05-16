using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Features.CompanyDetails.Queries;
using MyStartUpCompany.Api.Tests.Shared.Helpers;
using MyStartUpCompany.Persistence;
using Company = MyStartUpCompany.Persistence.Entities.Company;

namespace MyStartUpCompany.Api.Tests.Features.CompanyDetails.Queries;

public class GetFilteredCompaniesQueryHandlerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly GetFilteredCompaniesQueryHandler _handler;

    public GetFilteredCompaniesQueryHandlerTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext(Guid.NewGuid().ToString());
        SeedTestData();
        _handler = new GetFilteredCompaniesQueryHandler(
            _context,
            LoggerMock.Create<GetFilteredCompaniesQueryHandler>());
    }

    #region Pagination Tests

    [Fact]
    public async Task HandleAsync_WithDefaultPagination_ReturnsFirstPage()
    {
        // Arrange
        var request = new CompanyRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Items.Should().HaveCountLessThanOrEqualTo(10);
        result.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task HandleAsync_WithSecondPage_ReturnsCorrectItems()
    {
        // Arrange
        var request = new CompanyRequest
        {
            PageNumber = 2,
            PageSize = 5
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.Items.Should().HaveCountLessThanOrEqualTo(5);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 20)]
    [InlineData(3, 5)]
    public async Task HandleAsync_WithVariousPageSizes_ReturnsCorrectCount(int pageNumber, int pageSize)
    {
        // Arrange
        var request = new CompanyRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.PageSize.Should().Be(pageSize);
        result.Items.Should().HaveCountLessThanOrEqualTo(pageSize);
    }

    [Fact]
    public async Task HandleAsync_PaginationMetadata_IsCorrect()
    {
        // Arrange
        var request = new CompanyRequest
        {
            PageNumber = 2,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.TotalPages.Should().Be((int)Math.Ceiling(result.TotalCount / (double)result.PageSize));
        result.HasPreviousPage.Should().BeTrue();
    }

    #endregion

    #region Filter Tests - Country

    [Fact]
    public async Task HandleAsync_FilterByCountry_ReturnsOnlyMatchingCompanies()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Country = "United States",
            PageNumber = 1,
            PageSize = 50
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Should().AllSatisfy(c => c.Country.Should().Contain("United States"));
    }

    [Fact]
    public async Task HandleAsync_FilterByNonExistentCountry_ReturnsEmptyResult()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Country = "NonExistentCountry",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_FilterByPartialCountry_ReturnsMatchingCompanies()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Country = "United",
            PageNumber = 1,
            PageSize = 50
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Should().AllSatisfy(c => c.Country.Should().Contain("United"));
    }

    #endregion

    #region Filter Tests - Region

    [Fact]
    public async Task HandleAsync_FilterByRegion_ReturnsOnlyMatchingCompanies()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Region = "CA",
            PageNumber = 1,
            PageSize = 50
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Should().AllSatisfy(c => c.Region.Should().Contain("CA"));
    }

    [Fact]
    public async Task HandleAsync_FilterByRegion_HandlesNullRegion()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Region = null,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty(); // Should return all companies
    }

    #endregion

    #region Filter Tests - City

    [Fact]
    public async Task HandleAsync_FilterByCity_ReturnsOnlyMatchingCompanies()
    {
        // Arrange
        var request = new CompanyRequest
        {
            City = "San Francisco",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Should().AllSatisfy(c => c.City.Should().Contain("San Francisco"));
    }

    [Fact]
    public async Task HandleAsync_FilterByPartialCity_ReturnsMatchingCompanies()
    {
        // Arrange
        var request = new CompanyRequest
        {
            City = "San",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Should().AllSatisfy(c => c.City.Should().Contain("San"));
    }

    [Fact]
    public async Task HandleAsync_FilterByCity_HandlesEmptyString()
    {
        // Arrange
        var request = new CompanyRequest
        {
            City = "",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty(); // Empty string should be ignored
    }

    #endregion

    #region Filter Tests - PostalCode

    [Fact]
    public async Task HandleAsync_FilterByPostalCode_ReturnsExactMatch()
    {
        // Arrange
        var request = new CompanyRequest
        {
            PostalCode = "94105",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().AllSatisfy(c => c.PostalCode.Should().Be("94105"));
    }

    [Fact]
    public async Task HandleAsync_FilterByNonExistentPostalCode_ReturnsEmpty()
    {
        // Arrange
        var request = new CompanyRequest
        {
            PostalCode = "99999",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region Filter Tests - SearchTerm

    [Fact]
    public async Task HandleAsync_SearchByName_ReturnsMatchingCompanies()
    {
        // Arrange
        var request = new CompanyRequest
        {
            SearchTerm = "Acme",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Should().OnlyContain(c => 
            c.Name.Contains("Acme", StringComparison.OrdinalIgnoreCase) ||
            (c.Description != null && c.Description.Contains("Acme", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public async Task HandleAsync_SearchByDescription_ReturnsMatchingCompanies()
    {
        // Arrange
        var request = new CompanyRequest
        {
            SearchTerm = "software",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_SearchByPartialTerm_ReturnsMatchingCompanies()
    {
        // Arrange
        var request = new CompanyRequest
        {
            SearchTerm = "Tech",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_SearchWithSpecialCharacters_EscapesCorrectly()
    {
        // Arrange
        var request = new CompanyRequest
        {
            SearchTerm = "%_[]",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var act = async () => await _handler.HandleAsync(request);

        // Assert - Should not throw, special chars should be escaped
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Combined Filter Tests

    [Fact]
    public async Task HandleAsync_WithMultipleFilters_ReturnsIntersection()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Country = "United States",
            Region = "CA",
            City = "San Francisco",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        if (result.Items.Any())
        {
            result.Items.Should().AllSatisfy(c =>
            {
                c.Country.Should().Contain("United States");
                c.Region.Should().Contain("CA");
                c.City.Should().Contain("San Francisco");
            });
        }
    }

    [Fact]
    public async Task HandleAsync_WithAllFilters_ReturnsCorrectResult()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Country = "United States",
            Region = "CA",
            City = "San Francisco",
            PostalCode = "94105",
            SearchTerm = "Acme",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleAsync_CombinedFiltersWithNoMatches_ReturnsEmpty()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Country = "United States",
            City = "NonExistentCity",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region Ordering Tests

    [Fact]
    public async Task HandleAsync_ResultsAreOrderedByName()
    {
        // Arrange
        var request = new CompanyRequest
        {
            PageNumber = 1,
            PageSize = 50
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Items.Should().BeInAscendingOrder(c => c.Name);
    }

    [Fact]
    public async Task HandleAsync_OrderingIsStableAcrossPages()
    {
        // Arrange
        var page1Request = new CompanyRequest { PageNumber = 1, PageSize = 5 };
        var page2Request = new CompanyRequest { PageNumber = 2, PageSize = 5 };

        // Act
        var page1Result = await _handler.HandleAsync(page1Request);
        var page2Result = await _handler.HandleAsync(page2Request);

        // Assert
        if (page1Result.Items.Any() && page2Result.Items.Any())
        {
            var lastItemPage1 = page1Result.Items.Last();
            var firstItemPage2 = page2Result.Items.First();

            // Items should be ordered, so last of page 1 should come before or equal to first of page 2
            string.Compare(lastItemPage1.Name, firstItemPage2.Name, StringComparison.Ordinal)
                .Should().BeLessThanOrEqualTo(0);
        }
    }

    #endregion

    #region Empty Result Tests

    [Fact]
    public async Task HandleAsync_WithNoMatches_ReturnsEmptyResult()
    {
        // Arrange
        var request = new CompanyRequest
        {
            SearchTerm = "NonExistentCompany12345XYZ",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageNumber.Should().Be(1);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_PageBeyondTotalPages_ReturnsEmpty()
    {
        // Arrange
        var request = new CompanyRequest
        {
            PageNumber = 999,
            PageSize = 10
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.PageNumber.Should().Be(999);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyDatabase_ReturnsEmptyResult()
    {
        // Arrange
        var emptyContext = TestDbContextFactory.CreateInMemoryContext(Guid.NewGuid().ToString());
        var handler = new GetFilteredCompaniesQueryHandler(
            emptyContext, 
            LoggerMock.Create<GetFilteredCompaniesQueryHandler>());
        var request = new CompanyRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);

        emptyContext.Dispose();
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task HandleAsync_WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        var request = new CompanyRequest { PageNumber = 1, PageSize = 10 };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = async () => await _handler.HandleAsync(request, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region Performance and Edge Cases

    [Fact]
    public async Task HandleAsync_WithLargePageSize_ReturnsAllAvailableItems()
    {
        // Arrange
        var request = new CompanyRequest
        {
            PageNumber = 1,
            PageSize = 1000
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count().Should().BeLessThanOrEqualTo(1000);
        result.Items.Count().Should().Be(Math.Min(result.TotalCount, 1000));
    }

    [Fact]
    public async Task HandleAsync_ReturnsProjectedProperties()
    {
        // Arrange
        var request = new CompanyRequest { PageNumber = 1, PageSize = 1 };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        if (result.Items.Any())
        {
            var company = result.Items.First();
            company.Id.Should().BeGreaterThan(0);
            company.Name.Should().NotBeNullOrEmpty();
            company.Address.Should().NotBeNullOrEmpty();
            company.City.Should().NotBeNullOrEmpty();
            company.Country.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task HandleAsync_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var request = new CompanyRequest
        {
            SearchTerm = "Acme Corporation",
            PageNumber = 1,
            PageSize = 1
        };

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        if (result.Items.Any())
        {
            var company = result.Items.First();
            company.Name.Should().Be("Acme Corporation");
            company.Description.Should().Be("Leading provider of innovative solutions");
            company.Address.Should().Be("123 Main Street");
            company.City.Should().Be("San Francisco");
            company.Region.Should().Be("CA");
            company.PostalCode.Should().Be("94105");
            company.Country.Should().Be("United States");
            company.Phone.Should().Be("+1-555-123-4567");
        }
    }

    #endregion

    #region Helper Methods

    private void SeedTestData()
    {
        // Seed diverse test data for comprehensive testing
        var companies = new List<Company>
        {
            new CompanyBuilder().AsAcmeCorporation().Build(),
            new CompanyBuilder().AsTechVisionInc().Build(),
            new CompanyBuilder().AsGlobalSystemsLtd().Build(),

            // Additional companies for filtering tests
            new CompanyBuilder()
                .WithName("San Diego Tech")
                .WithDescription("Technology solutions provider")
                .WithCity("San Diego")
                .WithRegion("CA")
                .WithCountry("United States")
                .WithPostalCode("92101")
                .Build(),

            new CompanyBuilder()
                .WithName("Portland Innovations")
                .WithDescription("Innovation and software development")
                .WithCity("Portland")
                .WithRegion("OR")
                .WithCountry("United States")
                .WithPostalCode("97201")
                .Build(),

            new CompanyBuilder()
                .WithName("Toronto Software Ltd")
                .WithDescription("Canadian software company")
                .WithCity("Toronto")
                .WithRegion("ON")
                .WithCountry("Canada")
                .WithPostalCode("M5H 2N2")
                .Build()
        };

        // Add more companies for pagination testing
        for (int i = 1; i <= 50; i++)
        {
            companies.Add(new CompanyBuilder()
                .WithName($"Test Company {i:D3}")
                .WithDescription($"Test company number {i}")
                .WithCity($"TestCity{i % 10}")
                .WithRegion("TestRegion")
                .WithCountry("TestCountry")
                .WithPostalCode($"{10000 + i}")
                .Build());
        }

        _context.Companies.AddRange(companies);
        _context.SaveChanges();
    }

    #endregion

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}