using MyStartUpCompany.Worker.Handlers.AddCompany;

namespace MyStartUpCompany.Worker.Mappers.Sources
{
    /// <summary>
    /// Message model for Source B - represents the data structure from Source B system.
    /// Source B has a flatter structure than Source A, demonstrating different schema.
    /// </summary>
    public class SourceBMessage
    {
        /// <summary>
        /// Source B uses EntityName for the company name and includes entity type.
        /// </summary>
        public string? EntityName { get; set; }

        /// <summary>
        /// Source B includes an entity type identifier (e.g., "COMPANY").
        /// </summary>
        public string? EntityType { get; set; }

        /// <summary>
        /// Source B provides flat address components rather than nested objects.
        /// </summary>
        public string? Street { get; set; }
        public string? Municipality { get; set; }
        public string? Province { get; set; }
        public string? PostalArea { get; set; }
        public string? Locale { get; set; }

        /// <summary>
        /// Source B includes optional description.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Source B provides phone with optional extension.
        /// </summary>
        public string? Telephone { get; set; }
        public string? Extension { get; set; }
    }

    /// <summary>
    /// Mapper for Source B messages to CompanyInputDto.
    /// Demonstrates how different field names, flat structures, and optional fields are handled.
    /// </summary>
    public class SourceBMapper : IMessageMapper<object>
    {
        private readonly ILogger<SourceBMapper> _logger;

        public SourceBMapper(ILogger<SourceBMapper> logger)
        {
            _logger = logger;
        }

        public CompanyInputDto? Map(object sourceMessage)
        {
            try
            {
                var sourceB = sourceMessage as SourceBMessage;

                if (sourceB == null)
                {
                    _logger.LogWarning("Failed to cast message to SourceBMessage");
                    return null;
                }

                // Validate required fields from Source B
                if (string.IsNullOrWhiteSpace(sourceB.EntityName))
                {
                    _logger.LogWarning("Source B message missing required field: EntityName");
                    return null;
                }

                // Source B should have entity type "COMPANY"
                if (sourceB.EntityType != "COMPANY")
                {
                    _logger.LogWarning("Source B message has invalid EntityType: {EntityType}. Expected: COMPANY", sourceB.EntityType);
                    return null;
                }

                if (string.IsNullOrWhiteSpace(sourceB.Street))
                {
                    _logger.LogWarning("Source B message missing required field: Street");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(sourceB.Municipality))
                {
                    _logger.LogWarning("Source B message missing required field: Municipality");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(sourceB.PostalArea))
                {
                    _logger.LogWarning("Source B message missing required field: PostalArea");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(sourceB.Locale))
                {
                    _logger.LogWarning("Source B message missing required field: Locale");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(sourceB.Telephone))
                {
                    _logger.LogWarning("Source B message missing required field: Telephone");
                    return null;
                }

                // Build phone number with extension if provided
                var phone = sourceB.Telephone!.Trim();
                if (!string.IsNullOrWhiteSpace(sourceB.Extension))
                {
                    phone += $" ext. {sourceB.Extension.Trim()}";
                }

                // Map Source B fields to CompanyInputDto
                var companyDto = new CompanyInputDto
                {
                    Name = sourceB.EntityName!.Trim(),
                    Description = sourceB.Summary?.Trim(),
                    Address = sourceB.Street!.Trim(),
                    City = sourceB.Municipality!.Trim(),
                    Region = sourceB.Province?.Trim(),
                    PostalCode = sourceB.PostalArea!.Trim(),
                    Country = sourceB.Locale!.Trim(),
                    Phone = phone
                };

                _logger.LogDebug("Successfully mapped Source B message to CompanyInputDto: {CompanyName}", companyDto.Name);
                return companyDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping Source B message");
                return null;
            }
        }
    }
}
