using MyStartUpCompany.Api.Configuration;
using Microsoft.Extensions.Options;

namespace MyStartUpCompany.Api.Services
{
    /// <summary>
    /// Example service demonstrating how to consume typed options
    /// This service shows both IOptions<T> (immutable) and IOptionsMonitor<T> (dynamic)
    /// </summary>
    public class ConfigurationExampleService
    {
        private readonly IOptions<ApiOptions> _apiOptions;
        private readonly IOptionsMonitor<FeatureFlags> _featureFlags;
        private readonly IOptions<CacheOptions> _cacheOptions;
        private readonly ILogger<ConfigurationExampleService> _logger;

        public ConfigurationExampleService(
            IOptions<ApiOptions> apiOptions,
            IOptionsMonitor<FeatureFlags> featureFlags,
            IOptions<CacheOptions> cacheOptions,
            ILogger<ConfigurationExampleService> logger)
        {
            _apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));
            _featureFlags = featureFlags ?? throw new ArgumentNullException(nameof(featureFlags));
            _cacheOptions = cacheOptions ?? throw new ArgumentNullException(nameof(cacheOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Example: Get paginated results with defaults from ApiOptions
        /// </summary>
        public (int PageNumber, int PageSize) GetPaginationDefaults(
            int? requestedPageNumber = null,
            int? requestedPageSize = null)
        {
            var apiOptions = _apiOptions.Value;

            // Use configured defaults if not provided
            var pageNumber = requestedPageNumber ?? 1;
            var pageSize = Math.Min(
                requestedPageSize ?? apiOptions.DefaultPageSize,
                apiOptions.MaxPageSize);

            _logger.LogInformation(
                "Using pagination defaults - Page: {Page}, Size: {Size}, Max: {Max}",
                pageNumber,
                pageSize,
                apiOptions.MaxPageSize);

            return (pageNumber, pageSize);
        }

        /// <summary>
        /// Example: Check if export feature is enabled (uses dynamic IOptionsMonitor)
        /// </summary>
        public bool CanExport()
        {
            // ✅ Using IOptionsMonitor allows feature flags to be checked dynamically
            // If the appsettings.json file changes, this method will see the new value
            var enabled = _featureFlags.CurrentValue.EnableExport;

            if (!enabled)
            {
                _logger.LogWarning("Export feature is currently disabled");
            }

            return enabled;
        }

        /// <summary>
        /// Example: Get cache configuration
        /// </summary>
        public CacheConfiguration GetCacheConfiguration()
        {
            var cacheOptions = _cacheOptions.Value;

            if (!cacheOptions.Enabled)
            {
                _logger.LogInformation("Caching is disabled");
                return new CacheConfiguration { IsEnabled = false };
            }

            return new CacheConfiguration
            {
                IsEnabled = true,
                Provider = cacheOptions.Provider,
                DurationSeconds = cacheOptions.DefaultDurationSeconds,
                KeyPrefix = cacheOptions.KeyPrefix,
                MaxSizeInMegabytes = cacheOptions.MaxSizeInMegabytes
            };
        }

        /// <summary>
        /// Example: Get multiple feature flags at once
        /// </summary>
        public FeatureFlagSummary GetEnabledFeatures()
        {
            var flags = _featureFlags.CurrentValue;

            var summary = new FeatureFlagSummary
            {
                AdvancedSearch = flags.EnableAdvancedSearch,
                Export = flags.EnableExport,
                BatchOperations = flags.EnableBatchOperations,
                RealTimeNotifications = flags.EnableRealTimeNotifications,
                RateLimiting = flags.EnableRateLimiting,
                Versioning = flags.EnableVersioning,
                ResponseCaching = flags.EnableResponseCaching,
                Pagination = flags.EnablePagination,
                Filtering = flags.EnableFiltering,
            };

            _logger.LogInformation(
                "Active features: {Features}",
                string.Join(", ", summary.GetType()
                    .GetProperties()
                    .Where(p => (bool)p.GetValue(summary)!)
                    .Select(p => p.Name)));

            return summary;
        }

        /// <summary>
        /// Example: Check if request compression is enabled
        /// </summary>
        public bool IsRequestCompressionEnabled()
        {
            return _apiOptions.Value.EnableRequestCompression;
        }

        /// <summary>
        /// Example: Get API documentation URL
        /// </summary>
        public string? GetApiDocumentationUrl()
        {
            var apiOptions = _apiOptions.Value;

            if (!apiOptions.EnableApiDocumentation)
                return null;

            return $"{apiOptions.BaseUrl}/api/docs";
        }

        /// <summary>
        /// Example: Get CORS configuration
        /// </summary>
        public CorsConfiguration GetCorsConfiguration()
        {
            var apiOptions = _apiOptions.Value;

            return new CorsConfiguration
            {
                Enabled = apiOptions.EnableCors,
                AllowedOrigins = apiOptions.AllowedCorsOrigins
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => o.Trim())
                    .ToList()
            };
        }
    }

    /// <summary>
    /// Represents cache configuration
    /// </summary>
    public class CacheConfiguration
    {
        public bool IsEnabled { get; set; }
        public string Provider { get; set; } = "Memory";
        public int DurationSeconds { get; set; } = 300;
        public string KeyPrefix { get; set; } = "api:";
        public int MaxSizeInMegabytes { get; set; } = 100;
    }

    /// <summary>
    /// Represents enabled feature flags
    /// </summary>
    public class FeatureFlagSummary
    {
        public bool AdvancedSearch { get; set; }
        public bool Export { get; set; }
        public bool BatchOperations { get; set; }
        public bool RealTimeNotifications { get; set; }
        public bool RateLimiting { get; set; }
        public bool Versioning { get; set; }
        public bool ResponseCaching { get; set; }
        public bool Pagination { get; set; }
        public bool Filtering { get; set; }
    }

    /// <summary>
    /// Represents CORS configuration
    /// </summary>
    public class CorsConfiguration
    {
        public bool Enabled { get; set; }
        public List<string> AllowedOrigins { get; set; } = new();
    }
}
