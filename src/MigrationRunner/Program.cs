using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Extensions;

namespace MyStartUpCompany.MigrationRunner;

/// <summary>
/// Standalone Migration Runner for MyStartUpCompany
/// 
/// This application runs database migrations without starting the main API or Worker services.
/// It can be used for:
/// - Local development (dotnet run)
/// - CI/CD pipelines (before deploying services)
/// - Docker containers (separate migration container)
/// - Manual migration execution
/// 
/// Usage:
///   dotnet run --project src/MigrationRunner
///   dotnet run --project src/MigrationRunner -- --connection-string "Server=...;Database=...;"
///   dotnet run --project src/MigrationRunner -- --environment Production
/// 
/// Environment Variables:
///   ConnectionStrings__DefaultConnection  - Database connection string
///   ASPNETCORE_ENVIRONMENT               - Environment (Development/Production)
/// </summary>
class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            // Parse command-line arguments
            var options = ParseCommandLineArguments(args);

            // Build configuration
            var configuration = BuildConfiguration(options);

            // Build service provider
            var services = BuildServiceProvider(configuration, options);

            // Create logger
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("========================================");
            logger.LogInformation("MyStartUpCompany Database Migration Tool");
            logger.LogInformation("========================================");
            logger.LogInformation("");

            // Show configuration info
            var env = services.GetRequiredService<IHostEnvironment>();
            logger.LogInformation("Environment: {Environment}", env.EnvironmentName);
            logger.LogInformation("Connection String: {ConnectionString}", 
                MaskConnectionString(configuration.GetConnectionString("DefaultConnection") ?? ""));
            logger.LogInformation("");

            // Run migrations
            logger.LogInformation("Applying database migrations...");
            logger.LogInformation("");

            await services.ApplyMigrationsAsync(logger);

            logger.LogInformation("");
            logger.LogInformation("========================================");
            logger.LogInformation("Migration completed successfully!");
            logger.LogInformation("========================================");

            return 0; // Success
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("");
            Console.Error.WriteLine("========================================");
            Console.Error.WriteLine("Migration failed with error:");
            Console.Error.WriteLine("========================================");
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine("");

            if (!string.IsNullOrEmpty(ex.InnerException?.Message))
            {
                Console.Error.WriteLine("Inner Exception:");
                Console.Error.WriteLine(ex.InnerException.Message);
                Console.Error.WriteLine("");
            }

            if (Environment.GetEnvironmentVariable("DEBUG_MIGRATIONS") == "true")
            {
                Console.Error.WriteLine("Stack Trace:");
                Console.Error.WriteLine(ex.StackTrace);
            }

            return 1; // Failure
        }
    }

    /// <summary>
    /// Parses command-line arguments.
    /// Supported arguments:
    ///   --connection-string <value>    Override database connection string
    ///   --environment <value>          Set environment (Development/Staging/Production)
    ///   --help                         Show help message
    ///   --version                      Show version
    /// </summary>
    private static MigrationOptions ParseCommandLineArguments(string[] args)
    {
        var options = new MigrationOptions();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--connection-string":
                    if (i + 1 < args.Length)
                    {
                        options.ConnectionString = args[++i];
                    }
                    break;

                case "--environment":
                    if (i + 1 < args.Length)
                    {
                        options.Environment = args[++i];
                    }
                    break;

                case "--help":
                case "-h":
                    ShowHelp();
                    Environment.Exit(0);
                    break;

                case "--version":
                    ShowVersion();
                    Environment.Exit(0);
                    break;

                case "--list-migrations":
                case "--list":
                    options.ListMigrationsOnly = true;
                    break;
            }
        }

        return options;
    }

    /// <summary>
    /// Builds the configuration from multiple sources.
    /// Configuration hierarchy (later sources override earlier ones):
    /// 1. appsettings.json
    /// 2. appsettings.{Environment}.json
    /// 3. Environment variables
    /// 4. Command-line arguments
    /// </summary>
    private static IConfiguration BuildConfiguration(MigrationOptions options)
    {
        var environment = options.Environment ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddCommandLine(new[] { $"--environment={environment}" });

        // Override connection string if provided via command line
        if (!string.IsNullOrEmpty(options.ConnectionString))
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", options.ConnectionString }
            });
        }

        return configBuilder.Build();
    }

    /// <summary>
    /// Builds the service provider with all necessary services.
    /// </summary>
    private static IServiceProvider BuildServiceProvider(IConfiguration configuration, MigrationOptions options)
    {
        var services = new ServiceCollection();

        // Add configuration
        services.AddSingleton(configuration);

        // Add logging
        services.AddLogging(builder =>
        {
            builder
                .ClearProviders()
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information);

            // Add debug logging if DEBUG_MIGRATIONS is set
            if (Environment.GetEnvironmentVariable("DEBUG_MIGRATIONS") == "true")
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            }
        });

        // Add host environment
        var environment = options.Environment ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var hostEnvironment = new MigrationHostEnvironment { EnvironmentName = environment };
        services.AddSingleton<IHostEnvironment>(hostEnvironment);

        // Add database context
        services.AddAppDatabase(configuration, hostEnvironment);

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Masks sensitive connection string information for logging.
    /// </summary>
    private static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "(empty or not set)";

        // Mask password and keys
        var masked = System.Text.RegularExpressions.Regex.Replace(
            connectionString,
            @"(Password|pwd|AccessKey)=([^;]+)",
            "$1=***",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return masked;
    }

    private static void ShowHelp()
    {
        Console.WriteLine("""
            MyStartUpCompany Database Migration Tool

            Usage: MigrationRunner [options]

            Options:
              --connection-string <value>  Override database connection string
              --environment <value>        Set environment (Development/Staging/Production)
              --list-migrations            List pending migrations without applying them
              --help, -h                   Show this help message
              --version                    Show version information

            Environment Variables:
              ConnectionStrings__DefaultConnection  Database connection string
              ASPNETCORE_ENVIRONMENT               Environment name
              DEBUG_MIGRATIONS                     Enable debug logging

            Examples:
              # Run with default configuration
              dotnet run --project src/MigrationRunner

              # Override connection string
              dotnet run --project src/MigrationRunner -- --connection-string "Server=.;Database=MyDb;"

              # List pending migrations
              dotnet run --project src/MigrationRunner -- --list-migrations

              # Run in production environment
              dotnet run --project src/MigrationRunner -- --environment Production

              # With environment variables
              set ConnectionStrings__DefaultConnection=Server=.;Database=MyDb;
              dotnet run --project src/MigrationRunner
            """);
    }

    private static void ShowVersion()
    {
        var version = typeof(Program).Assembly.GetName().Version;
        Console.WriteLine($"MyStartUpCompany.MigrationRunner {version}");
    }
}

/// <summary>
/// Container for migration command-line options.
/// </summary>
internal class MigrationOptions
{
    public string? ConnectionString { get; set; }
    public string? Environment { get; set; }
    public bool ListMigrationsOnly { get; set; }
}

/// <summary>
/// Minimal IHostEnvironment implementation for migration runner.
/// </summary>
internal class MigrationHostEnvironment : IHostEnvironment
{
    public string EnvironmentName { get; set; } = "Production";
    public string ApplicationName { get; set; } = "MyStartUpCompany.MigrationRunner";
    public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
    public IFileProvider ContentRootFileProvider { get; set; } = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Directory.GetCurrentDirectory());
}
