using Microsoft.AspNetCore.Mvc;
using MyStartUpCompany.Api.Features.Admin.Utilities;

namespace MyStartUpCompany.Api.Features.Admin.Controllers;

/// <summary>
/// Endpoints for performance benchmarking and monitoring.
/// These endpoints help track query performance improvements and identify regressions.
/// </summary>
[ApiController]
[Route("api/admin/benchmarks")]
[Produces("application/json")]
public class BenchmarkController : ControllerBase
{
    private readonly OfficeSearchBenchmark _benchmark;
    private readonly ILogger<BenchmarkController> _logger;

    public BenchmarkController(
        OfficeSearchBenchmark benchmark,
        ILogger<BenchmarkController> logger)
    {
        _benchmark = benchmark;
        _logger = logger;
    }

    /// <summary>
    /// Runs comprehensive performance benchmark comparing denormalized vs normalized queries.
    /// This endpoint executes multiple search scenarios with various filter combinations
    /// to measure the performance impact of denormalization.
    /// </summary>
    /// <remarks>
    /// The benchmark runs the following scenarios:
    /// 1. Search by Building Name
    /// 2. Search by Location City
    /// 3. Search by Country + Region
    /// 4. Combined Building + Department
    /// 5. Combined Location + Department
    ///
    /// Each scenario runs multiple iterations (10-20) comparing:
    /// - **Denormalized Query** (Optimized): Single-table scan using composite indexes
    /// - **Normalized Query** (Joins): Multi-table joins for comparison
    ///
    /// Expected Results:
    /// - Denormalized should be 20-50% faster for typical datasets
    /// - 50-70% faster for large datasets (1M+ offices)
    ///
    /// **IMPORTANT**: This endpoint should be restricted to administrators only.
    /// **WARNING**: This operation performs heavy querying and may impact performance. Run during off-peak hours.
    /// 
    /// Duration: ~30-60 seconds depending on dataset size
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed benchmark results with performance metrics</returns>
    /// <response code="200">Benchmark completed successfully</response>
    /// <response code="500">Internal server error during benchmark</response>
    [HttpPost("run-office-search-suite")]
    public async Task<IActionResult> RunOfficeSearchBenchmark(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Admin initiated Office search performance benchmark");

            var results = await _benchmark.RunBenchmarkSuiteAsync(cancellationToken);
            var report = _benchmark.GeneratePerformanceReport(results);

            _logger.LogInformation("Benchmark completed successfully. Results: {ResultCount}", results.Count);

            return Ok(new BenchmarkSuiteResponse
            {
                TotalIterations = results.Count,
                ExecutedAt = DateTime.UtcNow,
                Results = results,
                FormattedReport = report
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Office search benchmark");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while running the benchmark" });
        }
    }

    /// <summary>
    /// Gets the formatted performance report only (without raw data).
    /// Useful for quick performance review without downloading large result sets.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Formatted performance report as text</returns>
    /// <response code="200">Report generated successfully</response>
    [HttpPost("run-office-search-suite/report")]
    [Produces("text/plain")]
    public async Task<IActionResult> RunOfficeSearchBenchmarkReport(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Admin requested Office search benchmark report");

            var results = await _benchmark.RunBenchmarkSuiteAsync(cancellationToken);
            var report = _benchmark.GeneratePerformanceReport(results);

            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating benchmark report");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while generating the benchmark report" });
        }
    }
}

/// <summary>
/// Response DTO for benchmark suite results
/// </summary>
public record BenchmarkSuiteResponse
{
    /// <summary>
    /// Total number of queries executed during benchmark
    /// </summary>
    public int TotalIterations { get; init; }

    /// <summary>
    /// Timestamp when benchmark was executed
    /// </summary>
    public DateTime ExecutedAt { get; init; }

    /// <summary>
    /// Detailed results for each query execution
    /// </summary>
    public List<OfficeSearchBenchmark.BenchmarkResult> Results { get; init; }

    /// <summary>
    /// Human-readable formatted performance report
    /// </summary>
    public string FormattedReport { get; init; }
}
