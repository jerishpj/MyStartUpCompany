namespace MyStartUpCompany.Api.Tests.Integration;

public class CompanyApiIntegrationTests : IntegrationTestBase
{
    public CompanyApiIntegrationTests(CustomWebApplicationFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task GetCompany_ReturnsSuccess_WhenCompanyExists()
    {
        // Arrange
        SeedData(CompanyTestData.SeedSingleCompany);

        // Act
        var response = await Client.GetAsync("/api/company/1");

        // Assert
        var content = await response.ShouldBeOkWithContent();
        content.Should().Contain("Test Company");
    }

    [Fact]
    public async Task GetCompany_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Act
        var response = await Client.GetAsync("/api/company/999");

        // Assert
        response.ShouldBeNotFound();
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsEmptyList_WhenNoData()
    {
        // Act
        var response = await Client.GetAsync("/api/company");

        // Assert
        await response.ShouldBeOkWithContent();
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsCompanies_WhenDataExists()
    {
        // Arrange
        SeedData(CompanyTestData.SeedMultipleCompanies);

        // Act
        var response = await Client.GetAsync("/api/company");

        // Assert
        await response.ShouldContainInContent("Company A", "Company B");
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsStandardCompanies_WhenStandardDataSeeded()
    {
        // Arrange
        SeedData(CompanyTestData.SeedStandardCompanies);

        // Act
        var response = await Client.GetAsync("/api/company");

        // Assert
        await response.ShouldContainInContent("Acme Corporation", "TechVision Inc", "Global Systems Ltd");
    }
}