using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Common;

namespace VehicleX.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogWarning("The request was cancelled by the client.");
            await WriteErrorAsync(context, StatusCodes.Status499ClientClosedRequest, "The request was cancelled by the client.");
        }
        catch (BadHttpRequestException exception)
        {
            _logger.LogWarning(exception, "A malformed request was received.");
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, "The request could not be processed.");
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogWarning(exception, "A forbidden operation was attempted.");
            await WriteErrorAsync(context, HttpStatusCode.Forbidden, "You are not allowed to perform this operation.");
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogError(exception, "A database concurrency conflict occurred.");
            await WriteErrorAsync(context, HttpStatusCode.Conflict, "The record was updated or deleted by another process.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "A database update error occurred.");
            await WriteErrorAsync(context, HttpStatusCode.Conflict, "A database conflict occurred while processing the request.");
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(exception, "An invalid operation occurred.");
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred.");
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        return WriteErrorAsync(context, (HttpStatusCode)statusCode, message);
    }
}
