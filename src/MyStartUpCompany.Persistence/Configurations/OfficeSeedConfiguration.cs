using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Seed data configuration for Office entity
/// Creates 3-8 offices per building for a total of ~5000-8000 offices
/// </summary>
public class OfficeSeedConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        // Seed data configuration removed - use separate seeding strategy
        // to avoid oversized migrations
    }

    private static IEnumerable<Office> GetSeedData()
    {
        var offices = new List<Office>();
        int officeId = 1;

        // Generate 3-8 offices per building
        // We have ~1400 buildings, so we'll create approximately 5000-6000 offices

        for (int buildingId = 1; buildingId <= 1600; buildingId++)
        {
            int officesPerBuilding = (buildingId % 11) switch
            {
                0 => 8,
                1 => 7,
                2 => 6,
                3 => 5,
                4 => 4,
                5 => 8,
                6 => 3,
                7 => 6,
                8 => 5,
                9 => 7,
                _ => 4
            };

            offices.AddRange(GenerateOfficesForBuilding(buildingId, ref officeId, officesPerBuilding));
        }

        return offices;
    }

    private static IEnumerable<Office> GenerateOfficesForBuilding(
        int buildingId,
        ref int officeId,
        int officesPerBuilding)
    {
        var offices = new List<Office>();
        var departments = GetDepartments();
        var officeTypes = GetOfficeTypes();

        for (int i = 0; i < officesPerBuilding; i++)
        {
            var floor = Random.Shared.Next(1, 25);
            var section = ((char)('A' + (i % 4))).ToString();
            var officeCode = $"OFF{buildingId:000}-{floor:00}{section}";
            var capacity = Random.Shared.Next(1, 50);
            var department = departments[Random.Shared.Next(departments.Count)];
            var officeType = officeTypes[Random.Shared.Next(officeTypes.Count)];

            offices.Add(new Office
            {
                Id = officeId++,
                BuildingId = buildingId,
                Name = $"{section}-{floor:00} {department} Office",
                OfficeCode = officeCode,
                Description = $"{officeType} office for {department} department",
                FloorNumber = floor,
                Section = section,
                Capacity = capacity,
                OfficeType = officeType,
                SquareMeters = capacity * Random.Shared.Next(10, 30),
                Department = department,
                Manager = GetManagerName(officeId),
                Phone = GeneratePhoneNumber(),
                Email = $"{department.ToLower().Replace(" ", "")}.{floor}{section.ToLower()}@company.com",
                IsActive = Random.Shared.Next(0, 100) > 5, // 95% active
                CreatedAt = DateTime.UtcNow.AddMonths(-Random.Shared.Next(1, 24)),
                UpdatedAt = DateTime.UtcNow.AddMonths(-Random.Shared.Next(0, 12))
            });
        }

        return offices;
    }

    private static List<string> GetDepartments()
    {
        return new()
        {
            "Engineering",
            "Sales",
            "Marketing",
            "Human Resources",
            "Finance",
            "Operations",
            "Research & Development",
            "Quality Assurance",
            "Customer Support",
            "Legal",
            "Administration",
            "IT Infrastructure",
            "Product Management",
            "Strategy",
            "Executive"
        };
    }

    private static List<string> GetOfficeTypes()
    {
        return new()
        {
            "Standard Office",
            "Open Space",
            "Executive Suite",
            "Conference Room",
            "Collaborative Space",
            "Hot Desk",
            "Private Office",
            "Shared Office"
        };
    }

    private static string GetManagerName(int index)
    {
        var firstNames = new[] 
        { 
            "James", "Mary", "Robert", "Patricia", "Michael", "Jennifer", "William", "Linda", 
            "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah",
            "Christopher", "Karen", "Daniel", "Lisa", "Matthew", "Nancy", "Mark", "Betty",
            "Donald", "Margaret", "Steven", "Sandra", "Paul", "Ashley", "Andrew", "Kimberly"
        };
        var lastNames = new[] 
        { 
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", 
            "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", 
            "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", 
            "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson"
        };

        return $"{firstNames[index % firstNames.Length]} {lastNames[(index / 3) % lastNames.Length]}";
    }

    private static string GeneratePhoneNumber()
    {
        var areaCode = Random.Shared.Next(200, 999);
        var subscriber = Random.Shared.Next(1000, 9999);
        return $"+1-{areaCode}-555-{subscriber % 1000:0000}";
    }
}
