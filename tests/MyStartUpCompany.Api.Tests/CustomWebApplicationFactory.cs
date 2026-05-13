using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestDatabaseName = "IntegrationTestDatabase";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment to trigger InMemory database in AppDbContextFactory
        builder.UseEnvironment("AutomatedIntegrationTest");
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Ensure database is created
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        return host;
    }

    /// <summary>
    /// Clears all data from the in-memory database while keeping the schema
    /// </summary>
    public void ClearDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Remove all entities
        context.Companies.RemoveRange(context.Companies);
        context.Employees.RemoveRange(context.Employees);
        context.SaveChanges();
    }

    /// <summary>
    /// Resets the database by deleting and recreating it
    /// </summary>
    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    /// <summary>
    /// Seeds test data into the database
    /// </summary>
    public void SeedTestData(Action<AppDbContext> seedAction)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        seedAction(context);
        context.SaveChanges();
    }

    /// <summary>
    /// Gets a DbContext instance for test setup/assertions
    /// </summary>
    public AppDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}
