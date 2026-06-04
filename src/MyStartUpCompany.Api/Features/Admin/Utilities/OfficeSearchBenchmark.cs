using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Persistence;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Api.Features.Admin.Utilities;

/// <summary>
/// Performance benchmark utility for Office search queries.
/// Compares denormalized (optimized) vs normalized (join-based) query performance.
/// </summary>
public class OfficeSearchBenchmark
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<OfficeSearchBenchmark> _logger;

    public OfficeSearchBenchmark(AppDbContext dbContext, ILogger<OfficeSearchBenchmark> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Benchmark result for a single query
    /// </summary>
    public record BenchmarkResult
    {
        public string QueryName { get; init; }
        public string QueryType { get; init; } // "Denormalized" or "Normalized"
        public int RecordsReturned { get; init; }
        public long ElapsedMilliseconds { get; init; }
        public double ThroughputPerSecond { get; init; }
        public DateTime ExecutedAt { get; init; }
    }

    /// <summary>
    /// Benchmark scenario with parameters
    /// </summary>
    public record BenchmarkScenario
    {
        public string ScenarioName { get; init; }
        public string? BuildingName { get; init; }
        public string? LocationCity { get; init; }
        public string? LocationRegion { get; init; }
        public string? LocationCountry { get; init; }
        public string? Department { get; init; }
        public int Iterations { get; init; } = 10;
    }

    /// <summary>
    /// Runs benchmark suite comparing denormalized vs normalized queries
    /// </summary>
    public async Task<List<BenchmarkResult>> RunBenchmarkSuiteAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Office search performance benchmark suite");

        var scenarios = new[]
        {
            new BenchmarkScenario
            {
                ScenarioName = "Search by Building Name",
                BuildingName = "Building A",
                Iterations = 20
            },
            new BenchmarkScenario
            {
                ScenarioName = "Search by Location City",
                LocationCity = "New York",
                Iterations = 20
            },
            new BenchmarkScenario
            {
                ScenarioName = "Search by Country + Region",
                LocationCountry = "United States",
                LocationRegion = "NY",
                Iterations = 15
            },
            new BenchmarkScenario
            {
                ScenarioName = "Combined Building + Department",
                BuildingName = "Building A",
                Department = "Sales",
                Iterations = 15
            },
            new BenchmarkScenario
            {
                ScenarioName = "Combined Location + Department",
                LocationCity = "New York",
                Department = "Engineering",
                Iterations = 15
            }
        };

        var results = new List<BenchmarkResult>();

        foreach (var scenario in scenarios)
        {
            _logger.LogInformation("Running scenario: {ScenarioName}", scenario.ScenarioName);

            // Denormalized query (optimized - uses single table + indexes)
            var denormResults = await RunDenormalizedQueryBenchmarkAsync(
                scenario, cancellationToken);
            results.AddRange(denormResults);

            // Normalized query (join-based - for comparison)
            var normResults = await RunNormalizedQueryBenchmarkAsync(
                scenario, cancellationToken);
            results.AddRange(normResults);

            _logger.LogInformation(
                "Scenario '{ScenarioName}' completed. " +
                "Denormalized avg: {DenormAvg}ms, Normalized avg: {NormAvg}ms",
                scenario.ScenarioName,
                denormResults.Average(r => r.ElapsedMilliseconds),
                normResults.Average(r => r.ElapsedMilliseconds));
        }

        return results;
    }

    /// <summary>
    /// Runs denormalized query benchmark (optimized path)
    /// </summary>
    private async Task<List<BenchmarkResult>> RunDenormalizedQueryBenchmarkAsync(
        BenchmarkScenario scenario,
        CancellationToken cancellationToken)
    {
        var results = new List<BenchmarkResult>();

        for (int i = 0; i < scenario.Iterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();

            var query = _dbContext.Offices.AsNoTracking();

            // Apply filters on denormalized fields
            if (!string.IsNullOrEmpty(scenario.BuildingName))
                query = query.Where(o => o.BuildingName != null && 
                    o.BuildingName.Contains(scenario.BuildingName));

            if (!string.IsNullOrEmpty(scenario.LocationCity))
                query = query.Where(o => o.LocationCity == scenario.LocationCity);

            if (!string.IsNullOrEmpty(scenario.LocationRegion))
                query = query.Where(o => o.LocationRegion == scenario.LocationRegion);

            if (!string.IsNullOrEmpty(scenario.LocationCountry))
                query = query.Where(o => o.LocationCountry == scenario.LocationCountry);

            if (!string.IsNullOrEmpty(scenario.Department))
                query = query.Where(o => o.Department == scenario.Department);

            var count = await query.CountAsync(cancellationToken);

            stopwatch.Stop();

            results.Add(new BenchmarkResult
            {
                QueryName = scenario.ScenarioName,
                QueryType = "Denormalized (Optimized)",
                RecordsReturned = count,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                ThroughputPerSecond = count > 0 ? (count * 1000.0) / stopwatch.ElapsedMilliseconds : 0,
                ExecutedAt = DateTime.UtcNow
            });
        }

        return results;
    }

    /// <summary>
    /// Runs normalized query benchmark (join-based path)
    /// Used for comparison to show performance improvement
    /// </summary>
    private async Task<List<BenchmarkResult>> RunNormalizedQueryBenchmarkAsync(
        BenchmarkScenario scenario,
        CancellationToken cancellationToken)
    {
        var results = new List<BenchmarkResult>();

        for (int i = 0; i < scenario.Iterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();

            IQueryable<Office> query = _dbContext.Offices
                .AsNoTracking()
                .Include(o => o.Building)
                .ThenInclude(b => b!.Location);

            // Apply filters using joined tables (normalized path)
            if (!string.IsNullOrEmpty(scenario.BuildingName))
                query = query.Where(o => o.Building != null && 
                    o.Building.Name.Contains(scenario.BuildingName));

            if (!string.IsNullOrEmpty(scenario.LocationCity))
                query = query.Where(o => o.Building != null && 
                    o.Building.Location != null &&
                    o.Building.Location.City == scenario.LocationCity);

            if (!string.IsNullOrEmpty(scenario.LocationRegion))
                query = query.Where(o => o.Building != null && 
                    o.Building.Location != null &&
                    o.Building.Location.Region == scenario.LocationRegion);

            if (!string.IsNullOrEmpty(scenario.LocationCountry))
                query = query.Where(o => o.Building != null && 
                    o.Building.Location != null &&
                    o.Building.Location.Country == scenario.LocationCountry);

            if (!string.IsNullOrEmpty(scenario.Department))
                query = query.Where(o => o.Department == scenario.Department);

            var count = await query.CountAsync(cancellationToken);

            stopwatch.Stop();

            results.Add(new BenchmarkResult
            {
                QueryName = scenario.ScenarioName,
                QueryType = "Normalized (Joins)",
                RecordsReturned = count,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                ThroughputPerSecond = count > 0 ? (count * 1000.0) / stopwatch.ElapsedMilliseconds : 0,
                ExecutedAt = DateTime.UtcNow
            });
        }

        return results;
    }

    /// <summary>
    /// Generates a formatted performance report
    /// </summary>
    public string GeneratePerformanceReport(List<BenchmarkResult> results)
    {
        var groupedByScenario = results.GroupBy(r => r.QueryName);

        var report = new System.Text.StringBuilder();
        report.AppendLine("╔════════════════════════════════════════════════════════════════════════════╗");
        report.AppendLine("║          OFFICE SEARCH PERFORMANCE BENCHMARK REPORT                        ║");
        report.AppendLine("╚════════════════════════════════════════════════════════════════════════════╝");
        report.AppendLine();

        foreach (var scenario in groupedByScenario)
        {
            report.AppendLine($"📊 Scenario: {scenario.Key}");
            report.AppendLine(new string('─', 80));

            var denormalized = scenario
                .Where(r => r.QueryType == "Denormalized (Optimized)")
                .ToList();

            var normalized = scenario
                .Where(r => r.QueryType == "Normalized (Joins)")
                .ToList();

            if (denormalized.Any())
            {
                var denormAvg = denormalized.Average(r => r.ElapsedMilliseconds);
                report.AppendLine($"  Denormalized (Optimized): {denormAvg:F2}ms avg");
                report.AppendLine($"    - Min: {denormalized.Min(r => r.ElapsedMilliseconds)}ms");
                report.AppendLine($"    - Max: {denormalized.Max(r => r.ElapsedMilliseconds)}ms");
            }

            if (normalized.Any())
            {
                var normAvg = normalized.Average(r => r.ElapsedMilliseconds);
                report.AppendLine($"  Normalized (Joins): {normAvg:F2}ms avg");
                report.AppendLine($"    - Min: {normalized.Min(r => r.ElapsedMilliseconds)}ms");
                report.AppendLine($"    - Max: {normalized.Max(r => r.ElapsedMilliseconds)}ms");
            }

            if (denormalized.Any() && normalized.Any())
            {
                var denormAvg = denormalized.Average(r => r.ElapsedMilliseconds);
                var normAvg = normalized.Average(r => r.ElapsedMilliseconds);
                var improvement = ((normAvg - denormAvg) / normAvg) * 100;

                report.AppendLine();
                if (improvement > 0)
                {
                    report.AppendLine($"  ✅ PERFORMANCE IMPROVEMENT: {improvement:F1}% faster");
                    report.AppendLine($"     ({normAvg / denormAvg:F1}x speedup)");
                }
                else
                {
                    report.AppendLine($"  ⚠️  Denormalized is slower by {Math.Abs(improvement):F1}%");
                }
            }

            report.AppendLine();
        }

        // Summary statistics
        var totalDenormTime = results
            .Where(r => r.QueryType == "Denormalized (Optimized)")
            .Sum(r => r.ElapsedMilliseconds);

        var totalNormTime = results
            .Where(r => r.QueryType == "Normalized (Joins)")
            .Sum(r => r.ElapsedMilliseconds);

        report.AppendLine("📈 Overall Summary");
        report.AppendLine(new string('─', 80));
        report.AppendLine($"  Total Denormalized Queries: {results.Count(r => r.QueryType == "Denormalized (Optimized)")}");
        report.AppendLine($"  Total Time (Denormalized): {totalDenormTime}ms");
        report.AppendLine();
        report.AppendLine($"  Total Normalized Queries: {results.Count(r => r.QueryType == "Normalized (Joins)")}");
        report.AppendLine($"  Total Time (Normalized): {totalNormTime}ms");
        report.AppendLine();

        if (totalDenormTime > 0 && totalNormTime > 0)
        {
            var overallImprovement = ((totalNormTime - totalDenormTime) / totalNormTime) * 100;
            report.AppendLine($"  🎯 Overall Speedup: {overallImprovement:F1}%");
            report.AppendLine($"     Time saved: {totalNormTime - totalDenormTime}ms");
        }

        report.AppendLine();
        report.AppendLine($"  Report Generated: {DateTime.UtcNow:O}");

        return report.ToString();
    }
}
