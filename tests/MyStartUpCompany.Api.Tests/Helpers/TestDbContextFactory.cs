using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.Helpers;

public static class TestDbContextFactory
{
    public static AppDbContext CreateInMemoryContext(string databaseName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        var context = new AppDbContext(options);
        return context;
    }

    public static void SeedTestData(AppDbContext context)
    {
        var companies = new List<Company>
        {
            new()
            {
                Id = 1,
                Name = "Acme Corporation",
                Description = "Leading provider of innovative solutions",
                Address = "123 Main Street",
                City = "San Francisco",
                Region = "CA",
                PostalCode = "94105",
                Country = "United States",
                Phone = "+1-555-123-4567",
            },
            new()
            {
                Id = 2,
                Name = "TechVision Inc",
                Description = "Software development company",
                Address = "456 Tech Avenue",
                City = "Seattle",
                Region = "WA",
                PostalCode = "98101",
                Country = "United States",
                Phone = "+1-555-987-6543",
            },
            new()
            {
                Id = 3,
                Name = "Global Systems Ltd",
                Description = "Enterprise solutions provider",
                Address = "789 Business Blvd",
                City = "New York",
                Region = "NY",
                PostalCode = "10001",
                Country = "United States",
                Phone = "+1-555-456-7890",
            }
        };

        context.Companies.AddRange(companies);
        context.SaveChanges();
    }
}