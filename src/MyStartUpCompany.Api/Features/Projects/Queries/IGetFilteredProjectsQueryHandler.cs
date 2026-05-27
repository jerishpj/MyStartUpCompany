using MyStartUpCompany.Api.Features.Projects.Models;
using MyStartUpCompany.Api.Shared.Models;

namespace MyStartUpCompany.Api.Features.Projects.Queries;

/// <summary>
/// Query handler for retrieving filtered and paginated projects
/// </summary>
public interface IGetFilteredProjectsQueryHandler
{
    /// <summary>
    /// Handles the query to get filtered and paginated projects
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated result of projects matching the filter criteria</returns>
    Task<PagedResult<ProjectResponse>> HandleAsync(
        ProjectFilterRequest request,
        CancellationToken cancellationToken = default);
}
