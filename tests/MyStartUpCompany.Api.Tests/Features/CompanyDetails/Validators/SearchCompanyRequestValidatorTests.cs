using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Features.CompanyDetails.Validators;

namespace MyStartUpCompany.Api.Tests.Features.CompanyDetails.Validators;

public class SearchCompanyRequestValidatorTests
{
    private readonly SearchCompanyRequestValidator _validator;

    public SearchCompanyRequestValidatorTests()
    {
        _validator = new SearchCompanyRequestValidator();
    }

    #region PageNumber Validation Tests

    [Fact]
    public void PageNumber_WithValidNumber_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void PageNumber_WithMinimumValue_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void PageNumber_WithMaximumValue_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 10000, PageSize = 10 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void PageNumber_WithLessThanMinimum_ShouldFail(int pageNumber)
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = pageNumber, PageSize = 10 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be at least 1.");
    }

    [Theory]
    [InlineData(10001)]
    [InlineData(20000)]
    public void PageNumber_WithGreaterThanMaximum_ShouldFail(int pageNumber)
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = pageNumber, PageSize = 10 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number cannot exceed 10000.");
    }

    #endregion

    #region PageSize Validation Tests

    [Fact]
    public void PageSize_WithValidSize_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 50 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void PageSize_WithMinimumValue_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 1 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void PageSize_WithMaximumValue_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 100 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    public void PageSize_WithLessThanMinimum_ShouldFail(int pageSize)
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = pageSize };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be at least 1.");
    }

    [Theory]
    [InlineData(101)]
    [InlineData(200)]
    public void PageSize_WithGreaterThanMaximum_ShouldFail(int pageSize)
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = pageSize };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size cannot exceed 100.");
    }

    #endregion

    #region Region Validation Tests

    [Fact]
    public void Region_WithNull_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Region = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Region);
    }

    [Fact]
    public void Region_WithEmpty_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Region = "" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Region);
    }

    [Fact]
    public void Region_WithWhitespace_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Region = "   " };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Region);
    }

    [Fact]
    public void Region_WithValidAlphabetic_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Region = "California" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Region);
    }

    [Fact]
    public void Region_WithValidAlphabeticWithSpaces_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Region = "New York" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Region);
    }

    [Fact]
    public void Region_WithValidAlphabeticWithHyphens_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Region = "South-Dakota" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Region);
    }

    [Theory]
    [InlineData("Region123")]
    [InlineData("Region@")]
    [InlineData("Region#Name")]
    public void Region_WithInvalidCharacters_ShouldFail(string region)
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Region = region };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Region)
            .WithErrorMessage("Region can only contain letters, spaces, and hyphens.");
    }

    [Fact]
    public void Region_WithLengthExceededByOne_ShouldFail()
    {
        // Arrange
        var region = new string('A', 101);
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Region = region };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Region)
            .WithErrorMessage("Region cannot exceed 100 characters.");
    }

    [Fact]
    public void Region_WithMaxLength_ShouldPass()
    {
        // Arrange
        var region = new string('A', 100);
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Region = region };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Region);
    }

    #endregion

    #region Country Validation Tests

    [Fact]
    public void Country_WithNull_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Country = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Country);
    }

    [Fact]
    public void Country_WithValidAlphabetic_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Country = "United States" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Country);
    }

    [Theory]
    [InlineData("Country123")]
    [InlineData("Country@")]
    public void Country_WithInvalidCharacters_ShouldFail(string country)
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Country = country };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country)
            .WithErrorMessage("Country can only contain letters, spaces, and hyphens.");
    }

    [Fact]
    public void Country_WithLengthExceeded_ShouldFail()
    {
        // Arrange
        var country = new string('A', 101);
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, Country = country };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country)
            .WithErrorMessage("Country cannot exceed 100 characters.");
    }

    #endregion

    #region City Validation Tests

    [Fact]
    public void City_WithNull_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, City = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.City);
    }

    [Fact]
    public void City_WithValidAlphabetic_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, City = "San Francisco" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.City);
    }

    [Theory]
    [InlineData("City123")]
    [InlineData("City@")]
    public void City_WithInvalidCharacters_ShouldFail(string city)
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, City = city };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City can only contain letters, spaces, and hyphens.");
    }

    [Fact]
    public void City_WithLengthExceeded_ShouldFail()
    {
        // Arrange
        var city = new string('A', 101);
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, City = city };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City cannot exceed 100 characters.");
    }

    #endregion

    #region PostalCode Validation Tests

    [Fact]
    public void PostalCode_WithNull_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, PostalCode = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PostalCode);
    }

    [Fact]
    public void PostalCode_WithValidNumeric_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, PostalCode = "12345" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PostalCode);
    }

    [Fact]
    public void PostalCode_WithValidAlphanumeric_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, PostalCode = "M5V 3A8" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PostalCode);
    }

    [Fact]
    public void PostalCode_WithValidAlphanumericWithHyphens_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, PostalCode = "12345-6789" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PostalCode);
    }

    [Theory]
    [InlineData("12345@")]
    [InlineData("12345#67")]
    public void PostalCode_WithInvalidCharacters_ShouldFail(string postalCode)
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, PostalCode = postalCode };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PostalCode)
            .WithErrorMessage("Postal code can only contain alphanumeric characters, spaces, and hyphens.");
    }

    [Fact]
    public void PostalCode_WithLengthExceeded_ShouldFail()
    {
        // Arrange
        var postalCode = new string('A', 21);
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, PostalCode = postalCode };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PostalCode)
            .WithErrorMessage("Postal code cannot exceed 20 characters.");
    }

    #endregion

    #region SearchTerm Validation Tests

    [Fact]
    public void SearchTerm_WithNull_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, SearchTerm = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public void SearchTerm_WithValidText_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, SearchTerm = "Tech Company" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public void SearchTerm_WithMaxLength_ShouldPass()
    {
        // Arrange
        var searchTerm = new string('A', 100);
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, SearchTerm = searchTerm };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public void SearchTerm_WithLengthExceeded_ShouldFail()
    {
        // Arrange
        var searchTerm = new string('A', 101);
        var request = new SearchCompanyRequest { PageNumber = 1, PageSize = 10, SearchTerm = searchTerm };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
            .WithErrorMessage("Search term cannot exceed 100 characters.");
    }

    #endregion

    #region Combined Validation Tests

    [Fact]
    public void ValidRequest_WithAllFieldsValid_ShouldPass()
    {
        // Arrange
        var request = new SearchCompanyRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Region = "California",
            Country = "United States",
            City = "San Francisco",
            PostalCode = "94102",
            SearchTerm = "Technology"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void InvalidRequest_WithMultipleErrors_ShouldFailForAllInvalidFields()
    {
        // Arrange
        var request = new SearchCompanyRequest
        {
            PageNumber = -1,
            PageSize = 150,
            Region = "California123",
            PostalCode = "12345@"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
        result.ShouldHaveValidationErrorFor(x => x.Region);
        result.ShouldHaveValidationErrorFor(x => x.PostalCode);
    }

    #endregion
}
