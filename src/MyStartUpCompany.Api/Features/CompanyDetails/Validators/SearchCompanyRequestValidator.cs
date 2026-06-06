using FluentValidation;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Shared.Constants;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Validators;

/// <summary>
/// Validator for SearchCompanyRequest search and filter parameters.
/// Ensures pagination parameters are within valid ranges and search fields are properly formatted.
/// </summary>
public class SearchCompanyRequestValidator : AbstractValidator<SearchCompanyRequest>
{
    /// <summary>
    /// Maximum length for location fields (city, country, region).
    /// </summary>
    private const int MaxLocationFieldLength = 100;

    public SearchCompanyRequestValidator()
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

        // Region validation - optional but if provided, validate format
        RuleFor(x => x.Region)
            .MaximumLength(MaxLocationFieldLength)
            .WithMessage($"Region cannot exceed {MaxLocationFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Region))
            .DependentRules(() =>
            {
                RuleFor(x => x.Region)
                    .Matches(@"^[a-zA-Z\s\-]+$")
                    .WithMessage("Region can only contain letters, spaces, and hyphens.")
                    .When(x => !string.IsNullOrWhiteSpace(x.Region));
            });

        // Country validation - optional but if provided, validate format
        RuleFor(x => x.Country)
            .MaximumLength(MaxLocationFieldLength)
            .WithMessage($"Country cannot exceed {MaxLocationFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Country))
            .DependentRules(() =>
            {
                RuleFor(x => x.Country)
                    .Matches(@"^[a-zA-Z\s\-]+$")
                    .WithMessage("Country can only contain letters, spaces, and hyphens.")
                    .When(x => !string.IsNullOrWhiteSpace(x.Country));
            });

        // City validation - optional but if provided, validate format
        RuleFor(x => x.City)
            .MaximumLength(MaxLocationFieldLength)
            .WithMessage($"City cannot exceed {MaxLocationFieldLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.City))
            .DependentRules(() =>
            {
                RuleFor(x => x.City)
                    .Matches(@"^[a-zA-Z\s\-]+$")
                    .WithMessage("City can only contain letters, spaces, and hyphens.")
                    .When(x => !string.IsNullOrWhiteSpace(x.City));
            });

        // Postal Code validation - optional but if provided, validate format
        RuleFor(x => x.PostalCode)
            .MaximumLength(20)
            .WithMessage("Postal code cannot exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode))
            .DependentRules(() =>
            {
                RuleFor(x => x.PostalCode)
                    .Matches(@"^[a-zA-Z0-9\s\-]+$")
                    .WithMessage("Postal code can only contain alphanumeric characters, spaces, and hyphens.")
                    .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));
            });

        // Search Term validation - optional but if provided, validate length
        RuleFor(x => x.SearchTerm)
            .MaximumLength(ValidationConstants.Search.MaxSearchTermLength)
            .WithMessage($"Search term cannot exceed {ValidationConstants.Search.MaxSearchTermLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));
    }
}
