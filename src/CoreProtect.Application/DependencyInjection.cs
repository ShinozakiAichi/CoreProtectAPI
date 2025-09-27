using CoreProtect.Application.Abstractions;
using CoreProtect.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CoreProtect.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICoreProtectQueryService, CoreProtectQueryService>();
        return services;
    }
}
