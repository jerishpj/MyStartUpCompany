using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MyStartUpCompany.Observability;

/// <summary>
/// Middleware for managing trace context and correlation IDs
/// Ensures W3C Trace Context is properly propagated across requests
/// </summary>
public class TraceContextMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Header names for trace context
    /// </summary>
    private const string TraceParentHeader = "traceparent";
    private const string TraceStateHeader = "tracestate";
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public TraceContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Process the HTTP request and ensure trace context is properly set
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract or create correlation ID
        var correlationId = ExtractOrCreateCorrelationId(context);
        context.Items["CorrelationId"] = correlationId;

        // Extract trace context from headers (W3C Trace Context)
        var traceContext = ExtractTraceContext(context);

        // Set the activity (which creates the trace span)
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.SetTag("correlation_id", correlationId);
            activity.SetTag("http.client_ip", GetClientIp(context));
            activity.SetTag("http.user_agent", context.Request.Headers.UserAgent);
        }

        // Add correlation ID to response headers for client tracking
        context.Response.Headers.Add(CorrelationIdHeader, correlationId);

        try
        {
            await _next(context);
        }
        finally
        {
            // Log correlation ID with response
            if (activity != null)
            {
                activity.SetTag("correlation_id", correlationId);
                activity.SetTag("http.response_status_code", context.Response.StatusCode);
            }
        }
    }

    /// <summary>
    /// Extract existing correlation ID or create a new one
    /// </summary>
    private static string ExtractOrCreateCorrelationId(HttpContext context)
    {
        // Check for correlation ID header
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            return correlationId.ToString();
        }

        // Use trace ID from Activity if available
        if (Activity.Current?.Id != null)
        {
            return Activity.Current.Id;
        }

        // Generate new correlation ID
        return Guid.NewGuid().ToString("D");
    }

    /// <summary>
    /// Extract W3C Trace Context from request headers
    /// </summary>
    private static (string? TraceParent, string? TraceState) ExtractTraceContext(HttpContext context)
    {
        var traceParent = context.Request.Headers.TryGetValue(TraceParentHeader, out var tp) 
            ? tp.ToString() 
            : null;

        var traceState = context.Request.Headers.TryGetValue(TraceStateHeader, out var ts)
            ? ts.ToString()
            : null;

        return (traceParent, traceState);
    }

    /// <summary>
    /// Get client IP address, considering X-Forwarded-For header (for proxies)
    /// </summary>
    private static string GetClientIp(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var ips = forwardedFor.ToString().Split(',');
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

/// <summary>
/// Extension methods for adding trace context middleware
/// </summary>
public static class TraceContextMiddlewareExtensions
{
    /// <summary>
    /// Add trace context middleware to the application pipeline
    /// Should be added early in the pipeline
    /// </summary>
    public static IApplicationBuilder UseTraceContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TraceContextMiddleware>();
    }
}

/// <summary>
/// Helper class for accessing correlation ID throughout the request
/// </summary>
public class CorrelationIdAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Get the current correlation ID
    /// </summary>
    public string GetCorrelationId()
    {
        var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();
        return correlationId ?? "unknown";
    }

    /// <summary>
    /// Get the current trace ID
    /// </summary>
    public string GetTraceId()
    {
        return Activity.Current?.Id ?? Activity.Current?.RootId ?? "unknown";
    }

    /// <summary>
    /// Get the current span ID
    /// </summary>
    public string GetSpanId()
    {
        return Activity.Current?.SpanId.ToString() ?? "unknown";
    }
}

/// <summary>
/// Extension methods for HTTP client instrumentation with trace context propagation
/// </summary>
public static class HttpClientTraceExtensions
{
    /// <summary>
    /// Create an HttpClient with automatic trace context propagation
    /// </summary>
    public static HttpClient CreateTracingHttpClient(this IServiceProvider services)
    {
        var handler = new HttpClientHandler();
        var client = new HttpClient(handler, disposeHandler: true);

        // Add default headers for trace context
        client.DefaultRequestHeaders.Add("User-Agent", "MyStartUpCompany/1.0");

        return client;
    }

    /// <summary>
    /// Add trace context to outgoing HTTP requests
    /// W3C Trace Context is automatically propagated by OpenTelemetry instrumentation
    /// </summary>
    public static void AddTraceContext(this HttpRequestMessage request)
    {
        var activity = Activity.Current;
        if (activity == null)
        {
            return;
        }

        // OpenTelemetry automatically adds traceparent and tracestate headers
        // This method can be extended for custom context propagation
        request.Headers.Add("X-Trace-ID", activity.Id ?? "unknown");
    }
}

/// <summary>
/// Background service for propagating trace context to background tasks
/// </summary>
public class TraceContextScope : IDisposable
{
    private readonly Activity? _activity;
    private readonly IDisposable? _activityScope;

    public TraceContextScope(string operationName)
    {
        // Create new activity linked to current context
        _activity = new Activity(operationName);
        _activity.Start();
    }

    /// <summary>
    /// Set a tag on the current span
    /// </summary>
    public TraceContextScope SetTag(string key, object? value)
    {
        if (_activity != null)
        {
            _activity.SetTag(key, value);
        }
        return this;
    }

    /// <summary>
    /// Record an event
    /// </summary>
    public TraceContextScope RecordEvent(string eventName)
    {
        _activity?.AddEvent(new ActivityEvent(eventName));
        return this;
    }

    /// <summary>
    /// Set exception information
    /// </summary>
    public TraceContextScope RecordException(Exception exception)
    {
        if (_activity != null)
        {
            _activity.SetTag("exception.type", exception.GetType().Name);
            _activity.SetTag("exception.message", exception.Message);
            _activity.SetTag("exception.stacktrace", exception.StackTrace);
        }
        return this;
    }

    public void Dispose()
    {
        _activity?.Stop();
        _activityScope?.Dispose();
    }
}
