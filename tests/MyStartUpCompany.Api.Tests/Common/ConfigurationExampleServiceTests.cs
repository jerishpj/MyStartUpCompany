using MyStartUpCompany.Api.Configuration;
using MyStartUpCompany.Api.Services;
using Microsoft.Extensions.Options;

namespace MyStartUpCompany.Api.Tests.Common
{
    /// <summary>
    /// Unit tests demonstrating how to test services that consume typed options
    /// </summary>
    public class ConfigurationExampleServiceTests
    {
        private readonly ILogger<ConfigurationExampleService> _mockLogger;

        public ConfigurationExampleServiceTests()
        {
            _mockLogger = new Mock<ILogger<ConfigurationExampleService>>().Object;
        }

        #region Pagination Tests

        [Fact]
        public void GetPaginationDefaults_WithNoRequestValues_ShouldReturnConfiguredDefaults()
        {
            // Arrange
            var apiOptions = new ApiOptions
            {
                DefaultPageSize = 20,
                MaxPageSize = 100
            };
            var optionsMock = Options.Create(apiOptions);
            var featureFlagsMock = Options.Create(new FeatureFlags());
            var cacheOptionsMock = Options.Create(new CacheOptions());

            var service = new ConfigurationExampleService(
                optionsMock,
                new OptionsMonitorFactory(featureFlagsMock),
                cacheOptionsMock,
                _mockLogger);

            // Act
            var (pageNumber, pageSize) = service.GetPaginationDefaults();

            // Assert
            pageNumber.Should().Be(1);
            pageSize.Should().Be(20);
        }

        [Fact]
        public void GetPaginationDefaults_WithRequestedPageSize_ShouldNotExceedMaxPageSize()
        {
            // Arrange
            var apiOptions = new ApiOptions
            {
                DefaultPageSize = 20,
                MaxPageSize = 100
            };
            var optionsMock = Options.Create(apiOptions);
            var featureFlagsMock = Options.Create(new FeatureFlags());
            var cacheOptionsMock = Options.Create(new CacheOptions());

            var service = new ConfigurationExampleService(
                optionsMock,
                new OptionsMonitorFactory(featureFlagsMock),
                cacheOptionsMock,
                _mockLogger);

            // Act - Request page size larger than max
            var (pageNumber, pageSize) = service.GetPaginationDefaults(
                requestedPageNumber: 2,
                requestedPageSize: 500);

            // Assert
            pageNumber.Should().Be(2);
            pageSize.Should().Be(100);  // ← Capped at MaxPageSize
        }

        [Fact]
        public void GetPaginationDefaults_WithValidRequestedPageSize_ShouldUseRequestedValue()
        {
            // Arrange
            var apiOptions = new ApiOptions
            {
                DefaultPageSize = 20,
                MaxPageSize = 100
            };
            var optionsMock = Options.Create(apiOptions);
            var featureFlagsMock = Options.Create(new FeatureFlags());
            var cacheOptionsMock = Options.Create(new CacheOptions());

            var service = new ConfigurationExampleService(
                optionsMock,
                new OptionsMonitorFactory(featureFlagsMock),
                cacheOptionsMock,
                _mockLogger);

            // Act
            var (pageNumber, pageSize) = service.GetPaginationDefaults(
                requestedPageNumber: 3,
                requestedPageSize: 50);

            // Assert
            pageNumber.Should().Be(3);
            pageSize.Should().Be(50);
        }

        #endregion

        #region Feature Flag Tests

