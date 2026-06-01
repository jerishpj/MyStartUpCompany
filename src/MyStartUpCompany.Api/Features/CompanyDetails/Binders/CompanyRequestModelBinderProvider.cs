using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails.Binders;

/// <summary>
/// Provider that registers the CompanyRequestModelBinder for automatic use.
/// </summary>
public class CompanyRequestModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(CompanyRequest))
        {
            return new CompanyRequestModelBinder();
        }

        return null;
    }
}
