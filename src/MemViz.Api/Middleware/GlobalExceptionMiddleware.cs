using System.Net;
using System.Text.Json;
using FluentValidation;

namespace MemViz.Api.Middleware;

/// <summary>
/// Global exception handling middleware for consistent error responses
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Title = "Validation Error";
                response.Detail = "One or more validation errors occurred.";
                response.Errors = validationEx.Errors.ToDictionary(
                    e => e.PropertyName,
                    e => new[] { e.ErrorMessage });
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Title = "Unauthorized";
                response.Detail = "You are not authorized to perform this action.";
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Title = "Bad Request";
                response.Detail = argEx.Message;
                break;

            case InvalidOperationException invalidOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Title = "Invalid Operation";
                response.Detail = invalidOpEx.Message;
                break;

            case NotSupportedException notSupportedEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Title = "Not Supported";
                response.Detail = notSupportedEx.Message;
                break;

            case TimeoutException:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                response.Title = "Request Timeout";
                response.Detail = "The request took too long to process.";
                break;

            case TaskCanceledException:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                response.Title = "Request Cancelled";
                response.Detail = "The request was cancelled.";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Title = "Internal Server Error";
                response.Detail = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An internal server error occurred.";
                break;
        }

        // Add additional debug information in development
        if (_environment.IsDevelopment())
        {
            response.DeveloperMessage = exception.ToString();
            response.TraceId = context.TraceIdentifier;
        }

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Standardized error response model
/// </summary>
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string? Instance { get; set; }
    public string? TraceId { get; set; }
    public string? DeveloperMessage { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}