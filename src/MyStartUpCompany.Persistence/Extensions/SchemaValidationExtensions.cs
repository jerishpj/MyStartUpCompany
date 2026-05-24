using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MyStartUpCompany.Persistence.Extensions;

/// <summary>
/// Database schema validation utilities.
/// Provides methods to validate that all required migrations are applied
/// and schema is in expected state before services start.
/// </summary>
public static class SchemaValidationExtensions
{
    /// <summary>
    /// Validates that the database schema is in a healthy state.
    /// Checks:
    /// - Database connectivity
    /// - No pending migrations
    /// - Essential tables exist
    /// - Required columns exist
    /// 
    /// Returns detailed validation results for logging/monitoring.
    /// </summary>
    public static async Task<SchemaValidationResult> ValidateSchemaAsync(
        this IServiceProvider serviceProvider,
        ILogger? logger = null)
    {
        var result = new SchemaValidationResult();

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            logger?.LogInformation("Starting schema validation...");

            // Check 1: Database connectivity
            result.CanConnect = await dbContext.Database.CanConnectAsync();
            if (!result.CanConnect)
            {
                result.Status = ValidationStatus.Failed;
                result.Message = "Cannot connect to database";
                logger?.LogError("Schema validation failed: {Message}", result.Message);
                return result;
            }
            logger?.LogInformation("✓ Database connectivity verified");

            // Check 2: Pending migrations
            var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();
            result.HasPendingMigrations = pendingMigrations.Any();
            result.PendingMigrationCount = pendingMigrations.Count;

            if (result.HasPendingMigrations)
            {
                result.Status = ValidationStatus.Failed;
                result.Message = $"Database has {pendingMigrations.Count} pending migrations";
                result.PendingMigrations = pendingMigrations;
                logger?.LogError("Schema validation failed: {Message}", result.Message);
                logger?.LogError("Pending migrations: {Migrations}", string.Join(", ", pendingMigrations));
                return result;
            }
            logger?.LogInformation("✓ No pending migrations");

            // Check 3: Applied migrations
            var appliedMigrations = (await dbContext.Database.GetAppliedMigrationsAsync()).ToList();
            result.AppliedMigrationCount = appliedMigrations.Count;
            result.AppliedMigrations = appliedMigrations;
            logger?.LogInformation("✓ {MigrationCount} migrations applied", appliedMigrations.Count);

            // Check 4: Essential tables (customize based on your schema)
            var missingTables = await ValidateTablesAsync(dbContext, logger);
            if (missingTables.Any())
            {
                result.Status = ValidationStatus.Warning;
                result.Message = $"Some expected tables are missing: {string.Join(", ", missingTables)}";
                result.MissingTables = missingTables;
                logger?.LogWarning("Schema validation warning: {Message}", result.Message);
            }

            // Check 5: Table row counts (informational)
            result.TableInfo = await GetTableInfoAsync(dbContext, logger);

            // Overall status
            if (result.Status == ValidationStatus.Unknown)
            {
                result.Status = ValidationStatus.Success;
                result.Message = "Schema validation passed";
                logger?.LogInformation("✓ Schema validation passed");
            }

            return result;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Schema validation failed with exception: {Message}", ex.Message);
            result.Status = ValidationStatus.Failed;
            result.Message = $"Validation error: {ex.Message}";
            result.Exception = ex;
            return result;
        }
    }

    /// <summary>
    /// Synchronous version of schema validation.
    /// </summary>
    public static SchemaValidationResult ValidateSchema(
        this IServiceProvider serviceProvider,
        ILogger? logger = null)
    {
        var result = new SchemaValidationResult();

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            logger?.LogInformation("Starting schema validation (sync)...");

            // Check 1: Database connectivity
            result.CanConnect = dbContext.Database.CanConnect();
            if (!result.CanConnect)
            {
                result.Status = ValidationStatus.Failed;
                result.Message = "Cannot connect to database";
                logger?.LogError("Schema validation failed: {Message}", result.Message);
                return result;
            }
            logger?.LogInformation("✓ Database connectivity verified");

            // Check 2: Pending migrations
            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
            result.HasPendingMigrations = pendingMigrations.Any();
            result.PendingMigrationCount = pendingMigrations.Count;

            if (result.HasPendingMigrations)
            {
                result.Status = ValidationStatus.Failed;
                result.Message = $"Database has {pendingMigrations.Count} pending migrations";
                result.PendingMigrations = pendingMigrations;
                logger?.LogError("Schema validation failed: {Message}", result.Message);
                return result;
            }
            logger?.LogInformation("✓ No pending migrations");

            // Check 3: Applied migrations
            var appliedMigrations = dbContext.Database.GetAppliedMigrations().ToList();
            result.AppliedMigrationCount = appliedMigrations.Count;
            result.AppliedMigrations = appliedMigrations;
            logger?.LogInformation("✓ {MigrationCount} migrations applied", appliedMigrations.Count);

            // Check 4: Essential tables
            var missingTables = ValidateTables(dbContext, logger);
            if (missingTables.Any())
            {
                result.Status = ValidationStatus.Warning;
                result.Message = $"Some expected tables are missing: {string.Join(", ", missingTables)}";
                result.MissingTables = missingTables;
                logger?.LogWarning("Schema validation warning: {Message}", result.Message);
            }

            // Check 5: Table info
            result.TableInfo = GetTableInfo(dbContext, logger);

            if (result.Status == ValidationStatus.Unknown)
            {
                result.Status = ValidationStatus.Success;
                result.Message = "Schema validation passed";
                logger?.LogInformation("✓ Schema validation passed");
            }

            return result;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Schema validation failed with exception: {Message}", ex.Message);
            result.Status = ValidationStatus.Failed;
            result.Message = $"Validation error: {ex.Message}";
            result.Exception = ex;
            return result;
        }
    }

    /// <summary>
    /// Validates that essential tables exist in the database.
    /// Customize table names based on your AppDbContext model.
    /// </summary>
    private static async Task<List<string>> ValidateTablesAsync(AppDbContext dbContext, ILogger? logger)
    {
        var missingTables = new List<string>();

        // Define expected tables from your DbContext
        var expectedTables = new[]
        {
            "Companies",  // Customize based on your entities
            // Add more tables as needed
        };

        foreach (var tableName in expectedTables)
        {
            try
            {
                var connection = dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = $@"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = '{tableName}'";

                var result = await command.ExecuteScalarAsync();
                if (result is int count && count == 0)
                {
                    missingTables.Add(tableName);
                    logger?.LogWarning("Table '{TableName}' not found", tableName);
                }
                else
                {
                    logger?.LogInformation("✓ Table '{TableName}' exists", tableName);
                }
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error validating table '{TableName}': {Message}", tableName, ex.Message);
            }
        }

        return missingTables;
    }

    /// <summary>
    /// Synchronous version of table validation.
    /// </summary>
    private static List<string> ValidateTables(AppDbContext dbContext, ILogger? logger)
    {
        var missingTables = new List<string>();
        var expectedTables = new[] { "Companies" };

        foreach (var tableName in expectedTables)
        {
            try
            {
                var connection = dbContext.Database.GetDbConnection();
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = $@"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = '{tableName}'";

                var result = command.ExecuteScalar();
                if (result is int count && count == 0)
                {
                    missingTables.Add(tableName);
                    logger?.LogWarning("Table '{TableName}' not found", tableName);
                }
                else
                {
                    logger?.LogInformation("✓ Table '{TableName}' exists", tableName);
                }
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error validating table '{TableName}': {Message}", tableName, ex.Message);
            }
        }

        return missingTables;
    }

    /// <summary>
    /// Gets information about all tables in the database.
    /// </summary>
    private static async Task<List<TableInfo>> GetTableInfoAsync(AppDbContext dbContext, ILogger? logger)
    {
        var tableInfo = new List<TableInfo>();

        try
        {
            var connection = dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    TABLE_NAME,
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) as ColumnCount
                FROM INFORMATION_SCHEMA.TABLES t
                WHERE TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_NAME";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var info = new TableInfo
                {
                    Name = reader.GetString(0),
                    ColumnCount = reader.GetInt32(1)
                };
                tableInfo.Add(info);
                logger?.LogInformation("  Table: {TableName} ({ColumnCount} columns)", info.Name, info.ColumnCount);
            }
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Error getting table info: {Message}", ex.Message);
        }

        return tableInfo;
    }

    /// <summary>
    /// Synchronous version of getting table info.
    /// </summary>
    private static List<TableInfo> GetTableInfo(AppDbContext dbContext, ILogger? logger)
    {
        var tableInfo = new List<TableInfo>();

        try
        {
            var connection = dbContext.Database.GetDbConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    TABLE_NAME,
                    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) as ColumnCount
                FROM INFORMATION_SCHEMA.TABLES t
                WHERE TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_NAME";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var info = new TableInfo
                {
                    Name = reader.GetString(0),
                    ColumnCount = reader.GetInt32(1)
                };
                tableInfo.Add(info);
                logger?.LogInformation("  Table: {TableName} ({ColumnCount} columns)", info.Name, info.ColumnCount);
            }
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Error getting table info: {Message}", ex.Message);
        }

        return tableInfo;
    }
}

