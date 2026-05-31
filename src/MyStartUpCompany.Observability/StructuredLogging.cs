using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace MyStartUpCompany.Observability;

/// <summary>
/// Structured logging context that includes correlation ID and trace information
/// </summary>
public class LoggingContext
{
    public string CorrelationId { get; set; }
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }
    public string? UserId { get; set; }
    public Dictionary<string, object> CustomProperties { get; set; }

    public LoggingContext(string correlationId)
    {
        CorrelationId = correlationId;
        TraceId = Activity.Current?.Id;
        SpanId = Activity.Current?.SpanId.ToString();
        CustomProperties = new Dictionary<string, object>();
    }

    /// <summary>
    /// Add custom property to logging context
    /// </summary>
    public LoggingContext AddProperty(string key, object value)
    {
        CustomProperties[key] = value;
        return this;
    }

    /// <summary>
    /// Set the user ID for tracking user-specific operations
    /// </summary>
    public LoggingContext WithUserId(string? userId)
    {
        UserId = userId;
        return this;
    }

    /// <summary>
    /// Convert to dictionary for structured logging
    /// </summary>
    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>
        {
            { "CorrelationId", CorrelationId },
            { "TraceId", TraceId ?? "unknown" },
            { "SpanId", SpanId ?? "unknown" }
        };

        if (!string.IsNullOrEmpty(UserId))
        {
            dict["UserId"] = UserId;
        }

        foreach (var prop in CustomProperties)
        {
            dict[prop.Key] = prop.Value;
        }

        return dict;
    }
}

/// <summary>
/// Structured logger that automatically includes correlation ID and trace context
/// </summary>
public class StructuredLogger
{
    private readonly ILogger _logger;
    private readonly LoggingContext _context;

    public StructuredLogger(ILogger logger, LoggingContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Log information with structured context
    /// </summary>
    public void LogInformation(string message, params object?[] args)
    {
        using (_logger.BeginScope(_context.ToDictionary()))
        {
            _logger.LogInformation(message, args);
        }
    }

    /// <summary>
    /// Log warning with structured context
    /// </summary>
    public void LogWarning(string message, params object?[] args)
    {
        using (_logger.BeginScope(_context.ToDictionary()))
        {
            _logger.LogWarning(message, args);
        }
    }

    /// <summary>
    /// Log error with structured context
    /// </summary>
    public void LogError(Exception? exception, string message, params object?[] args)
    {
        using (_logger.BeginScope(_context.ToDictionary()))
        {
            if (exception != null)
            {
                _logger.LogError(exception, message, args);
            }
            else
            {
                _logger.LogError(message, args);
            }
        }
    }

    /// <summary>
    /// Log debug with structured context
    /// </summary>
    public void LogDebug(string message, params object?[] args)
    {
        using (_logger.BeginScope(_context.ToDictionary()))
        {
            _logger.LogDebug(message, args);
        }
    }

    /// <summary>
    /// Log trace with structured context
    /// </summary>
    public void LogTrace(string message, params object?[] args)
    {
        using (_logger.BeginScope(_context.ToDictionary()))
        {
            _logger.LogTrace(message, args);
        }
    }
}

/// <summary>
/// Extension methods for ILogger to support structured logging with context
/// </summary>
public static class StructuredLoggingExtensions
{
    private static readonly ConcurrentDictionary<string, object> _contextCache = new();

    /// <summary>
    /// Create a structured logger with correlation ID
    /// </summary>
    public static StructuredLogger CreateStructuredLogger(
        this ILogger logger,
        string correlationId)
    {
        var context = new LoggingContext(correlationId);
        return new StructuredLogger(logger, context);
    }

    /// <summary>
    /// Log with correlation ID using scope
    /// </summary>
    public static IDisposable LogScope(
        this ILogger logger,
        string correlationId,
        string? userId = null)
    {
        var scope = new Dictionary<string, object>
        {
            { "CorrelationId", correlationId },
            { "TraceId", Activity.Current?.Id ?? "unknown" },
            { "SpanId", Activity.Current?.SpanId.ToString() ?? "unknown" }
        };

        if (!string.IsNullOrEmpty(userId))
        {
            scope["UserId"] = userId;
        }

        return logger.BeginScope(scope);
    }

    /// <summary>
    /// Log information with correlation ID
    /// </summary>
    public static void LogInfoWithContext(
        this ILogger logger,
        string correlationId,
        string message,
        params object?[] args)
    {
        using (logger.LogScope(correlationId))
        {
            logger.LogInformation(message, args);
        }
    }

    /// <summary>
    /// Log warning with correlation ID
    /// </summary>
    public static void LogWarnWithContext(
        this ILogger logger,
        string correlationId,
        string message,
        params object?[] args)
    {
        using (logger.LogScope(correlationId))
        {
            logger.LogWarning(message, args);
        }
    }

    /// <summary>
    /// Log error with correlation ID and exception
    /// </summary>
    public static void LogErrorWithContext(
        this ILogger logger,
        string correlationId,
        Exception exception,
        string message,
        params object?[] args)
    {
        using (logger.LogScope(correlationId))
        {
            logger.LogError(exception, message, args);
        }
    }

