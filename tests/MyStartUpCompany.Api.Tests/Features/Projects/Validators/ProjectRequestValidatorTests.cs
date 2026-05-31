using FluentAssertions;
using FluentValidation.TestHelper;
using MyStartUpCompany.Api.Features.Projects.Models;
using MyStartUpCompany.Api.Features.Projects.Validators;

namespace MyStartUpCompany.Api.Tests.Features.Projects.Validators;

public class CreateProjectRequestValidatorTests
{
    private readonly CreateProjectRequestValidator _validator;

    public CreateProjectRequestValidatorTests()
    {
        _validator = new CreateProjectRequestValidator();
    }

    #region ProjectIdentifier Validation Tests

    [Fact]
    public void ProjectIdentifier_WithValidValue_ShouldPass()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProjectIdentifier);
    }

    [Fact]
    public void ProjectIdentifier_WithEmpty_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectIdentifier)
            .WithErrorMessage("Project identifier is required");
    }

    [Fact]
    public void ProjectIdentifier_WithMaxLength_ShouldPass()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = new string('A', 50),
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProjectIdentifier);
    }

    [Fact]
    public void ProjectIdentifier_ExceedsMaxLength_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = new string('A', 51),
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectIdentifier)
            .WithErrorMessage("Project identifier must not exceed 50 characters");
    }

    [Theory]
    [InlineData("proj-2024-001")]  // lowercase
    [InlineData("PROJ_2024_001")]  // underscore
    [InlineData("PROJ@2024")]      // special char
    public void ProjectIdentifier_WithInvalidFormat_ShouldFail(string identifier)
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = identifier,
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectIdentifier)
            .WithErrorMessage("Project identifier must contain only uppercase letters, digits, and hyphens");
    }

    #endregion

    #region Name Validation Tests

    [Fact]
    public void Name_WithValidValue_ShouldPass()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_WithEmpty_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Project name is required");
    }

    [Fact]
    public void Name_ExceedsMaxLength_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = new string('A', 501),
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Project name must not exceed 500 characters");
    }

    #endregion

    #region Code Validation Tests

    [Fact]
    public void Code_WithValidValue_ShouldPass()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Code_WithEmpty_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("Project code is required");
    }

    [Theory]
    [InlineData("test")]      // lowercase
    [InlineData("Test")]      // mixed case
    [InlineData("TEST-123")]  // hyphen
    [InlineData("TEST 123")]  // space
    public void Code_WithInvalidFormat_ShouldFail(string code)
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = code,
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("Project code must contain only uppercase letters and digits");
    }

    #endregion

    #region Location Validation Tests

    [Fact]
    public void Location_WithValidValue_ShouldPass()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Location);
    }

    [Fact]
    public void Location_WithEmpty_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Location)
            .WithErrorMessage("Project location is required");
    }

    [Fact]
    public void Location_ExceedsMaxLength_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = new string('A', 201),
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Location)
            .WithErrorMessage("Project location must not exceed 200 characters");
    }

    #endregion

    #region CompanyId Validation Tests

    [Fact]
    public void CompanyId_WithPositiveValue_ShouldPass()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CompanyId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CompanyId_WithNonPositiveValue_ShouldFail(int companyId)
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = companyId,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CompanyId)
            .WithErrorMessage("Company ID must be a positive number");
    }

    #endregion

    #region Type Validation Tests

    [Fact]
    public void Type_WithEmpty_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage("Project type is required");
    }

    [Fact]
    public void Type_WithInvalidValue_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "InvalidType",
            Details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    #endregion

    #region Details Validation Tests

    [Fact]
    public void Details_WithNull_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = null!
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Details)
            .WithErrorMessage("Project details are required");
    }

    [Fact]
    public void Details_WithInvalidStatus_ShouldFail()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto 
            { 
                Status = "InvalidStatus", 
                Budget = 10000, 
                StartDate = DateTime.Now 
            }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Details.Status");
    }

    #endregion

    #region Combined Validation Tests

    [Fact]
    public void ValidRequest_WithAllFieldsValid_ShouldPass()
    {
        // Arrange
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "PROJ-2024-001",
            Name = "Test Project",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            Details = new ProjectDetailsDto 
            { 
                Status = "Active", 
                Budget = 10000, 
                StartDate = DateTime.Now 
            }
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
        var request = new CreateProjectRequest
        {
            ProjectIdentifier = "",
            Name = "",
            Code = "test",
            Location = "",
            CompanyId = 0,
            Type = "",
            Details = null!
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectIdentifier);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Code);
        result.ShouldHaveValidationErrorFor(x => x.Location);
        result.ShouldHaveValidationErrorFor(x => x.CompanyId);
        result.ShouldHaveValidationErrorFor(x => x.Type);
        result.ShouldHaveValidationErrorFor(x => x.Details);
    }

    #endregion
}

