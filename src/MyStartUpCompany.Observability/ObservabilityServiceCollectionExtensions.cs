using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Reflection;

namespace MyStartUpCompany.Observability;

/// <summary>
/// Extension methods for adding OpenTelemetry observability to services
/// </summary>
public static class ObservabilityServiceCollectionExtensions
{
    /// <summary>
    /// Add OpenTelemetry observability to the service collection
    /// </summary>
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var options = new ObservabilityOptions();
        configuration.GetSection(ObservabilityOptions.SectionName).Bind(options);

        if (!options.Enabled)
        {
            return services;
        }

        // Create resource with service identity
        var resource = CreateResource(options, environment);

        // Add tracing
        AddTracing(services, options, resource);

        // Add metrics
        AddMetrics(services, options, resource);

        // Add logging
        AddLogging(services, options, resource);

        // Store options for middleware access
        services.AddSingleton(options);

        return services;
    }

    /// <summary>
    /// Create OpenTelemetry resource with service metadata
    /// </summary>
    private static Resource CreateResource(ObservabilityOptions options, IHostEnvironment environment)
    {
        return ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: options.ServiceName,
                serviceVersion: options.ServiceVersion,
                serviceNamespace: options.ServiceNamespace,
                autoGenerateServiceInstanceId: true)
            .AddAttributes(new Dictionary<string, object>
            {
                { "deployment.environment", options.Environment },
                { "service.instance.id", Environment.MachineName },
                { "host.name", Environment.MachineName },
                { "process.pid", Environment.ProcessId }
            })
            .AddEnvironmentVariableDetector()
            .Build();
    }

    /// <summary>
    /// Add OpenTelemetry tracing
    /// </summary>
    private static void AddTracing(
        IServiceCollection services,
        ObservabilityOptions options,
        Resource resource)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.Clear().AddAttributes(resource.Attributes))
            .WithTracing(tracerProvider =>
            {
                tracerProvider
                    .AddSource("MyStartUpCompany*")  // Capture traces from our services
                    .AddSource("Azure.Messaging.ServiceBus")  // Azure Service Bus traces
                    .AddSource("Microsoft.EntityFrameworkCore")  // EF Core traces
                    .AddSource("System.Net.Http");  // HTTP client traces

                // Configure instrumentation
                if (options.Instrumentation.AspNetCore)
                {
                    tracerProvider
                        .AddAspNetCoreInstrumentation(config =>
                        {
                            config.RecordException = true;
                            config.EnrichWithHttpRequest = EnrichSpanWithHttpRequest;
                            config.EnrichWithHttpResponse = EnrichSpanWithHttpResponse;
                            config.EnrichWithException = EnrichSpanWithException;
                        });
                }

                if (options.Instrumentation.Http)
                {
                    tracerProvider
                        .AddHttpClientInstrumentation(config =>
                        {
                            config.RecordException = true;
                            config.EnrichWithHttpRequestMessage = EnrichSpanWithHttpRequestMessage;
                            config.EnrichWithHttpResponseMessage = EnrichSpanWithHttpResponseMessage;
                            config.EnrichWithException = EnrichSpanWithException;
                        });
                }

                if (options.Instrumentation.SqlClient)
                {
                    tracerProvider
                        .AddSqlClientInstrumentation(config =>
                        {
                            config.RecordException = true;
                        });
                }

                // Configure sampling
                tracerProvider.SetSampler(CreateSampler(options.Sampling));

                // Configure exporters
                if (options.Exporters.Otlp.Enabled)
                {
                    tracerProvider.AddOtlpExporter(config =>
                    {
                        config.Endpoint = new Uri(options.Exporters.Otlp.Endpoint);
                        config.Protocol = options.Exporters.Otlp.Protocol == "grpc"
                            ? OtlpExportProtocol.Grpc
                            : OtlpExportProtocol.HttpProtobuf;
                        config.ExportProcessorType = ExportProcessorType.Batch;
                        config.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
                        {
                            MaxExportBatchSize = options.BatchSize,
                            ScheduledDelayMilliseconds = options.ExportInterval,
                            MaxQueueSize = options.BatchSize * 10
                        };
                    });
                }

                if (options.Exporters.AzureMonitor.Enabled && !string.IsNullOrEmpty(options.Exporters.AzureMonitor.ConnectionString))
                {
                    // Note: Azure Monitor trace exporter requires Azure.Monitor.OpenTelemetry.AspNetCore package
                    // Uncomment if available
                    // tracerProvider.AddAzureMonitorTraceExporter(config =>
                    // {
                    //     config.ConnectionString = options.Exporters.AzureMonitor.ConnectionString;
                    // });
                }

                if (options.Exporters.Console.Enabled && options.Exporters.Console.Targets.HasFlag(ConsoleExporterType.Traces))
                {
                    tracerProvider.AddConsoleExporter();
                }
            });
    }

    /// <summary>
    /// Add OpenTelemetry metrics
    /// </summary>
    private static void AddMetrics(
        IServiceCollection services,
        ObservabilityOptions options,
        Resource resource)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddAttributes(resource.Attributes))
            .WithMetrics(meterProvider =>
            {
                meterProvider
                    .AddMeter("MyStartUpCompany*");  // Our custom metrics

                // Instrumentation
                if (options.Instrumentation.AspNetCore)
                {
                    meterProvider.AddAspNetCoreInstrumentation();
                }

                if (options.Instrumentation.Http)
                {
                    meterProvider.AddHttpClientInstrumentation();
                }

                if (options.Instrumentation.Runtime)
                {
                    meterProvider.AddRuntimeInstrumentation();
                }

                // Exporters
                if (options.Exporters.Otlp.Enabled)
                {
                    meterProvider.AddOtlpExporter(config =>
                    {
                        config.Endpoint = new Uri(options.Exporters.Otlp.Endpoint);
                        config.Protocol = options.Exporters.Otlp.Protocol == "grpc"
                            ? OtlpExportProtocol.Grpc
                            : OtlpExportProtocol.HttpProtobuf;
                    });
                }

                if (options.Exporters.AzureMonitor.Enabled && !string.IsNullOrEmpty(options.Exporters.AzureMonitor.ConnectionString))
                {
                    // Note: Azure Monitor metric exporter requires separate Azure.Monitor.OpenTelemetry.Exporter package
                    // Uncomment and install the package if Azure Monitor is needed for metrics
                    // meterProvider.AddAzureMonitorMetricExporter(config =>
                    // {
                    //     config.ConnectionString = options.Exporters.AzureMonitor.ConnectionString;
                    // });
                }

                if (options.Exporters.Console.Enabled && options.Exporters.Console.Targets.HasFlag(ConsoleExporterType.Metrics))
                {
                    meterProvider.AddConsoleExporter();
                }
            });
    }

    /// <summary>
    /// Add OpenTelemetry logging
    /// </summary>
    private static void AddLogging(
        IServiceCollection services,
        ObservabilityOptions options,
        Resource resource)
    {
        // Logging is typically configured through IHostBuilder.ConfigureLogging
        // This method can be called separately if needed
    }

    /// <summary>
    /// Create sampler based on configuration
    /// </summary>
    private static Sampler CreateSampler(SamplingOptions options)
    {
        // Use ratio-based sampling with configured default rate
        // Note: Custom samplers (ErrorActivitySampler, SlowActivitySampler) removed in OpenTelemetry 1.15.x
        // Consider implementing custom sampling logic using ActivityListener if needed
        return new TraceIdRatioBasedSampler(options.DefaultRate);
    }

    /// <summary>
    /// Enrich spans with HTTP request data
    /// </summary>
    private static void EnrichSpanWithHttpRequest(Activity activity, Microsoft.AspNetCore.Http.HttpRequest request)
    {
        activity?.SetTag("http.request_content_type", request.ContentType);
        activity?.SetTag("http.request_content_length", request.ContentLength);
    }

    /// <summary>
    /// Enrich spans with HTTP response data
    /// </summary>
    private static void EnrichSpanWithHttpResponse(Activity activity, Microsoft.AspNetCore.Http.HttpResponse response)
    {
        activity?.SetTag("http.response_content_type", response.ContentType);
        activity?.SetTag("http.response_content_length", response.ContentLength);
    }

    /// <summary>
    /// Enrich spans with exception data
    /// </summary>
    private static void EnrichSpanWithException(Activity activity, Exception exception)
    {
        activity?.SetTag("exception.type", exception.GetType().Name);
        activity?.SetTag("exception.stacktrace", exception.StackTrace);
    }

    /// <summary>
    /// Enrich spans with HTTP request message data
    /// </summary>
    private static void EnrichSpanWithHttpRequestMessage(Activity activity, HttpRequestMessage request)
    {
        activity?.SetTag("http.request_content_type", request.Content?.Headers.ContentType?.ToString());
        activity?.SetTag("http.request_content_length", request.Content?.Headers.ContentLength);
    }

    /// <summary>
    /// Enrich spans with HTTP response message data
    /// </summary>
    private static void EnrichSpanWithHttpResponseMessage(Activity activity, HttpResponseMessage response)
    {
        activity?.SetTag("http.response_content_type", response.Content?.Headers.ContentType?.ToString());
        activity?.SetTag("http.response_content_length", response.Content?.Headers.ContentLength);
    }
}
