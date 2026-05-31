using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MyStartUpCompany.Observability;

/// <summary>
/// Middleware for recording HTTP request metrics
/// </summary>
public class HttpMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpMetricsMiddleware> _logger;

    public HttpMetricsMiddleware(RequestDelegate next, ILogger<HttpMetricsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var recorder = new HttpMetricsRecorder(context.Request.Method, context.Request.Path);

        // Store in context items for use by endpoints if needed
        context.Items["HttpMetricsRecorder"] = recorder;

        try
        {
            await _next(context);
        }
        finally
        {
            // Record completion with status code
            recorder.RecordCompletion(context.Response.StatusCode, context.Response.ContentLength);
        }
    }
}

/// <summary>
/// Extension method to add HTTP metrics middleware
/// </summary>
public static class HttpMetricsMiddlewareExtensions
{
    /// <summary>
    /// Add HTTP metrics recording middleware
    /// </summary>
    public static IApplicationBuilder UseHttpMetrics(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HttpMetricsMiddleware>();
    }
}

/// <summary>
/// Middleware for instrumenting Entity Framework Core queries
/// </summary>
public class DatabaseMetricsInterceptor
{
    /// <summary>
    /// Record database operation with metrics
    /// </summary>
    public static void RecordOperation(string operationName, string tableName, double durationMs)
    {
        DatabaseMetricsRecorder.RecordQuery(
            tableName: tableName,
            operation: operationName,
            durationMs: durationMs,
            success: true);

        Activity.Current?.AddEvent(new ActivityEvent(
            "database_operation",
            tags: new ActivityTagsCollection(new[]
            {
                new KeyValuePair<string, object?>("db.operation", operationName),
                new KeyValuePair<string, object?>("db.table", tableName),
                new KeyValuePair<string, object?>("duration_ms", durationMs)
            })));
    }
}

/// <summary>
/// Factory for creating instrumented query handlers
/// Wraps business logic with automatic metrics collection
/// </summary>
public abstract class InstrumentedQueryHandler<TRequest, TResponse> where TRequest : class
{
    protected readonly ILogger _logger;
    protected readonly string OperationName;

    protected InstrumentedQueryHandler(ILogger logger, string operationName)
    {
        _logger = logger;
        OperationName = operationName;
    }

    /// <summary>
    /// Execute handler with automatic instrumentation
    /// </summary>
    public async Task<TResponse> HandleAsync(TRequest request)
    {
        using (var activity = new Activity(OperationName))
        {
            activity.Start();

            try
            {
                activity.SetTag("handler.type", GetType().Name);
                activity.SetTag("request.type", request?.GetType().Name);

                _logger.LogInformation("Executing handler: {HandlerName}", OperationName);

                var response = await ExecuteAsync(request);

                activity.SetTag("handler.status", "success");
                _logger.LogInformation("Handler completed: {HandlerName}", OperationName);

                return response;
            }
            catch (Exception ex)
            {
                // Record exception as activity event (RecordException was removed in OpenTelemetry 1.15.x)
                activity.AddEvent(new ActivityEvent("exception",
                    tags: new ActivityTagsCollection(new Dictionary<string, object?>
                    {
                        { "exception.type", ex.GetType().FullName },
                        { "exception.message", ex.Message },
                        { "exception.stacktrace", ex.StackTrace }
                    })));
                activity.SetTag("handler.status", "failure");
                activity.SetTag("exception.type", ex.GetType().Name);

                _logger.LogError(ex, "Handler failed: {HandlerName}", OperationName);
                throw;
            }
            finally
            {
                activity.Stop();
            }
        }
    }

    /// <summary>
    /// Implement by subclass to define handler logic
    /// </summary>
    protected abstract Task<TResponse> ExecuteAsync(TRequest request);
}

/// <summary>
/// Service Bus message handler instrumentation
/// </summary>
public abstract class InstrumentedMessageHandler
{
    protected readonly ILogger _logger;

    protected InstrumentedMessageHandler(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Execute handler with message metrics recording
    /// </summary>
    protected async Task<T> HandleMessageAsync<T>(
        string messageId,
        string messageType,
        Func<Task<T>> handler)
    {
        var recorder = new MessageQueueMetricsRecorder(messageType, messageId);

        try
        {
            _logger.LogInformation(
                "Processing message: Type={MessageType}, Id={MessageId}",
                messageType,
                messageId);

            using (var activity = new Activity($"message.{messageType}"))
            {
                activity.Start();
                activity.SetTag("messaging.message_id", messageId);
                activity.SetTag("messaging.message_type", messageType);

                var result = await handler();

                recorder.RecordSuccess();
                activity.SetTag("messaging.status", "success");

                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process message: Type={MessageType}, Id={MessageId}",
                messageType,
                messageId);

            recorder.RecordFailure(ex);
            throw;
        }
    }

    /// <summary>
    /// Handle message with dead-letter support
    /// </summary>
    protected async Task HandleMessageWithDeadLetterAsync(
        string messageId,
        string messageType,
        Func<Task> handler,
        int maxRetries = 3)
    {
        var recorder = new MessageQueueMetricsRecorder(messageType, messageId);
        int retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                await HandleMessageAsync(
                    messageId,
                    messageType,
                    async () =>
                    {
                        await handler();
                        return true;
                    });

                recorder.RecordSuccess();
                return;
            }
            catch (Exception ex) when (retryCount < maxRetries - 1)
            {
                retryCount++;
                _logger.LogWarning(
                    ex,
                    "Message processing failed, retrying: Type={MessageType}, Id={MessageId}, Attempt={Attempt}",
                    messageType,
                    messageId,
                    retryCount);

                recorder.RecordFailure(ex, retryCount);
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount))); // Exponential backoff
            }
            catch (Exception ex)
            {
                recorder.RecordDeadLetter($"Max retries ({maxRetries}) exceeded");
                throw;
            }
        }
    }
}

/// <summary>
/// Extension methods for handler instrumentation
/// </summary>
public static class HandlerInstrumentationExtensions
{
    /// <summary>
    /// Create instrumented scope for business operations
    /// </summary>
    public static IDisposable CreateInstrumentedScope(
        this ILogger logger,
        string operationName,
        Dictionary<string, object>? tags = null)
    {
        var activity = new Activity(operationName);
        activity.Start();

        if (tags != null)
        {
            foreach (var tag in tags)
            {
                activity.SetTag(tag.Key, tag.Value);
            }
        }

        return new InstrumentedScopeDisposer(activity, logger, operationName);
    }

    private class InstrumentedScopeDisposer : IDisposable
    {
        private readonly Activity _activity;
        private readonly ILogger _logger;
        private readonly string _operationName;

        public InstrumentedScopeDisposer(Activity activity, ILogger logger, string operationName)
        {
            _activity = activity;
            _logger = logger;
            _operationName = operationName;
        }

        public void Dispose()
        {
            _activity?.Stop();
            _logger?.LogInformation("Operation completed: {OperationName}", _operationName);
        }
    }
}
