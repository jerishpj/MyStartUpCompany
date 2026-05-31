using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace MyStartUpCompany.Observability.Tests.Integration;

/// <summary>
/// Integration tests for OpenTelemetry service registration and configuration.
/// </summary>
public class ObservabilityServiceCollectionTests
{
    private static IConfiguration CreateConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Observability:Enabled", "true" },
                { "Observability:ServiceName", "TestService" },
                { "Observability:ServiceVersion", "1.0.0" },
                { "Observability:Environment", "test" },
            });
        return configBuilder.Build();
    }

    private static IHostEnvironment CreateHostEnvironment()
    {
        var environment = new Moq.Mock<IHostEnvironment>();
        environment.Setup(e => e.EnvironmentName).Returns("test");
        return environment.Object;
    }

    [Fact]
    public void AddObservability_ShouldRegisterTracerProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var environment = CreateHostEnvironment();

        // Act
        services.AddObservability(configuration, environment);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tracerProvider = serviceProvider.GetService<TracerProvider>();
        tracerProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddObservability_ShouldRegisterMeterProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();
        var environment = CreateHostEnvironment();

        // Act
        services.AddObservability(configuration, environment);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var meterProvider = serviceProvider.GetService<MeterProvider>();
        meterProvider.Should().NotBeNull();
    }

    [Fact]
    public void AddObservability_WithDisabledFlag_ShouldNotRegisterProviders()
    {
        // Arrange
        var services = new ServiceCollection();
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Observability:Enabled", "false" },
            });
        var configuration = configBuilder.Build();
        var environment = CreateHostEnvironment();

        // Act
        services.AddObservability(configuration, environment);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tracerProvider = serviceProvider.GetService<TracerProvider>();
        tracerProvider.Should().BeNull();
    }

    [Fact]
    public void AddObservability_ShouldConfigureCorrectServiceName()
    {
        // Arrange
        var services = new ServiceCollection();
        var expectedServiceName = "MyTestService";
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Observability:Enabled", "true" },
                { "Observability:ServiceName", expectedServiceName },
                { "Observability:ServiceVersion", "2.0.0" },
                { "Observability:Environment", "test" },
            });
        var configuration = configBuilder.Build();
        var environment = CreateHostEnvironment();

        // Act
        services.AddObservability(configuration, environment);

        var serviceProvider = services.BuildServiceProvider();

        // Assert - verify that configuration was applied
        var options = serviceProvider.GetService<ObservabilityOptions>();
        options.Should().NotBeNull();
        options!.ServiceName.Should().Be(expectedServiceName);
    }

    [Fact]
    public void AddObservability_ShouldEnableAllInstrumentation_WhenConfigured()
    {
        // Arrange
        var services = new ServiceCollection();
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Observability:Enabled", "true" },
                { "Observability:ServiceName", "TestService" },
                { "Observability:ServiceVersion", "1.0.0" },
                { "Observability:Environment", "test" },
                { "Observability:Instrumentation:AspNetCore", "true" },
                { "Observability:Instrumentation:Http", "true" },
                { "Observability:Instrumentation:SqlClient", "true" },
                { "Observability:Instrumentation:Runtime", "true" },
            });
        var configuration = configBuilder.Build();
        var environment = CreateHostEnvironment();

        // Act
        services.AddObservability(configuration, environment);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<ObservabilityOptions>();
        options.Should().NotBeNull();
        options!.Instrumentation.AspNetCore.Should().BeTrue();
        options.Instrumentation.Http.Should().BeTrue();
        options.Instrumentation.SqlClient.Should().BeTrue();
        options.Instrumentation.Runtime.Should().BeTrue();
    }

    [Fact]
    public void AddObservability_ShouldApplySamplingConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var expectedSamplingRate = 0.5;
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Observability:Enabled", "true" },
                { "Observability:ServiceName", "TestService" },
                { "Observability:ServiceVersion", "1.0.0" },
                { "Observability:Environment", "test" },
                { "Observability:SamplingRate", expectedSamplingRate.ToString() },
            });
        var configuration = configBuilder.Build();
        var environment = CreateHostEnvironment();

        // Act
        services.AddObservability(configuration, environment);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<ObservabilityOptions>();
        options.Should().NotBeNull();
        options!.SamplingRate.Should().Be(expectedSamplingRate);
    }
}
