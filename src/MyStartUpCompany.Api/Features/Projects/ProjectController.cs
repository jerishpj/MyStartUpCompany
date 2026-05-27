using Microsoft.AspNetCore.Mvc;
using MyStartUpCompany.Api.Features.Projects.Models;
using MyStartUpCompany.Api.Features.Projects.Queries;
using MyStartUpCompany.Api.Shared.Models;

namespace MyStartUpCompany.Api.Features.Projects;

/// <summary>
/// Project management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectController : ControllerBase
{
    private readonly IGetProjectQueryHandler _getProjectHandler;
    private readonly IGetAllProjectsQueryHandler _getAllProjectsHandler;
    private readonly IGetFilteredProjectsQueryHandler _getFilteredProjectsHandler;
    private readonly ILogger<ProjectController> _logger;

    public ProjectController(
        IGetProjectQueryHandler getProjectHandler,
        IGetAllProjectsQueryHandler getAllProjectsHandler,
        IGetFilteredProjectsQueryHandler getFilteredProjectsHandler,
        ILogger<ProjectController> logger)
    {
        _getProjectHandler = getProjectHandler;
        _getAllProjectsHandler = getAllProjectsHandler;
        _getFilteredProjectsHandler = getFilteredProjectsHandler;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a project by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the project</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The project details if found</returns>
    /// <response code="200">Returns the project details</response>
    /// <response code="404">If the project is not found</response>
    /// <response code="400">If the id is invalid</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProject(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for project with ID: {ProjectId}", id);

        var project = await _getProjectHandler.HandleAsync(id, cancellationToken);

        return Ok(project);
    }

    /// <summary>
    /// Retrieves all projects
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all projects</returns>
    /// <response code="200">Returns the list of projects</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProjects(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for all projects");

        var projects = await _getAllProjectsHandler.HandleAsync(cancellationToken);

        return Ok(projects);
    }

    /// <summary>
    /// Retrieves filtered and paginated projects
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of projects matching the filter criteria</returns>
    /// <response code="200">Returns the paginated list of projects</response>
    /// <response code="400">If the request parameters are invalid</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<ProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFilteredProjects(
        [FromQuery] ProjectFilterRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for filtered projects");

        var result = await _getFilteredProjectsHandler.HandleAsync(request, cancellationToken);

        return Ok(result);
    }
}
