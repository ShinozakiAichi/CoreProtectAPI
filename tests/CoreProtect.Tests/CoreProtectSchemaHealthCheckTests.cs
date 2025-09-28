using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreProtect.Infrastructure.Data;
using CoreProtect.Infrastructure.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace CoreProtect.Tests;

public class CoreProtectSchemaHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_ShouldReturnHealthy_WhenAllTablesArePresent()
    {
        var healthCheck = new CoreProtectSchemaHealthCheck(new FakeSchemaVerifier(Array.Empty<string>()));

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact]
    public async Task CheckHealthAsync_ShouldReturnUnhealthy_WhenTablesAreMissing()
    {
        var missing = new[] { "co_block", "co_item" };
        var healthCheck = new CoreProtectSchemaHealthCheck(new FakeSchemaVerifier(missing));

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Contains("co_block", result.Description, StringComparison.Ordinal);
        Assert.Contains("co_item", result.Description, StringComparison.Ordinal);
    }

    private sealed class FakeSchemaVerifier : ICoreProtectSchemaVerifier
    {
        private readonly IReadOnlyCollection<string> _missingTables;

        public FakeSchemaVerifier(IReadOnlyCollection<string> missingTables)
        {
            _missingTables = missingTables;
        }

        public Task<IReadOnlyCollection<string>> GetMissingTablesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_missingTables);
        }
    }
}
