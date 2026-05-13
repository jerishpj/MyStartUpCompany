using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyStartUpCompany.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AppDbContext to the service collection with environment-based database provider selection.
    /// Uses SQL Server for production/development and InMemory for automated integration tests.
    /// </summary>
    public static IServiceCollection AddAppDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment? environment = null)
    {
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            AppDbContextFactory.ConfigureDbContext(
                (DbContextOptionsBuilder<AppDbContext>)options,
                configuration, 
                environment?.EnvironmentName);
        });

        return services;
    }

    // Keep these for backward compatibility or specific use cases
    public static IServiceCollection AddSqlServerDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

        return services;
    }

    public static IServiceCollection AddInMemoryDatabase(
        this IServiceCollection services,
        string databaseName = "TestDb")
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName);
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        return services;
    }
}