public class ProjectDetailsDtoValidatorTests
{
    private readonly ProjectDetailsDtoValidator _validator;

    public ProjectDetailsDtoValidatorTests()
    {
        _validator = new ProjectDetailsDtoValidator();
    }

    #region Status Validation Tests

    [Fact]
    public void Status_WithValidStatus_ShouldPass()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData("Planning")]
    [InlineData("Active")]
    [InlineData("OnHold")]
    [InlineData("Completed")]
    [InlineData("Archived")]
    public void Status_WithAllValidStatuses_ShouldPass(string status)
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = status, Budget = 10000, StartDate = DateTime.Now };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Status_WithEmpty_ShouldFail()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "", Budget = 10000, StartDate = DateTime.Now };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Status_WithInvalidValue_ShouldFail()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "InvalidStatus", Budget = 10000, StartDate = DateTime.Now };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Project status must be one of: Planning, Active, OnHold, Completed, Archived");
    }

    [Fact]
    public void Status_ExceedsMaxLength_ShouldFail()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = new string('A', 51), Budget = 10000, StartDate = DateTime.Now };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    #endregion

    #region Budget Validation Tests

    [Fact]
    public void Budget_WithZero_ShouldPass()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "Active", Budget = 0, StartDate = DateTime.Now };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Budget);
    }

    [Fact]
    public void Budget_WithPositiveValue_ShouldPass()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Budget);
    }

    [Fact]
    public void Budget_WithNegativeValue_ShouldFail()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "Active", Budget = -1000, StartDate = DateTime.Now };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Budget)
            .WithErrorMessage("Budget must be greater than or equal to 0");
    }

    #endregion

    #region StartDate Validation Tests

    [Fact]
    public void StartDate_WithValidDate_ShouldPass()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void StartDate_WithMinValue_ShouldFail()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.MinValue };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
            .WithErrorMessage("Start date must be a valid date");
    }

    #endregion

    #region EndDate Validation Tests

    [Fact]
    public void EndDate_WithNull_ShouldPass()
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now,
            EndDate = null 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void EndDate_AfterStartDate_ShouldPass()
    {
        // Arrange
        var startDate = DateTime.Now;
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = startDate,
            EndDate = startDate.AddDays(10) 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void EndDate_BeforeOrEqualToStartDate_ShouldFail()
    {
        // Arrange
        var startDate = DateTime.Now;
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = startDate,
            EndDate = startDate.AddDays(-1) 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate)
            .WithErrorMessage("End date must be after start date");
    }

    #endregion

    #region Priority Validation Tests

    [Fact]
    public void Priority_WithNull_ShouldPass()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now, Priority = null };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }

    [Theory]
    [InlineData("Critical")]
    [InlineData("High")]
    [InlineData("Medium")]
    [InlineData("Low")]
    public void Priority_WithValidValue_ShouldPass(string priority)
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now, Priority = priority };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void Priority_WithInvalidValue_ShouldFail()
    {
        // Arrange
        var details = new ProjectDetailsDto { Status = "Active", Budget = 10000, StartDate = DateTime.Now, Priority = "Invalid" };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Priority)
            .WithErrorMessage("Priority must be one of: Critical, High, Medium, Low");
    }

    #endregion

    #region ProgressPercentage Validation Tests

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void ProgressPercentage_WithValidValue_ShouldPass(int percentage)
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now, 
            ProgressPercentage = percentage 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProgressPercentage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ProgressPercentage_WithOutOfRangeValue_ShouldFail(int percentage)
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now, 
            ProgressPercentage = percentage 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProgressPercentage)
            .WithErrorMessage("Progress percentage must be between 0 and 100");
    }

    #endregion

    #region BudgetSpent Validation Tests

    [Fact]
    public void BudgetSpent_WithNull_ShouldPass()
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now, 
            BudgetSpent = null 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BudgetSpent);
    }

    [Fact]
    public void BudgetSpent_WithZero_ShouldPass()
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now, 
            BudgetSpent = 0 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BudgetSpent);
    }

    [Fact]
    public void BudgetSpent_WithNegativeValue_ShouldFail()
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now, 
            BudgetSpent = -1000 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BudgetSpent)
            .WithErrorMessage("Budget spent must be greater than or equal to 0");
    }

    #endregion

    #region RiskLevel Validation Tests

    [Fact]
    public void RiskLevel_WithNull_ShouldPass()
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now, 
            RiskLevel = null 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RiskLevel);
    }

    [Theory]
    [InlineData("Low")]
    [InlineData("Medium")]
    [InlineData("High")]
    [InlineData("Critical")]
    public void RiskLevel_WithValidValue_ShouldPass(string riskLevel)
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now, 
            RiskLevel = riskLevel 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RiskLevel);
    }

    [Fact]
    public void RiskLevel_WithInvalidValue_ShouldFail()
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now, 
            RiskLevel = "Invalid" 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RiskLevel)
            .WithErrorMessage("Risk level must be one of: Low, Medium, High, Critical");
    }

    #endregion

    #region Description Length Tests

    [Fact]
    public void Description_WithValidLength_ShouldPass()
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now, 
            Description = "This is a test description" 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_ExceedsMaxLength_ShouldFail()
    {
        // Arrange
        var details = new ProjectDetailsDto 
        { 
            Status = "Active", 
            Budget = 10000, 
            StartDate = DateTime.Now, 
            Description = new string('A', 2001) 
        };

        // Act
        var result = _validator.TestValidate(details);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 2000 characters");
    }

    #endregion
}

