using Microsoft.EntityFrameworkCore;
using MyStartUpCompany.Api.Features.Projects.Models;
using MyStartUpCompany.Api.Shared.Exceptions;
using MyStartUpCompany.Persistence;

namespace MyStartUpCompany.Api.Features.Projects.Queries;

/// <summary>
/// Implementation of query handler for retrieving a single project by ID
/// </summary>
public class GetProjectQueryHandler : IGetProjectQueryHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GetProjectQueryHandler> _logger;

    public GetProjectQueryHandler(
        AppDbContext dbContext,
        ILogger<GetProjectQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a project by its ID and returns as ProjectResponse DTO
    /// </summary>
    public async Task<ProjectResponse> HandleAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving project with ID: {ProjectId}", id);

        var project = await _dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (project is null)
        {
            _logger.LogWarning("Project with ID: {ProjectId} not found", id);
            throw new NotFoundException($"Project with ID {id} not found");
        }

        var response = new ProjectResponse
        {
            Id = project.Id,
            ProjectIdentifier = project.ProjectIdentifier,
            Name = project.Name,
            Code = project.Code,
            Location = project.Location,
            CompanyId = project.CompanyId,
            Details = MapProjectDetails(project.Details),
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        _logger.LogInformation("Successfully retrieved project: {ProjectIdentifier}", project.ProjectIdentifier);

        return response;
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
