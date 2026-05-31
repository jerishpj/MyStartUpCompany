using System.Diagnostics;

namespace MyStartUpCompany.Observability;

/// <summary>
/// Metrics recorder for HTTP requests
/// Records latency, status codes, and request/response sizes
/// </summary>
public class HttpMetricsRecorder
{
    private readonly Stopwatch _stopwatch;
    private readonly string _method;
    private readonly string _path;

    public HttpMetricsRecorder(string method, string path)
    {
        _method = method;
        _path = path;
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Record completion of an HTTP request
    /// </summary>
    public void RecordCompletion(int statusCode, long? responseSize = null)
    {
        _stopwatch.Stop();
        var durationMs = _stopwatch.Elapsed.TotalMilliseconds;

        var tags = new KeyValuePair<string, object?>[]
        {
            new("http.method", _method),
            new("http.path", _path),
            new("http.status_code", statusCode),
            new("http.status_class", GetStatusClass(statusCode))
        };

        // Record metrics
        BusinessMetrics.HttpRequestsTotal.Add(1, tags);
        BusinessMetrics.HttpRequestDuration.Record(durationMs, tags);

        if (responseSize.HasValue)
        {
            var sizeTagsWithSize = tags.Append(new KeyValuePair<string, object?>("http.response_size", responseSize.Value)).ToArray();
            // Could add a separate response size metric if needed
        }
    }

    /// <summary>
    /// Get HTTP status class (2xx, 3xx, 4xx, 5xx)
    /// </summary>
    private static string GetStatusClass(int statusCode)
    {
        return (statusCode / 100) switch
        {
            2 => "2xx",
            3 => "3xx",
            4 => "4xx",
            5 => "5xx",
            _ => "unknown"
        };
    }
}

/// <summary>
/// Metrics recorder for database operations
/// Records query execution time, connection pool state, and operation types
/// </summary>
public class DatabaseMetricsRecorder
{
    /// <summary>
    /// Record a database query execution
    /// </summary>
    public static void RecordQuery(string tableName, string operation, double durationMs, bool success = true, string? exceptionType = null)
    {
        var statusTag = success ? "success" : "failure";
        var tags = new List<KeyValuePair<string, object?>>
        {
            new("db.table", tableName),
            new("db.operation", operation),
            new("db.status", statusTag)
        };

        if (!success && exceptionType != null)
        {
            tags.Add(new("db.exception_type", exceptionType));
        }

        BusinessMetrics.DatabaseQueriesTotal.Add(1, tags.ToArray());
        BusinessMetrics.DatabaseQueryDuration.Record(durationMs, tags.ToArray());

        // Log slow queries
        if (durationMs > 1000)
        {
            var activityTags = tags.Append(new KeyValuePair<string, object?>("db.slow_query", true)).ToArray();
            Activity.Current?.AddEvent(new ActivityEvent("slow_database_query", tags: new ActivityTagsCollection(activityTags)));
        }
    }

    /// <summary>
    /// Record connection pool state
    /// </summary>
    public static void RecordConnectionPoolState(int activeConnections, int availableConnections)
    {
        BusinessMetrics.DatabaseConnectionPoolActive.Add(activeConnections);
    }

    /// <summary>
    /// Record query with exception handling
    /// </summary>
    public static void RecordQueryWithException(
        string tableName,
        string operation,
        double durationMs,
        Exception exception)
    {
        RecordQuery(
            tableName,
            operation,
            durationMs,
            success: false,
            exceptionType: exception.GetType().Name);

        // Record exception as activity event (RecordException was removed in OpenTelemetry 1.15.x)
        Activity.Current?.AddEvent(new ActivityEvent("exception", 
            tags: new ActivityTagsCollection(new Dictionary<string, object?>
            {
                { "exception.type", exception.GetType().FullName },
                { "exception.message", exception.Message },
                { "exception.stacktrace", exception.StackTrace }
            })));
    }
}

/// <summary>
/// Metrics recorder for message queue operations
/// Records message processing, failures, and queue depth
/// </summary>
public class MessageQueueMetricsRecorder
{
    private readonly Stopwatch _stopwatch;
    private readonly string _messageType;
    private readonly string _messageId;

