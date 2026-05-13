using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.TestData.Builders;

/// <summary>
/// Builder pattern for creating Company test data with sensible defaults
/// </summary>
public class CompanyBuilder
{
    private int _id = 1;
    private string _name = "Default Company";
    private string _description = "Default company description";
    private string _address = "123 Default Street";
    private string _city = "DefaultCity";
    private string _region = "Default Region";
    private string _country = "Default Country";
    private string _postalCode = "12345";
    private string _phone = "123-456-7890";

    public CompanyBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public CompanyBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CompanyBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public CompanyBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public CompanyBuilder WithCity(string city)
    {
        _city = city;
        return this;
    }

    public CompanyBuilder WithRegion(string region)
    {
        _region = region;
        return this;
    }

    public CompanyBuilder WithCountry(string country)
    {
        _country = country;
        return this;
    }

    public CompanyBuilder WithPostalCode(string postalCode)
    {
        _postalCode = postalCode;
        return this;
    }

    public CompanyBuilder WithPhone(string phone)
    {
        _phone = phone;
        return this;
    }

    /// <summary>
    /// Creates a company with test-specific naming
    /// </summary>
    public CompanyBuilder AsTestCompany(int id = 1)
    {
        _id = id;
        _name = "Test Company";
        _description = "A test company for integration testing";
        _address = "123 Test St";
        _city = "Testville";
        _region = "Test Region";
        _country = "Testland";
        _postalCode = "12345";
        _phone = "123-456-7890";
        return this;
    }

    /// <summary>
    /// Creates a company with specific preset data
    /// </summary>
    public CompanyBuilder AsCompanyA()
    {
        _id = 1;
        _name = "Company A";
        _description = "First test company";
        _address = "123 Test St";
        _city = "Testville";
        _region = "Test Region";
        _country = "Testland";
        _postalCode = "12345";
        _phone = "123-456-7890";
        return this;
    }

    /// <summary>
    /// Creates a company with specific preset data
    /// </summary>
    public CompanyBuilder AsCompanyB()
    {
        _id = 2;
        _name = "Company B";
        _description = "Second test company";
        _address = "456 Test Ave";
        _city = "Testopolis";
        _region = "Test Region";
        _country = "Testland";
        _postalCode = "67890";
        _phone = "098-765-4321";
        return this;
    }

    public Company Build()
    {
        return new Company
        {
            Id = _id,
            Name = _name,
            Description = _description,
            Address = _address,
            City = _city,
            Region = _region,
            Country = _country,
            PostalCode = _postalCode,
            Phone = _phone
        };
    }

    /// <summary>
    /// Builds an anonymous object suitable for HTTP POST requests
    /// </summary>
    public object BuildDto()
    {
        return new
        {
            Name = _name,
            Description = _description,
            Address = _address,
            City = _city,
            Region = _region,
            Country = _country,
            PostalCode = _postalCode,
            Phone = _phone
        };
    }
}