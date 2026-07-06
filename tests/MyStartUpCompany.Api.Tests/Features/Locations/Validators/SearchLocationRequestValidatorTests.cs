using FluentValidation.TestHelper;
using MyStartUpCompany.Api.Features.Locations.Models;
using MyStartUpCompany.Api.Features.Locations.Validators;
using MyStartUpCompany.Api.Shared.Constants;

namespace MyStartUpCompany.Api.Tests.Features.Locations.Validators;

/// <summary>
/// Unit tests for SearchLocationRequestValidator to verify validation rules
/// </summary>
public class SearchLocationRequestValidatorTests
{
    private readonly SearchLocationRequestValidator _validator;

    public SearchLocationRequestValidatorTests()
    {
        _validator = new SearchLocationRequestValidator();
    }

    #region PageNumber Validation Tests

    [Fact]
    public void Validate_WithValidPageNumber_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Validate_WithPageNumberZero_ShouldHaveError()
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 0, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    #endregion

    #region PageSize Validation Tests

    [Fact]
    public void Validate_WithValidPageSize_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WithPageSizeZero_ShouldHaveError()
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 0);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    #endregion

    #region CompanyId Validation Tests

    [Fact]
    public void Validate_WithValidCompanyId_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, CompanyId: 5);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CompanyId);
    }

    [Fact]
    public void Validate_WithCompanyIdZero_ShouldHaveError()
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, CompanyId: 0);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CompanyId);
    }

    [Fact]
    public void Validate_WithNullCompanyId_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, CompanyId: null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CompanyId);
    }

    #endregion

    #region SearchTerm Validation Tests

    [Fact]
    public void Validate_WithValidSearchTerm_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, SearchTerm: "San Francisco");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public void Validate_WithSearchTermExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var longSearchTerm = new string('a', ValidationConstants.Search.MaxSearchTermLength + 1);
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, SearchTerm: longSearchTerm);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
    }

    #endregion

    #region Country Validation Tests

    [Fact]
    public void Validate_WithValidCountry_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, Country: "USA");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Country);
    }

    [Fact]
    public void Validate_WithCountryExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var longCountry = new string('a', 101);
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, Country: longCountry);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    #endregion

    #region City Validation Tests

    [Fact]
    public void Validate_WithValidCity_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, City: "San Francisco");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.City);
    }

    [Fact]
    public void Validate_WithCityExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var longCity = new string('a', 101);
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, City: longCity);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    #endregion

    #region SortBy Validation Tests

    [Theory]
    [InlineData("name")]
    [InlineData("city")]
    [InlineData("country")]
    [InlineData("createdat")]
    [InlineData("updatedat")]
    public void Validate_WithValidSortField_ShouldNotHaveErrors(string sortField)
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, SortBy: sortField);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("region")]
    [InlineData("description")]
    public void Validate_WithInvalidSortField_ShouldHaveError(string sortField)
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, SortBy: sortField);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    #endregion

    #region SortOrder Validation Tests

    [Theory]
    [InlineData("asc")]
    [InlineData("desc")]
    public void Validate_WithValidSortOrder_ShouldNotHaveErrors(string sortOrder)
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, SortOrder: sortOrder);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortOrder);
    }

    [Theory]
    [InlineData("ascending")]
    [InlineData("descending")]
    public void Validate_WithInvalidSortOrder_ShouldHaveError(string sortOrder)
    {
        // Arrange
        var request = new SearchLocationRequest(PageNumber: 1, PageSize: 10, SortOrder: sortOrder);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortOrder);
    }

    #endregion

    #region Comprehensive Validation Tests

    [Fact]
    public void Validate_WithAllValidParameters_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchLocationRequest(
            PageNumber: 2,
            PageSize: 20,
            CompanyId: 5,
            SearchTerm: "test",
            Country: "USA",
            City: "San Francisco",
            SortBy: "name",
            SortOrder: "asc");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
