using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.Shared.TestData.Builders;

/// <summary>
/// Builder pattern for creating Company test data with sensible defaults
/// </summary>
public class CompanyBuilder
{
    private int? _id = null; // Changed to nullable - let database generate ID
    private string _name = "Default Company";
    private string? _description = "Default company description";
    private string _address = "123 Default Street";
    private string _city = "DefaultCity";
    private string? _region = "Default Region";
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

    public CompanyBuilder WithDescription(string? description)
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

    public CompanyBuilder WithRegion(string? region)
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
    /// Preset: Acme Corporation
    /// </summary>
    public CompanyBuilder AsAcmeCorporation()
    {
        _id = null; // Let database generate ID
        _name = "Acme Corporation";
        _description = "Leading provider of innovative solutions";
        _address = "123 Main Street";
        _city = "San Francisco";
        _region = "CA";
        _postalCode = "94105";
        _country = "United States";
        _phone = "+1-555-123-4567";
        return this;
    }

    /// <summary>
    /// Preset: TechVision Inc
    /// </summary>
    public CompanyBuilder AsTechVisionInc()
    {
        _id = null; // Let database generate ID
        _name = "TechVision Inc";
        _description = "Software development company";
        _address = "456 Tech Avenue";
        _city = "Seattle";
        _region = "WA";
        _postalCode = "98101";
        _country = "United States";
        _phone = "+1-555-987-6543";
        return this;
    }

    /// <summary>
    /// Preset: Global Systems Ltd
    /// </summary>
    public CompanyBuilder AsGlobalSystemsLtd()
    {
        _id = null; // Let database generate ID
        _name = "Global Systems Ltd";
        _description = "Enterprise solutions provider";
        _address = "789 Business Blvd";
        _city = "New York";
        _region = "NY";
        _postalCode = "10001";
        _country = "United States";
        _phone = "+1-555-456-7890";
        return this;
    }

    /// <summary>
    /// Generic test company template
    /// </summary>
    public CompanyBuilder AsTestCompany(int? id = null)
    {
        _id = id; // Allow optional ID
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

    public Company Build()
    {
        var company = new Company
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

        // Only set Id if explicitly provided
        if (_id.HasValue)
        {
            company.Id = _id.Value;
        }

        return company;
    }
}