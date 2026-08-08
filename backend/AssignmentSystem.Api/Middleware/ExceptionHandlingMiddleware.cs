using System.Net;
using System.Text.Json;
using AssignmentSystem.Api.Models;
using AssignmentSystem.Domain.Exceptions;

namespace AssignmentSystem.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        string message;

        switch (exception)
        {
            case NotFoundException ex:
                statusCode = (int)HttpStatusCode.NotFound;
                message = ex.Message;
                break;
            case ForbiddenException ex:
                statusCode = (int)HttpStatusCode.Forbidden;
                message = ex.Message;
                break;
            case UnauthorizedException ex:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = ex.Message;
                break;
            case DeadlinePassedException ex:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = ex.Message;
                break;
            case AssignmentSystem.Domain.Exceptions.ValidationException ex:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = ex.Errors.Count > 0 
                    ? string.Join("; ", ex.Errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"))
                    : ex.Message;
                break;
            case KeyNotFoundException ex:
                statusCode = (int)HttpStatusCode.NotFound;
                message = ex.Message;
                break;
            case UnauthorizedAccessException ex:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = ex.Message;
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An unhandled internal server error occurred.";
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = new ApiResponse<object>(
            success: false,
            statusCode: statusCode,
            message: message,
            data: null
        );

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
