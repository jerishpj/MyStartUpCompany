using Bogus;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.Shared.TestData.Builders;

/// <summary>
/// Builder pattern for creating Office test data with Bogus-generated realistic data
/// </summary>
public class OfficeBuilder
{
    private int? _id;
    private int _buildingId = 1;
    private string _name;
    private string? _officeCode;
    private string? _description;
    private int? _floorNumber;
    private string? _section;
    private int? _capacity;
    private string? _officeType;
    private decimal? _squareMeters;
    private string? _department;
    private string? _manager;
    private string? _phone;
    private string? _email;
    private string? _buildingName;
    private string? _locationCity;
    private string? _locationRegion;
    private string? _locationCountry;
    private bool _isActive = true;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime? _updatedAt;

    private readonly Faker _faker = new();

    public OfficeBuilder()
    {
        // Initialize with Bogus-generated data
        var officeTypes = new[] { "Open Office", "Cubicles", "Private", "Meeting Room", "Executive Suite", "Collaboration Space", "Hot Desking" };
        var departments = new[] { "Sales", "Marketing", "Engineering", "HR", "Finance", "Operations", "Support", "Management" };
        var sections = new[] { "Wing A", "Wing B", "Section North", "Section South", "East Wing", "West Wing" };

        _name = $"Floor {_faker.Random.Int(1, 50)} - {_faker.PickRandom(departments)}";
        _officeCode = $"F{_faker.Random.Int(1, 50)}-{_faker.Random.AlphaNumeric(2).ToUpper()}";
        _description = _faker.Lorem.Sentence();
        _floorNumber = _faker.Random.Int(1, 50);
        _section = _faker.PickRandom(sections);
        _capacity = _faker.Random.Int(5, 200);
        _officeType = _faker.PickRandom(officeTypes);
        _squareMeters = _faker.Random.Decimal(50, 5000);
        _department = _faker.PickRandom(departments);
        _manager = _faker.Person.FullName;
        _phone = _faker.Phone.PhoneNumber();
        _email = _faker.Internet.Email();
    }

    public OfficeBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public OfficeBuilder WithBuildingId(int buildingId)
    {
        _buildingId = buildingId;
        return this;
    }

    public OfficeBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public OfficeBuilder WithOfficeCode(string? officeCode)
    {
        _officeCode = officeCode;
        return this;
    }

    public OfficeBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public OfficeBuilder WithFloorNumber(int? floorNumber)
    {
        _floorNumber = floorNumber;
        return this;
    }

    public OfficeBuilder WithSection(string? section)
    {
        _section = section;
        return this;
    }

    public OfficeBuilder WithCapacity(int? capacity)
    {
        _capacity = capacity;
        return this;
    }

    public OfficeBuilder WithOfficeType(string? officeType)
    {
        _officeType = officeType;
        return this;
    }

    public OfficeBuilder WithSquareMeters(decimal? squareMeters)
    {
        _squareMeters = squareMeters;
        return this;
    }

    public OfficeBuilder WithDepartment(string? department)
    {
        _department = department;
        return this;
    }

    public OfficeBuilder WithManager(string? manager)
    {
        _manager = manager;
        return this;
    }

    public OfficeBuilder WithPhone(string? phone)
    {
        _phone = phone;
        return this;
    }

    public OfficeBuilder WithEmail(string? email)
    {
        _email = email;
        return this;
    }

    public OfficeBuilder WithBuildingName(string? buildingName)
    {
        _buildingName = buildingName;
        return this;
    }

    public OfficeBuilder WithLocationCity(string? locationCity)
    {
        _locationCity = locationCity;
        return this;
    }

    public OfficeBuilder WithLocationRegion(string? locationRegion)
    {
        _locationRegion = locationRegion;
        return this;
    }

    public OfficeBuilder WithLocationCountry(string? locationCountry)
    {
        _locationCountry = locationCountry;
        return this;
    }

