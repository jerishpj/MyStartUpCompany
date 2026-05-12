namespace MyStartUpCompany.Api.Shared.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
