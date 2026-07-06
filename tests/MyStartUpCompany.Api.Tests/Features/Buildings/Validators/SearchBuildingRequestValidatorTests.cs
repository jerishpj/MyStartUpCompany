using FluentValidation.TestHelper;
using MyStartUpCompany.Api.Features.Buildings.Models;
using MyStartUpCompany.Api.Features.Buildings.Validators;
using MyStartUpCompany.Api.Shared.Constants;

namespace MyStartUpCompany.Api.Tests.Features.Buildings.Validators;

/// <summary>
/// Unit tests for SearchBuildingRequestValidator to verify validation rules
/// </summary>
public class SearchBuildingRequestValidatorTests
{
    private readonly SearchBuildingRequestValidator _validator;

    public SearchBuildingRequestValidatorTests()
    {
        _validator = new SearchBuildingRequestValidator();
    }

    #region PageNumber Validation Tests

    [Fact]
    public void Validate_WithValidPageNumber_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Validate_WithPageNumberZero_ShouldHaveError()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 0, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Validate_WithPageNumberNegative_ShouldHaveError()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: -1, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void Validate_WithValidPageNumbers_ShouldNotHaveErrors(int pageNumber)
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: pageNumber, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    #endregion

    #region PageSize Validation Tests

    [Fact]
    public void Validate_WithValidPageSize_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WithPageSizeZero_ShouldHaveError()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 0);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WithPageSizeNegative_ShouldHaveError()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: -5);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validate_WithValidPageSizes_ShouldNotHaveErrors(int pageSize)
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: pageSize);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    #endregion

    #region LocationId Validation Tests

    [Fact]
    public void Validate_WithValidLocationId_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, LocationId: 5);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LocationId);
    }

    [Fact]
    public void Validate_WithLocationIdZero_ShouldHaveError()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, LocationId: 0);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LocationId);
    }

    [Fact]
    public void Validate_WithLocationIdNegative_ShouldHaveError()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, LocationId: -1);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LocationId);
    }

    [Fact]
    public void Validate_WithNullLocationId_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, LocationId: null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LocationId);
    }

    #endregion

    #region SearchTerm Validation Tests

    [Fact]
    public void Validate_WithValidSearchTerm_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, SearchTerm: "Main Building");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public void Validate_WithEmptySearchTerm_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, SearchTerm: "");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SearchTerm);
    }

    [Fact]
    public void Validate_WithNullSearchTerm_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, SearchTerm: null);

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
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, SearchTerm: longSearchTerm);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
    }

    #endregion

    #region BuildingCode Validation Tests

    [Fact]
    public void Validate_WithValidBuildingCode_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, BuildingCode: "BLD001");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BuildingCode);
    }

    [Fact]
    public void Validate_WithBuildingCodeExceedingMaxLength_ShouldHaveError()
    {
        // Arrange
        var longCode = new string('a', 51);
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, BuildingCode: longCode);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BuildingCode);
    }

    #endregion

    #region SortBy Validation Tests

    [Theory]
    [InlineData("name")]
    [InlineData("code")]
    [InlineData("createdat")]
    [InlineData("updatedat")]
    [InlineData("Name")]
    [InlineData("CODE")]
    public void Validate_WithValidSortField_ShouldNotHaveErrors(string sortField)
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, SortBy: sortField);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("unknown")]
    [InlineData("description")]
    public void Validate_WithInvalidSortField_ShouldHaveError(string sortField)
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, SortBy: sortField);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public void Validate_WithNullSortField_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, SortBy: null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    #endregion

    #region SortOrder Validation Tests

    [Theory]
    [InlineData("asc")]
    [InlineData("desc")]
    public void Validate_WithValidSortOrder_ShouldNotHaveErrors(string sortOrder)
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, SortOrder: sortOrder);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortOrder);
    }

    [Theory]
    [InlineData("ascending")]
    [InlineData("descending")]
    [InlineData("invalid")]
    public void Validate_WithInvalidSortOrder_ShouldHaveError(string sortOrder)
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, SortOrder: sortOrder);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortOrder);
    }

    [Fact]
    public void Validate_WithNullSortOrder_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(PageNumber: 1, PageSize: 10, SortOrder: null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortOrder);
    }

    #endregion

    #region Comprehensive Validation Tests

    [Fact]
    public void Validate_WithAllValidParameters_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(
            PageNumber: 2,
            PageSize: 20,
            LocationId: 5,
            SearchTerm: "test",
            BuildingCode: "BLD001",
            SortBy: "name",
            SortOrder: "asc");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithMultipleErrors_ShouldReportAllErrors()
    {
        // Arrange
        var request = new SearchBuildingRequest(
            PageNumber: 0,
            PageSize: 0,
            LocationId: -1,
            SortBy: "invalid",
            SortOrder: "invalid");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
        result.ShouldHaveValidationErrorFor(x => x.LocationId);
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
        result.ShouldHaveValidationErrorFor(x => x.SortOrder);
    }

    #endregion
}
