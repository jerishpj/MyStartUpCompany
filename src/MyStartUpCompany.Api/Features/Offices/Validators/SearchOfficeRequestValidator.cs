using FluentValidation;
using MyStartUpCompany.Api.Features.Offices.Models;
using MyStartUpCompany.Api.Shared.Constants;

namespace MyStartUpCompany.Api.Features.Offices.Validators;

/// <summary>
/// Validator for SearchOfficeRequest search and filter parameters.
/// Ensures pagination parameters are within valid ranges and search fields are properly formatted.
/// </summary>
public class SearchOfficeRequestValidator : AbstractValidator<SearchOfficeRequest>
{
    /// <summary>
    /// Maximum length for string filter fields.
    /// </summary>
    private const int MaxStringFieldLength = 100;

    public SearchOfficeRequestValidator()
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

        // Building ID validation
        RuleFor(x => x.BuildingId)
            .GreaterThan(0)
            .WithMessage("Building ID must be greater than 0")
            .When(x => x.BuildingId.HasValue);

        // Search term validation
        RuleFor(x => x.SearchTerm)
            .MaximumLength(ValidationConstants.Search.MaxSearchTermLength)
            .WithMessage($"Search term cannot exceed {ValidationConstants.Search.MaxSearchTermLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

        // Department validation
        RuleFor(x => x.Department)
            .MaximumLength(MaxStringFieldLength)
            .WithMessage($"Department cannot exceed {MaxStringFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Department));

        // Office type validation
        RuleFor(x => x.OfficeType)
            .MaximumLength(MaxStringFieldLength)
            .WithMessage($"Office type cannot exceed {MaxStringFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.OfficeType));

        // Building name validation
        RuleFor(x => x.BuildingName)
            .MaximumLength(MaxStringFieldLength)
            .WithMessage($"Building name cannot exceed {MaxStringFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.BuildingName));

        // Location city validation
        RuleFor(x => x.LocationCity)
            .MaximumLength(MaxStringFieldLength)
            .WithMessage($"Location city cannot exceed {MaxStringFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.LocationCity));

        // Location region validation
        RuleFor(x => x.LocationRegion)
            .MaximumLength(MaxStringFieldLength)
            .WithMessage($"Location region cannot exceed {MaxStringFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.LocationRegion));

        // Location country validation
        RuleFor(x => x.LocationCountry)
            .MaximumLength(MaxStringFieldLength)
            .WithMessage($"Location country cannot exceed {MaxStringFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.LocationCountry));

        // Sort field validation
        RuleFor(x => x.SortBy)
            .Must(IsValidSortField)
            .WithMessage("Invalid sort field. Valid options: Name, Department, OfficeType, CreatedAt, UpdatedAt")
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
            "department" => true,
            "officetype" => true,
            "createdat" => true,
            "updatedat" => true,
            _ => false
        };
    }
}
