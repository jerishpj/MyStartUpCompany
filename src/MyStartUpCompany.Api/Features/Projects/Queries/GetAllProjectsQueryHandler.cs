using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Projects.Models;
using MyStartUpCompany.Observability;
using MyStartUpCompany.Persistence;
using System.Diagnostics;

namespace MyStartUpCompany.Api.Features.Projects.Queries;

/// <summary>
/// Implementation of query handler for retrieving all projects
/// </summary>
public class GetAllProjectsQueryHandler : IGetAllProjectsQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetAllProjectsQueryHandler> _logger;

    public GetAllProjectsQueryHandler(
        AppDbContext dbContext,
        ILogger<GetAllProjectsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all projects ordered by name
    /// </summary>
    public async Task<IEnumerable<ProjectResponse>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Retrieving all projects");

            // Project-to-response mapping is done at database level for optimal performance
            var responses = await _dbContext.Projects
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ProjectToProjectResponse()
                .ToListAsync(cancellationToken);

            stopwatch.Stop();

            // Record metrics
            BusinessMetrics.RecordDbQuery("GetAllProjects", responses.Count, stopwatch.ElapsedMilliseconds);

            _logger.LogInformation("Retrieved {Count} projects in {ElapsedMs}ms", 
                responses.Count, stopwatch.ElapsedMilliseconds);

            return responses;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            BusinessMetrics.RecordDbQueryError("GetAllProjects", stopwatch.ElapsedMilliseconds);
            _logger.LogError(ex, "Error retrieving all projects");
            throw;
        }
    }
}
