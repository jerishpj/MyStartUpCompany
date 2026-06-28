using MyStartUpCompany.Persistence;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyStartUpCompany.Worker.Tests.Utilities
{
    /// <summary>
    /// In-memory test context that inherits AppDbContext for testing.
    /// Seed data handling is left to the parent OnModelCreating.
    /// </summary>
    internal class TestAppDbContext : AppDbContext
    {
        public TestAppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }

    /// <summary>
    /// Factory for creating test instances with mocked or in-memory dependencies.
    /// </summary>
    public static class TestDataFactory
    {
        /// <summary>
        /// Creates a valid CompanyInputDto for testing.
        /// </summary>
        public static CompanyInputDto CreateValidCompanyInputDto(
            string name = "Test Company",
            string address = "123 Main St",
            string city = "Springfield",
            string postalCode = "12345",
            string country = "USA",
            string phone = "555-0100",
            string? description = null,
            string? region = null)
        {
            return new CompanyInputDto
            {
                Name = name,
                Address = address,
                City = city,
                PostalCode = postalCode,
                Country = country,
                Phone = phone,
                Description = description,
                Region = region
            };
        }

        /// <summary>
        /// Creates SQLite in-memory DbContextOptions for AppDbContext testing.
        /// </summary>
        public static DbContextOptions<AppDbContext> CreateInMemoryDbContextOptions(string databaseName)
        {
            var connectionString = $"Data Source=file:{databaseName}?mode=memory&cache=private;";
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connectionString)
                .Options;
        }

        /// <summary>
        /// Creates a fresh AppDbContext with SQLite in-memory database for testing (no seed data).
        /// </summary>
        public static AppDbContext CreateInMemoryAppDbContext(string databaseName)
        {
            var connectionString = $"Data Source=file:{databaseName}?mode=memory&cache=shared;";
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connectionString)
                .Options;
            var context = new TestAppDbContext(options);

            // Ensure schema is created
            context.Database.EnsureCreated();

            // Clear any seeded data that was applied by OnModelCreating
            // Must delete in order: child entities first due to foreign key constraints
            var offices = context.Offices.ToList();
            if (offices.Any())
            {
                context.Offices.RemoveRange(offices);
            }

            var buildings = context.Buildings.ToList();
            if (buildings.Any())
            {
                context.Buildings.RemoveRange(buildings);
            }

            var locations = context.Locations.ToList();
            if (locations.Any())
            {
                context.Locations.RemoveRange(locations);
            }

            var companies = context.Companies.ToList();
            if (companies.Any())
            {
                context.Companies.RemoveRange(companies);
            }

            var employees = context.Employees.ToList();
            if (employees.Any())
            {
                context.Employees.RemoveRange(employees);
            }

            var projects = context.Projects.ToList();
            if (projects.Any())
            {
                context.Projects.RemoveRange(projects);
            }

            var projectTypes = context.ProjectTypeReferences.ToList();
            if (projectTypes.Any())
            {
                context.ProjectTypeReferences.RemoveRange(projectTypes);
            }

            context.SaveChanges();
            return context;
        }

        /// <summary>
        /// Creates a list of valid CompanyInputDto objects for batch testing.
        /// </summary>
        public static List<CompanyInputDto> CreateValidCompanyBatch(int count = 3)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateValidCompanyInputDto(
                    name: $"Company {i}",
                    phone: $"555-{i:D4}"))
                .ToList();
        }

        /// <summary>
        /// Creates a CompanyInputDto with missing required field for validation testing.
        /// </summary>
        public static CompanyInputDto CreateInvalidCompanyInputDto(string? missingField = "Name")
        {
            var dto = CreateValidCompanyInputDto();

            return missingField switch
            {
                "Name" => new CompanyInputDto { Name = "", Address = dto.Address, City = dto.City, PostalCode = dto.PostalCode, Country = dto.Country, Phone = dto.Phone },
                "Address" => new CompanyInputDto { Name = dto.Name, Address = "", City = dto.City, PostalCode = dto.PostalCode, Country = dto.Country, Phone = dto.Phone },
                "City" => new CompanyInputDto { Name = dto.Name, Address = dto.Address, City = "", PostalCode = dto.PostalCode, Country = dto.Country, Phone = dto.Phone },
                "PostalCode" => new CompanyInputDto { Name = dto.Name, Address = dto.Address, City = dto.City, PostalCode = "", Country = dto.Country, Phone = dto.Phone },
                "Country" => new CompanyInputDto { Name = dto.Name, Address = dto.Address, City = dto.City, PostalCode = dto.PostalCode, Country = "", Phone = dto.Phone },
                "Phone" => new CompanyInputDto { Name = dto.Name, Address = dto.Address, City = dto.City, PostalCode = dto.PostalCode, Country = dto.Country, Phone = "" },
                _ => dto
            };
        }

        /// <summary>
        /// Creates sample JSON content for file processing tests.
        /// </summary>
        public static string CreateValidCompanyJsonContent()
        {
            return """
                [
                  {
                    "name": "Acme Corporation",
                    "description": "A leading company in innovation",
                    "address": "123 Business Ave",
                    "city": "New York",
                    "region": "NY",
                    "postalCode": "10001",
                    "country": "USA",
                    "phone": "555-1234"
                  },
                  {
                    "name": "Tech Solutions Inc",
                    "address": "456 Tech Park",
                    "city": "San Francisco",
                    "postalCode": "94105",
                    "country": "USA",
                    "phone": "555-5678"
                  }
                ]
                """;
        }

        /// <summary>
        /// Creates invalid JSON content for deserialization testing.
        /// </summary>
        public static string CreateInvalidJsonContent()
        {
            return """
                {
                  invalid json content [
                }
                """;
        }

        /// <summary>
        /// Creates JSON with missing required fields.
        /// </summary>
        public static string CreateJsonWithMissingFields()
        {
            return """
                [
                  {
                    "Name": "Company Without Address",
                    "Address": "",
                    "City": "Springfield",
                    "PostalCode": "12345",
                    "Country": "USA",
                    "Phone": "555-0100"
                  }
                ]
                """;
        }
    }
}
