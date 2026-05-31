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

            var projects = await _dbContext.Projects
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);

            var responses = projects.Select(p => new ProjectResponse
            {
                Id = p.Id,
                ProjectIdentifier = p.ProjectIdentifier,
                Name = p.Name,
                Code = p.Code,
                Location = p.Location,
                CompanyId = p.CompanyId,
                Type = p.Type.ToString(),
                Details = MapProjectDetails(p.Details),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

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

    /// <summary>
    /// Maps ProjectDetails value object to ProjectDetailsDto
    /// </summary>
    private static ProjectDetailsDto MapProjectDetails(Persistence.Entities.ValueObjects.ProjectDetails details)
    {
        return new ProjectDetailsDto
        {
            Budget = details.Budget,
            Status = details.Status,
            StartDate = details.StartDate,
            EndDate = details.EndDate,
            Description = details.Description,
            ProjectManager = details.ProjectManager,
            TeamMembers = details.TeamMembers,
            Priority = details.Priority,
            Tags = details.Tags,
            Metrics = details.Metrics,
            Metadata = details.Metadata,
            ProgressPercentage = details.ProgressPercentage,
            Notes = details.Notes,
            BudgetSpent = details.BudgetSpent,
            Outcome = details.Outcome,
            RiskLevel = details.RiskLevel,
            Deliverables = details.Deliverables,
            Dependencies = details.Dependencies
        };
    }
}
