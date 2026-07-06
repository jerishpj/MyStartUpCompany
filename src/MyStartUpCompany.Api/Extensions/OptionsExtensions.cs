using MyStartUpCompany.Api.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace MyStartUpCompany.Api.Extensions
{
    /// <summary>
    /// Extension methods for registering and configuring application options
    /// Excluded from code coverage as configuration registration is tested through integration tests.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class OptionsExtensions
    {
        /// <summary>
        /// Register all application configuration options with validation.
        /// Options are validated at startup to catch configuration errors early.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The application configuration</param>
        /// <returns>The service collection for method chaining</returns>
        /// <exception cref="ArgumentNullException">If services or configuration is null</exception>
        public static IServiceCollection AddApplicationOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Register ApiOptions
            // These options are validated at startup to catch errors immediately
            services.AddOptions<ApiOptions>()
                .BindConfiguration(ApiOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();  // ← Validates at startup, not when accessed

            // Register DatabaseOptions
            // Connection string and other DB settings are critical, validate immediately
            services.AddOptions<DatabaseOptions>()
                .BindConfiguration(DatabaseOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            // Register CacheOptions
            // Cache configuration is important for performance
            services.AddOptions<CacheOptions>()
                .BindConfiguration(CacheOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            // Register FeatureFlags
            // Feature flags are used for gradual rollout, validate but be lenient
            services.AddOptions<FeatureFlags>()
                .BindConfiguration(FeatureFlags.SectionName)
                .ValidateDataAnnotations();  // ← Don't validate on start for flags
                                            // They can be changed dynamically

            return services;
        }
    }
}
