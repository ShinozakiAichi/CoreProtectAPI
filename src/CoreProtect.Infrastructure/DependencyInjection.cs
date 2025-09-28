using System;
using System.Linq;
using CoreProtect.Application.Abstractions;
using CoreProtect.Infrastructure.Configuration;
using CoreProtect.Infrastructure.Data;
using CoreProtect.Infrastructure.Decoding;
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
        services.AddScoped<ICoreProtectReadRepository>(sp =>
        {
            var implType = typeof(CoreProtectReadRepository);
            var iface = typeof(ICoreProtectReadRepository);

            var map = implType.GetInterfaceMap(iface);
            var ifaceMethodNames = map.InterfaceMethods.Select(m => m.ToString()).ToHashSet();
            var targetMethodNames = map.TargetMethods.Select(m => m?.ToString() ?? "<null>").ToHashSet();

            var missing = ifaceMethodNames.Except(targetMethodNames).ToArray();
            if (missing.Length > 0)
            {
                throw new TypeLoadException("Missing interface methods: " + string.Join("; ", missing));
            }

            return (ICoreProtectReadRepository)ActivatorUtilities.CreateInstance(sp, implType);
        });
        return services;
    }
}
