using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Tests.Shared.Helpers;

/// <summary>
/// Factory for creating SQLite in-memory database contexts for unit tests
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// Creates a SQLite in-memory AppDbContext for unit testing
    /// Uses a unique connection string per instance to ensure test isolation
    /// </summary>
    public static AppDbContext CreateInMemoryContext(string databaseName = "TestDb")
    {
        // SQLite in-memory database with mode=memory and unique cache name for test isolation
        var connectionString = $"Data Source=file:{databaseName}?mode=memory&cache=shared";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connectionString)
            .Options;

        var context = new AppDbContext(options);

        // Ensure schema is created for the SQLite database
        context.Database.EnsureCreated();

        // Clear seed data so tests can control data setup
        context.Companies.RemoveRange(context.Companies);
        context.Employees.RemoveRange(context.Employees);
        context.Projects.RemoveRange(context.Projects);
        context.Locations.RemoveRange(context.Locations);
        context.Buildings.RemoveRange(context.Buildings);
        context.Offices.RemoveRange(context.Offices);
        context.ProjectTypeReferences.RemoveRange(context.ProjectTypeReferences);
        context.SaveChanges();

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