    public MessageQueueMetricsRecorder(string messageType, string messageId)
    {
        _messageType = messageType;
        _messageId = messageId;
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Record successful message processing
    /// </summary>
    public void RecordSuccess()
    {
        _stopwatch.Stop();
        var durationMs = _stopwatch.Elapsed.TotalMilliseconds;

        var tags = new KeyValuePair<string, object?>[]
        {
            new("message.type", _messageType),
            new("message.id", _messageId),
            new("message.status", "success")
        };

        BusinessMetrics.MessagesReceivedTotal.Add(1);
        BusinessMetrics.MessagesProcessedTotal.Add(1, tags);
        BusinessMetrics.MessageProcessingDuration.Record(durationMs, tags);

        Activity.Current?.AddEvent(new ActivityEvent(
            "message_processing_completed",
            tags: new ActivityTagsCollection(tags)));
    }

    /// <summary>
    /// Record message processing failure
    /// </summary>
    public void RecordFailure(Exception exception, int retryAttempt = 0)
    {
        _stopwatch.Stop();
        var durationMs = _stopwatch.Elapsed.TotalMilliseconds;

        var tags = new KeyValuePair<string, object?>[]
        {
            new("message.type", _messageType),
            new("message.id", _messageId),
            new("message.status", "failure"),
            new("message.exception_type", exception.GetType().Name),
            new("message.retry_attempt", retryAttempt)
        };

        BusinessMetrics.MessagesFailedTotal.Add(1, tags);
        BusinessMetrics.MessageProcessingDuration.Record(durationMs, tags);

        // Record exception as activity event (RecordException was removed in OpenTelemetry 1.15.x)
        Activity.Current?.AddEvent(new ActivityEvent("exception",
            tags: new ActivityTagsCollection(new Dictionary<string, object?>
            {
                { "exception.type", exception.GetType().FullName },
                { "exception.message", exception.Message },
                { "exception.stacktrace", exception.StackTrace }
            })));
        Activity.Current?.AddEvent(new ActivityEvent(
            "message_processing_failed",
            tags: new ActivityTagsCollection(tags)));
    }

    /// <summary>
    /// Record a dead-letter message
    /// </summary>
    public void RecordDeadLetter(string reason)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("message.type", _messageType),
            new("message.id", _messageId),
            new("message.status", "dead_letter"),
            new("dead_letter.reason", reason)
        };

        BusinessMetrics.MessagesFailedTotal.Add(1, tags);

        Activity.Current?.SetTag("message.dead_letter", true);
        Activity.Current?.SetTag("dead_letter.reason", reason);
        Activity.Current?.AddEvent(new ActivityEvent("message_dead_lettered", tags: new ActivityTagsCollection(tags)));
    }
}

/// <summary>
/// Metrics recorder for notification delivery
/// Records sent, failed, and delivery times
/// </summary>
public class NotificationMetricsRecorder
{
    private readonly Stopwatch _stopwatch;
    private readonly string _channel;
    private readonly string _notificationId;

