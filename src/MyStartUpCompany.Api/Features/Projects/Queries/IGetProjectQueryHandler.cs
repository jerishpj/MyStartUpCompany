using MyStartUpCompany.Api.Features.Projects.Models;

namespace MyStartUpCompany.Api.Features.Projects.Queries;

/// <summary>
/// Query handler for retrieving a single project by ID
/// </summary>
public interface IGetProjectQueryHandler
{
    /// <summary>
    /// Handles the query to get a project by its ID
    /// </summary>
    /// <param name="id">The project ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The project response if found; throws exception if not found</returns>
    Task<ProjectResponse> HandleAsync(
        int id,
        CancellationToken cancellationToken = default);
}
