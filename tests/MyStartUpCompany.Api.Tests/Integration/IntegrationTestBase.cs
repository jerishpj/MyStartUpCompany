using Microsoft.Extensions.DependencyInjection;
using MyStartUpCompany.Persistence;
using Xunit;

namespace MyStartUpCompany.Api.Tests.Integration;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();

        // Clear database before each test
        Factory.ClearDatabase();
    }

    /// <summary>
    /// Seeds test data using a callback
    /// </summary>
    protected void SeedData(Action<AppDbContext> seedAction)
    {
        Factory.SeedTestData(seedAction);
    }

    /// <summary>
    /// Gets a DbContext for assertions
    /// </summary>
    protected AppDbContext GetDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}