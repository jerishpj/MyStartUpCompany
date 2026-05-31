namespace MyStartUpCompany.Worker.Health;

/// <summary>
/// Health check service for worker background service
/// Used for monitoring and diagnostics
/// </summary>
public interface IWorkerHealthCheck
{
    /// <summary>
    /// Get current health status
    /// </summary>
    HealthStatus GetStatus();

    /// <summary>
    /// Record successful operation
    /// </summary>
    void RecordSuccess();

    /// <summary>
    /// Record failed operation
    /// </summary>
    void RecordFailure(Exception ex);
}

/// <summary>
/// Health status information
/// </summary>
public class HealthStatus
{
    public string Status { get; set; } = "healthy";
    public string Service { get; set; } = "MyStartUpCompany.Worker";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int SuccessfulOperations { get; set; }
    public int FailedOperations { get; set; }
    public DateTime? LastOperationTime { get; set; }
    public string? LastError { get; set; }
    public int Uptime { get; set; } // in seconds
}

/// <summary>
/// Default implementation of worker health check
/// </summary>
public class WorkerHealthCheck : IWorkerHealthCheck
{
    private readonly ILogger<WorkerHealthCheck> _logger;
    private readonly DateTime _startTime;
    private int _successCount;
    private int _failureCount;
    private DateTime? _lastOperationTime;
    private string? _lastError;

    public WorkerHealthCheck(ILogger<WorkerHealthCheck> logger)
    {
        _logger = logger;
        _startTime = DateTime.UtcNow;
    }

    public HealthStatus GetStatus()
    {
        var uptime = (int)(DateTime.UtcNow - _startTime).TotalSeconds;

        return new HealthStatus
        {
            Status = _failureCount == 0 ? "healthy" : "degraded",
            Service = "MyStartUpCompany.Worker",
            Timestamp = DateTime.UtcNow,
            SuccessfulOperations = _successCount,
            FailedOperations = _failureCount,
            LastOperationTime = _lastOperationTime,
            LastError = _lastError,
            Uptime = uptime
        };
    }

    public void RecordSuccess()
    {
        Interlocked.Increment(ref _successCount);
        _lastOperationTime = DateTime.UtcNow;
        _lastError = null;

        _logger.LogDebug("Worker operation succeeded. Total: {SuccessCount}", _successCount);
    }

    public void RecordFailure(Exception ex)
    {
        Interlocked.Increment(ref _failureCount);
        _lastOperationTime = DateTime.UtcNow;
        _lastError = ex.Message;

        _logger.LogWarning(ex, "Worker operation failed. Total: {FailureCount}", _failureCount);
    }
}