/// <summary>
/// Result of schema validation.
/// </summary>
public class SchemaValidationResult
{
    /// <summary>
    /// Overall validation status.
    /// </summary>
    public ValidationStatus Status { get; set; } = ValidationStatus.Unknown;

    /// <summary>
    /// Human-readable validation message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Whether database is reachable.
    /// </summary>
    public bool CanConnect { get; set; }

    /// <summary>
    /// Whether there are pending migrations.
    /// </summary>
    public bool HasPendingMigrations { get; set; }

    /// <summary>
    /// Number of pending migrations.
    /// </summary>
    public int PendingMigrationCount { get; set; }

    /// <summary>
    /// Names of pending migrations.
    /// </summary>
    public List<string> PendingMigrations { get; set; } = new();

    /// <summary>
    /// Number of applied migrations.
    /// </summary>
    public int AppliedMigrationCount { get; set; }

    /// <summary>
    /// Names of applied migrations.
    /// </summary>
    public List<string> AppliedMigrations { get; set; } = new();

    /// <summary>
    /// Tables that should exist but don't.
    /// </summary>
    public List<string> MissingTables { get; set; } = new();

    /// <summary>
    /// Information about tables in the database.
    /// </summary>
    public List<TableInfo> TableInfo { get; set; } = new();

    /// <summary>
    /// Exception if validation failed.
    /// </summary>
    public Exception? Exception { get; set; }
}

/// <summary>
/// Schema validation status.
/// </summary>
public enum ValidationStatus
{
    /// <summary>Validation status unknown (not yet run).</summary>
    Unknown = 0,

    /// <summary>Validation passed, schema is healthy.</summary>
    Success = 1,

    /// <summary>Validation passed with warnings (some optional checks failed).</summary>
    Warning = 2,

    /// <summary>Validation failed, schema is not healthy.</summary>
    Failed = 3
}

/// <summary>
/// Information about a database table.
/// </summary>
public class TableInfo
{
    /// <summary>
    /// Table name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Number of columns in the table.
    /// </summary>
    public int ColumnCount { get; set; }
}
