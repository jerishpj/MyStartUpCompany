using FluentValidation;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Validators;

/// <summary>
/// Validator for SearchCompanyRequest search and filter parameters.
/// Ensures pagination parameters are within valid ranges and search fields are properly formatted.
/// </summary>
public class SearchCompanyRequestValidator : AbstractValidator<SearchCompanyRequest>
{
    /// <summary>
    /// Pagination and search field constraints.
    /// </summary>
    private const int MinPageNumber = 1;
    private const int MaxPageNumber = 10000;
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;
    private const int MaxStringLength = 100;

    public SearchCompanyRequestValidator()
    {
        // Page Number validation
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(MinPageNumber)
            .WithMessage($"Page number must be at least {MinPageNumber}.")
            .LessThanOrEqualTo(MaxPageNumber)
            .WithMessage($"Page number cannot exceed {MaxPageNumber}.");

        // Page Size validation
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(MinPageSize)
            .WithMessage($"Page size must be at least {MinPageSize}.")
            .LessThanOrEqualTo(MaxPageSize)
            .WithMessage($"Page size cannot exceed {MaxPageSize}.");

        // Region validation - optional but if provided, validate format
        RuleFor(x => x.Region)
            .MaximumLength(MaxStringLength)
            .WithMessage($"Region cannot exceed {MaxStringLength} characters.")
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
            .MaximumLength(MaxStringLength)
            .WithMessage($"Country cannot exceed {MaxStringLength} characters.")
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
            .MaximumLength(MaxStringLength)
            .WithMessage($"City cannot exceed {MaxStringLength} characters.")
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
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));
    }
}
