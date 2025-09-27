using MySqlConnector;

namespace CoreProtect.Infrastructure.Data;

public interface IDbConnectionFactory
{
    ValueTask<MySqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken);
}
