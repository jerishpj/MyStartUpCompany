using Microsoft.AspNetCore.Mvc;
using MyStartUpCompany.Api.Features.Buildings.Models;
using MyStartUpCompany.Api.Features.Buildings.Queries;
using MyStartUpCompany.Api.Shared.Models;

namespace MyStartUpCompany.Api.Features.Buildings;

/// <summary>
/// Building management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BuildingsController : ControllerBase
{
    private readonly IGetBuildingQueryHandler _getBuildingHandler;
    private readonly IGetAllBuildingsQueryHandler _getAllBuildingsHandler;
    private readonly IGetFilteredBuildingsQueryHandler _getFilteredBuildingsHandler;
    private readonly ILogger<BuildingsController> _logger;

    public BuildingsController(
        IGetBuildingQueryHandler getBuildingHandler,
        IGetAllBuildingsQueryHandler getAllBuildingsHandler,
        IGetFilteredBuildingsQueryHandler getFilteredBuildingsHandler,
        ILogger<BuildingsController> logger)
    {
        _getBuildingHandler = getBuildingHandler;
        _getAllBuildingsHandler = getAllBuildingsHandler;
        _getFilteredBuildingsHandler = getFilteredBuildingsHandler;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a building by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the building</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The building details if found</returns>
    /// <response code="200">Returns the building details</response>
    /// <response code="404">If the building is not found</response>
    /// <response code="400">If the id is invalid</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Building), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBuilding(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for building with ID: {BuildingId}", id);

        var building = await _getBuildingHandler.HandleAsync(id, cancellationToken);

        return Ok(building);
    }

    /// <summary>
    /// Retrieves all buildings
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all buildings</returns>
    /// <response code="200">Returns the list of buildings</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Building>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBuildings(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for all buildings");

        var buildings = await _getAllBuildingsHandler.HandleAsync(cancellationToken);

        return Ok(buildings);
    }

    /// <summary>
    /// Retrieves filtered and paginated buildings
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of buildings matching the filter criteria</returns>
    /// <response code="200">Returns the paginated list of buildings</response>
    /// <response code="400">If the request parameters are invalid</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<Building>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFilteredBuildings(
        [FromQuery] BuildingRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "GET request received for filtered buildings. SearchTerm: {SearchTerm}, " +
            "LocationId: {LocationId}, PageNumber: {PageNumber}",
            request.SearchTerm, request.LocationId, request.PageNumber);

        var result = await _getFilteredBuildingsHandler.HandleAsync(request, cancellationToken);

        return Ok(result);
    }
}
