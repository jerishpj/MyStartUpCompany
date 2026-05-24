using MyStartUpCompany.Persistence;
using MyStartUpCompany.Worker.Handlers.AddCompany;
using Microsoft.EntityFrameworkCore;

namespace MyStartUpCompany.Worker.Tests.Utilities
{
    /// <summary>
    /// In-memory test context that disables seed data for isolated testing.
    /// </summary>
    internal class TestAppDbContext : AppDbContext
    {
        public TestAppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply only entity configurations, not seed data
            // Apply Company configuration without seed data
            var companyBuilder = modelBuilder.Entity<MyStartUpCompany.Persistence.Entities.Company>();

            // Apply Employee configuration without seed data
            var employeeBuilder = modelBuilder.Entity<MyStartUpCompany.Persistence.Entities.Employee>();
        }
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
        /// Creates an in-memory DbContextOptions for AppDbContext testing.
        /// </summary>
        public static DbContextOptions<AppDbContext> CreateInMemoryDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
        }

        /// <summary>
        /// Creates a fresh AppDbContext with in-memory database for testing (no seed data).
        /// </summary>
        public static AppDbContext CreateInMemoryAppDbContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
            var context = new TestAppDbContext(options);
            context.Database.EnsureCreated();
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
