using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Binders;

/// <summary>
/// Provider that registers the SearchCompanyRequestModelBinder for automatic use.
/// </summary>
public class SearchCompanyRequestModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(SearchCompanyRequest))
        {
            return new SearchCompanyRequestModelBinder();
        }

        return null;
    }
}
