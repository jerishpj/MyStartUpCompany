using Microsoft.Extensions.DependencyInjection;
using MyStartUpCompany.Persistence;

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
    /// Gets a DbContext for assertions (creates a new scope)
    /// </summary>
    protected AppDbContext GetDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// Executes an action within a database context scope
    /// </summary>
    protected void WithDbContext(Action<AppDbContext> action)
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        action(context);
    }

    /// <summary>
    /// Executes a function within a database context scope and returns a result
    /// </summary>
    protected TResult WithDbContext<TResult>(Func<AppDbContext, TResult> func)
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return func(context);
    }
}