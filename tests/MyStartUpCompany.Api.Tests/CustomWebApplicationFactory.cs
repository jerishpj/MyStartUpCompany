using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestDatabaseName = "IntegrationTestDatabase";
    private readonly string _uniqueDatabaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment to trigger InMemory database in AppDbContextFactory
        builder.UseEnvironment("AutomatedIntegrationTest");

        // Override DbContext registration to use unique in-memory database per factory instance
        builder.ConfigureServices(services =>
        {
            // Remove existing AppDbContext registration
            var descriptors = services.Where(d => d.ServiceType == typeof(AppDbContext) || 
                                                   d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                                                   (d.ServiceType.IsGenericType && 
                                                    d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                                      .ToList();
            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Register with unique database name
            services.AddDbContextPool<AppDbContext>((serviceProvider, options) =>
            {
                options.UseInMemoryDatabase(_uniqueDatabaseName);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }, poolSize: 128);
        });
    }

    public string GetTestDatabaseName() => _uniqueDatabaseName;

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

        var isInMemory = context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";

        try
        {
            if (isInMemory)
            {
                // For in-memory database, clear entities in reverse dependency order
                var projectTypes = context.ProjectTypeReferences.ToList();
                if (projectTypes.Any())
                    context.ProjectTypeReferences.RemoveRange(projectTypes);

                var projects = context.Projects.ToList();
                if (projects.Any())
                    context.Projects.RemoveRange(projects);

                var employees = context.Employees.ToList();
                if (employees.Any())
                    context.Employees.RemoveRange(employees);

                var companies = context.Companies.ToList();
                if (companies.Any())
                    context.Companies.RemoveRange(companies);

                context.SaveChanges();
            }
            else
            {
                // For SQL Server, truncate tables
                context.Database.ExecuteSqlRaw("DELETE FROM [ProjectTypeReferences]");
                context.Database.ExecuteSqlRaw("DELETE FROM [Projects]");
                context.Database.ExecuteSqlRaw("DELETE FROM [Employees]");
                context.Database.ExecuteSqlRaw("DELETE FROM [Companies]");
            }
        }
        catch
        {
            // Silent fail if context is in inconsistent state
        }
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
    /// Seeds test data into the database with IDENTITY_INSERT support
    /// </summary>
    public void SeedTestData(Action<AppDbContext> seedAction)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Enable IDENTITY_INSERT for SQL Server (not needed for InMemory)
        var isInMemory = context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        
        if (!isInMemory)
        {
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Companies ON");
        }

        try
        {
            seedAction(context);
            context.SaveChanges();
        }
        finally
        {
            if (!isInMemory)
            {
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Companies OFF");
            }
        }
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
