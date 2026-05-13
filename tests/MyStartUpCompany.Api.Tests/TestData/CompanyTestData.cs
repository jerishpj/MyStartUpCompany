using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Tests.TestData;

/// <summary>
/// Centralized test data seeding scenarios for Company-related tests
/// </summary>
public static class CompanyTestData
{
    /// <summary>
    /// Seeds a single test company with ID 1
    /// </summary>
    public static void SeedSingleCompany(AppDbContext context)
    {
        var company = new Tests.Builders.CompanyBuilder()
            .AsTestCompany()
            .Build();

        context.Companies.Add(company);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds the standard three companies (Acme, TechVision, Global Systems)
    /// </summary>
    public static void SeedStandardCompanies(AppDbContext context)
    {
        var companies = new[]
        {
            new Tests.Builders.CompanyBuilder().AsAcmeCorporation().Build(),
            new Tests.Builders.CompanyBuilder().AsTechVisionInc().Build(),
            new Tests.Builders.CompanyBuilder().AsGlobalSystemsLtd().Build()
        };

        context.Companies.AddRange(companies);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds two test companies (A and B)
    /// </summary>
    public static void SeedMultipleCompanies(AppDbContext context)
    {
        var companies = new[]
        {
            new Tests.Builders.CompanyBuilder()
                .WithId(1)
                .WithName("Company A")
                .WithDescription("First test company")
                .WithAddress("123 Test St")
                .WithCity("Testville")
                .WithRegion("Test Region")
                .WithCountry("Testland")
                .WithPostalCode("12345")
                .WithPhone("123-456-7890")
                .Build(),
            new Tests.Builders.CompanyBuilder()
                .WithId(2)
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
    /// Seeds a specific company by ID and name
    /// </summary>
    public static void SeedCompanyById(AppDbContext context, int id, string name)
    {
        var company = new Tests.Builders.CompanyBuilder()
            .WithId(id)
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
            .Select(i => new Tests.Builders.CompanyBuilder()
                .WithId(i)
                .WithName($"Company {i}")
                .WithDescription($"Test company number {i}")
                .WithCity($"City {i}")
                .Build())
            .ToArray();

        context.Companies.AddRange(companies);
        context.SaveChanges();
    }
}