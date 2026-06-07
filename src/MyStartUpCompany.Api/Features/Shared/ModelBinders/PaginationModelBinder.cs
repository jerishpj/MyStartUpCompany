using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace MyStartUpCompany.Api.Features.Shared.ModelBinders;

/// <summary>
/// Generic model binder for pagination-aware request types.
/// Handles empty/null pagination parameters by applying safe defaults.
/// Also handles collection parameters like IEnumerable&lt;string&gt;.
/// </summary>
public class PaginationModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            return Task.CompletedTask;
        }

        var modelType = bindingContext.ModelType;
        var valueProvider = bindingContext.ValueProvider;
        var modelName = bindingContext.ModelName;

        try
        {
            // Get the primary constructor parameters in order
            var constructors = modelType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"No public constructor found for type {modelType.Name}");
            }

            // Use the constructor with the most parameters (primary constructor)
            var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();
            var ctorParams = constructor.GetParameters();

            var args = new object?[ctorParams.Length];

            for (int i = 0; i < ctorParams.Length; i++)
            {
                var param = ctorParams[i];
                var fieldName = param.Name!;

                // Check if parameter type is a collection (IEnumerable<T>)
                var isCollection = IsCollectionType(param.ParameterType);

                ValueProviderResult value;

                if (isCollection)
                {
                    // For collections, get all values matching the field name
                    value = valueProvider.GetValue(fieldName);

                    if (value == ValueProviderResult.None)
                    {
                        // No values provided for collection
                        args[i] = param.HasDefaultValue ? param.DefaultValue : null;
                        continue;
                    }

                    // Handle multiple values for the collection
                    var collectionValues = value.Values;
                    if (collectionValues.Count == 0)
                    {
                        args[i] = param.HasDefaultValue ? param.DefaultValue : null;
                        continue;
                    }

                    // Convert to IEnumerable<string> (or appropriate collection type)
                    var elementType = param.ParameterType.GetGenericArguments().FirstOrDefault() ?? typeof(string);
                    if (elementType == typeof(string))
                    {
                        // Return as IEnumerable<string>
                        args[i] = collectionValues.Where(v => !string.IsNullOrEmpty(v)).ToList();
                    }
                    else
                    {
                        args[i] = collectionValues;
                    }
                    continue;
                }

                value = valueProvider.GetValue(fieldName);

                if (value == ValueProviderResult.None)
                {
                    // Parameter not provided, use default
                    if (param.HasDefaultValue)
                    {
                        args[i] = param.DefaultValue;
                    }
                    else if (param.ParameterType.IsValueType)
                    {
                        args[i] = Activator.CreateInstance(param.ParameterType);
                    }
                    else
                    {
                        args[i] = null;
                    }
                    continue;
                }

                var stringValue = value.FirstValue?.Trim();

                // Handle PageNumber with empty string defaulting
                if (fieldName.Equals("PageNumber", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        args[i] = 1;
                    }
                    else if (int.TryParse(stringValue, out var pageNum) && pageNum > 0)
                    {
                        args[i] = pageNum;
                    }
                    else
                    {
                        args[i] = 1; // Default on parse failure
                    }
                    continue;
                }

                // Handle PageSize with empty string defaulting
                if (fieldName.Equals("PageSize", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        args[i] = 10;
                    }
                    else if (int.TryParse(stringValue, out var pageSize) && pageSize > 0)
                    {
                        args[i] = pageSize;
                    }
                    else
                    {
                        args[i] = 10; // Default on parse failure
                    }
                    continue;
                }

                // For string parameters, handle as-is (null or value)
                if (param.ParameterType == typeof(string))
                {
                    args[i] = string.IsNullOrEmpty(stringValue) ? null : stringValue;
                    continue;
                }

                // For other types, attempt to parse or use default
                args[i] = string.IsNullOrEmpty(stringValue) ? 
                    (param.HasDefaultValue ? param.DefaultValue : null) : 
                    stringValue;
            }

            var instance = constructor.Invoke(args);
            bindingContext.Result = ModelBindingResult.Success(instance);
        }
        catch (Exception ex)
        {
            bindingContext.ModelState.AddModelError(modelName, $"Error binding {modelName}: {ex.Message}");
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if a type is a collection type (IEnumerable&lt;T&gt;).
    /// </summary>
    private static bool IsCollectionType(Type type)
    {
        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();

            // Check for IEnumerable<T>, IList<T>, ICollection<T>, List<T>, etc.
            if (genericDef == typeof(IEnumerable<>) ||
                genericDef == typeof(IList<>) ||
                genericDef == typeof(ICollection<>) ||
                genericDef == typeof(List<>) ||
                genericDef == typeof(HashSet<>))
            {
                return true;
            }

            // Check if it implements IEnumerable<T>
            var interfaces = type.GetInterfaces();
            return interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        return false;
    }
}
