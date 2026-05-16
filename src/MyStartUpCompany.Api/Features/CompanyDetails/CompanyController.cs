using Microsoft.AspNetCore.Mvc;
using MyStartUpCompany.Api.Features.CompanyDetails.Models;
using MyStartUpCompany.Api.Features.CompanyDetails.Queries;
using MyStartUpCompany.Api.Shared.Models;

namespace MyStartUpCompany.Api.Features.CompanyDetails;

/// <summary>
/// Company management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CompanyController : ControllerBase
{
    private readonly IGetCompanyQueryHandler _getCompanyHandler;
    private readonly IGetAllCompaniesQueryHandler _getAllCompaniesHandler;
    private readonly IGetFilteredCompaniesQueryHandler _getFilteredCompaniesHandler;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(
        IGetCompanyQueryHandler getCompanyHandler,
        IGetAllCompaniesQueryHandler getAllCompaniesQueryHandler,
        IGetFilteredCompaniesQueryHandler getFilteredCompaniesHandler,
        ILogger<CompanyController> logger)
    {
        _getCompanyHandler = getCompanyHandler;
        _getAllCompaniesHandler = getAllCompaniesQueryHandler;
        _getFilteredCompaniesHandler = getFilteredCompaniesHandler;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a company by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the company</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The company details if found</returns>
    /// <response code="200">Returns the company details</response>
    /// <response code="404">If the company is not found</response>
    /// <response code="400">If the id is invalid</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCompany(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for company with ID: {CompanyId}", id);

        var company = await _getCompanyHandler.HandleAsync(id, cancellationToken);

        return Ok(company);
    }

    /// <summary>
    /// Retrieves all companies
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all companies</returns>
    /// <response code="200">Returns the list of companies</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Company>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCompanies(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for all companies");

        var companies = await _getAllCompaniesHandler.HandleAsync(cancellationToken);

        return Ok(companies);
    }

    /// <summary>
    /// Retrieves filtered and paginated companies
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of companies matching the filter criteria</returns>
    /// <response code="200">Returns the paginated list of companies</response>
    /// <response code="400">If the request parameters are invalid</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<Company>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFilteredCompanies(
        [FromQuery] CompanyRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for filtered companies");

        var result = await _getFilteredCompaniesHandler.HandleAsync(request, cancellationToken);

        return Ok(result);
    }
}
