using Microsoft.AspNetCore.Mvc;
using MyStartUpCompany.Api.Features.Locations.Models;
using MyStartUpCompany.Api.Features.Locations.Queries;
using MyStartUpCompany.Api.Shared.Models;

namespace MyStartUpCompany.Api.Features.Locations;

/// <summary>
/// Location management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LocationsController : ControllerBase
{
    private readonly IGetLocationQueryHandler _getLocationHandler;
    private readonly IGetAllLocationsQueryHandler _getAllLocationsHandler;
    private readonly IGetFilteredLocationsQueryHandler _getFilteredLocationsHandler;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(
        IGetLocationQueryHandler getLocationHandler,
        IGetAllLocationsQueryHandler getAllLocationsHandler,
        IGetFilteredLocationsQueryHandler getFilteredLocationsHandler,
        ILogger<LocationsController> logger)
    {
        _getLocationHandler = getLocationHandler;
        _getAllLocationsHandler = getAllLocationsHandler;
        _getFilteredLocationsHandler = getFilteredLocationsHandler;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a location by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the location</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The location details if found</returns>
    /// <response code="200">Returns the location details</response>
    /// <response code="404">If the location is not found</response>
    /// <response code="400">If the id is invalid</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Location), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLocation(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for location with ID: {LocationId}", id);

        var location = await _getLocationHandler.HandleAsync(id, cancellationToken);

        return Ok(location);
    }

    /// <summary>
    /// Retrieves all locations
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all locations</returns>
    /// <response code="200">Returns the list of locations</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Location>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllLocations(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for all locations");

        var locations = await _getAllLocationsHandler.HandleAsync(cancellationToken);

        return Ok(locations);
    }

    /// <summary>
    /// Retrieves filtered and paginated locations
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of locations matching the filter criteria</returns>
    /// <response code="200">Returns the paginated list of locations</response>
    /// <response code="400">If the request parameters are invalid</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<Location>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFilteredLocations(
        [FromQuery] LocationRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "GET request received for filtered locations. SearchTerm: {SearchTerm}, " +
            "CompanyId: {CompanyId}, PageNumber: {PageNumber}",
            request.SearchTerm, request.CompanyId, request.PageNumber);

        var result = await _getFilteredLocationsHandler.HandleAsync(request, cancellationToken);

        return Ok(result);
    }
}
