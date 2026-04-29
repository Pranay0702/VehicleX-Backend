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
        // Client disconnected — don't log as error, just stop silently
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = 499; // Client Closed Request
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // If response already started (headers sent), we can't change anything — bail out
        if (context.Response.HasStarted)
            return;

        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ArgumentNullException   => (HttpStatusCode.BadRequest,          "A required argument was not provided."),
            ArgumentException       => (HttpStatusCode.BadRequest,          "An invalid argument was provided."),
            KeyNotFoundException    => (HttpStatusCode.NotFound,            "The requested resource was not found."),
            InvalidOperationException => (HttpStatusCode.BadRequest,        "The operation is invalid for the current state."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,    "You are not authorized to perform this action."),
            TimeoutException        => (HttpStatusCode.RequestTimeout,      "The operation timed out. Please try again."),

            // PostgreSQL unique/FK constraint violation (23505 / 23503)
            Microsoft.EntityFrameworkCore.DbUpdateException dbEx
                when IsConstraintViolation(dbEx)
                => (HttpStatusCode.Conflict, "A conflict occurred. The record may already exist or violates a constraint."),

            // Any other EF / DB failure
            Microsoft.EntityFrameworkCore.DbUpdateException
                => (HttpStatusCode.InternalServerError, "A database error occurred. Please try again."),

            _  => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        context.Response.StatusCode = (int)statusCode;

        // Only expose inner exception message in Development
        // In Production this will just be the friendly message above
        var response = ApiResponse<object>.FailureResponse(message);

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// PostgreSQL error codes:
    ///   23505 = unique_violation (duplicate key)
    ///   23503 = foreign_key_violation
    /// </summary>
    private static bool IsConstraintViolation(
        Microsoft.EntityFrameworkCore.DbUpdateException ex)
    {
        if (ex.InnerException is Npgsql.PostgresException pgEx)
            return pgEx.SqlState is "23505" or "23503";

        return false;
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(
        this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionMiddleware>();
}