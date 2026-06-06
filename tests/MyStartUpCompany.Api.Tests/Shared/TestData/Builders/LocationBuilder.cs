using Bogus;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.Shared.TestData.Builders;

/// <summary>
/// Builder pattern for creating Location test data with Bogus-generated realistic data
/// </summary>
public class LocationBuilder
{
    private int? _id;
    private int _companyId = 1;
    private string _name;
    private string? _description;
    private string _address;
    private string _city;
    private string? _region;
    private string _postalCode;
    private string _country;
    private string? _phone;
    private bool _isActive = true;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime? _updatedAt;

    private readonly Faker _faker = new();

    public LocationBuilder()
    {
        // Initialize with Bogus-generated data
        _name = _faker.Company.CompanyName() + " Branch";
        _address = _faker.Address.StreetAddress();
        _city = _faker.Address.City();
        _region = _faker.Address.StateAbbr();
        _postalCode = _faker.Address.ZipCode();
        _country = _faker.Address.Country();
        _phone = _faker.Phone.PhoneNumber();
        _description = _faker.Lorem.Sentence();
    }

    public LocationBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public LocationBuilder WithCompanyId(int companyId)
    {
        _companyId = companyId;
        return this;
    }

    public LocationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public LocationBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public LocationBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public LocationBuilder WithCity(string city)
    {
        _city = city;
        return this;
    }

    public LocationBuilder WithRegion(string? region)
    {
        _region = region;
        return this;
    }

    public LocationBuilder WithPostalCode(string postalCode)
    {
        _postalCode = postalCode;
        return this;
    }

    public LocationBuilder WithCountry(string country)
    {
        _country = country;
        return this;
    }

    public LocationBuilder WithPhone(string? phone)
    {
        _phone = phone;
        return this;
    }

    public LocationBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }

    public LocationBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public LocationBuilder WithUpdatedAt(DateTime? updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    #region Presets
    public LocationBuilder AsSanFranciscoHeadquarters()
    {
        _name = "San Francisco Headquarters";
        _address = "123 Market Street";
        _city = "San Francisco";
        _region = "CA";
        _postalCode = "94102";
        _country = "United States";
        _phone = "+1-415-555-0123";
        _description = "Main headquarters";
        return this;
    }

    public LocationBuilder AsNewYorkBranch()
    {
        _name = "New York Branch";
        _address = "456 5th Avenue";
        _city = "New York";
        _region = "NY";
        _postalCode = "10022";
        _country = "United States";
        _phone = "+1-212-555-0456";
        _description = "East coast branch";
        return this;
    }

    public LocationBuilder AsLondonOffice()
    {
        _name = "London Office";
        _address = "789 Piccadilly";
        _city = "London";
        _region = "England";
        _postalCode = "W1V 9PA";
        _country = "United Kingdom";
        _phone = "+44-20-7123-4567";
        _description = "European headquarters";
        return this;
    }

    public LocationBuilder AsTokyoOffice()
    {
        _name = "Tokyo Office";
        _address = "123 Chiyoda Ward";
        _city = "Tokyo";
        _region = "Tokyo";
        _postalCode = "100-0001";
        _country = "Japan";
        _phone = "+81-3-1234-5678";
        _description = "Asia-Pacific hub";
        return this;
    }

    public LocationBuilder AsTestLocation()
    {
        _name = "Test Location";
        _address = "123 Test Street";
        _city = "Test City";
        _region = "TC";
        _postalCode = "12345";
        _country = "Test Country";
        _phone = "+1-555-1234";
        _description = "Test location for unit tests";
        return this;
    }
    #endregion

    public Location Build()
    {
        var location = new Location
        {
            Id = _id ?? 0,
            CompanyId = _companyId,
            Name = _name,
            Description = _description,
            Address = _address,
            City = _city,
            Region = _region,
            PostalCode = _postalCode,
            Country = _country,
            Phone = _phone,
            IsActive = _isActive,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt
        };

        return location;
    }
}
