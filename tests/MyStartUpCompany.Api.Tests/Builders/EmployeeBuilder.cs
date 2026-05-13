using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Tests.Builders;

/// <summary>
/// Builder pattern for creating Employee test data with sensible defaults
/// </summary>
public class EmployeeBuilder
{
    private int _id = 1;
    private string _name = "Default Employee";
    private string _title = "Default Title";
    private string? _description = null;
    private string? _address = null;
    private string? _city = null;
    private string? _region = null;
    private string? _postalCode = null;
    private string? _country = null;
    private string? _phone = null;
    private string? _email = null;

    public EmployeeBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public EmployeeBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public EmployeeBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public EmployeeBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public EmployeeBuilder WithAddress(string? address)
    {
        _address = address;
        return this;
    }

    public EmployeeBuilder WithCity(string? city)
    {
        _city = city;
        return this;
    }

    public EmployeeBuilder WithRegion(string? region)
    {
        _region = region;
        return this;
    }

    public EmployeeBuilder WithPostalCode(string? postalCode)
    {
        _postalCode = postalCode;
        return this;
    }

    public EmployeeBuilder WithCountry(string? country)
    {
        _country = country;
        return this;
    }

    public EmployeeBuilder WithPhone(string? phone)
    {
        _phone = phone;
        return this;
    }

    public EmployeeBuilder WithEmail(string? email)
    {
        _email = email;
        return this;
    }

    /// <summary>
    /// Preset: Senior Developer
    /// </summary>
    public EmployeeBuilder AsSeniorDeveloper(int id = 1)
    {
        _id = id;
        _name = "John Doe";
        _title = "Senior Software Developer";
        _description = "Experienced full-stack developer";
        _address = "123 Developer Lane";
        _city = "Tech City";
        _region = "CA";
        _postalCode = "94000";
        _country = "United States";
        _phone = "+1-555-100-2000";
        _email = "john.doe@example.com";
        return this;
    }

    /// <summary>
    /// Preset: Project Manager
    /// </summary>
    public EmployeeBuilder AsProjectManager(int id = 2)
    {
        _id = id;
        _name = "Jane Smith";
        _title = "Project Manager";
        _description = "Experienced in agile methodologies";
        _address = "456 Management Blvd";
        _city = "Business City";
        _region = "NY";
        _postalCode = "10000";
        _country = "United States";
        _phone = "+1-555-200-3000";
        _email = "jane.smith@example.com";
        return this;
    }

    public Employee Build()
    {
        return new Employee
        {
            Id = _id,
            Name = _name,
            Title = _title,
            Description = _description,
            Address = _address,
            City = _city,
            Region = _region,
            PostalCode = _postalCode,
            Country = _country,
            Phone = _phone,
            Email = _email
        };
    }

    /// <summary>
    /// Builds an anonymous object suitable for HTTP POST requests (no Id)
    /// </summary>
    public object BuildDto()
    {
        return new
        {
            Name = _name,
            Title = _title,
            Description = _description,
            Address = _address,
            City = _city,
            Region = _region,
            PostalCode = _postalCode,
            Country = _country,
            Phone = _phone,
            Email = _email
        };
    }
}