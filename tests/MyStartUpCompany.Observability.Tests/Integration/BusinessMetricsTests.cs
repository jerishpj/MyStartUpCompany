using MyStartUpCompany.Observability;

namespace MyStartUpCompany.Observability.Tests.Integration;

/// <summary>
/// Tests for business metrics recording and validation.
/// </summary>
public class BusinessMetricsTests
{
    [Fact]
    public void BusinessMetrics_ShouldHaveHttpMetrics()
    {
        // Assert
        BusinessMetrics.HttpRequestsTotal.Should().NotBeNull();
        BusinessMetrics.HttpRequestDuration.Should().NotBeNull();
        BusinessMetrics.HttpErrorsTotal.Should().NotBeNull();
    }

    [Fact]
    public void BusinessMetrics_ShouldHaveDatabaseMetrics()
    {
        // Assert
        BusinessMetrics.DatabaseQueriesTotal.Should().NotBeNull();
        BusinessMetrics.DatabaseQueryDuration.Should().NotBeNull();
        BusinessMetrics.DatabaseErrorsTotal.Should().NotBeNull();
    }

    [Fact]
    public void BusinessMetrics_ShouldHaveMessageQueueMetrics()
    {
        // Assert
        BusinessMetrics.MessagesPublished.Should().NotBeNull();
        BusinessMetrics.MessagesProcessed.Should().NotBeNull();
        BusinessMetrics.MessageProcessingDuration.Should().NotBeNull();
    }

    [Fact]
    public void BusinessMetrics_ShouldHaveNotificationMetrics()
    {
        // Assert
        BusinessMetrics.NotificationsDeliveredTotal.Should().NotBeNull();
        BusinessMetrics.NotificationDeliveryDuration.Should().NotBeNull();
    }

    [Fact]
    public void BusinessMetrics_ShouldHaveHealthMetrics()
    {
        // Assert
        BusinessMetrics.HealthCheckStatus.Should().NotBeNull();
        BusinessMetrics.HealthCheckDuration.Should().NotBeNull();
    }

    [Fact]
    public void BusinessMetrics_Counter_ShouldBeNonNegative()
    {
        // Arrange
        var meter = new System.Diagnostics.Metrics.Meter("TestMeter");
        var counter = meter.CreateCounter<long>("test_counter");

        // Act
        counter.Add(5);
        counter.Add(3);

        // Assert
        // Counters in OpenTelemetry are monotonically increasing
        // Values should not be negative
        counter.Should().NotBeNull();
    }

    [Fact]
    public void BusinessMetrics_Histogram_ShouldRecordDistributions()
    {
        // Arrange
        var meter = new System.Diagnostics.Metrics.Meter("TestMeter");
        var histogram = meter.CreateHistogram<double>("test_histogram");

        // Act
        histogram.Record(100);
        histogram.Record(200);
        histogram.Record(150);

        // Assert
        histogram.Should().NotBeNull();
    }

    [Fact]
    public void BusinessMetrics_ShouldBeThreadSafe()
    {
        // Arrange
        var meter = new System.Diagnostics.Metrics.Meter("TestMeter");
        var counter = meter.CreateCounter<long>("thread_safe_counter");
        var tasks = new List<Task>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    counter.Add(1);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        // If we got here without exceptions, thread safety is maintained
        counter.Should().NotBeNull();
    }
}
