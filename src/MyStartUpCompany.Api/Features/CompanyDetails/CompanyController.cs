using Microsoft.AspNetCore.Mvc;
using MyStartUpCompany.Api.Features.CompanyDetails.Queries;

namespace MyStartUpCompany.Api.Features.CompanyDetails;

/// <summary>
/// Company management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CompanyController : ControllerBase
{
    private readonly GetCompanyQueryHandler _getCompanyHandler;
    private readonly GetAllCompaniesQueryHandler _getAllCompaniesHandler;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(
        GetCompanyQueryHandler getCompanyHandler,
        GetAllCompaniesQueryHandler getAllCompaniesHandler,
        ILogger<CompanyController> logger)
    {
        _getCompanyHandler = getCompanyHandler;
        _getAllCompaniesHandler = getAllCompaniesHandler;
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
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompanyDto>> GetCompany(
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
    [ProducesResponseType(typeof(IEnumerable<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllCompanies(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET request received for all companies");

        var companies = await _getAllCompaniesHandler.HandleAsync(cancellationToken);

        return Ok(companies);
    }
}
