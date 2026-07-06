using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Shared.Exceptions
{
    /// <summary>
    /// Exception thrown when model validation fails.
    /// Excluded from code coverage as this is a simple exception wrapper with no business logic.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ValidationException : Exception
    {
        public ValidationException() : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IDictionary<string, string[]> errors) : this()
        {
            Errors = errors;
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}
