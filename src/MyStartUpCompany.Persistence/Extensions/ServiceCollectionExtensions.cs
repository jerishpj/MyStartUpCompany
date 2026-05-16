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
    /// Uses DbContext pooling for better performance with high concurrency.
    /// </summary>
    public static IServiceCollection AddAppDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // USE ONLY AddDbContextPool for better performance
        // Remove the duplicate AddDbContext registration
        services.AddDbContextPool<AppDbContext>((serviceProvider, options) =>
        {
            ConfigureDbContextOptions(options, connectionString, environment);
        }, poolSize: 128); // Adjust based on your expected concurrent requests

        return services;
    }

    /// <summary>
    /// Configures DbContext options (shared configuration logic)
    /// </summary>
    private static void ConfigureDbContextOptions(
        DbContextOptionsBuilder options,
        string connectionString,
        IHostEnvironment environment)
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            // Performance optimizations
            sqlOptions.CommandTimeout(30);
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);

            // Use query splitting for better performance with complex queries
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });

        // Enable sensitive data logging only in development
        if (environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }

        // Performance: Disable client-side evaluation warnings
        options.ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore
                .Diagnostics.CoreEventId.RowLimitingOperationWithoutOrderByWarning));
    }
}
