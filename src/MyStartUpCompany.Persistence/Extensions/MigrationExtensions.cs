using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MyStartUpCompany.Persistence.Extensions;

/// <summary>
/// Extension methods for database migration operations.
/// Provides centralized, safe migration execution with logging and error handling.
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Applies all pending migrations to the database.
    /// Logs migration progress and handles errors gracefully.
    /// 
    /// Usage:
    ///   var services = new ServiceCollection();
    ///   services.AddAppDatabase(config, env);
    ///   using var serviceProvider = services.BuildServiceProvider();
    ///   await serviceProvider.ApplyMigrationsAsync();
    /// </summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider, ILogger? logger = null)
    {
        logger?.LogInformation("Starting database migration process...");

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            logger?.LogInformation("Checking for pending migrations...");

            // Get pending migrations
            var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();

            if (!pendingMigrations.Any())
            {
                logger?.LogInformation("No pending migrations found. Database is up to date.");
                return;
            }

            logger?.LogInformation("Found {PendingMigrationCount} pending migrations:", pendingMigrations.Count);
            foreach (var migration in pendingMigrations)
            {
                logger?.LogInformation("  - {MigrationName}", migration);
            }

            logger?.LogInformation("Applying migrations...");

            // Apply migrations
            await dbContext.Database.MigrateAsync();

            logger?.LogInformation("Database migration completed successfully. Applied {MigrationCount} migrations.", pendingMigrations.Count);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Database migration failed with error: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Applies migrations synchronously (for scenarios where async is not possible).
    /// Use ApplyMigrationsAsync when possible as it's more efficient.
    /// </summary>
    public static void ApplyMigrations(this IServiceProvider serviceProvider, ILogger? logger = null)
    {
        logger?.LogInformation("Starting database migration process (sync)...");

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            logger?.LogInformation("Checking for pending migrations...");

            // Get pending migrations
            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

            if (!pendingMigrations.Any())
            {
                logger?.LogInformation("No pending migrations found. Database is up to date.");
                return;
            }

            logger?.LogInformation("Found {PendingMigrationCount} pending migrations:", pendingMigrations.Count);
            foreach (var migration in pendingMigrations)
            {
                logger?.LogInformation("  - {MigrationName}", migration);
            }

            logger?.LogInformation("Applying migrations...");

            // Apply migrations
            dbContext.Database.Migrate();

            logger?.LogInformation("Database migration completed successfully. Applied {MigrationCount} migrations.", pendingMigrations.Count);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Database migration failed with error: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Checks if the database is reachable and all migrations have been applied.
    /// Useful for health checks and startup validation.
    /// </summary>
    /// <returns>True if database is reachable and migrated; false otherwise.</returns>
    public static async Task<bool> IsDatabaseHealthyAsync(this IServiceProvider serviceProvider, ILogger? logger = null)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Test database connectivity
            var canConnect = await dbContext.Database.CanConnectAsync();
            if (!canConnect)
            {
                logger?.LogWarning("Cannot connect to database");
                return false;
            }

            // Check if there are any pending migrations
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger?.LogWarning("Database has {PendingCount} pending migrations", pendingMigrations.Count());
                return false;
            }

            logger?.LogInformation("Database is healthy and up to date");
            return true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Health check failed: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Synchronous version of health check (for scenarios where async is not possible).
    /// </summary>
    public static bool IsDatabaseHealthy(this IServiceProvider serviceProvider, ILogger? logger = null)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Test database connectivity
            var canConnect = dbContext.Database.CanConnect();
            if (!canConnect)
            {
                logger?.LogWarning("Cannot connect to database");
                return false;
            }

            // Check if there are any pending migrations
            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                logger?.LogWarning("Database has {PendingCount} pending migrations", pendingMigrations.Count());
                return false;
            }

            logger?.LogInformation("Database is healthy and up to date");
            return true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Health check failed: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Gets a list of applied migrations.
    /// Useful for diagnostic and audit purposes.
    /// </summary>
    public static async Task<IEnumerable<string>> GetAppliedMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await dbContext.Database.GetAppliedMigrationsAsync();
    }

    /// <summary>
    /// Synchronous version of getting applied migrations.
    /// </summary>
    public static IEnumerable<string> GetAppliedMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return dbContext.Database.GetAppliedMigrations();
    }

    /// <summary>
    /// Gets a list of pending migrations.
    /// Useful for checking if migrations need to be run.
    /// </summary>
    public static async Task<IEnumerable<string>> GetPendingMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await dbContext.Database.GetPendingMigrationsAsync();
    }

    /// <summary>
    /// Synchronous version of getting pending migrations.
    /// </summary>
    public static IEnumerable<string> GetPendingMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return dbContext.Database.GetPendingMigrations();
    }
}
