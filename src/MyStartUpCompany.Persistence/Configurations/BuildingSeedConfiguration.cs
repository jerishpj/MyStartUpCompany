using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Seed data configuration for Building entity
/// Creates 2-5 buildings per location for a total of ~1000-2000 buildings
/// </summary>
public class BuildingSeedConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        // Seed data configuration removed - use separate seeding strategy
        // to avoid oversized migrations
    }

    private static IEnumerable<Building> GetSeedData()
    {
        var buildings = new List<Building>();
        int buildingId = 1;

        // Generate 2-5 buildings per location
        // We have ~400 locations, so we'll create approximately 1200-1600 buildings

        for (int locationId = 1; locationId <= 420; locationId++)
        {
            int buildingsPerLocation = (locationId % 7) switch
            {
                0 => 5,
                1 => 4,
                2 => 3,
                3 => 5,
                4 => 2,
                5 => 4,
                _ => 3
            };

            buildings.AddRange(GenerateBuildingsForLocation(locationId, ref buildingId, buildingsPerLocation));
        }

        return buildings;
    }

    private static IEnumerable<Building> GenerateBuildingsForLocation(
        int locationId,
        ref int buildingId,
        int buildingsPerLocation)
    {
        var buildings = new List<Building>();

        for (int i = 0; i < buildingsPerLocation; i++)
        {
            var buildingCode = $"BLD{locationId:000}-{i + 1:00}";
            var floors = Random.Shared.Next(5, 25);
            var yearConstructed = Random.Shared.Next(1990, 2024);

            buildings.Add(new Building
            {
                Id = buildingId++,
                LocationId = locationId,
                Name = $"Building {buildingCode}",
                BuildingCode = buildingCode,
                Description = $"Office building with {floors} floors constructed in {yearConstructed}",
                Address = $"{1000 + (buildingId * 111)} Building Street",
                NumberOfFloors = floors,
                YearConstructed = yearConstructed,
                TotalFloorArea = Random.Shared.Next(15000, 500000),
                ContactPerson = GetContactName(buildingId),
                Phone = GeneratePhoneNumber(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-Random.Shared.Next(1, 24)),
                UpdatedAt = DateTime.UtcNow.AddMonths(-Random.Shared.Next(0, 12))
            });
        }

        return buildings;
    }

    private static string GetContactName(int index)
    {
        var firstNames = new[] 
        { 
            "John", "Sarah", "Michael", "Emily", "David", "Jessica", "Robert", "Amanda", 
            "Christopher", "Nicole", "Daniel", "Elizabeth", "Matthew", "Susan", "Anthony" 
        };
        var lastNames = new[] 
        { 
            "Anderson", "Bennett", "Campbell", "Davidson", "Edwards", "Foster", "Graham", 
            "Harrison", "Irving", "Jackson", "Kelly", "Lawrence", "Martin", "Nelson", "O'Connor" 
        };

        return $"{firstNames[index % firstNames.Length]} {lastNames[(index / 5) % lastNames.Length]}";
    }

    private static string GeneratePhoneNumber()
    {
        var areaCode = Random.Shared.Next(200, 999);
        var exchange = Random.Shared.Next(200, 999);
        var subscriber = Random.Shared.Next(1000, 9999);
        return $"+1-{areaCode}-555-{subscriber % 1000:0000}";
    }
}
