using System.Net;
using System.Text.Json;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace FlatFlow.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationException => HandleValidationException(validationException),
            DomainValidationException domainValidationException => HandleDomainValidationException(domainValidationException),
            NotFoundException notFoundException => (HttpStatusCode.NotFound, new { message = notFoundException.Message }),
            ForbiddenException forbiddenException => (HttpStatusCode.Forbidden, new { message = forbiddenException.Message }),
            AuthenticationException authenticationException => (HttpStatusCode.Unauthorized, new { message = authenticationException.Message }),
            DomainException domainException => (HttpStatusCode.UnprocessableEntity, new { message = domainException.Message }),
            _ => HandleUnhandledException(exception)
        };

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }

    private static (HttpStatusCode, object) HandleValidationException(ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return (HttpStatusCode.BadRequest, new { errors });
    }

    private static (HttpStatusCode, object) HandleDomainValidationException(DomainValidationException exception)
    {
        var errors = new Dictionary<string, string[]>
        {
            { exception.PropertyName, new[] { exception.Message } }
        };

        return (HttpStatusCode.BadRequest, new { errors });
    }

    private (HttpStatusCode, object) HandleUnhandledException(Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred.");
        return (HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred." });
    }
}
