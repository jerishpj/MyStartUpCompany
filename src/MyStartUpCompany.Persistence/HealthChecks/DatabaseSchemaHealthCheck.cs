using MyStartUpCompany.Persistence.Extensions;

namespace MyStartUpCompany.Persistence.HealthChecks;

/// <summary>
/// Database health check utility for validating schema state.
/// 
/// Usage in Program.cs:
/// 
///   var result = await app.Services.CheckDatabaseHealthAsync();
///   if (result.Status != ValidationStatus.Success)
///   {
///       logger.LogError("Database schema validation failed: {Message}", result.Message);
///       // Handle accordingly
///   }
/// </summary>
public static class DatabaseHealthCheck
{
    /// <summary>
    /// Checks database health by validating schema.
    /// Useful for startup validation and health endpoints.
    /// </summary>
    public static async Task<SchemaValidationResult> CheckDatabaseHealthAsync(
        this IServiceProvider serviceProvider)
    {
        return await serviceProvider.ValidateSchemaAsync();
    }

    /// <summary>
    /// Checks database health synchronously.
    /// </summary>
    public static SchemaValidationResult CheckDatabaseHealth(
        this IServiceProvider serviceProvider)
    {
        return serviceProvider.ValidateSchema();
    }

    /// <summary>
    /// Checks if database is ready (no pending migrations).
    /// </summary>
    public static async Task<bool> IsDatabaseReadyAsync(
        this IServiceProvider serviceProvider)
    {
        var result = await serviceProvider.ValidateSchemaAsync();
        return result.Status == ValidationStatus.Success;
    }

    /// <summary>
    /// Synchronous version of IsDatabaseReady.
    /// </summary>
    public static bool IsDatabaseReady(
        this IServiceProvider serviceProvider)
    {
        var result = serviceProvider.ValidateSchema();
        return result.Status == ValidationStatus.Success;
    }
}