public class ProjectFilterRequestValidatorTests
{
    private readonly ProjectFilterRequestValidator _validator;

    public ProjectFilterRequestValidatorTests()
    {
        _validator = new ProjectFilterRequestValidator();
    }

    #region ProjectIdentifier Filter Tests

    [Fact]
    public void ProjectIdentifier_WithNull_ShouldPass()
    {
        // Arrange
        var request = new ProjectFilterRequest { ProjectIdentifier = null, PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProjectIdentifier);
    }

    [Fact]
    public void ProjectIdentifier_WithValidValue_ShouldPass()
    {
        // Arrange
        var request = new ProjectFilterRequest { ProjectIdentifier = "PROJ", PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ProjectIdentifier);
    }

    [Fact]
    public void ProjectIdentifier_ExceedsMaxLength_ShouldFail()
    {
        // Arrange
        var request = new ProjectFilterRequest { ProjectIdentifier = new string('A', 51), PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectIdentifier)
            .WithErrorMessage("Project identifier filter must not exceed 50 characters");
    }

    #endregion

    #region Name Filter Tests

    [Fact]
    public void Name_WithValidValue_ShouldPass()
    {
        // Arrange
        var request = new ProjectFilterRequest { Name = "Test Project", PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_ExceedsMaxLength_ShouldFail()
    {
        // Arrange
        var request = new ProjectFilterRequest { Name = new string('A', 501), PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Project name filter must not exceed 500 characters");
    }

    #endregion

    #region Code Filter Tests

    [Fact]
    public void Code_WithValidValue_ShouldPass()
    {
        // Arrange
        var request = new ProjectFilterRequest { Code = "TEST", PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Code_ExceedsMaxLength_ShouldFail()
    {
        // Arrange
        var request = new ProjectFilterRequest { Code = new string('A', 51), PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("Project code filter must not exceed 50 characters");
    }

    #endregion

    #region Location Filter Tests

    [Fact]
    public void Location_WithValidValue_ShouldPass()
    {
        // Arrange
        var request = new ProjectFilterRequest { Location = "New York", PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Location);
    }

    [Fact]
    public void Location_ExceedsMaxLength_ShouldFail()
    {
        // Arrange
        var request = new ProjectFilterRequest { Location = new string('A', 201), PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Location)
            .WithErrorMessage("Project location filter must not exceed 200 characters");
    }

    #endregion

    #region CompanyId Filter Tests

    [Fact]
    public void CompanyId_WithNull_ShouldPass()
    {
        // Arrange
        var request = new ProjectFilterRequest { CompanyId = null, PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CompanyId);
    }

    [Fact]
    public void CompanyId_WithPositiveValue_ShouldPass()
    {
        // Arrange
        var request = new ProjectFilterRequest { CompanyId = 1, PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CompanyId);
    }

    [Fact]
    public void CompanyId_WithZero_ShouldFail()
    {
        // Arrange
        var request = new ProjectFilterRequest { CompanyId = 0, PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CompanyId)
            .WithErrorMessage("Company ID filter must be a positive number");
    }

    #endregion

    #region Type Filter Tests

    [Fact]
    public void Type_WithNull_ShouldPass()
    {
        // Arrange
        var request = new ProjectFilterRequest { Type = null, PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Type);
    }

    #endregion

    #region SortOrder Tests

    [Theory]
    [InlineData("asc")]
    [InlineData("ASC")]
    [InlineData("desc")]
    [InlineData("DESC")]
    public void SortOrder_WithValidValue_ShouldPass(string sortOrder)
    {
        // Arrange
        var request = new ProjectFilterRequest { SortOrder = sortOrder, PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortOrder);
    }

    [Fact]
    public void SortOrder_WithInvalidValue_ShouldFail()
    {
        // Arrange
        var request = new ProjectFilterRequest { SortOrder = "invalid", PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortOrder)
            .WithErrorMessage("Sort order must be 'asc' or 'desc'");
    }

    #endregion

    #region SortBy Tests

    [Theory]
    [InlineData("Name")]
    [InlineData("Code")]
    [InlineData("Location")]
    [InlineData("CreatedAt")]
    public void SortBy_WithValidValue_ShouldPass(string sortBy)
    {
        // Arrange
        var request = new ProjectFilterRequest { SortBy = sortBy, PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public void SortBy_WithInvalidValue_ShouldFail()
    {
        // Arrange
        var request = new ProjectFilterRequest { SortBy = "Invalid", PageNumber = 1, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("Sort by must be one of: Name, Code, Location, CreatedAt");
    }

    #endregion

    #region Pagination Tests

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void PageNumber_WithValidValue_ShouldPass(int pageNumber)
    {
        // Arrange
        var request = new ProjectFilterRequest { PageNumber = pageNumber, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageNumber_WithInvalidValue_ShouldFail(int pageNumber)
    {
        // Arrange
        var request = new ProjectFilterRequest { PageNumber = pageNumber, PageSize = 20 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be greater than 0");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void PageSize_WithValidValue_ShouldPass(int pageSize)
    {
        // Arrange
        var request = new ProjectFilterRequest { PageNumber = 1, PageSize = pageSize };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void PageSize_WithZero_ShouldFail()
    {
        // Arrange
        var request = new ProjectFilterRequest { PageNumber = 1, PageSize = 0 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be greater than 0");
    }

    [Fact]
    public void PageSize_ExceedsMaximum_ShouldFail()
    {
        // Arrange
        var request = new ProjectFilterRequest { PageNumber = 1, PageSize = 101 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must not exceed 100");
    }

    #endregion

    #region Combined Validation Tests

    [Fact]
    public void ValidRequest_WithAllFieldsValid_ShouldPass()
    {
        // Arrange
        var request = new ProjectFilterRequest
        {
            ProjectIdentifier = "PROJ",
            Name = "Test",
            Code = "TEST",
            Location = "New York",
            CompanyId = 1,
            Type = "GameDevelopment",
            SortOrder = "asc",
            SortBy = "Name",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion
}
