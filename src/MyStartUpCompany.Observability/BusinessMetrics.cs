using System.Diagnostics.Metrics;

namespace MyStartUpCompany.Observability;

/// <summary>
/// Centralized business metrics counters and histograms for OpenTelemetry
/// </summary>
public static class BusinessMetrics
{
    private static readonly Meter _meter = new("MyStartUpCompany.BusinessMetrics", "1.0.0");

    // HTTP Metrics
    public static readonly Counter<int> HttpRequestsTotal = _meter.CreateCounter<int>(
        "http.requests.total",
        unit: "{requests}",
        description: "Total number of HTTP requests");

    public static readonly Counter<int> HttpErrorsTotal = _meter.CreateCounter<int>(
        "http.errors.total",
        unit: "{errors}",
        description: "Total number of HTTP errors");

    public static readonly Histogram<double> HttpRequestDuration = _meter.CreateHistogram<double>(
        "http.request.duration",
        unit: "ms",
        description: "HTTP request duration in milliseconds");

    // Database Metrics
    public static readonly Counter<int> DatabaseQueriesTotal = _meter.CreateCounter<int>(
        "db.queries.total",
        unit: "{queries}",
        description: "Total number of database queries");

    public static readonly Counter<int> DatabaseErrorsTotal = _meter.CreateCounter<int>(
        "db.errors.total",
        unit: "{errors}",
        description: "Total number of database errors");

    public static readonly Histogram<double> DatabaseQueryDuration = _meter.CreateHistogram<double>(
        "db.query.duration",
        unit: "ms",
        description: "Database query duration in milliseconds");

    public static readonly UpDownCounter<int> DatabaseConnectionPoolActive = _meter.CreateUpDownCounter<int>(
        "db.connection.pool.active",
        unit: "{connections}",
        description: "Number of active database connections in the pool");

    // Message Queue Metrics
    public static readonly Counter<int> MessagesReceivedTotal = _meter.CreateCounter<int>(
        "messaging.messages.received",
        unit: "{messages}",
        description: "Total number of messages received");

    public static readonly Counter<int> MessagesPublished = _meter.CreateCounter<int>(
        "messaging.messages.published",
        unit: "{messages}",
        description: "Total number of messages published");

    public static readonly Counter<int> MessagesProcessedTotal = _meter.CreateCounter<int>(
        "messaging.messages.processed",
        unit: "{messages}",
        description: "Total number of messages processed successfully");

    public static readonly Counter<int> MessagesProcessed = _meter.CreateCounter<int>(
        "messaging.messages.processed.total",
        unit: "{messages}",
        description: "Total number of messages processed (alias for MessagesProcessedTotal)");

    public static readonly Counter<int> MessagesFailedTotal = _meter.CreateCounter<int>(
        "messaging.messages.failed",
        unit: "{messages}",
        description: "Total number of failed message processing attempts");

    public static readonly Histogram<double> MessageProcessingDuration = _meter.CreateHistogram<double>(
        "messaging.message.duration",
        unit: "ms",
        description: "Message processing duration in milliseconds");

    public static readonly UpDownCounter<int> MessageQueueDepth = _meter.CreateUpDownCounter<int>(
        "messaging.queue.depth",
        unit: "{messages}",
        description: "Current depth of message queue");

    // Notification Metrics
    public static readonly Counter<int> NotificationsDeliveredTotal = _meter.CreateCounter<int>(
        "notifications.delivered",
        unit: "{notifications}",
        description: "Total number of notifications delivered");

    public static readonly Counter<int> NotificationsFailedTotal = _meter.CreateCounter<int>(
        "notifications.failed",
        unit: "{notifications}",
        description: "Total number of failed notification delivery attempts");

    public static readonly Histogram<double> NotificationDeliveryDuration = _meter.CreateHistogram<double>(
        "notifications.delivery.duration",
        unit: "ms",
        description: "Notification delivery duration in milliseconds");

    // Business Domain Metrics
    public static readonly Counter<int> CompaniesCreatedTotal = _meter.CreateCounter<int>(
        "business.companies.created",
        unit: "{companies}",
        description: "Total number of companies created");

    public static readonly Counter<int> ProjectsCreatedTotal = _meter.CreateCounter<int>(
        "business.projects.created",
        unit: "{projects}",
        description: "Total number of projects created");

    public static readonly Counter<int> ProjectsAssignedTypeTotal = _meter.CreateCounter<int>(
        "business.projects.type.assigned",
        unit: "{projects}",
        description: "Total number of projects assigned a type");

    // Paging/Pagination Metrics
    public static readonly Histogram<int> PaginationPageSize = _meter.CreateHistogram<int>(
        "pagination.page_size",
        unit: "{items}",
        description: "Size of paginated result sets");

    public static readonly Histogram<int> PaginationPageNumber = _meter.CreateHistogram<int>(
        "pagination.page_number",
        unit: "{pages}",
        description: "Page number in pagination");

    public static readonly Histogram<int> PaginationTotalItems = _meter.CreateHistogram<int>(
        "pagination.total_items",
        unit: "{items}",
        description: "Total items available for pagination");

    // Health/Availability Metrics
    public static readonly UpDownCounter<int> ServiceAvailability = _meter.CreateUpDownCounter<int>(
        "service.availability",
        unit: "{status}",
        description: "Service availability status (1 = available, 0 = unavailable)");

    public static readonly UpDownCounter<int> HealthCheckStatus = _meter.CreateUpDownCounter<int>(
        "health.check.status",
        unit: "{status}",
        description: "Health check status (1 = healthy, 0 = unhealthy)");

    public static readonly Histogram<double> HealthCheckDuration = _meter.CreateHistogram<double>(
        "health.check.duration",
        unit: "ms",
        description: "Health check duration in milliseconds");

    public static readonly Counter<int> HealthChecksTotal = _meter.CreateCounter<int>(
        "health.checks.total",
        unit: "{checks}",
        description: "Total number of health checks performed");

    /// <summary>
    /// Record a database query operation
    /// </summary>
    public static void RecordDbQuery(string queryName, int rowsAffected, long durationMs)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("query.name", queryName),
            new("query.rows_affected", rowsAffected)
        };

        DatabaseQueriesTotal.Add(1, tags);
        DatabaseQueryDuration.Record(durationMs, tags);
    }

    /// <summary>
    /// Record a database query error
    /// </summary>
    public static void RecordDbQueryError(string queryName, long durationMs)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("query.name", queryName),
            new("query.status", "error")
        };

        DatabaseQueriesTotal.Add(1, tags);
        DatabaseQueryDuration.Record(durationMs, tags);
    }

    /// <summary>
    /// Record pagination metrics
    /// </summary>
    public static void RecordPagingMetrics(string entityType, int pageNumber, int pageSize, int totalItems)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("entity.type", entityType)
        };

        PaginationPageNumber.Record(pageNumber, tags);
        PaginationPageSize.Record(pageSize, tags);
        PaginationTotalItems.Record(totalItems, tags);
    }

    /// <summary>
    /// Set service availability status
    /// </summary>
    public static void SetServiceAvailability(int status)
    {
        ServiceAvailability.Add(status);
    }

    /// <summary>
    /// Record a health check
    /// </summary>
    public static void RecordHealthCheck(string serviceName, bool healthy)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service.name", serviceName),
            new("health.status", healthy ? "healthy" : "unhealthy")
        };

        HealthChecksTotal.Add(1, tags);
    }
}
