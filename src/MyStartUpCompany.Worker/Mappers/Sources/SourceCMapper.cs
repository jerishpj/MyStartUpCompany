using MyStartUpCompany.Worker.Handlers.AddCompany;

namespace MyStartUpCompany.Worker.Mappers.Sources
{
    /// <summary>
    /// Message model for Source C - represents the data structure from Source C system.
    /// Source C uses different property names than SourceA and SourceB.
    /// 
    /// Example mapping:
    /// SourceC Property        →  Database Property
    /// ────────────────────────────────────────────
    /// BizName                 →  Name
    /// BizAddress              →  Address
    /// BizCity                 →  City
    /// </summary>
    public class SourceCMessage
    {
        public string? BizName { get; set; }
        public string? BizDescription { get; set; }
        public string? BizAddress { get; set; }
        public string? BizCity { get; set; }
        public string? BizProvince { get; set; }
        public string? BizZip { get; set; }
        public string? BizCountry { get; set; }
        public string? BizPhone { get; set; }
    }

    /// <summary>
    /// Mapper for Source C messages to CompanyInputDto.
    /// Handles the specific property name transformations for Source C format.
    /// 
    /// This is a perfect example of how new sources are added with MINIMAL changes
    /// to the core system. Just create this mapper and register it - no core logic changes!
    /// </summary>
    public class SourceCMapper : IMessageMapper<object>
    {
        private readonly ILogger<SourceCMapper> _logger;

        public SourceCMapper(ILogger<SourceCMapper> logger)
        {
            _logger = logger;
        }

        public CompanyInputDto? Map(object sourceMessage)
        {
            try
            {
                var sourceC = sourceMessage as SourceCMessage;

                if (sourceC == null)
                {
                    _logger.LogWarning("Failed to cast message to SourceCMessage");
                    return null;
                }

                // Validate required fields from Source C
                if (string.IsNullOrWhiteSpace(sourceC.BizName))
                {
                    _logger.LogWarning("Source C message missing required field: BizName");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(sourceC.BizAddress))
                {
                    _logger.LogWarning("Source C message missing required field: BizAddress");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(sourceC.BizCity))
                {
                    _logger.LogWarning("Source C message missing required field: BizCity");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(sourceC.BizZip))
                {
                    _logger.LogWarning("Source C message missing required field: BizZip");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(sourceC.BizCountry))
                {
                    _logger.LogWarning("Source C message missing required field: BizCountry");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(sourceC.BizPhone))
                {
                    _logger.LogWarning("Source C message missing required field: BizPhone");
                    return null;
                }

                // ✅ PROPERTY MAPPING: Source C → Database Schema
                var companyDto = new CompanyInputDto
                {
                    Name = sourceC.BizName!.Trim(),                    // BizName → Name
                    Description = sourceC.BizDescription?.Trim(),     // BizDescription → Description
                    Address = sourceC.BizAddress!.Trim(),             // BizAddress → Address
                    City = sourceC.BizCity!.Trim(),                   // BizCity → City
                    Region = sourceC.BizProvince?.Trim(),             // BizProvince → Region
                    PostalCode = sourceC.BizZip!.Trim(),              // BizZip → PostalCode
                    Country = sourceC.BizCountry!.Trim(),             // BizCountry → Country
                    Phone = sourceC.BizPhone!.Trim()                  // BizPhone → Phone
                };

                _logger.LogDebug("Successfully mapped Source C message to CompanyInputDto: {CompanyName}", companyDto.Name);
                return companyDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping Source C message");
                return null;
            }
        }
    }
}
