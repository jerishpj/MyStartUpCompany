using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;
using Xunit;

namespace MyStartUpCompany.Api.Tests.Integration;

public class CompanyApiIntegrationTests : IntegrationTestBase
{
    private readonly CustomWebApplicationFactory _factory;

    public CompanyApiIntegrationTests(CustomWebApplicationFactory factory) 
        : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCompany_ReturnsSuccess_WhenCompanyExists()
    {
        // Arrange
        SeedData(context =>
        {
            context.Companies.Add(new Company
            {
                Id = 1,
                Name = "Test Company",
                Description = "A test company for integration testing",
                Address = "123 Test St",
                City = "Testville",
                Region = "Test Region",
                Country = "Testland",
                PostalCode = "12345",
                Phone = "123-456-7890",
            });
        });

        // Act
        var response = await Client.GetAsync("/api/company/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeEmpty();
        content.Should().Contain("Test Company");
    }

    [Fact]
    public async Task GetCompany_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Act - No data seeded
        var response = await Client.GetAsync("/api/companies/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsEmptyList_WhenNoData()
    {
        // Act - No data seeded
        var response = await Client.GetAsync("/api/company");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsCompanies_WhenDataExists()
    {
        // Arrange - Seed multiple companies
        SeedData(context =>
        {
            context.Companies.AddRange(
                new Company
                {
                    Id = 1,
                    Name = "Company A",
                    Description = "First test company",
                    Address = "123 Test St",
                    City = "Testville",
                    Region = "Test Region",
                    Country = "Testland",
                    PostalCode = "12345",
                    Phone = "123-456-7890",
                },
                new Company
                {
                    Id = 2,
                    Name = "Company B",
                    Description = "Second test company",
                    Address = "456 Test Ave",
                    City = "Testopolis",
                    Region = "Test Region",
                    Country = "Testland",
                    PostalCode = "67890",
                    Phone = "098-765-4321",
                }
            );
        });

        // Act
        var response = await Client.GetAsync("/api/company");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Company A");
        content.Should().Contain("Company B");
    }
}