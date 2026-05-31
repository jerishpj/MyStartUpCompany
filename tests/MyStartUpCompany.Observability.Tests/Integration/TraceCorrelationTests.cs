using System.Diagnostics;

namespace MyStartUpCompany.Observability.Tests.Integration;

/// <summary>
/// Tests for trace ID generation and correlation across service boundaries.
/// </summary>
public class TraceCorrelationTests
{
    [Fact]
    public void Activity_ShouldGenerateValidTraceId()
    {
        // Arrange & Act
        using var activity = new Activity("TestOperation").Start();

        // Assert
        activity.Should().NotBeNull();
        activity!.Id.Should().NotBeNullOrEmpty();
        activity.TraceId.Should().NotBe(ActivityTraceId.CreateRandom());
    }

    [Fact]
    public void Activity_ShouldPropagateTraceIdToChildren()
    {
        // Arrange
        using var parentActivity = new Activity("ParentOperation").Start();
        var parentTraceId = parentActivity!.TraceId;

        // Act
        using var childActivity = new Activity("ChildOperation").Start();
        var childTraceId = childActivity!.TraceId;

        // Assert
        childTraceId.Should().Be(parentTraceId);
    }

    [Fact]
    public void Activity_ShouldGenerateUniqueSpanIds()
    {
        // Arrange & Act
        using var activity1 = new Activity("Operation1").Start();
        using var activity2 = new Activity("Operation2").Start();

        // Assert
        activity1!.SpanId.Should().NotBe(activity2!.SpanId);
    }

    [Fact]
    public void Activity_ShouldRecordTags()
    {
        // Arrange & Act
        using var activity = new Activity("TaggedOperation").Start();
        activity!.SetTag("userId", "123");
        activity.SetTag("requestId", "req-456");

        // Assert
        activity.Tags.Should().Contain(x => x.Key == "userId" && x.Value == "123");
        activity.Tags.Should().Contain(x => x.Key == "requestId" && x.Value == "req-456");
    }

    [Fact]
    public void Activity_ShouldRecordEvents()
    {
        // Arrange & Act
        using var activity = new Activity("EventOperation").Start();
        var eventTags = new ActivityTagsCollection
        {
            { "event.type", "warning" },
            { "message", "Test warning event" }
        };
        activity!.AddEvent(new ActivityEvent("TestEvent", tags: eventTags));

        // Assert
        activity.Events.Should().HaveCount(1);
        activity.Events.First().Name.Should().Be("TestEvent");
    }

    [Fact]
    public void Activity_ShouldRecordExceptionAsEvent()
    {
        // Arrange & Act
        using var activity = new Activity("ExceptionOperation").Start();
        var exception = new InvalidOperationException("Test exception");

        var exceptionTags = new ActivityTagsCollection
        {
            { "exception.type", exception.GetType().Name },
            { "exception.message", exception.Message },
            { "exception.stacktrace", exception.StackTrace ?? string.Empty }
        };
        activity!.AddEvent(new ActivityEvent("exception", tags: exceptionTags));

        // Assert
        activity.Events.Should().HaveCount(1);
        activity.Events.First().Name.Should().Be("exception");
    }

    [Fact]
    public void ActivityListener_ShouldCaptureActivityLifecycle()
    {
        // Arrange
        var activities = new List<Activity>();
        var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = a => activities.Add(a),
        };

        ActivitySource.AddActivityListener(listener);

        var source = new ActivitySource("TestSource");

        // Act
        using var activity = source.StartActivity("TestOperation");
        activity?.SetTag("test", "value");

        // Assert
        activities.Should().HaveCount(1);
        activities[0].OperationName.Should().Be("TestOperation");
    }
}
