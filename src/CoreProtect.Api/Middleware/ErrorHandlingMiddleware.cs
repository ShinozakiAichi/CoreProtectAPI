using System.Diagnostics;
using System.Net;
using System.Text.Json;
using CoreProtect.Api.Exceptions;

namespace CoreProtect.Api.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (ApiException ex)
        {
            await WriteProblemAsync(context, ex.StatusCode, ex.Message).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing {Path}", context.Request.Path);
            await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.").ConfigureAwait(false);
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        var payload = new
        {
            error = statusCode == HttpStatusCode.BadRequest ? "BadRequest" : statusCode.ToString(),
            message,
            traceId
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload)).ConfigureAwait(false);
    }
}
