using CoreProtect.Application.Abstractions;
using CoreProtect.Infrastructure.Configuration;
using CoreProtect.Infrastructure.Data;
using CoreProtect.Infrastructure.Decoding;
using CoreProtect.Infrastructure.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreProtect.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CoreProtectDatabaseOptions>(configuration.GetSection(CoreProtectDatabaseOptions.SectionName));
        services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
        services.AddSingleton<IMetadataDecoder, MetadataDecoder>();
        services.AddSingleton<ICoreProtectSchemaVerifier, CoreProtectSchemaVerifier>();
        services.AddSingleton<CoreProtectSchemaHealthCheck>();
        services.AddTransient<ICoreProtectReadRepository, CoreProtectReadRepository>();
        return services;
    }
}
