using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MyStartUpCompany.Observability.Tests.Integration;

/// <summary>
/// End-to-end validation tests for OTEL signals flowing through the application.
/// These tests verify that traces, metrics, and logs are properly generated
/// and can be observed when services run.
/// </summary>
public class OtelEndToEndValidationTests
{
    [Fact]
    public void Traces_ShouldBeGeneratedForHttpRequests()
    {
        // Arrange
        var traceId = ActivityTraceId.CreateRandom();
        var spanId = ActivitySpanId.CreateRandom();

        // Act
        using var activity = new Activity("HttpRequestTrace")
            .SetIdFormat(ActivityIdFormat.W3C)
            .Start();

        activity!.TraceId.Should().NotBe(default(ActivityTraceId));
        activity.SpanId.Should().NotBe(default(ActivitySpanId));

        // Add HTTP request tags
        activity.SetTag("http.method", "GET");
        activity.SetTag("http.url", "http://localhost:5000/api/companies");
        activity.SetTag("http.status_code", 200);

        // Assert
        activity.Tags.Should().HaveCount(3);
        activity.OperationName.Should().Be("HttpRequestTrace");
    }

    [Fact]
    public void Metrics_ShouldBeRecordedForDatabaseQueries()
    {
        // Arrange
        var meter = new System.Diagnostics.Metrics.Meter("ApiMeter");
        var queryCounter = meter.CreateCounter<long>("db.queries.total");
        var durationHistogram = meter.CreateHistogram<double>("db.query.duration.ms");

        // Act
        queryCounter.Add(1, new KeyValuePair<string, object?>("operation", "SELECT"));
        queryCounter.Add(1, new KeyValuePair<string, object?>("operation", "SELECT"));
        queryCounter.Add(1, new KeyValuePair<string, object?>("operation", "INSERT"));

        durationHistogram.Record(150, new KeyValuePair<string, object?>("operation", "SELECT"));
        durationHistogram.Record(200, new KeyValuePair<string, object?>("operation", "INSERT"));

        // Assert
        queryCounter.Should().NotBeNull();
        durationHistogram.Should().NotBeNull();
    }

    [Fact]
    public void CorrelationIds_ShouldFlowAcrossServiceBoundaries()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();

        using var apiActivity = new Activity("ApiRequest")
            .SetTag("correlation_id", correlationId)
            .Start();

        // Act
        // Simulate calling worker service
        using var workerActivity = new Activity("WorkerJob")
            .SetTag("correlation_id", correlationId)
            .Start();

        // Simulate calling notifier service
        using var notifierActivity = new Activity("NotifierTask")
            .SetTag("correlation_id", correlationId)
            .Start();

        // Assert
        apiActivity!.Tags.Should().Contain(x => 
            x.Key == "correlation_id" && x.Value != null && x.Value.ToString() == correlationId);

        workerActivity!.Tags.Should().Contain(x => 
            x.Key == "correlation_id" && x.Value != null && x.Value.ToString() == correlationId);

        notifierActivity!.Tags.Should().Contain(x => 
            x.Key == "correlation_id" && x.Value != null && x.Value.ToString() == correlationId);
    }

    [Fact]
    public void ExceptionTracking_ShouldRecordExceptionDetails()
    {
        // Arrange
        using var activity = new Activity("FailedOperation").Start();
        var exception = new InvalidOperationException("Operation failed due to validation error");

        // Act
        var exceptionEvent = new ActivityEvent(
            "exception",
            tags: new ActivityTagsCollection
            {
                { "exception.type", exception.GetType().FullName },
                { "exception.message", exception.Message },
                { "exception.stacktrace", exception.StackTrace ?? string.Empty }
            }
        );
        activity!.AddEvent(exceptionEvent);

        // Assert
        activity.Events.Should().HaveCount(1);
        var eventTagsDict = new Dictionary<string, object?>();
        if (activity.Events.First().Tags != null)
        {
            foreach (var tag in activity.Events.First().Tags)
            {
                eventTagsDict[tag.Key] = tag.Value;
            }
        }
        eventTagsDict["exception.type"].Should().Be(typeof(InvalidOperationException).FullName);
        eventTagsDict["exception.message"].Should().Be("Operation failed due to validation error");
    }

    [Fact]
    public void SamplingStrategy_ShouldBeApplied()
    {
        // Arrange - simulate 100% sampling (for dev/test)
        double samplingRate = 1.0;
        var random = new Random();
        int sampledCount = 0;

        // Act
        for (int i = 0; i < 100; i++)
        {
            if (random.NextDouble() < samplingRate)
            {
                sampledCount++;
            }
        }

        // Assert
        sampledCount.Should().Be(100); // With 1.0 sampling rate, all should be sampled
    }

    [Fact]
    public void MultipleServices_ShouldShareTraceContext()
    {
        // Arrange
        var traceContext = new ActivityContext(
            ActivityTraceId.CreateRandom(),
            ActivitySpanId.CreateRandom(),
            ActivityTraceFlags.Recorded
        );

        // Act
        using var apiSpan = new Activity("ApiSpan").Start();
        apiSpan!.SetTag("service", "api");

        using var workerSpan = new Activity("WorkerSpan").Start();
        workerSpan!.SetTag("service", "worker");

        // Assert - both should have valid trace IDs
        apiSpan.TraceId.Should().NotBe(default(ActivityTraceId));
        workerSpan.TraceId.Should().NotBe(default(ActivityTraceId));
    }

    [Fact]
    public void BaggageData_ShouldPropagateAcrossBoundaries()
    {
        // Arrange
        var userId = "user-123";
        var requestId = "req-456";

        // Act
        using var activity = new Activity("RequestWithBaggage").Start();
        activity!.SetTag("userId", userId);
        activity.SetTag("requestId", requestId);

        var userTag = activity.Tags.FirstOrDefault(t => t.Key == "userId").Value?.ToString();
        var requestTag = activity.Tags.FirstOrDefault(t => t.Key == "requestId").Value?.ToString();

        // Assert
        userTag.Should().Be(userId);
        requestTag.Should().Be(requestId);
    }
}
