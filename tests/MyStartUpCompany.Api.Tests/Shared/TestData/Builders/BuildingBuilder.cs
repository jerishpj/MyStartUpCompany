using Bogus;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.Shared.TestData.Builders;

/// <summary>
/// Builder pattern for creating Building test data with Bogus-generated realistic data
/// </summary>
public class BuildingBuilder
{
    private int? _id;
    private int _locationId = 1;
    private string _name;
    private string? _buildingCode;
    private string? _description;
    private string _address;
    private int? _numberOfFloors;
    private int? _yearConstructed;
    private decimal? _totalFloorArea;
    private string? _contactPerson;
    private string? _phone;
    private bool _isActive = true;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime? _updatedAt;

    private readonly Faker _faker = new();

    public BuildingBuilder()
    {
        // Initialize with Bogus-generated data
        var buildingIdentifier = new[] { "Tower", "Building", "Complex", "Center", "Plaza", "House" };
        var buildingType = new[] { "A", "B", "C", "North", "South", "East", "West", "Main", "Annex" };

        _name = $"{_faker.PickRandom(buildingIdentifier)} {_faker.PickRandom(buildingType)}";
        _buildingCode = _faker.Random.AlphaNumeric(5).ToUpper();
        _description = _faker.Lorem.Sentence();
        _address = _faker.Address.StreetAddress();
        _numberOfFloors = _faker.Random.Int(1, 50);
        _yearConstructed = _faker.Random.Int(1980, 2024);
        _totalFloorArea = _faker.Random.Decimal(1000, 100000);
        _contactPerson = _faker.Person.FullName;
        _phone = _faker.Phone.PhoneNumber();
    }

    public BuildingBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public BuildingBuilder WithLocationId(int locationId)
    {
        _locationId = locationId;
        return this;
    }

    public BuildingBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public BuildingBuilder WithBuildingCode(string? buildingCode)
    {
        _buildingCode = buildingCode;
        return this;
    }

    public BuildingBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public BuildingBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public BuildingBuilder WithNumberOfFloors(int? numberOfFloors)
    {
        _numberOfFloors = numberOfFloors;
        return this;
    }

    public BuildingBuilder WithYearConstructed(int? yearConstructed)
    {
        _yearConstructed = yearConstructed;
        return this;
    }

    public BuildingBuilder WithTotalFloorArea(decimal? totalFloorArea)
    {
        _totalFloorArea = totalFloorArea;
        return this;
    }

    public BuildingBuilder WithContactPerson(string? contactPerson)
    {
        _contactPerson = contactPerson;
        return this;
    }

    public BuildingBuilder WithPhone(string? phone)
    {
        _phone = phone;
        return this;
    }

    public BuildingBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public BuildingBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public BuildingBuilder WithUpdatedAt(DateTime? updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Preset: Modern Office Tower
    /// </summary>
    public BuildingBuilder AsModernOfficeTower()
    {
        _id = null;
        _name = "Modern Office Tower";
        _buildingCode = "MOT-001";
        _description = "State-of-the-art office tower with modern amenities";
        _address = "123 Tech Drive, Suite 1000";
        _numberOfFloors = 45;
        _yearConstructed = 2020;
        _totalFloorArea = 450000;
        _contactPerson = "John Smith";
        _phone = "+1-555-123-4567";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Preset: Historic Building
    /// </summary>
    public BuildingBuilder AsHistoricBuilding()
    {
        _id = null;
        _name = "Historic Building";
        _buildingCode = "HIST-001";
        _description = "Historic landmark building with restored interiors";
        _address = "456 Heritage Lane";
        _numberOfFloors = 8;
        _yearConstructed = 1925;
        _totalFloorArea = 85000;
        _contactPerson = "Mary Johnson";
        _phone = "+1-555-987-6543";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Preset: Industrial Complex
    /// </summary>
    public BuildingBuilder AsIndustrialComplex()
    {
        _id = null;
        _name = "Industrial Complex";
        _buildingCode = "IND-001";
        _description = "Multi-use industrial and manufacturing facility";
        _address = "789 Industrial Way";
        _numberOfFloors = 3;
        _yearConstructed = 2015;
        _totalFloorArea = 250000;
        _contactPerson = "Robert Wilson";
        _phone = "+1-555-456-7890";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Preset: Research Center
    /// </summary>
    public BuildingBuilder AsResearchCenter()
    {
        _id = null;
        _name = "Research & Development Center";
        _buildingCode = "RDC-001";
        _description = "Dedicated research and development facility";
        _address = "321 Innovation Parkway";
        _numberOfFloors = 6;
        _yearConstructed = 2018;
        _totalFloorArea = 120000;
        _contactPerson = "Dr. Susan Chen";
        _phone = "+1-555-321-0987";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Generic test building
    /// </summary>
    public BuildingBuilder AsTestBuilding(int? id = null)
    {
        _id = id;
        _name = "Test Building";
        _buildingCode = "TEST-001";
        _description = "A test building for unit testing";
        _address = "123 Test Street";
        _numberOfFloors = 5;
        _yearConstructed = 2020;
        _totalFloorArea = 50000;
        _contactPerson = "Test Manager";
        _phone = "123-456-7890";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Inactive building (for testing inactive scenarios)
    /// </summary>
    public BuildingBuilder AsInactiveBuilding()
    {
        _isActive = false;
        _updatedAt = DateTime.UtcNow.AddDays(-30);
        return this;
    }

    public Building Build()
    {
        var building = new Building
        {
            LocationId = _locationId,
            Name = _name,
            BuildingCode = _buildingCode,
            Description = _description,
            Address = _address,
            NumberOfFloors = _numberOfFloors,
            YearConstructed = _yearConstructed,
            TotalFloorArea = _totalFloorArea,
            ContactPerson = _contactPerson,
            Phone = _phone,
            IsActive = _isActive,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt
        };

        if (_id.HasValue)
        {
            building.Id = _id.Value;
        }

        return building;
    }

    /// <summary>
    /// Creates a building with an associated location (for integration testing)
    /// </summary>
    public (Building Building, Location Location) BuildWithLocation()
    {
        var location = new LocationBuilder().WithId(1).AsTestLocation().Build();
        var building = WithLocationId(location.Id).Build();
        building.Location = location;
        return (building, location);
    }
}
