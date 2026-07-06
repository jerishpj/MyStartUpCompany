using FluentValidation.TestHelper;
using MyStartUpCompany.Api.Features.Offices.Models;
using MyStartUpCompany.Api.Features.Offices.Validators;
using MyStartUpCompany.Api.Shared.Constants;

namespace MyStartUpCompany.Api.Tests.Features.Offices.Validators;

/// <summary>
/// Unit tests for SearchOfficeRequestValidator to verify validation rules
/// </summary>
public class SearchOfficeRequestValidatorTests
{
    private readonly SearchOfficeRequestValidator _validator;
    private const int MaxStringFieldLength = 100;

    public SearchOfficeRequestValidatorTests()
    {
        _validator = new SearchOfficeRequestValidator();
    }

    #region PageNumber Validation Tests

    [Fact]
    public void Validate_WithValidPageNumber_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Validate_WithPageNumberZero_ShouldHaveError()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 0, PageSize: 10);

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
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WithPageSizeZero_ShouldHaveError()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 0);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    #endregion

    #region BuildingId Validation Tests

    [Fact]
    public void Validate_WithValidBuildingId_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, BuildingId: 5);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BuildingId);
    }

    [Fact]
    public void Validate_WithBuildingIdZero_ShouldHaveError()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, BuildingId: 0);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BuildingId);
    }

    [Fact]
    public void Validate_WithNullBuildingId_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, BuildingId: null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BuildingId);
    }

    #endregion

    #region SearchTerm Validation Tests

    [Fact]
    public void Validate_WithValidSearchTerm_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, SearchTerm: "Sales Office");

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
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, SearchTerm: longSearchTerm);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
    }

    #endregion

    #region Department Validation Tests

    [Fact]
    public void Validate_WithValidDepartment_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, Department: "Sales");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Department);
    }

    [Fact]
    public void Validate_WithDepartmentExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var longDepartment = new string('a', MaxStringFieldLength + 1);
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, Department: longDepartment);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Department);
    }

    #endregion

    #region OfficeType Validation Tests

    [Fact]
    public void Validate_WithValidOfficeType_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, OfficeType: "Open Plan");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.OfficeType);
    }

    [Fact]
    public void Validate_WithOfficeTypeExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var longOfficeType = new string('a', MaxStringFieldLength + 1);
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, OfficeType: longOfficeType);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OfficeType);
    }

    #endregion

    #region BuildingName Validation Tests

    [Fact]
    public void Validate_WithValidBuildingName_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, BuildingName: "Main Building");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BuildingName);
    }

    [Fact]
    public void Validate_WithBuildingNameExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var longBuildingName = new string('a', MaxStringFieldLength + 1);
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, BuildingName: longBuildingName);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BuildingName);
    }

    #endregion

    #region Location Fields Validation Tests

    [Fact]
    public void Validate_WithValidLocationCity_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, LocationCity: "New York");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LocationCity);
    }

    [Fact]
    public void Validate_WithLocationCityExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var longCity = new string('a', MaxStringFieldLength + 1);
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, LocationCity: longCity);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LocationCity);
    }

    [Fact]
    public void Validate_WithValidLocationRegion_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, LocationRegion: "NY");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LocationRegion);
    }

    [Fact]
    public void Validate_WithValidLocationCountry_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, LocationCountry: "USA");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LocationCountry);
    }

    #endregion

    #region SortBy Validation Tests

    [Theory]
    [InlineData("name")]
    [InlineData("department")]
    [InlineData("officetype")]
    [InlineData("createdat")]
    [InlineData("updatedat")]
    public void Validate_WithValidSortField_ShouldNotHaveErrors(string sortField)
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, SortBy: sortField);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("building")]
    [InlineData("description")]
    public void Validate_WithInvalidSortField_ShouldHaveError(string sortField)
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, SortBy: sortField);

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
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, SortOrder: sortOrder);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortOrder);
    }

    [Theory]
    [InlineData("ascending")]
    [InlineData("invalid")]
    public void Validate_WithInvalidSortOrder_ShouldHaveError(string sortOrder)
    {
        // Arrange
        var request = new SearchOfficeRequest(PageNumber: 1, PageSize: 10, SortOrder: sortOrder);

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
        var request = new SearchOfficeRequest(
            PageNumber: 2,
            PageSize: 20,
            BuildingId: 5,
            SearchTerm: "test",
            Department: "Sales",
            OfficeType: "Open Plan",
            BuildingName: "Main",
            LocationCity: "New York",
            LocationRegion: "NY",
            LocationCountry: "USA",
            SortBy: "name",
            SortOrder: "asc");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
