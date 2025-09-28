using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace CoreProtect.Infrastructure.Data;

public sealed class CoreProtectSchemaVerifier : ICoreProtectSchemaVerifier
{
    private static readonly string[] RequiredTables =
    {
        "co_art_map",
        "co_block",
        "co_blockdata_map",
        "co_chat",
        "co_command",
        "co_container",
        "co_database_lock",
        "co_entity",
        "co_entity_map",
        "co_item",
        "co_material_map",
        "co_session",
        "co_sign",
        "co_skull",
        "co_user",
        "co_username_log",
        "co_version",
        "co_world"
    };

    private readonly IDbConnectionFactory _connectionFactory;

    public CoreProtectSchemaVerifier(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<string>> GetMissingTablesAsync(CancellationToken cancellationToken)
    {
        const string sql =
            "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @tableName";

        await using var connection =
            await _connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        var missingTables = new List<string>();
        foreach (var tableName in RequiredTables)
        {
            var count = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, new { tableName }, cancellationToken: cancellationToken)).ConfigureAwait(false);

            if (count == 0)
            {
                missingTables.Add(tableName);
            }
        }

        return missingTables;
    }
}
