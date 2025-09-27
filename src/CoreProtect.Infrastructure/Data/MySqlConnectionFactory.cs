using CoreProtect.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace CoreProtect.Infrastructure.Data;

public sealed class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly IOptionsMonitor<CoreProtectDatabaseOptions> _options;

    public MySqlConnectionFactory(IOptionsMonitor<CoreProtectDatabaseOptions> options)
    {
        _options = options;
    }

    public async ValueTask<MySqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connectionString = _options.CurrentValue.CoreProtect;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("CoreProtect connection string is not configured.");
        }

        var connection = new MySqlConnection(connectionString)
        {
            ConnectionTimeout = (uint)_options.CurrentValue.CommandTimeout.TotalSeconds
        };

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }
}
