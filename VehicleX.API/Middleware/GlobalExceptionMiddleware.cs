using System.Net;
using System.Text.Json;
using VehicleX.Application.Common;

namespace VehicleX.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next   = next   ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        // Client disconnected — log a warning and return 499, don't treat as a server error
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogWarning("The request was cancelled by the client.");
            await WriteErrorAsync(context, StatusCodes.Status499ClientClosedRequest,
                "The request was cancelled by the client.");
        }
        catch (BadHttpRequestException exception)
        {
            _logger.LogWarning(exception, "A malformed request was received.");
            await WriteErrorAsync(context, HttpStatusCode.BadRequest,
                "The request could not be processed.");
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogWarning(exception, "A forbidden operation was attempted.");
            await WriteErrorAsync(context, HttpStatusCode.Forbidden,
                "You are not allowed to perform this operation.");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException exception)
        {
            _logger.LogError(exception, "A database concurrency conflict occurred.");
            await WriteErrorAsync(context, HttpStatusCode.Conflict,
                "The record was updated or deleted by another process.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles the general Exception fallthrough using a rich switch expression.
    /// Covers EF constraint violations, common BCL exceptions, and a safe catch-all.
    /// </summary>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // If response already started (headers sent), we can't change anything — bail out
        if (context.Response.HasStarted)
            return;

        var (statusCode, message) = exception switch
        {
            ArgumentNullException
                => (HttpStatusCode.BadRequest,       "A required argument was not provided."),
            ArgumentException
                => (HttpStatusCode.BadRequest,       "An invalid argument was provided."),
            KeyNotFoundException
                => (HttpStatusCode.NotFound,         "The requested resource was not found."),
            InvalidOperationException
                => (HttpStatusCode.BadRequest,       "The operation is invalid for the current state."),
            UnauthorizedAccessException
                => (HttpStatusCode.Unauthorized,     "You are not authorized to perform this action."),
            TimeoutException
                => (HttpStatusCode.RequestTimeout,   "The operation timed out. Please try again."),

            // PostgreSQL unique/FK constraint violation (23505 / 23503)
            Microsoft.EntityFrameworkCore.DbUpdateException dbEx
                when IsConstraintViolation(dbEx)
                => (HttpStatusCode.Conflict,         "A conflict occurred. The record may already exist or violates a constraint."),

            // Any other EF / DB failure
            Microsoft.EntityFrameworkCore.DbUpdateException
                => (HttpStatusCode.InternalServerError, "A database error occurred. Please try again."),

            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        await WriteErrorAsync(context, statusCode, message);
    }

    /// <summary>
    /// Writes a JSON error response. Guards against writing to an already-started response.
    /// </summary>
    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        var response = ApiResponse<object>.Fail(message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    // Overload so callers can pass a raw int (e.g. StatusCodes.Status499ClientClosedRequest)
    private static Task WriteErrorAsync(HttpContext context, int statusCode, string message)
        => WriteErrorAsync(context, (HttpStatusCode)statusCode, message);

    /// <summary>
    /// PostgreSQL error codes:
    ///   23505 = unique_violation (duplicate key)
    ///   23503 = foreign_key_violation
    /// </summary>
    private static bool IsConstraintViolation(Microsoft.EntityFrameworkCore.DbUpdateException ex)
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