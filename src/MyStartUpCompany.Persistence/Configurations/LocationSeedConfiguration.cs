using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Seed data configuration for Location entity
/// Creates 3-5 locations per company for a total of ~300-500 locations
/// </summary>
public class LocationSeedConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        // Seed data configuration removed - use separate seeding strategy
        // to avoid oversized migrations
    }

    private static IEnumerable<Location> GetSeedData()
    {
        var locations = new List<Location>();
        int locationId = 1;

        // Generate 3-5 locations per company
        // We have 100 companies, so we'll create approximately 400 locations

        // Company 1-10: TechVision Inc and similar - West Coast locations
        for (int companyId = 1; companyId <= 10; companyId++)
        {
            locations.AddRange(GenerateLocationsForCompany(companyId, ref locationId, 4));
        }

        // Company 11-20: CloudSync and similar - Multi-region
        for (int companyId = 11; companyId <= 20; companyId++)
        {
            locations.AddRange(GenerateLocationsForCompany(companyId, ref locationId, 5));
        }

        // Company 21-30: East Coast tech companies
        for (int companyId = 21; companyId <= 30; companyId++)
        {
            locations.AddRange(GenerateLocationsForCompany(companyId, ref locationId, 4));
        }

        // Company 31-40: Midwest operations
        for (int companyId = 31; companyId <= 40; companyId++)
        {
            locations.AddRange(GenerateLocationsForCompany(companyId, ref locationId, 3));
        }

        // Company 41-50: Financial sector
        for (int companyId = 41; companyId <= 50; companyId++)
        {
            locations.AddRange(GenerateLocationsForCompany(companyId, ref locationId, 5));
        }

        // Company 51-60: Healthcare
        for (int companyId = 51; companyId <= 60; companyId++)
        {
            locations.AddRange(GenerateLocationsForCompany(companyId, ref locationId, 4));
        }

        // Company 61-70: Manufacturing
        for (int companyId = 61; companyId <= 70; companyId++)
        {
            locations.AddRange(GenerateLocationsForCompany(companyId, ref locationId, 3));
        }

        // Company 71-80: Retail and Distribution
        for (int companyId = 71; companyId <= 80; companyId++)
        {
            locations.AddRange(GenerateLocationsForCompany(companyId, ref locationId, 5));
        }

        // Company 81-90: Energy and Utilities
        for (int companyId = 81; companyId <= 90; companyId++)
        {
            locations.AddRange(GenerateLocationsForCompany(companyId, ref locationId, 4));
        }

        // Company 91-100: Education and Services
        for (int companyId = 91; companyId <= 100; companyId++)
        {
            locations.AddRange(GenerateLocationsForCompany(companyId, ref locationId, 3));
        }

        return locations;
    }

    private static IEnumerable<Location> GenerateLocationsForCompany(
        int companyId,
        ref int locationId,
        int locationsPerCompany)
    {
        var locations = new List<Location>();
        var cityStates = GetCityStateData();
        var baseIndex = (companyId - 1) % cityStates.Count;

        for (int i = 0; i < locationsPerCompany; i++)
        {
            var cityStateIndex = (baseIndex + i) % cityStates.Count;
            var (city, state, country, phone) = cityStates[cityStateIndex];

            locations.Add(new Location
            {
                Id = locationId++,
                CompanyId = companyId,
                Name = $"{city} Regional Office",
                Description = $"Regional headquarters in {city}",
                Address = GetAddressForCity(city, i),
                City = city,
                Region = state,
                PostalCode = GetPostalCodeForState(state, i),
                Country = country,
                Phone = phone,
                Email = $"{city.ToLower().Replace(" ", "")}.office@company{companyId}.com",
                ManagerName = GetManagerName(i),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-Random.Shared.Next(1, 24)),
                UpdatedAt = DateTime.UtcNow.AddMonths(-Random.Shared.Next(0, 12))
            });
        }

        return locations;
    }

    private static List<(string City, string State, string Country, string Phone)> GetCityStateData()
    {
        return new()
        {
            ("New York", "NY", "United States", "+1-212-555-0101"),
            ("Los Angeles", "CA", "United States", "+1-213-555-0102"),
            ("Chicago", "IL", "United States", "+1-312-555-0103"),
            ("Houston", "TX", "United States", "+1-713-555-0104"),
            ("Phoenix", "AZ", "United States", "+1-602-555-0105"),
            ("Philadelphia", "PA", "United States", "+1-215-555-0106"),
            ("San Antonio", "TX", "United States", "+1-210-555-0107"),
            ("San Diego", "CA", "United States", "+1-619-555-0108"),
            ("Dallas", "TX", "United States", "+1-214-555-0109"),
            ("San Jose", "CA", "United States", "+1-408-555-0110"),
            ("Austin", "TX", "United States", "+1-512-555-0111"),
            ("Jacksonville", "FL", "United States", "+1-904-555-0112"),
            ("Denver", "CO", "United States", "+1-303-555-0113"),
            ("Boston", "MA", "United States", "+1-617-555-0114"),
            ("Seattle", "WA", "United States", "+1-206-555-0115"),
            ("Toronto", "ON", "Canada", "+1-416-555-0116"),
            ("Vancouver", "BC", "Canada", "+1-604-555-0117"),
            ("Montreal", "QC", "Canada", "+1-514-555-0118"),
            ("Mexico City", "MX", "Mexico", "+52-55-555-0119"),
            ("London", "England", "United Kingdom", "+44-20-5555-0120")
        };
    }

    private static string GetAddressForCity(string city, int index)
    {
        var addresses = new[]
        {
            $"{1000 + (index * 111)} Main Street",
            $"{2000 + (index * 222)} Business Avenue",
            $"{3000 + (index * 333)} Commerce Drive",
            $"{4000 + (index * 444)} Corporate Plaza",
            $"{5000 + (index * 555)} Enterprise Boulevard"
        };
        return addresses[index % addresses.Length];
    }

    private static string GetPostalCodeForState(string state, int index)
    {
        var postalCodes = state switch
        {
            "CA" => new[] { "94102", "90001", "95101", "92101", "95110" },
            "NY" => new[] { "10001", "10002", "10003", "10004", "10005" },
            "TX" => new[] { "75001", "75002", "75003", "77001", "78201" },
            "IL" => new[] { "60601", "60602", "60603", "60604", "60605" },
            "PA" => new[] { "19101", "19102", "19103", "19104", "19105" },
            "FL" => new[] { "32099", "32801", "32802", "32803", "32804" },
            "CO" => new[] { "80201", "80202", "80203", "80204", "80205" },
            "MA" => new[] { "02101", "02102", "02103", "02104", "02105" },
            "WA" => new[] { "98101", "98102", "98103", "98104", "98105" },
            "AZ" => new[] { "85001", "85002", "85003", "85004", "85005" },
            "ON" => new[] { "M1A", "M1B", "M1C", "M1E", "M1G" },
            "BC" => new[] { "V6A", "V6B", "V6C", "V6E", "V6G" },
            "QC" => new[] { "H1A", "H1B", "H1C", "H1E", "H1G" },
            "MX" => new[] { "01000", "01001", "01002", "01003", "01004" },
            "England" => new[] { "EC1A", "EC1B", "EC1C", "EC1D", "EC1E" },
            _ => new[] { "10000", "10001", "10002", "10003", "10004" }
        };
        return postalCodes[index % postalCodes.Length];
    }

    private static string GetManagerName(int index)
    {
        var firstNames = new[] { "James", "Mary", "Robert", "Patricia", "Michael", "Jennifer", "William", "Linda", "David", "Barbara" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };

        return $"{firstNames[index % firstNames.Length]} {lastNames[index % lastNames.Length]}";
    }
}
