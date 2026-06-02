using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Binders;

/// <summary>
/// Custom model binder for SearchCompanyRequest that silently defaults invalid pagination values.
/// If PageNumber or PageSize are not valid positive numbers, they are set to their defaults without validation errors.
/// </summary>
public class SearchCompanyRequestModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType != typeof(SearchCompanyRequest))
        {
            return Task.CompletedTask;
        }

        var request = new SearchCompanyRequest();
        var valueProvider = bindingContext.ValueProvider;

        // Extract and bind properties
        var region = GetStringValue(valueProvider, nameof(SearchCompanyRequest.Region));
        var country = GetStringValue(valueProvider, nameof(SearchCompanyRequest.Country));
        var city = GetStringValue(valueProvider, nameof(SearchCompanyRequest.City));
        var postalCode = GetStringValue(valueProvider, nameof(SearchCompanyRequest.PostalCode));
        var searchTerm = GetStringValue(valueProvider, nameof(SearchCompanyRequest.SearchTerm));

        // Extract and safely default PageNumber
        var pageNumber = GetIntValueOrDefault(valueProvider, nameof(SearchCompanyRequest.PageNumber), 1);

        // Extract and safely default PageSize
        var pageSize = GetIntValueOrDefault(valueProvider, nameof(SearchCompanyRequest.PageSize), 10);

        // Create the request with bound values
        request = new SearchCompanyRequest
        {
            Region = region,
            Country = country,
            City = city,
            PostalCode = postalCode,
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        bindingContext.Result = ModelBindingResult.Success(request);
        return Task.CompletedTask;
    }

    private static string? GetStringValue(IValueProvider valueProvider, string key)
    {
        var valueResult = valueProvider.GetValue(key);
        if (valueResult == ValueProviderResult.None)
        {
            return null;
        }

        var value = valueResult.FirstValue?.Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static int GetIntValueOrDefault(IValueProvider valueProvider, string key, int defaultValue)
    {
        var valueResult = valueProvider.GetValue(key);
        if (valueResult == ValueProviderResult.None)
        {
            return defaultValue;
        }

        var value = valueResult.FirstValue?.Trim();

        // If value is null, empty, or not a valid positive integer, return default
        if (string.IsNullOrWhiteSpace(value) || 
            !int.TryParse(value, out var intValue) || 
            intValue <= 0)
        {
            return defaultValue;
        }

        return intValue;
    }
}