        [Fact]
        public void CanExport_WhenFeatureEnabled_ShouldReturnTrue()
        {
            // Arrange
            var apiOptions = Options.Create(new ApiOptions());
            var featureFlags = new FeatureFlags { EnableExport = true };
            var featureFlagsMock = new OptionsMonitorFactory(Options.Create(featureFlags));
            var cacheOptionsMock = Options.Create(new CacheOptions());

            var service = new ConfigurationExampleService(
                apiOptions,
                featureFlagsMock,
                cacheOptionsMock,
                _mockLogger);

            // Act
            var result = service.CanExport();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CanExport_WhenFeatureDisabled_ShouldReturnFalse()
        {
            // Arrange
            var apiOptions = Options.Create(new ApiOptions());
            var featureFlags = new FeatureFlags { EnableExport = false };
            var featureFlagsMock = new OptionsMonitorFactory(Options.Create(featureFlags));
            var cacheOptionsMock = Options.Create(new CacheOptions());

            var service = new ConfigurationExampleService(
                apiOptions,
                featureFlagsMock,
                cacheOptionsMock,
                _mockLogger);

            // Act
            var result = service.CanExport();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region Cache Configuration Tests

        [Fact]
        public void GetCacheConfiguration_WhenCachingEnabled_ShouldReturnConfiguration()
        {
            // Arrange
            var apiOptions = Options.Create(new ApiOptions());
            var featureFlagsMock = Options.Create(new FeatureFlags());
            var cacheOptions = new CacheOptions
            {
                Enabled = true,
                Provider = "Redis",
                DefaultDurationSeconds = 600,
                KeyPrefix = "myapp:",
                MaxSizeInMegabytes = 256
            };
            var cacheOptionsMock = Options.Create(cacheOptions);

            var service = new ConfigurationExampleService(
                apiOptions,
                new OptionsMonitorFactory(Options.Create(featureFlagsMock.Value)),
                cacheOptionsMock,
                _mockLogger);

            // Act
            var config = service.GetCacheConfiguration();

            // Assert
            config.IsEnabled.Should().BeTrue();
            config.Provider.Should().Be("Redis");
            config.DurationSeconds.Should().Be(600);
            config.KeyPrefix.Should().Be("myapp:");
            config.MaxSizeInMegabytes.Should().Be(256);
        }

        [Fact]
        public void GetCacheConfiguration_WhenCachingDisabled_ShouldReturnDisabledConfiguration()
        {
            // Arrange
            var apiOptions = Options.Create(new ApiOptions());
            var featureFlagsMock = Options.Create(new FeatureFlags());
            var cacheOptions = new CacheOptions { Enabled = false };
            var cacheOptionsMock = Options.Create(cacheOptions);

            var service = new ConfigurationExampleService(
                apiOptions,
                new OptionsMonitorFactory(Options.Create(featureFlagsMock.Value)),
                cacheOptionsMock,
                _mockLogger);

            // Act
            var config = service.GetCacheConfiguration();

            // Assert
            config.IsEnabled.Should().BeFalse();
        }

        #endregion

        #region CORS Configuration Tests

        [Fact]
        public void GetCorsConfiguration_ShouldParseCorsOrigins()
        {
            // Arrange
            var apiOptions = new ApiOptions
            {
                EnableCors = true,
                AllowedCorsOrigins = "http://localhost:3000, http://localhost:5173, https://example.com"
            };
            var apiOptionsMock = Options.Create(apiOptions);
            var featureFlagsMock = Options.Create(new FeatureFlags());
            var cacheOptionsMock = Options.Create(new CacheOptions());

            var service = new ConfigurationExampleService(
                apiOptionsMock,
                new OptionsMonitorFactory(Options.Create(featureFlagsMock.Value)),
                cacheOptionsMock,
                _mockLogger);

            // Act
            var corsConfig = service.GetCorsConfiguration();

            // Assert
            corsConfig.Enabled.Should().BeTrue();
            corsConfig.AllowedOrigins.Should().HaveCount(3);
            corsConfig.AllowedOrigins.Should().Contain("http://localhost:3000");
            corsConfig.AllowedOrigins.Should().Contain("http://localhost:5173");
            corsConfig.AllowedOrigins.Should().Contain("https://example.com");
        }

        #endregion

        #region API Documentation Tests

        [Fact]
        public void GetApiDocumentationUrl_WhenEnabled_ShouldReturnUrl()
        {
            // Arrange
            var apiOptions = new ApiOptions
            {
                BaseUrl = "https://api.example.com",
                EnableApiDocumentation = true
            };
            var apiOptionsMock = Options.Create(apiOptions);
            var featureFlagsMock = Options.Create(new FeatureFlags());
            var cacheOptionsMock = Options.Create(new CacheOptions());

            var service = new ConfigurationExampleService(
                apiOptionsMock,
                new OptionsMonitorFactory(Options.Create(featureFlagsMock.Value)),
                cacheOptionsMock,
                _mockLogger);

            // Act
            var url = service.GetApiDocumentationUrl();

            // Assert
            url.Should().Be("https://api.example.com/api/docs");
        }

        [Fact]
        public void GetApiDocumentationUrl_WhenDisabled_ShouldReturnNull()
        {
            // Arrange
            var apiOptions = new ApiOptions { EnableApiDocumentation = false };
            var apiOptionsMock = Options.Create(apiOptions);
            var featureFlagsMock = Options.Create(new FeatureFlags());
            var cacheOptionsMock = Options.Create(new CacheOptions());

            var service = new ConfigurationExampleService(
                apiOptionsMock,
                new OptionsMonitorFactory(Options.Create(featureFlagsMock.Value)),
                cacheOptionsMock,
                _mockLogger);

            // Act
            var url = service.GetApiDocumentationUrl();

            // Assert
            url.Should().BeNull();
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Test helper to create an IOptionsMonitor<T> from an IOptions<T>
        /// In production, this comes from the DI container
        /// </summary>
        private class OptionsMonitorFactory : IOptionsMonitor<FeatureFlags>
        {
            private readonly IOptions<FeatureFlags> _options;

            public OptionsMonitorFactory(IOptions<FeatureFlags> options)
            {
                _options = options ?? throw new ArgumentNullException(nameof(options));
            }

            public FeatureFlags CurrentValue => _options.Value;

            public FeatureFlags Get(string? name) => _options.Value;

            public IDisposable? OnChange(Action<FeatureFlags, string?> listener) =>
                null;  // ← In tests, we typically don't need dynamic reload notifications
        }

        #endregion
    }
}
