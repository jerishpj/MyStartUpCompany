using Microsoft.AspNetCore.Mvc;
using MyStartUpCompany.Api.Features.Offices.Models;
using MyStartUpCompany.Api.Features.Offices.Queries;
using MyStartUpCompany.Api.Shared.Models;

namespace MyStartUpCompany.Api.Features.Offices;

/// <summary>
/// Office management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OfficesController : ControllerBase
{
    private readonly IGetOfficeQueryHandler _getOfficeHandler;
    private readonly IGetAllOfficesQueryHandler _getAllOfficesHandler;
    private readonly IGetFilteredOfficesQueryHandler _getFilteredOfficesHandler;
    private readonly ILogger<OfficesController> _logger;

    public OfficesController(
        IGetOfficeQueryHandler getOfficeHandler,
        IGetAllOfficesQueryHandler getAllOfficesHandler,
        IGetFilteredOfficesQueryHandler getFilteredOfficesHandler,
        ILogger<OfficesController> logger)
    {
        _getOfficeHandler = getOfficeHandler;
        _getAllOfficesHandler = getAllOfficesHandler;
        _getFilteredOfficesHandler = getFilteredOfficesHandler;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves an office by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the office</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The office details if found</returns>
    /// <response code="200">Returns the office details</response>
    /// <response code="404">If the office is not found</response>
    /// <response code="400">If the id is invalid</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Office), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOffice(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for office with ID: {OfficeId}", id);

        var office = await _getOfficeHandler.HandleAsync(id, cancellationToken);

        return Ok(office);
    }

    /// <summary>
    /// Retrieves all offices
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all offices</returns>
    /// <response code="200">Returns the list of offices</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Office>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOffices(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for all offices");

        var offices = await _getAllOfficesHandler.HandleAsync(cancellationToken);

        return Ok(offices);
    }

    /// <summary>
    /// Retrieves filtered and paginated offices
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of offices matching the filter criteria</returns>
    /// <response code="200">Returns the paginated list of offices</response>
    /// <response code="400">If the request parameters are invalid</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<Office>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFilteredOffices(
        [FromQuery] OfficeRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "GET request received for filtered offices. SearchTerm: {SearchTerm}, " +
            "BuildingId: {BuildingId}, Department: {Department}, PageNumber: {PageNumber}",
            request.SearchTerm, request.BuildingId, request.Department, request.PageNumber);

        var result = await _getFilteredOfficesHandler.HandleAsync(request, cancellationToken);

        return Ok(result);
    }
}
