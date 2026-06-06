using MyStartUpCompany.Api.Features.Projects.Models;
using MyStartUpCompany.Persistence.Entities;
using MyStartUpCompany.Persistence.Entities.ValueObjects;

namespace MyStartUpCompany.Api.Features.Projects.Queries;

/// <summary>
/// Mapping extensions for project entities within the Projects feature.
/// These mappings are scoped to this feature for proper vertical slice encapsulation.
/// 
/// Architecture Note:
/// - These extensions use database-level LINQ projections for efficiency
/// - They are marked 'internal' to prevent cross-feature dependencies
/// - Maintains feature independence while preserving performance benefits
/// </summary>
internal static class ProjectMappingExtensions
{
    /// <summary>
    /// Projects a queryable collection of projects to ProjectResponse DTOs at the database level.
    /// </summary>
    public static IQueryable<ProjectResponse> ProjectToProjectResponse(
        this IQueryable<Project> query)
    {
        return query.Select(p => new ProjectResponse
        {
            Id = p.Id,
            ProjectIdentifier = p.ProjectIdentifier,
            Name = p.Name,
            Code = p.Code,
            Location = p.Location,
            CompanyId = p.CompanyId,
            Type = p.Type.ToString(),
            Details = new ProjectDetailsDto
            {
                Budget = p.Details.Budget,
                Status = p.Details.Status,
                StartDate = p.Details.StartDate,
                EndDate = p.Details.EndDate,
                Description = p.Details.Description,
                ProjectManager = p.Details.ProjectManager,
                TeamMembers = p.Details.TeamMembers,
                Priority = p.Details.Priority,
                Tags = p.Details.Tags,
                Metrics = p.Details.Metrics,
                Metadata = p.Details.Metadata,
                ProgressPercentage = p.Details.ProgressPercentage,
                Notes = p.Details.Notes,
                BudgetSpent = p.Details.BudgetSpent,
                Outcome = p.Details.Outcome,
                RiskLevel = p.Details.RiskLevel,
                Deliverables = p.Details.Deliverables,
                Dependencies = p.Details.Dependencies
            },
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });
    }

    /// <summary>
    /// Maps a single project entity to a ProjectResponse DTO.
    /// </summary>
    public static ProjectResponse MapToProjectResponse(this Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        return new ProjectResponse
        {
            Id = project.Id,
            ProjectIdentifier = project.ProjectIdentifier,
            Name = project.Name,
            Code = project.Code,
            Location = project.Location,
            CompanyId = project.CompanyId,
            Type = project.Type.ToString(),
            Details = MapProjectDetails(project.Details),
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }

    /// <summary>
    /// Maps project details value object to ProjectDetailsDto.
    /// </summary>
    private static ProjectDetailsDto MapProjectDetails(ProjectDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);

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
