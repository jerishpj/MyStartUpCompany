using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Tests.Shared.Helpers;

/// <summary>
/// Factory for creating in-memory database contexts for unit tests
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// Creates an in-memory AppDbContext for unit testing
    /// </summary>
    public static AppDbContext CreateInMemoryContext(string databaseName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        var context = new AppDbContext(options);
        return context;
    }

    /// <summary>
    /// Seeds standard test data using builders
    /// </summary>
    public static void SeedTestData(AppDbContext context)
    {
        SeedCompanies(context);
        SeedEmployees(context);
    }

    /// <summary>
    /// Seeds standard company test data
    /// </summary>
    public static void SeedCompanies(AppDbContext context)
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
    /// Seeds standard employee test data
    /// </summary>
    public static void SeedEmployees(AppDbContext context)
    {
        var employees = new[]
        {
            new EmployeeBuilder().AsSeniorDeveloper().Build(),
            new EmployeeBuilder().AsProjectManager().Build()
        };

        context.Employees.AddRange(employees);
        context.SaveChanges();
    }
}