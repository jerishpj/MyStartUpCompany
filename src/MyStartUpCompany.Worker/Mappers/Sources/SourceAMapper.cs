using MyStartUpCompany.Worker.Handlers.AddCompany;

namespace MyStartUpCompany.Worker.Mappers.Sources
{
    /// <summary>
    /// Message model for Source A - represents the data structure from Source A system.
    /// Each source can have a completely different schema.
    /// </summary>
    public class SourceAMessage
    {
        /// <summary>
        /// Source A uses CompanyTitle for the company name.
        /// </summary>
        public string? CompanyTitle { get; set; }

        /// <summary>
        /// Source A includes CompanyInfo as a structured object.
        /// </summary>
        public SourceACompanyInfo? CompanyInfo { get; set; }

        /// <summary>
        /// Source A provides contact information separately.
        /// </summary>
        public SourceAContact? Contact { get; set; }
    }

    public class SourceACompanyInfo
    {
        public string? Description { get; set; }
        public string? StreetAddress { get; set; }
        public string? CityName { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? CountryName { get; set; }
    }

    public class SourceAContact
    {
        public string? PhoneNumber { get; set; }
    }

    /// <summary>
    /// Mapper for Source A messages to CompanyInputDto.
    /// Demonstrates how different field names and nested structures are handled.
    /// </summary>
    public class SourceAMapper : IMessageMapper<object>
    {
        private readonly ILogger<SourceAMapper> _logger;

        public SourceAMapper(ILogger<SourceAMapper> logger)
        {
            _logger = logger;
        }

        public CompanyInputDto? Map(object sourceMessage)
        {
            try
            {
                // Deserialize if needed (if coming as JSON)
                var sourceA = sourceMessage as SourceAMessage;

                if (sourceA == null)
                {
                    _logger.LogWarning("Failed to cast message to SourceAMessage");
                    return null;
                }

                // Validate required fields from Source A
                if (string.IsNullOrWhiteSpace(sourceA.CompanyTitle))
                {
                    _logger.LogWarning("Source A message missing required field: CompanyTitle");
                    return null;
                }

                var companyInfo = sourceA.CompanyInfo;
                if (companyInfo == null)
                {
                    _logger.LogWarning("Source A message missing required field: CompanyInfo");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(companyInfo.StreetAddress))
                {
                    _logger.LogWarning("Source A message missing required field: StreetAddress");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(companyInfo.CityName))
                {
                    _logger.LogWarning("Source A message missing required field: CityName");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(companyInfo.ZipCode))
                {
                    _logger.LogWarning("Source A message missing required field: ZipCode");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(companyInfo.CountryName))
                {
                    _logger.LogWarning("Source A message missing required field: CountryName");
                    return null;
                }

                if (sourceA.Contact == null || string.IsNullOrWhiteSpace(sourceA.Contact.PhoneNumber))
                {
                    _logger.LogWarning("Source A message missing required field: PhoneNumber");
                    return null;
                }

                // Map Source A fields to CompanyInputDto
                var companyDto = new CompanyInputDto
                {
                    Name = sourceA.CompanyTitle!.Trim(),
                    Description = companyInfo.Description?.Trim(),
                    Address = companyInfo.StreetAddress!.Trim(),
                    City = companyInfo.CityName!.Trim(),
                    Region = companyInfo.State?.Trim(),
                    PostalCode = companyInfo.ZipCode!.Trim(),
                    Country = companyInfo.CountryName!.Trim(),
                    Phone = sourceA.Contact.PhoneNumber!.Trim()
                };

                _logger.LogDebug("Successfully mapped Source A message to CompanyInputDto: {CompanyName}", companyDto.Name);
                return companyDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping Source A message");
                return null;
            }
        }
    }
}
