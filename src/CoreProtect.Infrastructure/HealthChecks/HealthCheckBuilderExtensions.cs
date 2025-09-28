using System;
using CoreProtect.Infrastructure.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CoreProtect.Infrastructure;

public static class HealthCheckBuilderExtensions
{
    private const string CoreProtectSchemaCheckName = "coreprotect_schema";
    private static readonly string[] ReadyTags = new[] { "ready" };

    public static IHealthChecksBuilder AddCoreProtectSchemaCheck(this IHealthChecksBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddCheck<CoreProtectSchemaHealthCheck>(CoreProtectSchemaCheckName, tags: ReadyTags);
    }
}
