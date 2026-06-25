using System.ComponentModel.DataAnnotations;

namespace MyStartUpCompany.Api.Configuration
{
    /// <summary>
    /// Configuration options for caching behavior and storage
    /// </summary>
    public class CacheOptions
    {
        /// <summary>
        /// Configuration section name in appsettings.json
        /// </summary>
        public const string SectionName = "Cache";

        /// <summary>
        /// Enable caching entirely
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Cache provider type (Memory, Redis, Distributed)
        /// </summary>
        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; } = "Memory";

        /// <summary>
        /// Default cache duration in seconds
        /// </summary>
        [Range(0, 3600, ErrorMessage = "DefaultDurationSeconds must be between 0 and 3600")]
        public int DefaultDurationSeconds { get; set; } = 300;

        /// <summary>
        /// Redis connection string (used if Provider is Redis)
        /// </summary>
        public string? RedisConnectionString { get; set; }

        /// <summary>
        /// Cache key prefix for namespace separation
        /// </summary>
        public string KeyPrefix { get; set; } = "api:";

        /// <summary>
        /// Maximum cache size in MB (for memory cache)
        /// </summary>
        [Range(1, 1000, ErrorMessage = "MaxSizeInMegabytes must be between 1 and 1000")]
        public int MaxSizeInMegabytes { get; set; } = 100;

        /// <summary>
        /// Enable compression for cached values
        /// </summary>
        public bool EnableCompression { get; set; } = false;
    }
}
