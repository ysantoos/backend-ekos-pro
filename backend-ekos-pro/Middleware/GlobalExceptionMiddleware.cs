using Domain.Service.DTOs;
using Domain.Service.Exceptions;
using System.Net;
using System.Text.Json;

namespace backend_ekos_pro.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        ApiResponse<object> response;

        switch (exception)
        {
            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                response = ApiResponse<object>.FailureResponse(
                    notFoundException.Message,
                    new[] { notFoundException.Message });
                break;

            case Domain.Service.Exceptions.ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                response = ApiResponse<object>.FailureResponse(
                    "Validation failed",
                    validationException.Errors);
                break;

            case BusinessException businessException:
                statusCode = HttpStatusCode.BadRequest;
                response = ApiResponse<object>.FailureResponse(
                    businessException.Message,
                    new[] { businessException.Message });
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                response = ApiResponse<object>.FailureResponse(
                    "An internal server error occurred",
                    new[] { "An unexpected error occurred. Please try again later." });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
