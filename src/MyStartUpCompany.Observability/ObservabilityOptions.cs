namespace MyStartUpCompany.Observability;

/// <summary>
/// Configuration options for OpenTelemetry observability
/// </summary>
public class ObservabilityOptions
{
    /// <summary>
    /// Section name in appsettings.json
    /// </summary>
    public const string SectionName = "Observability";

    /// <summary>
    /// Enable or disable observability entirely
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Service name for identification in telemetry
    /// </summary>
    public string ServiceName { get; set; } = "MyStartUpCompany";

    /// <summary>
    /// Service version for tracking deployments
    /// </summary>
    public string ServiceVersion { get; set; } = "1.0.0";

    /// <summary>
    /// Environment name (development, staging, production)
    /// </summary>
    public string Environment { get; set; } = "production";

    /// <summary>
    /// Namespace for resource attribution
    /// </summary>
    public string ServiceNamespace { get; set; } = "mystartupcorp";

    /// <summary>
    /// Trace sampling rate (0.0 to 1.0)
    /// 0.1 = 10% sampling, 1.0 = 100% sampling
    /// </summary>
    public double SamplingRate { get; set; } = 0.1;

    /// <summary>
    /// Batch size for exporting spans
    /// Larger batches reduce export overhead but increase memory
    /// </summary>
    public int BatchSize { get; set; } = 1024;

    /// <summary>
    /// Export interval in milliseconds
    /// How often to export accumulated spans/metrics
    /// </summary>
    public int ExportInterval { get; set; } = 30000; // 30 seconds

    /// <summary>
    /// Exporter-specific configuration
    /// </summary>
    public ExporterOptions Exporters { get; set; } = new();

    /// <summary>
    /// Instrumentation settings
    /// </summary>
    public InstrumentationOptions Instrumentation { get; set; } = new();

    /// <summary>
    /// Sampling strategy settings
    /// </summary>
    public SamplingOptions Sampling { get; set; } = new();
}

/// <summary>
/// Exporter configuration options
/// </summary>
public class ExporterOptions
{
    /// <summary>
    /// OTLP exporter settings
    /// </summary>
    public OtlpExporterOptions Otlp { get; set; } = new();

    /// <summary>
    /// Azure Monitor exporter settings
    /// </summary>
    public AzureMonitorExporterOptions AzureMonitor { get; set; } = new();

    /// <summary>
    /// Console exporter (development only)
    /// </summary>
    public ConsoleExporterOptions Console { get; set; } = new();
}

/// <summary>
/// OTLP (OpenTelemetry Protocol) exporter configuration
/// </summary>
public class OtlpExporterOptions
{
    /// <summary>
    /// Enable OTLP exporter
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// OTLP collector endpoint (e.g., http://otel-collector:4317)
    /// </summary>
    public string Endpoint { get; set; } = "http://otel-collector:4317";

    /// <summary>
    /// Protocol: grpc or http/protobuf
    /// </summary>
    public string Protocol { get; set; } = "grpc";

    /// <summary>
    /// Export timeout in milliseconds
    /// </summary>
    public int ExportTimeout { get; set; } = 30000;

    /// <summary>
    /// Maximum number of concurrent exports
    /// </summary>
    public int MaxConcurrentRequests { get; set; } = 10;
}

/// <summary>
/// Azure Monitor exporter configuration
/// </summary>
public class AzureMonitorExporterOptions
{
    /// <summary>
    /// Enable Azure Monitor exporter
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Application Insights connection string
    /// </summary>
    public string ConnectionString { get; set; } = "";

    /// <summary>
    /// Export timeout in milliseconds
    /// </summary>
    public int ExportTimeout { get; set; } = 30000;
}

/// <summary>
/// Console exporter configuration (development only)
/// </summary>
public class ConsoleExporterOptions
{
    /// <summary>
    /// Enable console exporter (useful for debugging)
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Display targets: Traces, Metrics, Logs, or All
    /// </summary>
    public ConsoleExporterType Targets { get; set; } = ConsoleExporterType.All;
}

/// <summary>
/// Console exporter target types
/// </summary>
[Flags]
public enum ConsoleExporterType
{
    None = 0,
    Traces = 1,
    Metrics = 2,
    Logs = 4,
    All = Traces | Metrics | Logs
}

/// <summary>
/// Instrumentation configuration
/// </summary>
public class InstrumentationOptions
{
    /// <summary>
    /// Instrument ASP.NET Core requests
    /// </summary>
    public bool AspNetCore { get; set; } = true;

    /// <summary>
    /// Instrument HTTP client calls
    /// </summary>
    public bool Http { get; set; } = true;

    /// <summary>
    /// Instrument SQL Client (Entity Framework Core)
    /// </summary>
    public bool SqlClient { get; set; } = true;

    /// <summary>
    /// Instrument runtime metrics
    /// </summary>
    public bool Runtime { get; set; } = true;

    /// <summary>
    /// Instrument process metrics
    /// </summary>
    public bool Process { get; set; } = true;

    /// <summary>
    /// Instrument StackExchange.Redis
    /// </summary>
    public bool Redis { get; set; } = false;

    /// <summary>
    /// Custom instrumentation libraries to load
    /// </summary>
    public List<string> Custom { get; set; } = new();
}

/// <summary>
/// Sampling strategy configuration
/// </summary>
public class SamplingOptions
{
    /// <summary>
    /// Default sampling rate for all traces
    /// </summary>
    public double DefaultRate { get; set; } = 0.1;

    /// <summary>
    /// Sampling rate for error traces (1.0 = always sample)
    /// </summary>
    public double ErrorRate { get; set; } = 1.0;

    /// <summary>
    /// Sampling rate for slow requests (latency > threshold)
    /// </summary>
    public double SlowRequestRate { get; set; } = 1.0;

    /// <summary>
    /// Latency threshold in milliseconds for slow request detection
    /// </summary>
    public int SlowRequestThreshold { get; set; } = 1000;

    /// <summary>
    /// Enable adaptive sampling based on volume
    /// </summary>
    public bool AdaptiveSampling { get; set; } = true;

    /// <summary>
    /// Target traces per second for adaptive sampling
    /// </summary>
    public int AdaptiveTargetTracesPerSecond { get; set; } = 100;
}
