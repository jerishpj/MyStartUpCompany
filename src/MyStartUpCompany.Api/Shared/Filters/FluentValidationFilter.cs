using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using ValidationException = MyStartUpCompany.Api.Shared.Exceptions.ValidationException;

namespace MyStartUpCompany.Api.Shared.Filters;

/// <summary>
/// Action filter that validates model state using FluentValidation validators.
/// Automatically validates models and throws ValidationException if validation fails.
/// </summary>
public class FluentValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public FluentValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value!.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            throw new ValidationException(errors);
        }

        // Validate using FluentValidation if a validator exists
        foreach (var (key, value) in context.ActionArguments)
        {
            if (value is null)
                continue;

            var valueType = value.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(valueType);

            if (context.HttpContext.RequestServices.GetService(validatorType) is IValidator validator)
            {
                var validationContext = new ValidationContext<object>(value);
                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    var validationErrors = result.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(e => e.ErrorMessage).ToArray()
                        );

                    throw new ValidationException(validationErrors);
                }
            }
        }

        await next();
    }
}