    public NotificationMetricsRecorder(string channel, string notificationId)
    {
        _channel = channel;
        _notificationId = notificationId;
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Record successful notification delivery
    /// </summary>
    public void RecordSuccess()
    {
        _stopwatch.Stop();
        var durationMs = _stopwatch.Elapsed.TotalMilliseconds;

        var tags = new KeyValuePair<string, object?>[]
        {
            new("notification.channel", _channel),
            new("notification.id", _notificationId),
            new("notification.status", "success")
        };

        BusinessMetrics.NotificationsDeliveredTotal.Add(1, tags);
        BusinessMetrics.NotificationDeliveryDuration.Record(durationMs, tags);

        Activity.Current?.AddEvent(new ActivityEvent("notification_delivered", tags: new ActivityTagsCollection(tags)));
    }

    /// <summary>
    /// Record failed notification delivery
    /// </summary>
    public void RecordFailure(Exception exception, int retryAttempt = 0)
    {
        _stopwatch.Stop();
        var durationMs = _stopwatch.Elapsed.TotalMilliseconds;

        var tags = new KeyValuePair<string, object?>[]
        {
            new("notification.channel", _channel),
            new("notification.id", _notificationId),
            new("notification.status", "failure"),
            new("notification.exception_type", exception.GetType().Name),
            new("notification.retry_attempt", retryAttempt)
        };

        BusinessMetrics.NotificationsFailedTotal.Add(1, tags);
        BusinessMetrics.NotificationDeliveryDuration.Record(durationMs, tags);

        // Record exception as activity event
        Activity.Current?.AddEvent(new ActivityEvent("exception",
            tags: new ActivityTagsCollection(new Dictionary<string, object?>
            {
                { "exception.type", exception.GetType().FullName },
                { "exception.message", exception.Message }
            })));
        Activity.Current?.AddEvent(new ActivityEvent("notification_failed", tags: new ActivityTagsCollection(tags)));
    }
}

/// <summary>
/// Metrics recorder for business domain events
/// </summary>
public class BusinessEventMetricsRecorder
{
    /// <summary>
    /// Record company created event
    /// </summary>
    public static void RecordCompanyCreated(string companyId, string? industry = null)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("company.id", companyId),
            new("event.type", "company_created"),
            industry != null ? new("company.industry", industry) : default
        };

        BusinessMetrics.CompaniesCreatedTotal.Add(1, tags.Where(t => t.Value != null).ToArray());

        Activity.Current?.AddEvent(new ActivityEvent("company_created", tags: new ActivityTagsCollection(tags.Where(t => t.Value != null).ToArray())));
    }

    /// <summary>
    /// Record project created event
    /// </summary>
    public static void RecordProjectCreated(string projectId, string companyId, string? projectType = null)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("project.id", projectId),
            new("company.id", companyId),
            new("event.type", "project_created"),
            projectType != null ? new("project.type", projectType) : default
        };

        BusinessMetrics.ProjectsCreatedTotal.Add(1, tags.Where(t => t.Value != null).ToArray());

        Activity.Current?.AddEvent(new ActivityEvent("project_created", tags: new ActivityTagsCollection(tags.Where(t => t.Value != null).ToArray())));
    }

    /// <summary>
    /// Record project type assignment
    /// </summary>
    public static void RecordProjectTypeAssigned(string projectId, string projectType)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("project.id", projectId),
            new("project.type", projectType),
            new("event.type", "project_type_assigned")
        };

        BusinessMetrics.ProjectsAssignedTypeTotal.Add(1, tags);

        Activity.Current?.AddEvent(new ActivityEvent("project_type_assigned", tags: new ActivityTagsCollection(tags)));
    }
}

/// <summary>
/// Helper for recording metrics with automatic duration tracking
/// </summary>
public class MetricsScope : IDisposable
{
    private readonly Stopwatch _stopwatch;
    private readonly string _metricName;
    private readonly Dictionary<string, object> _tags;

    public MetricsScope(string metricName)
    {
        _metricName = metricName;
        _stopwatch = Stopwatch.StartNew();
        _tags = new Dictionary<string, object>();
    }

    /// <summary>
    /// Add a tag to the metric
    /// </summary>
    public MetricsScope WithTag(string key, object value)
    {
        _tags[key] = value;
        return this;
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        var durationMs = _stopwatch.Elapsed.TotalMilliseconds;

        // Record the metric (this can be extended for specific metric types)
        Activity.Current?.AddEvent(new ActivityEvent(
            _metricName,
            tags: new ActivityTagsCollection(_tags.Append(new KeyValuePair<string, object>("duration_ms", durationMs)))));
    }
}
