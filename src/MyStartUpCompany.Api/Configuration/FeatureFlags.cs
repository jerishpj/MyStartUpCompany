using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Configuration
{
    /// <summary>
    /// Feature flags for gradual feature rollout and experimentation
    /// Excluded from code coverage as it is a configuration model tested through services that consume it.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FeatureFlags
    {
        /// <summary>
        /// Configuration section name in appsettings.json
        /// </summary>
        public const string SectionName = "FeatureFlags";

        /// <summary>
        /// Enable advanced search features
        /// </summary>
        public bool EnableAdvancedSearch { get; set; } = false;

        /// <summary>
        /// Enable export functionality (CSV, Excel, etc.)
        /// </summary>
        public bool EnableExport { get; set; } = false;

        /// <summary>
        /// Enable batch operations on multiple records
        /// </summary>
        public bool EnableBatchOperations { get; set; } = false;

        /// <summary>
        /// Enable real-time notifications via WebSocket
        /// </summary>
        public bool EnableRealTimeNotifications { get; set; } = false;

        /// <summary>
        /// Enable rate limiting per user
        /// </summary>
        public bool EnableRateLimiting { get; set; } = false;

        /// <summary>
        /// Rate limit requests per minute per user (when enabled)
        /// </summary>
        public int RateLimitRequestsPerMinute { get; set; } = 100;

        /// <summary>
        /// Enable API versioning
        /// </summary>
        public bool EnableVersioning { get; set; } = true;

        /// <summary>
        /// Enable caching for GET requests
        /// </summary>
        public bool EnableResponseCaching { get; set; } = true;

        /// <summary>
        /// Enable pagination on all list endpoints
        /// </summary>
        public bool EnablePagination { get; set; } = true;

        /// <summary>
        /// Enable filtering on searchable endpoints
        /// </summary>
        public bool EnableFiltering { get; set; } = true;
    }
}
