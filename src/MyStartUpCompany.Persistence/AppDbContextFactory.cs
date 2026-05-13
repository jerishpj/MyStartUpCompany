using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyStartUpCompany.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    // For EF Core design-time tools (migrations)
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        ConfigureDbContext(optionsBuilder, configuration, null);

        return new AppDbContext(optionsBuilder.Options);
    }

    // Static method to configure DbContext based on environment
    public static void ConfigureDbContext(
        DbContextOptionsBuilder<AppDbContext> optionsBuilder,
        IConfiguration configuration,
        string? environmentName)
    {
        // Use InMemory database for automated integration tests
        if (environmentName == "AutomatedIntegrationTest")
        {
            optionsBuilder.UseInMemoryDatabase("TestDb");
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
        else
        {
            // Use SQL Server for all other environments
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found in configuration.");
            }

            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
        }
    }
}
