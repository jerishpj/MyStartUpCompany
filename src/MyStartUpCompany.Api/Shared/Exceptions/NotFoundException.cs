using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Shared.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource is not found
    /// Excluded from code coverage as it is a simple exception wrapper with no business logic.
    /// Tested through integration tests where it's thrown and caught.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string resourceName, object key)
            : base($"{resourceName} with identifier '{key}' was not found.")
        {
        }
    }
}
