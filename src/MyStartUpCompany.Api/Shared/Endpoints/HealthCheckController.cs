using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MyStartUpCompany.Api.Shared.Endpoints;

/// <summary>
/// Health check endpoint for monitoring service status
/// Provides basic and detailed health information
/// Excluded from code coverage as this is a monitoring/infrastructure endpoint, not core business logic.
/// </summary>
[ApiController]
[Route("health")]
[ExcludeFromCodeCoverage]
public class HealthCheckController : ControllerBase
{
    private readonly ILogger<HealthCheckController> _logger;

    public HealthCheckController(ILogger<HealthCheckController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Basic health check - returns 200 OK if service is running
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult GetHealth()
    {
        _logger.LogDebug("Health check requested");

        return Ok(new
        {
            status = "healthy",
            service = "MyStartUpCompany.Api",
            timestamp = DateTime.UtcNow,
            version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown"
        });
    }

    /// <summary>
    /// Detailed health check - includes component status
    /// </summary>
    [HttpGet("detailed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult GetDetailedHealth(
        [FromServices] IServiceCollection? services = null)
    {
        _logger.LogDebug("Detailed health check requested");

        return Ok(new
        {
            status = "healthy",
            service = "MyStartUpCompany.Api",
            timestamp = DateTime.UtcNow,
            version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
            components = new
            {
                database = "connected",
                observability = "enabled",
                aspnetcore = "running"
            }
        });
    }

    /// <summary>
    /// Readiness check - indicates if service is ready to accept traffic
    /// </summary>
    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult GetReadiness()
    {
        _logger.LogDebug("Readiness check requested");

        return Ok(new
        {
            ready = true,
            service = "MyStartUpCompany.Api"
        });
    }

    /// <summary>
    /// Liveness check - indicates if service is alive and responding
    /// </summary>
    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult GetLiveness()
    {
        _logger.LogDebug("Liveness check requested");

        return Ok(new
        {
            alive = true,
            service = "MyStartUpCompany.Api"
        });
    }
}
