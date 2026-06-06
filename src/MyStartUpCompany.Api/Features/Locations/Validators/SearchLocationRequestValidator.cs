using FluentValidation;
using MyStartUpCompany.Api.Features.Locations.Models;
using MyStartUpCompany.Api.Shared.Constants;

namespace MyStartUpCompany.Api.Features.Locations.Validators;

/// <summary>
/// Validator for SearchLocationRequest search and filter parameters.
/// Ensures pagination parameters are within valid ranges and search fields are properly formatted.
/// </summary>
public class SearchLocationRequestValidator : AbstractValidator<SearchLocationRequest>
{
    /// <summary>
    /// Maximum length for location fields (city, country, region).
    /// </summary>
    private const int MaxLocationFieldLength = 100;

    public SearchLocationRequestValidator()
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

        // Company ID validation
        RuleFor(x => x.CompanyId)
            .GreaterThan(0)
            .WithMessage("Company ID must be greater than 0")
            .When(x => x.CompanyId.HasValue);

        // Search term validation
        RuleFor(x => x.SearchTerm)
            .MaximumLength(ValidationConstants.Search.MaxSearchTermLength)
            .WithMessage($"Search term cannot exceed {ValidationConstants.Search.MaxSearchTermLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

        // Country validation
        RuleFor(x => x.Country)
            .MaximumLength(MaxLocationFieldLength)
            .WithMessage($"Country cannot exceed {MaxLocationFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Country));

        // City validation
        RuleFor(x => x.City)
            .MaximumLength(MaxLocationFieldLength)
            .WithMessage($"City cannot exceed {MaxLocationFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        // Sort field validation
        RuleFor(x => x.SortBy)
            .Must(IsValidSortField)
            .WithMessage("Invalid sort field. Valid options: Name, City, Country, CreatedAt, UpdatedAt")
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
            "city" => true,
            "country" => true,
            "createdat" => true,
            "updatedat" => true,
            _ => false
        };
    }
}
