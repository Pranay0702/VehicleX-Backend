using System.Net;
using System.Text.Json;
using VehicleX.Application.DTOs.Common;

namespace VehicleX.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, "A required argument was not provided."),
            ArgumentException => (HttpStatusCode.BadRequest, "An invalid argument was provided."),
            KeyNotFoundException => (HttpStatusCode.NotFound, "The requested resource was not found."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "You are not authorized to perform this action."),
            TimeoutException => (HttpStatusCode.RequestTimeout, "The operation timed out. Please try again."),
            Microsoft.EntityFrameworkCore.DbUpdateException dbEx when IsConstraintViolation(dbEx)
                => (HttpStatusCode.Conflict, "A database conflict occurred. The record may already exist."),
            Microsoft.EntityFrameworkCore.DbUpdateException
                => (HttpStatusCode.InternalServerError, "A database error occurred. Please try again."),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.FailureResponse(
            message,
            new List<string> { exception.Message });

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }

    /// <summary>
    /// Checks if a DbUpdateException is caused by a real constraint violation (duplicate key, FK violation)
    /// vs a transient failure (timeout, connection loss).
    /// </summary>
    private static bool IsConstraintViolation(Microsoft.EntityFrameworkCore.DbUpdateException ex)
    {
        // PostgreSQL error codes: 23505 = unique_violation, 23503 = foreign_key_violation
        var inner = ex.InnerException;
        if (inner is Npgsql.PostgresException pgEx)
        {
            return pgEx.SqlState is "23505" or "23503";
        }
        return false;
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
