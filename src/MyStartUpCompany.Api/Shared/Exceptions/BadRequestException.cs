using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Shared.Exceptions
{
    /// <summary>
    /// Exception thrown when a request contains invalid data
    /// Excluded from code coverage as it is a simple exception wrapper with no business logic.
    /// Tested through integration tests where it's thrown and caught.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
