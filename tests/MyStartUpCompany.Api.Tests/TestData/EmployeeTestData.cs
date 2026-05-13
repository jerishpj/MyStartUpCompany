using MyStartUpCompany.Persistence;
using MyStartUpCompany.Api.Tests.Builders;

namespace MyStartUpCompany.Api.Tests.TestData;

/// <summary>
/// Centralized test data seeding scenarios for Employee-related tests
/// </summary>
public static class EmployeeTestData
{
    /// <summary>
    /// Seeds standard employee test data
    /// </summary>
    public static void SeedStandardEmployees(AppDbContext context)
    {
        var employees = new[]
        {
            new EmployeeBuilder().AsSeniorDeveloper().Build(),
            new EmployeeBuilder().AsProjectManager().Build()
        };

        context.Employees.AddRange(employees);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds a single test employee
    /// </summary>
    public static void SeedSingleEmployee(AppDbContext context, int id = 1)
    {
        var employee = new EmployeeBuilder()
            .WithId(id)
            .WithName("Test Employee")
            .WithTitle("Test Title")
            .Build();

        context.Employees.Add(employee);
        context.SaveChanges();
    }
}