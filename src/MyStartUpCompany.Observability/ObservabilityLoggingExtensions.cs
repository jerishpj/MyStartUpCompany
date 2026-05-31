using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace MyStartUpCompany.Observability;

/// <summary>
/// Extension methods for configuring OpenTelemetry logging
/// </summary>
public static class ObservabilityLoggingExtensions
{
    /// <summary>
    /// Add OpenTelemetry logging configuration
    /// </summary>
    public static IHostBuilder ConfigureObservabilityLogging(
        this IHostBuilder builder,
        Action<ObservabilityOptions>? configureOptions = null)
    {
        return builder.ConfigureLogging((context, logging) =>
        {
            var options = new ObservabilityOptions();
            context.Configuration.GetSection(ObservabilityOptions.SectionName).Bind(options);
            configureOptions?.Invoke(options);

            if (!options.Enabled)
            {
                return;
            }

            // Clear default logging providers
            logging.ClearProviders();

            // Add console logging for development
            if (context.HostingEnvironment.IsDevelopment())
            {
                logging.AddConsole();
            }

            // Add structured logging with correlation ID
            logging.AddOpenTelemetry(config =>
            {
                // Create resource
                var resource = ResourceBuilder.CreateDefault()
                    .AddService(
                        serviceName: options.ServiceName,
                        serviceVersion: options.ServiceVersion,
                        serviceNamespace: options.ServiceNamespace,
                        autoGenerateServiceInstanceId: true)
                    .AddEnvironmentVariableDetector()
                    .Build();

                config.IncludeScopes = true;
                config.IncludeFormattedMessage = true;

                // Add processors/exporters
                if (options.Exporters.Otlp.Enabled)
                {
                    config.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(options.Exporters.Otlp.Endpoint);
                        otlpOptions.Protocol = options.Exporters.Otlp.Protocol == "grpc"
                            ? OpenTelemetry.Exporter.OtlpExportProtocol.Grpc
                            : OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    });
                }

                if (options.Exporters.Console.Enabled && options.Exporters.Console.Targets.HasFlag(ConsoleExporterType.Logs))
                {
                    config.AddConsoleExporter();
                }
            });

            // Configure log levels based on environment
            if (context.HostingEnvironment.IsProduction())
            {
                logging.SetMinimumLevel(LogLevel.Warning);
            }
            else
            {
                logging.SetMinimumLevel(LogLevel.Information);
            }

            // Add filters for specific namespaces
            logging.AddFilter("Microsoft.AspNetCore.Mvc", LogLevel.Warning);
            logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            logging.AddFilter("Azure.Core", LogLevel.Warning);
        });
    }

    /// <summary>
    /// Create a logger factory with OpenTelemetry support
    /// </summary>
    public static ILoggerFactory CreateLoggerFactoryWithOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var loggingBuilder = LoggerFactory.Create(config =>
        {
            var options = new ObservabilityOptions();
            configuration.GetSection(ObservabilityOptions.SectionName).Bind(options);

            config.AddOpenTelemetry(otlConfig =>
            {
                var resource = ResourceBuilder.CreateDefault()
                    .AddService(
                        serviceName: options.ServiceName,
                        serviceVersion: options.ServiceVersion)
                    .Build();

                otlConfig.IncludeScopes = true;
                otlConfig.IncludeFormattedMessage = true;

                if (options.Exporters.Otlp.Enabled)
                {
                    otlConfig.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(options.Exporters.Otlp.Endpoint);
                    });
                }
            });

            config.AddConsole();

            if (environment.IsProduction())
            {
                config.SetMinimumLevel(LogLevel.Warning);
            }
        });

        return loggingBuilder;
    }
}

/// <summary>
/// Helper for adding correlation ID to log messages
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Log with correlation ID
    /// </summary>
    public static void LogWithCorrelation(
        this ILogger logger,
        LogLevel logLevel,
        string correlationId,
        string message,
        params object?[] args)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            { "CorrelationId", correlationId },
            { "TraceId", System.Diagnostics.Activity.Current?.Id ?? "unknown" }
        }))
        {
            logger.Log(logLevel, message, args);
        }
    }

    /// <summary>
    /// Log information with correlation ID
    /// </summary>
    public static void LogInformationWithCorrelation(
        this ILogger logger,
        string correlationId,
        string message,
        params object?[] args)
    {
        LogWithCorrelation(logger, LogLevel.Information, correlationId, message, args);
    }

    /// <summary>
    /// Log error with correlation ID
    /// </summary>
    public static void LogErrorWithCorrelation(
        this ILogger logger,
        string correlationId,
        Exception exception,
        string message,
        params object?[] args)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            { "CorrelationId", correlationId },
            { "TraceId", System.Diagnostics.Activity.Current?.Id ?? "unknown" },
            { "Exception", exception }
        }))
        {
            logger.LogError(exception, message, args);
        }
    }

    /// <summary>
    /// Log warning with correlation ID
    /// </summary>
    public static void LogWarningWithCorrelation(
        this ILogger logger,
        string correlationId,
        string message,
        params object?[] args)
    {
        LogWithCorrelation(logger, LogLevel.Warning, correlationId, message, args);
    }
}
