using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace MyStartUpCompany.Notifier.Shared.Endpoints;

/// <summary>
/// Health check endpoint for notifier service
/// </summary>
[ApiController]
[Route("health")]
public class HealthCheckController : ControllerBase
{
    private readonly ILogger<HealthCheckController> _logger;

    public HealthCheckController(ILogger<HealthCheckController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Basic health check
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        _logger.LogDebug("Health check requested");

        return Ok(new
        {
            status = "healthy",
            service = "MyStartUpCompany.Notifier",
            timestamp = DateTime.UtcNow,
            version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown"
        });
    }

    /// <summary>
    /// Readiness check
    /// </summary>
    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetReadiness()
    {
        _logger.LogDebug("Readiness check requested");
        return Ok(new { ready = true, service = "MyStartUpCompany.Notifier" });
    }

    /// <summary>
    /// Liveness check
    /// </summary>
    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetLiveness()
    {
        _logger.LogDebug("Liveness check requested");
        return Ok(new { alive = true, service = "MyStartUpCompany.Notifier" });
    }
}
