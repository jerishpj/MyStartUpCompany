using System.ComponentModel.DataAnnotations;

namespace MyStartUpCompany.Api.Configuration
{
    /// <summary>
    /// Configuration options for database connectivity and behavior
    /// </summary>
    public class DatabaseOptions
    {
        /// <summary>
        /// Configuration section name in appsettings.json
        /// </summary>
        public const string SectionName = "Database";

        /// <summary>
        /// Database connection string
        /// </summary>
        [Required(ErrorMessage = "ConnectionString is required")]
        public string ConnectionString { get; set; } = "Server=localhost;Database=MyStartUpCompany;";

        /// <summary>
        /// Database provider type (SqlServer, PostgreSQL, Sqlite, etc.)
        /// </summary>
        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; } = "SqlServer";

        /// <summary>
        /// Enable detailed SQL logging
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;

        /// <summary>
        /// Command timeout in seconds
        /// </summary>
        [Range(1, 300, ErrorMessage = "CommandTimeoutSeconds must be between 1 and 300")]
        public int CommandTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Enable query result caching
        /// </summary>
        public bool EnableQueryCache { get; set; } = true;

        /// <summary>
        /// Connection pool size for connection pooling
        /// </summary>
        [Range(1, 100, ErrorMessage = "ConnectionPoolSize must be between 1 and 100")]
        public int ConnectionPoolSize { get; set; } = 20;

        /// <summary>
        /// Auto-retry configuration for transient failures
        /// </summary>
        public DatabaseRetryPolicy RetryPolicy { get; set; } = new();

        /// <summary>
        /// Nested retry policy configuration
        /// </summary>
        public class DatabaseRetryPolicy
        {
            /// <summary>
            /// Enable automatic retries on transient failures
            /// </summary>
            public bool Enabled { get; set; } = true;

            /// <summary>
            /// Maximum number of retry attempts
            /// </summary>
            [Range(0, 5, ErrorMessage = "MaxRetries must be between 0 and 5")]
            public int MaxRetries { get; set; } = 3;

            /// <summary>
            /// Delay between retries in milliseconds
            /// </summary>
            [Range(100, 5000, ErrorMessage = "DelayMilliseconds must be between 100 and 5000")]
            public int DelayMilliseconds { get; set; } = 1000;
        }
    }
}
