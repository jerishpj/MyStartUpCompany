using MyStartUpCompany.Api.Features.Projects.Models;

namespace MyStartUpCompany.Api.Features.Projects.Queries;

/// <summary>
/// Query handler for retrieving all projects without filtering
/// </summary>
public interface IGetAllProjectsQueryHandler
{
    /// <summary>
    /// Handles the query to get all projects
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of all projects</returns>
    Task<IEnumerable<ProjectResponse>> HandleAsync(
        CancellationToken cancellationToken = default);
}
