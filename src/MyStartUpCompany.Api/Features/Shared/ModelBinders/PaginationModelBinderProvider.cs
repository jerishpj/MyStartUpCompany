using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Features.Buildings.Models;
using MyStartUpCompany.Api.Features.Locations.Models;
using MyStartUpCompany.Api.Features.Offices.Models;

namespace MyStartUpCompany.Api.Features.Shared.ModelBinders;

/// <summary>
/// Model binder provider that registers custom pagination binders for request types.
/// Handles empty string pagination parameters by applying safe defaults before validation.
/// </summary>
public class PaginationModelBinderProvider : IModelBinderProvider
{
    private static readonly HashSet<Type> SupportedTypes = new()
    {
        typeof(SearchCompanyRequest),
        typeof(SearchBuildingRequest),
        typeof(SearchLocationRequest),
        typeof(SearchOfficeRequest),
    };

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            return null;
        }

        var modelType = context.Metadata.ModelType;

        // Only apply to supported pagination request types
        if (!SupportedTypes.Contains(modelType))
        {
            return null;
        }

        return new PaginationModelBinder();
    }
}
