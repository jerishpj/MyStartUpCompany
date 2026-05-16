using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Tests.Shared.TestData;

/// <summary>
/// Centralized test data seeding scenarios for Company-related tests
/// </summary>
public static class CompanyTestData
{
    /// <summary>
    /// Seeds a single test company (ID auto-generated)
    /// </summary>
    public static void SeedSingleCompany(AppDbContext context)
    {
        var company = new CompanyBuilder()
            .AsTestCompany() // Remove explicit ID
            .Build();

        context.Companies.Add(company);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds the standard three companies (IDs auto-generated)
    /// </summary>
    public static void SeedStandardCompanies(AppDbContext context)
    {
        var companies = new[]
        {
            new CompanyBuilder().AsAcmeCorporation().Build(),
            new CompanyBuilder().AsTechVisionInc().Build(),
            new CompanyBuilder().AsGlobalSystemsLtd().Build()
        };

        context.Companies.AddRange(companies);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds two test companies (IDs auto-generated)
    /// </summary>
    public static void SeedMultipleCompanies(AppDbContext context)
    {
        var companies = new[]
        {
            new CompanyBuilder()
                // .WithId(1) - REMOVED
                .WithName("Company A")
                .WithDescription("First test company")
                .WithAddress("123 Test St")
                .WithCity("Testville")
                .WithRegion("Test Region")
                .WithCountry("Testland")
                .WithPostalCode("12345")
                .WithPhone("123-456-7890")
                .Build(),
            new CompanyBuilder()
                // .WithId(2) - REMOVED
                .WithName("Company B")
                .WithDescription("Second test company")
                .WithAddress("456 Test Ave")
                .WithCity("Testopolis")
                .WithRegion("Test Region")
                .WithCountry("Testland")
                .WithPostalCode("67890")
                .WithPhone("098-765-4321")
                .Build()
        };

        context.Companies.AddRange(companies);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds a specific company by name (ID auto-generated)
    /// </summary>
    public static void SeedCompanyByName(AppDbContext context, string name)
    {
        var company = new CompanyBuilder()
            .WithName(name)
            .Build();

        context.Companies.Add(company);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds a large dataset of companies for performance/pagination testing
    /// </summary>
    public static void SeedLargeDataset(AppDbContext context, int count = 100)
    {
        var companies = Enumerable.Range(1, count)
            .Select(i => new CompanyBuilder()
                // Don't set explicit IDs - let database generate them
                .WithName($"Company {i}")
                .WithDescription($"Test company number {i}")
                .WithCity($"City {i}")
                .Build())
            .ToArray();

        context.Companies.AddRange(companies);
        context.SaveChanges();
    }
}