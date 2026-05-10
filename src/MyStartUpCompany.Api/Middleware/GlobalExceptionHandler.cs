using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace MyStartUpCompany.Api.Middleware;

/// <summary>
/// Global exception handler for the API
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var problemDetails = new
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An error occurred while processing your request",
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}