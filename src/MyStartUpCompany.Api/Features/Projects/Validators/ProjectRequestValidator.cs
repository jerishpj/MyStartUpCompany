using FluentValidation;
using MyStartUpCompany.Api.Features.Projects.Models;

namespace MyStartUpCompany.Api.Features.Projects.Validators;

/// <summary>
/// FluentValidation validator for ProjectDetailsDto
/// Validates non-searchable project details
/// </summary>
public class ProjectDetailsDtoValidator : AbstractValidator<ProjectDetailsDto>
{
    public ProjectDetailsDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Project status is required")
            .MaximumLength(50).WithMessage("Project status must not exceed 50 characters")
            .Must(s => new[] { "Planning", "Active", "OnHold", "Completed", "Archived" }.Contains(s))
            .WithMessage("Project status must be one of: Planning, Active, OnHold, Completed, Archived");

        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0).WithMessage("Budget must be greater than or equal to 0");

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.MinValue).WithMessage("Start date must be a valid date");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.ProjectManager)
            .MaximumLength(200).WithMessage("Project manager name must not exceed 200 characters");

        RuleFor(x => x.Priority)
            .Must(p => p == null || new[] { "Critical", "High", "Medium", "Low" }.Contains(p))
            .WithMessage("Priority must be one of: Critical, High, Medium, Low");

        RuleFor(x => x.ProgressPercentage)
            .InclusiveBetween(0, 100).WithMessage("Progress percentage must be between 0 and 100");

        RuleFor(x => x.BudgetSpent)
            .GreaterThanOrEqualTo(0).When(x => x.BudgetSpent.HasValue)
            .WithMessage("Budget spent must be greater than or equal to 0");

        RuleFor(x => x.RiskLevel)
            .Must(r => r == null || new[] { "Low", "Medium", "High", "Critical" }.Contains(r))
            .WithMessage("Risk level must be one of: Low, Medium, High, Critical");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters");

        RuleFor(x => x.Outcome)
            .MaximumLength(2000).WithMessage("Outcome must not exceed 2000 characters");
    }
}

/// <summary>
/// FluentValidation validator for CreateProjectRequest
/// Validates searchable project fields and delegates details validation
/// </summary>
public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        // Searchable field validations
        RuleFor(x => x.ProjectIdentifier)
            .NotEmpty().WithMessage("Project identifier is required")
            .MaximumLength(50).WithMessage("Project identifier must not exceed 50 characters")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Project identifier must contain only uppercase letters, digits, and hyphens");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(500).WithMessage("Project name must not exceed 500 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Project code is required")
            .MaximumLength(50).WithMessage("Project code must not exceed 50 characters")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Project code must contain only uppercase letters and digits");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Project location is required")
            .MaximumLength(200).WithMessage("Project location must not exceed 200 characters");

        RuleFor(x => x.CompanyId)
            .GreaterThan(0).WithMessage("Company ID must be a positive number");

        // Details validation
        RuleFor(x => x.Details)
            .NotNull().WithMessage("Project details are required")
            .SetValidator(new ProjectDetailsDtoValidator());
    }
}

/// <summary>
/// FluentValidation validator for ProjectFilterRequest
/// Validates search/filter parameters
/// </summary>
public class ProjectFilterRequestValidator : AbstractValidator<ProjectFilterRequest>
{
    public ProjectFilterRequestValidator()
    {
        RuleFor(x => x.ProjectIdentifier)
            .MaximumLength(50).WithMessage("Project identifier filter must not exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.ProjectIdentifier));

        RuleFor(x => x.Name)
            .MaximumLength(500).WithMessage("Project name filter must not exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Project code filter must not exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Code));

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Project location filter must not exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Location));

        RuleFor(x => x.CompanyId)
            .GreaterThan(0).WithMessage("Company ID filter must be a positive number")
            .When(x => x.CompanyId.HasValue);

        RuleFor(x => x.SortOrder)
            .Must(s => new[] { "asc", "desc" }.Contains(s.ToLower()))
            .WithMessage("Sort order must be 'asc' or 'desc'");

        RuleFor(x => x.SortBy)
            .Must(s => new[] { "Name", "Code", "Location", "CreatedAt" }.Contains(s))
            .WithMessage("Sort by must be one of: Name, Code, Location, CreatedAt");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");
    }
}
