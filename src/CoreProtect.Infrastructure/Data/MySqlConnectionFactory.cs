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

        var builder = new MySqlConnectionStringBuilder(connectionString);
        var commandTimeout = _options.CurrentValue.CommandTimeout;
        if (commandTimeout > TimeSpan.Zero)
        {
            var seconds = (uint)Math.Clamp((int)Math.Ceiling(commandTimeout.TotalSeconds), 1, int.MaxValue);
            builder.DefaultCommandTimeout = seconds;
        }

        var connection = new MySqlConnection(builder.ConnectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }
}
