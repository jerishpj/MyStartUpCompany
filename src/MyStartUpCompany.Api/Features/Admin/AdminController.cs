using Microsoft.AspNetCore.Mvc;
using MyStartUpCompany.Api.Features.Admin.Queries;

namespace MyStartUpCompany.Api.Features.Admin;

/// <summary>
/// Admin endpoints for data maintenance and auditing.
/// These endpoints are typically restricted to administrators only.
/// </summary>
[ApiController]
[Route("api/admin")]
[Produces("application/json")]
public class AdminController : ControllerBase
{
    private readonly IOfficeDataConsistencyAuditQueryHandler _consistencyAuditHandler;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IOfficeDataConsistencyAuditQueryHandler consistencyAuditHandler,
        ILogger<AdminController> logger)
    {
        _consistencyAuditHandler = consistencyAuditHandler;
        _logger = logger;
    }

    /// <summary>
    /// Audits denormalized fields in the Office table for consistency.
    /// Returns a list of any inconsistencies found between denormalized values and source tables.
    /// </summary>
    /// <remarks>
    /// This endpoint performs a consistency check on all offices, comparing:
    /// - BuildingName (from Office) vs Building.Name
    /// - LocationCity (from Office) vs Location.City
    /// - LocationRegion (from Office) vs Location.Region
    /// - LocationCountry (from Office) vs Location.Country
    ///
    /// Use this endpoint periodically to verify data integrity. Inconsistencies may occur if:
    /// 1. Database triggers fail silently
    /// 2. Manual SQL updates bypass application logic
    /// 3. Data corruption or migration issues
    ///
    /// **IMPORTANT**: This endpoint should be restricted to administrators only (add [Authorize(Roles = "Admin")])
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of inconsistent office records</returns>
    /// <response code="200">Audit completed successfully</response>
    /// <response code="500">Internal server error during audit</response>
    [HttpGet("office-consistency/audit")]
    public async Task<IActionResult> AuditOfficeConsistency(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Admin requested Office consistency audit");
            var results = await _consistencyAuditHandler.AuditDenormalizedFieldsAsync(cancellationToken);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Office consistency audit");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while auditing office consistency" });
        }
    }

    /// <summary>
    /// Repairs inconsistent denormalized fields in the Office table.
    /// Syncs denormalized columns with their source data from Building and Location tables.
    /// </summary>
    /// <remarks>
    /// This endpoint should be run as a periodic maintenance job (e.g., daily or weekly).
    /// It performs a bulk update to fix any inconsistencies found.
    ///
    /// Common scenarios where repair is needed:
    /// 1. After restoring from backup where triggers may not have been re-created
    /// 2. After bulk imports via direct SQL
    /// 3. After data correction operations on Building or Location tables
    ///
    /// **IMPORTANT**: This endpoint should be restricted to administrators only (add [Authorize(Roles = "Admin")])
    /// **CAUTION**: Ensure the endpoint is protected - this performs bulk updates.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of records repaired</returns>
    /// <response code="200">Repair completed successfully</response>
    /// <response code="500">Internal server error during repair</response>
    [HttpPost("office-consistency/repair")]
    public async Task<IActionResult> RepairOfficeConsistency(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Admin requested Office consistency repair");
            var repairCount = await _consistencyAuditHandler.RepairInconsistentFieldsAsync(cancellationToken);
            return Ok(new RepairResult
            {
                RecordsRepaired = repairCount,
                Message = $"Successfully repaired {repairCount} office records"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Office consistency repair");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while repairing office consistency" });
        }
    }
}

/// <summary>
/// Response DTO for repair operation result
/// </summary>
public record RepairResult
{
    /// <summary>
    /// Number of records that were repaired
    /// </summary>
    public int RecordsRepaired { get; init; }

    /// <summary>
    /// Human-readable message about the repair operation
    /// </summary>
    public string Message { get; init; }
}
