using MyStartUpCompany.Api.Tests.TestData.Builders;
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
        var company = new CompanyBuilder()
            .AsTestCompany()
            .Build();

        context.Companies.Add(company);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds two companies (Company A and Company B)
    /// </summary>
    public static void SeedMultipleCompanies(AppDbContext context)
    {
        var companies = new[]
        {
            new CompanyBuilder().AsCompanyA().Build(),
            new CompanyBuilder().AsCompanyB().Build()
        };

        context.Companies.AddRange(companies);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds a specific company by ID and name
    /// </summary>
    public static void SeedCompanyById(AppDbContext context, int id, string name)
    {
        var company = new CompanyBuilder()
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
        var companies = new List<MyStartUpCompany.Persistence.Entities.Company>();

        for (int i = 1; i <= count; i++)
        {
            var company = new CompanyBuilder()
                .WithId(i)
                .WithName($"Company {i}")
                .WithDescription($"Test company number {i}")
                .WithCity($"City {i}")
                .Build();

            companies.Add(company);
        }

        context.Companies.AddRange(companies);
        context.SaveChanges();
    }
}