    /// <summary>
    /// Log critical error with correlation ID
    /// </summary>
    public static void LogCriticalWithContext(
        this ILogger logger,
        string correlationId,
        Exception? exception,
        string message,
        params object?[] args)
    {
        using (logger.LogScope(correlationId))
        {
            if (exception != null)
            {
                logger.LogCritical(exception, message, args);
            }
            else
            {
                logger.LogCritical(message, args);
            }
        }
    }

    /// <summary>
    /// Log debug with correlation ID
    /// </summary>
    public static void LogDebugWithContext(
        this ILogger logger,
        string correlationId,
        string message,
        params object?[] args)
    {
        using (logger.LogScope(correlationId))
        {
            logger.LogDebug(message, args);
        }
    }

    /// <summary>
    /// Convert LoggingContext to dictionary
    /// </summary>
    private static Dictionary<string, object> ToDict(this LoggingContext context)
    {
        return context.ToDict();
    }
}

/// <summary>
/// Factory for creating properly configured loggers with observability
/// </summary>
public class ObservabilityLoggerFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public ObservabilityLoggerFactory(
        ILoggerFactory loggerFactory,
        IHttpContextAccessor? httpContextAccessor = null)
    {
        _loggerFactory = loggerFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Create a logger with automatic correlation ID injection
    /// </summary>
    public ILogger CreateLogger(string categoryName)
    {
        var logger = _loggerFactory.CreateLogger(categoryName);

        // If HTTP context is available, automatically include correlation ID
        if (_httpContextAccessor?.HttpContext != null)
        {
            var correlationId = _httpContextAccessor.HttpContext.Items["CorrelationId"]?.ToString()
                ?? Activity.Current?.Id
                ?? "unknown";

            // Create a wrapped logger that includes correlation ID
            return new CorrelatedLogger(logger, correlationId);
        }

        return logger;
    }

    /// <summary>
    /// Create a logger with explicit correlation ID
    /// </summary>
    public ILogger CreateLogger(string categoryName, string correlationId)
    {
        var logger = _loggerFactory.CreateLogger(categoryName);
        return new CorrelatedLogger(logger, correlationId);
    }
}

/// <summary>
/// Wraps ILogger to automatically include correlation ID in all log entries
/// </summary>
public class CorrelatedLogger : ILogger
{
    private readonly ILogger _innerLogger;
    private readonly string _correlationId;

    public CorrelatedLogger(ILogger innerLogger, string correlationId)
    {
        _innerLogger = innerLogger;
        _correlationId = correlationId;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var scope = new Dictionary<string, object>
        {
            { "CorrelationId", _correlationId },
            { "TraceId", Activity.Current?.Id ?? "unknown" }
        };

        if (state is not null)
        {
            var dict = new Dictionary<string, object>(scope);
            if (state is IDictionary<string, object> stateDictionary)
            {
                foreach (var kvp in stateDictionary)
                {
                    dict[kvp.Key] = kvp.Value;
                }
            }
            return _innerLogger.BeginScope(dict);
        }

        return _innerLogger.BeginScope(scope);
    }

    public bool IsEnabled(LogLevel logLevel) => _innerLogger.IsEnabled(logLevel);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        using (BeginScope(state))
        {
            _innerLogger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}

/// <summary>
/// Helper for batch logging operations
/// Useful for tracking related events
/// </summary>
public class BatchLogger
{
    private readonly ILogger _logger;
    private readonly LoggingContext _context;
    private readonly List<LogEntry> _entries;

    private class LogEntry
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public Exception? Exception { get; set; }
        public DateTime Timestamp { get; set; }
        public object?[] Args { get; set; }
    }

    public BatchLogger(ILogger logger, LoggingContext context)
    {
        _logger = logger;
        _context = context;
        _entries = new List<LogEntry>();
    }

    /// <summary>
    /// Add information entry to batch
    /// </summary>
    public BatchLogger AddInfo(string message, params object?[] args)
    {
        _entries.Add(new LogEntry
        {
            Level = LogLevel.Information,
            Message = message,
            Exception = null,
            Timestamp = DateTime.UtcNow,
            Args = args
        });
        return this;
    }

    /// <summary>
    /// Add warning entry to batch
    /// </summary>
    public BatchLogger AddWarning(string message, params object?[] args)
    {
        _entries.Add(new LogEntry
        {
            Level = LogLevel.Warning,
            Message = message,
            Exception = null,
            Timestamp = DateTime.UtcNow,
            Args = args
        });
        return this;
    }

    /// <summary>
    /// Add error entry to batch
    /// </summary>
    public BatchLogger AddError(Exception exception, string message, params object?[] args)
    {
        _entries.Add(new LogEntry
        {
            Level = LogLevel.Error,
            Message = message,
            Exception = exception,
            Timestamp = DateTime.UtcNow,
            Args = args
        });
        return this;
    }

    /// <summary>
    /// Flush all entries in the batch
    /// </summary>
    public void Flush()
    {
        using (_logger.LogScope(_context.CorrelationId))
        {
            foreach (var entry in _entries)
            {
                _logger.Log(
                    entry.Level,
                    entry.Exception,
                    entry.Message,
                    entry.Args);
            }
        }
        _entries.Clear();
    }
}