    public OfficeBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public OfficeBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public OfficeBuilder WithUpdatedAt(DateTime? updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Preset: Executive Suite
    /// </summary>
    public OfficeBuilder AsExecutiveSuite()
    {
        _id = null;
        _name = "Executive Suite";
        _officeCode = "EX-001";
        _description = "Premium executive office space with conference facilities";
        _floorNumber = 50;
        _section = "Wing A";
        _capacity = 30;
        _officeType = "Executive Suite";
        _squareMeters = 1500;
        _department = "Management";
        _manager = "Sarah Johnson";
        _phone = "+1-555-123-4567";
        _email = "sarah.johnson@company.com";
        _buildingName = "Corporate Tower";
        _locationCity = "San Francisco";
        _locationRegion = "CA";
        _locationCountry = "United States";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Preset: Sales Department Floor
    /// </summary>
    public OfficeBuilder AsSalesDepartmentFloor()
    {
        _id = null;
        _name = "Floor 10 - Sales";
        _officeCode = "F10-SALES";
        _description = "Open office space for sales team";
        _floorNumber = 10;
        _section = "Wing B";
        _capacity = 120;
        _officeType = "Open Office";
        _squareMeters = 2500;
        _department = "Sales";
        _manager = "Mike Davis";
        _phone = "+1-555-987-6543";
        _email = "mike.davis@company.com";
        _buildingName = "Corporate Tower";
        _locationCity = "New York";
        _locationRegion = "NY";
        _locationCountry = "United States";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Preset: Engineering Lab
    /// </summary>
    public OfficeBuilder AsEngineeringLab()
    {
        _id = null;
        _name = "Engineering Laboratory";
        _officeCode = "ENG-LAB";
        _description = "Advanced R&D laboratory with testing equipment";
        _floorNumber = 15;
        _section = "Section North";
        _capacity = 50;
        _officeType = "Collaboration Space";
        _squareMeters = 3000;
        _department = "Engineering";
        _manager = "Dr. Robert Chen";
        _phone = "+1-555-456-7890";
        _email = "robert.chen@company.com";
        _buildingName = "Tech Hub";
        _locationCity = "Seattle";
        _locationRegion = "WA";
        _locationCountry = "United States";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Preset: Meeting Room
    /// </summary>
    public OfficeBuilder AsMeetingRoom()
    {
        _id = null;
        _name = "Conference Room A";
        _officeCode = "CONF-A";
        _description = "Large conference room with video conferencing setup";
        _floorNumber = 5;
        _section = "Wing A";
        _capacity = 25;
        _officeType = "Meeting Room";
        _squareMeters = 200;
        _department = null;
        _manager = null;
        _phone = "+1-555-321-0987";
        _email = "conference-a@company.com";
        _buildingName = "Corporate Tower";
        _locationCity = "Boston";
        _locationRegion = "MA";
        _locationCountry = "United States";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Preset: HR Department
    /// </summary>
    public OfficeBuilder AsHRDepartment()
    {
        _id = null;
        _name = "Floor 3 - Human Resources";
        _officeCode = "F3-HR";
        _description = "Human Resources department office";
        _floorNumber = 3;
        _section = "Wing C";
        _capacity = 25;
        _officeType = "Cubicles";
        _squareMeters = 800;
        _department = "HR";
        _manager = "Emma Wilson";
        _phone = "+1-555-234-5678";
        _email = "emma.wilson@company.com";
        _buildingName = "Administrative Tower";
        _locationCity = "Chicago";
        _locationRegion = "IL";
        _locationCountry = "United States";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Generic test office
    /// </summary>
    public OfficeBuilder AsTestOffice(int? id = null)
    {
        _id = id;
        _name = "Test Office";
        _officeCode = "TEST-001";
        _description = "A test office for unit testing";
        _floorNumber = 1;
        _section = "Wing A";
        _capacity = 20;
        _officeType = "Open Office";
        _squareMeters = 500;
        _department = "Testing";
        _manager = "Test Manager";
        _phone = "123-456-7890";
        _email = "test@company.com";
        _buildingName = "Test Building";
        _locationCity = "Testville";
        _locationRegion = "Test";
        _locationCountry = "Testland";
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Inactive office (for testing inactive scenarios)
    /// </summary>
    public OfficeBuilder AsInactiveOffice()
    {
        _isActive = false;
        _updatedAt = DateTime.UtcNow.AddDays(-30);
        return this;
    }

    /// <summary>
    /// Small office with limited capacity
    /// </summary>
    public OfficeBuilder AsSmallOffice()
    {
        _capacity = _faker.Random.Int(5, 15);
        _squareMeters = _faker.Random.Decimal(50, 300);
        _officeType = "Private";
        return this;
    }

    /// <summary>
    /// Large open office space
    /// </summary>
    public OfficeBuilder AsLargeOpenOffice()
    {
        _capacity = _faker.Random.Int(100, 300);
        _squareMeters = _faker.Random.Decimal(2000, 5000);
        _officeType = "Open Office";
        return this;
    }

    public Office Build()
    {
        var office = new Office
        {
            BuildingId = _buildingId,
            Name = _name,
            OfficeCode = _officeCode,
            Description = _description,
            FloorNumber = _floorNumber,
            Section = _section,
            Capacity = _capacity,
            OfficeType = _officeType,
            SquareMeters = _squareMeters,
            Department = _department,
            Manager = _manager,
            Phone = _phone,
            Email = _email,
            BuildingName = _buildingName,
            LocationCity = _locationCity,
            LocationRegion = _locationRegion,
            LocationCountry = _locationCountry,
            IsActive = _isActive,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt
        };

        if (_id.HasValue)
        {
            office.Id = _id.Value;
        }

        return office;
    }

    /// <summary>
    /// Creates an office with associated building and location (for integration testing)
    /// </summary>
    public (Office Office, Building Building, Location Location) BuildWithBuildingAndLocation()
    {
        var (building, location) = new BuildingBuilder().WithId(1).AsTestBuilding().BuildWithLocation();
        var office = WithBuildingId(building.Id)
            .WithBuildingName(building.Name)
            .WithLocationCity(location.City)
            .WithLocationRegion(location.Region)
            .WithLocationCountry(location.Country)
            .Build();
        office.Building = building;
        return (office, building, location);
    }
}
