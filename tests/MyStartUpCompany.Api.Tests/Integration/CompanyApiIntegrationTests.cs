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
        int companyId = 0;
        SeedData(context =>
        {
            var company = new CompanyBuilder().AsTestCompany().Build();
            context.Companies.Add(company);
            context.SaveChanges();
            companyId = company.Id; // Capture the auto-generated ID
        });

        // Act
        var response = await Client.GetAsync($"/api/company/{companyId}");

        // Assert
        var content = await response.ShouldBeOkWithContent();
        content.Should().Contain("Test Company");
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsCompanies_WhenDataExists()
    {
        // Arrange
        SeedData(CompanyTestData.SeedMultipleCompanies);

        // Act
        var response = await Client.GetAsync("/api/company");

        // Assert
        // Don't check for specific IDs, just check for company names
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