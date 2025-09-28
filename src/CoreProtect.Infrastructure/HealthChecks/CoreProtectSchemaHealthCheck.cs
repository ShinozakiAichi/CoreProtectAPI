using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreProtect.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CoreProtect.Infrastructure.HealthChecks;

public sealed class CoreProtectSchemaHealthCheck : IHealthCheck
{
    private readonly ICoreProtectSchemaVerifier _schemaVerifier;

    public CoreProtectSchemaHealthCheck(ICoreProtectSchemaVerifier schemaVerifier)
    {
        _schemaVerifier = schemaVerifier;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var missingTables = await _schemaVerifier
                .GetMissingTablesAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!missingTables.Any())
            {
                return HealthCheckResult.Healthy("CoreProtect schema validated");
            }

            var description = $"Missing tables: {string.Join(", ", missingTables)}";
            return HealthCheckResult.Unhealthy(description);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Failed to validate CoreProtect schema", ex);
        }
    }
}
