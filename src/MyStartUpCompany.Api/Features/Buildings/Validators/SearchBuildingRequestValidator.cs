using FluentValidation;
using MyStartUpCompany.Api.Features.Buildings.Models;
using MyStartUpCompany.Api.Shared.Constants;

namespace MyStartUpCompany.Api.Features.Buildings.Validators;

/// <summary>
/// Validator for SearchBuildingRequest search and filter parameters.
/// Ensures pagination parameters are within valid ranges and search fields are properly formatted.
/// </summary>
public class SearchBuildingRequestValidator : AbstractValidator<SearchBuildingRequest>
{
    public SearchBuildingRequestValidator()
    {
        // Page Number validation
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(ValidationConstants.Pagination.MinPageNumber)
            .WithMessage(ValidationConstants.Pagination.PageNumberErrorMessage)
            .LessThanOrEqualTo(ValidationConstants.Pagination.MaxPageNumber)
            .WithMessage(ValidationConstants.Pagination.PageNumberErrorMessage);

        // Page Size validation - CRITICAL FOR SECURITY
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(ValidationConstants.Pagination.MinPageSize)
            .WithMessage(ValidationConstants.Pagination.PageSizeErrorMessage)
            .LessThanOrEqualTo(ValidationConstants.Pagination.MaxPageSize)
            .WithMessage(ValidationConstants.Pagination.PageSizeErrorMessage);

        // Location ID validation
        RuleFor(x => x.LocationId)
            .GreaterThan(0)
            .WithMessage("Location ID must be greater than 0")
            .When(x => x.LocationId.HasValue);

        // Search term validation
        RuleFor(x => x.SearchTerm)
            .MaximumLength(ValidationConstants.Search.MaxSearchTermLength)
            .WithMessage($"Search term cannot exceed {ValidationConstants.Search.MaxSearchTermLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

        // Building code validation
        RuleFor(x => x.BuildingCode)
            .MaximumLength(50)
            .WithMessage("Building code cannot exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.BuildingCode));

        // Sort field validation
        RuleFor(x => x.SortBy)
            .Must(IsValidSortField)
            .WithMessage("Invalid sort field. Valid options: Name, Code, CreatedAt, UpdatedAt")
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));

        // Sort order validation
        RuleFor(x => x.SortOrder)
            .Must(x => x == "asc" || x == "desc" || string.IsNullOrEmpty(x))
            .WithMessage("Sort order must be 'asc' or 'desc'")
            .When(x => !string.IsNullOrWhiteSpace(x.SortOrder));
    }

    private static bool IsValidSortField(string? sortField)
    {
        if (string.IsNullOrWhiteSpace(sortField))
            return true;

        return sortField.ToLower() switch
        {
            "name" => true,
            "code" => true,
            "createdat" => true,
            "updatedat" => true,
            _ => false
        };
    }
}
