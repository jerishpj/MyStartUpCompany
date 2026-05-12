using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MyStartUpCompany.Api.Shared.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {

        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;
        public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
        {
            var (statusCode, title, problemDetails) = exception switch
            {
                NotFoundException notFoundEx => HandleNotFoundException(httpContext, notFoundEx),
                BadRequestException badRequestEx => HandleBadRequestException(httpContext, badRequestEx),
                ValidationException validationEx => HandleValidationException(httpContext, validationEx),
                _ => HandleUnknownException(httpContext, exception)
            };

            _logger.LogError(exception,
                "Exception occurred: {ExceptionType} - {Message}",
                exception.GetType().Name,
                exception.Message);

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private (int StatusCode, string Title, ProblemDetails ProblemDetails) HandleNotFoundException(
        HttpContext httpContext,
        NotFoundException exception)
        {
            var statusCode = (int)HttpStatusCode.NotFound;
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Resource Not Found",
                Detail = exception.Message,
                Instance = httpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            };

            return (statusCode, "Not Found", problemDetails);
        }

        private (int StatusCode, string Title, ProblemDetails ProblemDetails) HandleBadRequestException(
        HttpContext httpContext,
        BadRequestException exception)
        {
            var statusCode = (int)HttpStatusCode.BadRequest;
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Bad Request",
                Detail = exception.Message,
                Instance = httpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            if (exception.Errors != null)
            {
                problemDetails.Extensions["errors"] = exception.Errors;
            }

            return (statusCode, "Bad Request", problemDetails);
        }

        private (int StatusCode, string Title, ProblemDetails ProblemDetails) HandleValidationException(
        HttpContext httpContext,
        ValidationException exception)
        {
            var statusCode = (int)HttpStatusCode.UnprocessableEntity;
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Validation Error",
                Detail = exception.Message,
                Instance = httpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc4918#section-11.2"
            };

            problemDetails.Extensions["errors"] = exception.Errors;

            return (statusCode, "Validation Failed", problemDetails);
        }

        private (int StatusCode, string Title, ProblemDetails ProblemDetails) HandleUnknownException(
            HttpContext httpContext,
            Exception exception)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Internal Server Error",
                Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "An error occurred while processing your request.",
                Instance = httpContext.Request.Path,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

            // Include stack trace only in development
            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
                problemDetails.Extensions["innerException"] = exception.InnerException?.Message;
            }

            return (statusCode, "Internal Server Error", problemDetails);
        }
    }
}
