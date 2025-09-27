using CoreProtect.Api.Configuration;
using CoreProtect.Api.Exceptions;
using Microsoft.Extensions.Options;

namespace CoreProtect.Api.Middleware;

public sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<ApiSettings> _settings;

    public ApiKeyMiddleware(RequestDelegate next, IOptionsMonitor<ApiSettings> settings)
    {
        _next = next;
        _settings = settings;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var options = _settings.CurrentValue;
        if (!options.EnableApiKey)
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        if (string.IsNullOrEmpty(options.ApiKey))
        {
            throw new UnauthorizedException("API key validation is enabled but no key is configured.");
        }

        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var provided) || !string.Equals(provided, options.ApiKey, StringComparison.Ordinal))
        {
            throw new UnauthorizedException("Invalid API key.");
        }

        await _next(context).ConfigureAwait(false);
    }
}
