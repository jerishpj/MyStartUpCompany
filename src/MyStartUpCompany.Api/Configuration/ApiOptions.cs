using System.ComponentModel.DataAnnotations;

namespace MyStartUpCompany.Api.Configuration
{
    /// <summary>
    /// Configuration options for API-specific settings
    /// </summary>
    public class ApiOptions
    {
        /// <summary>
        /// Configuration section name in appsettings.json
        /// </summary>
        public const string SectionName = "Api";

        /// <summary>
        /// Base URL for the API (used in links, response headers, etc.)
        /// </summary>
        [Required(ErrorMessage = "BaseUrl is required")]
        [Url(ErrorMessage = "BaseUrl must be a valid URL")]
        public string BaseUrl { get; set; } = "https://api.mystartupcorp.com";

        /// <summary>
        /// API version for versioning and documentation
        /// </summary>
        [Required(ErrorMessage = "Version is required")]
        public string Version { get; set; } = "v1";

        /// <summary>
        /// Maximum number of items returned in paginated responses
        /// </summary>
        [Range(1, 1000, ErrorMessage = "MaxPageSize must be between 1 and 1000")]
        public int MaxPageSize { get; set; } = 100;

        /// <summary>
        /// Default page size for paginated responses
        /// </summary>
        [Range(1, 1000, ErrorMessage = "DefaultPageSize must be between 1 and 1000")]
        public int DefaultPageSize { get; set; } = 20;

        /// <summary>
        /// Enable detailed error responses (only in development)
        /// </summary>
        public bool IncludeExceptionDetails { get; set; } = false;

        /// <summary>
        /// Request timeout in seconds
        /// </summary>
        [Range(1, 300, ErrorMessage = "RequestTimeoutSeconds must be between 1 and 300")]
        public int RequestTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Enable OpenAPI/Swagger documentation
        /// </summary>
        public bool EnableApiDocumentation { get; set; } = true;

        /// <summary>
        /// Enable CORS
        /// </summary>
        public bool EnableCors { get; set; } = true;

        /// <summary>
        /// Allowed CORS origins (comma-separated)
        /// </summary>
        public string AllowedCorsOrigins { get; set; } = "*";

        /// <summary>
        /// Enable request compression
        /// </summary>
        public bool EnableRequestCompression { get; set; } = true;

        /// <summary>
        /// Enable response compression
        /// </summary>
        public bool EnableResponseCompression { get; set; } = true;
    }
}
