namespace MyStartUpCompany.Api.Shared.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource is not found
    /// </summary>
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
