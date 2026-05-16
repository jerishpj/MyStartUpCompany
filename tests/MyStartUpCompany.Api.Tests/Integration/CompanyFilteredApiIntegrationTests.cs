using System.Net;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Shared.Models;

namespace MyStartUpCompany.Api.Tests.Integration;

/// <summary>
/// Integration tests for the GetFilteredCompanies endpoint
/// </summary>
public class CompanyFilteredApiIntegrationTests : IntegrationTestBase
{
    public CompanyFilteredApiIntegrationTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
    }

    #region Pagination Tests

    [Fact]
    public async Task GetFilteredCompanies_WithDefaultPagination_ReturnsFirstPageSuccessfully()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Items.Should().NotBeEmpty();
        result.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetFilteredCompanies_WithSecondPage_ReturnsCorrectPage()
    {
        // Arrange
        SeedData(context =>
        {
            for (int i = 1; i <= 25; i++)
            {
                context.Companies.Add(new CompanyBuilder()
                    .WithName($"Company {i:D3}")
                    .WithCity("TestCity")
                    .WithCountry("TestCountry")
                    .Build());
            }
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?pageNumber=2&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.Items.Should().HaveCount(10);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(1, 10)]
    [InlineData(1, 20)]
    public async Task GetFilteredCompanies_WithVariousPageSizes_ReturnsCorrectCount(int pageNumber, int pageSize)
    {
        // Arrange
        SeedData(context =>
        {
            for (int i = 1; i <= 30; i++)
            {
                context.Companies.Add(new CompanyBuilder()
                    .WithName($"Test Company {i}")
                    .Build());
            }
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync($"/api/company/search?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.PageSize.Should().Be(pageSize);
        result.Items.Should().HaveCountLessThanOrEqualTo(pageSize);
    }

    [Fact]
    public async Task GetFilteredCompanies_PaginationMetadata_IsCalculatedCorrectly()
    {
        // Arrange
        SeedData(context =>
        {
            for (int i = 1; i <= 35; i++)
            {
                context.Companies.Add(new CompanyBuilder()
                    .WithName($"Company {i}")
                    .Build());
            }
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?pageNumber=2&pageSize=10");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(35);
        result.TotalPages.Should().Be(4);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetFilteredCompanies_LastPage_HasCorrectMetadata()
    {
        // Arrange
        SeedData(context =>
        {
            for (int i = 1; i <= 25; i++)
            {
                context.Companies.Add(new CompanyBuilder()
                    .WithName($"Company {i}")
                    .Build());
            }
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?pageNumber=3&pageSize=10");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(5);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetFilteredCompanies_PageBeyondTotalPages_ReturnsEmptyResult()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?pageNumber=999&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
        result.PageNumber.Should().Be(999);
    }

    #endregion

    #region Filter Tests - Country

    [Fact]
    public async Task GetFilteredCompanies_FilterByCountry_ReturnsOnlyMatchingCompanies()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("US Company 1")
                .WithCountry("United States")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("US Company 2")
                .WithCountry("United States")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("Canada Company")
                .WithCountry("Canada")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?country=United%20States&pageSize=50");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(c => c.Country.Should().Contain("United States"));
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetFilteredCompanies_FilterByPartialCountry_ReturnsMatchingCompanies()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 1")
                .WithCountry("United States")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 2")
                .WithCountry("United Kingdom")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 3")
                .WithCountry("Canada")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?country=United&pageSize=50");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(c => c.Country.Should().Contain("United"));
    }

    [Fact]
    public async Task GetFilteredCompanies_FilterByNonExistentCountry_ReturnsEmptyResult()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?country=Atlantis&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region Filter Tests - Region

    [Fact]
    public async Task GetFilteredCompanies_FilterByRegion_ReturnsOnlyMatchingCompanies()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("CA Company 1")
                .WithRegion("CA")
                .WithCountry("United States")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("CA Company 2")
                .WithRegion("CA")
                .WithCountry("United States")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("NY Company")
                .WithRegion("NY")
                .WithCountry("United States")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?region=CA&pageSize=50");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(c => c.Region.Should().Contain("CA"));
    }

    [Fact]
    public async Task GetFilteredCompanies_FilterByRegion_HandlesNullRegion()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    #endregion

    #region Filter Tests - City

    [Fact]
    public async Task GetFilteredCompanies_FilterByCity_ReturnsOnlyMatchingCompanies()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("SF Company 1")
                .WithCity("San Francisco")
                .WithCountry("United States")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("SF Company 2")
                .WithCity("San Francisco")
                .WithCountry("United States")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("LA Company")
                .WithCity("Los Angeles")
                .WithCountry("United States")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?city=San%20Francisco&pageSize=50");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(c => c.City.Should().Contain("San Francisco"));
    }

    [Fact]
    public async Task GetFilteredCompanies_FilterByPartialCity_ReturnsMatchingCompanies()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 1")
                .WithCity("San Francisco")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 2")
                .WithCity("San Diego")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 3")
                .WithCity("Los Angeles")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?city=San&pageSize=50");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(c => c.City.Should().Contain("San"));
    }

    [Fact]
    public async Task GetFilteredCompanies_FilterByCity_HandlesEmptyString()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?city=&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    #endregion

    #region Filter Tests - PostalCode

    [Fact]
    public async Task GetFilteredCompanies_FilterByPostalCode_ReturnsExactMatch()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 1")
                .WithPostalCode("94105")
                .WithCity("San Francisco")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 2")
                .WithPostalCode("94105")
                .WithCity("San Francisco")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 3")
                .WithPostalCode("90210")
                .WithCity("Beverly Hills")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?postalCode=94105&pageSize=50");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(c => c.PostalCode.Should().Be("94105"));
    }

    [Fact]
    public async Task GetFilteredCompanies_FilterByNonExistentPostalCode_ReturnsEmpty()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?postalCode=99999&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region Filter Tests - SearchTerm

    [Fact]
    public async Task GetFilteredCompanies_SearchByName_ReturnsMatchingCompanies()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?searchTerm=Acme&pageSize=20");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.Items.Should().Contain(c => c.Name.Contains("Acme", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetFilteredCompanies_SearchByDescription_ReturnsMatchingCompanies()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 1")
                .WithDescription("We specialize in software development")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("Company 2")
                .WithDescription("Hardware manufacturing")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?searchTerm=software&pageSize=20");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetFilteredCompanies_SearchByPartialTerm_ReturnsMatchingCompanies()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("TechCorp")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("TechVision")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("GlobalSystems")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?searchTerm=Tech&pageSize=20");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetFilteredCompanies_SearchWithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?searchTerm=%25_%5B%5D&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetFilteredCompanies_SearchCaseInsensitive_ReturnsMatches()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var responseLower = await Client.GetAsync("/api/company/search?searchTerm=acme&pageSize=20");
        var responseUpper = await Client.GetAsync("/api/company/search?searchTerm=ACME&pageSize=20");

        // Assert
        var resultLower = await responseLower.ReadAsJsonAsync<PagedResult<Company>>();
        var resultUpper = await responseUpper.ReadAsJsonAsync<PagedResult<Company>>();

        resultLower.Should().NotBeNull();
        resultUpper.Should().NotBeNull();
        resultLower!.TotalCount.Should().Be(resultUpper!.TotalCount);
    }

    #endregion

    #region Combined Filter Tests

    [Fact]
    public async Task GetFilteredCompanies_WithMultipleFilters_ReturnsIntersection()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("Match Company")
                .WithCountry("United States")
                .WithRegion("CA")
                .WithCity("San Francisco")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("Partial Match")
                .WithCountry("United States")
                .WithRegion("CA")
                .WithCity("Los Angeles")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("No Match")
                .WithCountry("Canada")
                .WithRegion("ON")
                .WithCity("Toronto")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync(
            "/api/company/search?country=United%20States&region=CA&city=San%20Francisco&pageSize=10");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Match Company");
    }

    [Fact]
    public async Task GetFilteredCompanies_WithAllFilters_ReturnsCorrectResult()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync(
            "/api/company/search?country=United%20States&region=CA&city=San%20Francisco&postalCode=94105&searchTerm=Acme&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetFilteredCompanies_CombinedFiltersWithNoMatches_ReturnsEmpty()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync(
            "/api/company/search?country=United%20States&city=NonExistentCity&pageSize=10");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetFilteredCompanies_CountryAndSearchTerm_ReturnsCorrectResults()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("TechCorp USA")
                .WithCountry("United States")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("TechCorp Canada")
                .WithCountry("Canada")
                .Build());
            context.Companies.Add(new CompanyBuilder()
                .WithName("OtherCorp USA")
                .WithCountry("United States")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync(
            "/api/company/search?country=United%20States&searchTerm=Tech&pageSize=10");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("TechCorp USA");
    }

    #endregion

    #region Ordering Tests

    [Fact]
    public async Task GetFilteredCompanies_ResultsAreOrderedByName()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder().WithName("Zeta Corp").Build());
            context.Companies.Add(new CompanyBuilder().WithName("Alpha Inc").Build());
            context.Companies.Add(new CompanyBuilder().WithName("Beta LLC").Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?pageSize=50");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().BeInAscendingOrder(c => c.Name);
    }

    [Fact]
    public async Task GetFilteredCompanies_OrderingIsStableAcrossPages()
    {
        // Arrange
        SeedData(context =>
        {
            for (int i = 1; i <= 15; i++)
            {
                context.Companies.Add(new CompanyBuilder()
                    .WithName($"Company {i:D3}")
                    .Build());
            }
            context.SaveChanges();
        });

        // Act
        var page1Response = await Client.GetAsync("/api/company/search?pageNumber=1&pageSize=5");
        var page2Response = await Client.GetAsync("/api/company/search?pageNumber=2&pageSize=5");

        // Assert
        var page1Result = await page1Response.ReadAsJsonAsync<PagedResult<Api.Features.CompanyDetails.Models.Company>>();
        var page2Result = await page2Response.ReadAsJsonAsync<PagedResult<Api.Features.CompanyDetails.Models.Company>>();

        var lastItemPage1 = page1Result!.Items.Last();
        var firstItemPage2 = page2Result!.Items.First();

        string.Compare(lastItemPage1.Name, firstItemPage2.Name, StringComparison.Ordinal)
            .Should().BeLessThanOrEqualTo(0);
    }

    #endregion

    #region Empty Result Tests

    [Fact]
    public async Task GetFilteredCompanies_WithNoMatches_ReturnsEmptyResult()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?searchTerm=NonExistentCompany12345XYZ&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Api.Features.CompanyDetails.Models.Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageNumber.Should().Be(1);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task GetFilteredCompanies_WithEmptyDatabase_ReturnsEmptyResult()
    {
        // Arrange - No data seeded

        // Act
        var response = await Client.GetAsync("/api/company/search?pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    #endregion

    #region Response Structure Tests

    [Fact]
    public async Task GetFilteredCompanies_ReturnsCorrectResponseStructure()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
        result.PageNumber.Should().BeGreaterThan(0);
        result.PageSize.Should().BeGreaterThan(0);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetFilteredCompanies_ReturnsAllCompanyProperties()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?pageSize=1");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        if (result!.Items.Any())
        {
            var company = result.Items.First();
            company.Id.Should().BeGreaterThan(0);
            company.Name.Should().NotBeNullOrEmpty();
            company.Address.Should().NotBeNullOrEmpty();
            company.City.Should().NotBeNullOrEmpty();
            company.Country.Should().NotBeNullOrEmpty();
        }
    }

    #endregion

    #region URL Encoding Tests

    [Fact]
    public async Task GetFilteredCompanies_WithSpacesInParameters_HandlesCorrectly()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new CompanyBuilder()
                .WithName("Test Company")
                .WithCity("San Francisco")
                .WithCountry("United States")
                .Build());
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?city=San%20Francisco&country=United%20States&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GetFilteredCompanies_WithLargePageSize_ReturnsAllAvailableItems()
    {
        // Arrange
        SeedData(context =>
        {
            for (int i = 1; i <= 15; i++)
            {
                context.Companies.Add(new CompanyBuilder()
                    .WithName($"Company {i}")
                    .Build());
            }
            context.SaveChanges();
        });

        // Act
        var response = await Client.GetAsync("/api/company/search?pageSize=1000");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(15);
        result.TotalCount.Should().Be(15);
    }

    [Fact]
    public async Task GetFilteredCompanies_WithMinimalPageSize_ReturnsOneItem()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company/search?pageSize=1");

        // Assert
        var result = await response.ReadAsJsonAsync<PagedResult<Company>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(1);
    }

    #endregion
}