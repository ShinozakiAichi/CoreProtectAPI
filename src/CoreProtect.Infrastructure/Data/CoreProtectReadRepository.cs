using System.Data;
using System.Data.Common;
using System.Diagnostics;
using CoreProtect.Application.Abstractions;
using CoreProtect.Application.Common;
using CoreProtect.Domain.Entities;
using CoreProtect.Domain.ValueObjects;
using CoreProtect.Infrastructure.Configuration;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoreProtect.Infrastructure.Data;

public sealed class CoreProtectReadRepository : ICoreProtectReadRepository
{
    private const string UserCte = @"WITH user_ids AS (
  SELECT id
  FROM co_user
  WHERE user = COALESCE(@userExact, user)
    AND user LIKE COALESCE(@userLike, user) COLLATE utf8mb4_general_ci
  UNION
  SELECT u.id
  FROM co_username_log ul
  JOIN co_user u ON u.uuid = ul.uuid
  WHERE ul.user LIKE COALESCE(@userLike, ul.user) COLLATE utf8mb4_general_ci
     OR ul.user = COALESCE(@userExact, ul.user)
)";

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IMetadataDecoder _metadataDecoder;
    private readonly ILogger<CoreProtectReadRepository> _logger;
    private readonly IOptionsMonitor<CoreProtectDatabaseOptions> _options;

    public CoreProtectReadRepository(
        IDbConnectionFactory connectionFactory,
        IMetadataDecoder metadataDecoder,
        ILogger<CoreProtectReadRepository> logger,
        IOptionsMonitor<CoreProtectDatabaseOptions> options)
    {
        _connectionFactory = connectionFactory;
        _metadataDecoder = metadataDecoder;
        _logger = logger;
        _options = options;
    }

    public async Task<IReadOnlyList<WorldInfo>> GetWorldsAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT id, world FROM co_world ORDER BY world";
        return await QueryAsync(sql, new DynamicParameters(), MapWorld, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<UserSearchResult>> SearchUsersAsync(string term, int limit, CancellationToken cancellationToken)
    {
        const string sql = "SELECT id, user AS user_name FROM co_user WHERE user LIKE @pattern COLLATE utf8mb4_general_ci ORDER BY user LIMIT @limit";
        var parameters = new DynamicParameters();
        parameters.Add("@pattern", $"%{term}%");
        parameters.Add("@limit", limit);
        return await QueryAsync(sql, parameters, MapUserSearch, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<UserResolution>> ResolveUserAsync(string name, CancellationToken cancellationToken)
    {
        const string sql = @"WITH name_history AS (
    SELECT u.id, u.user AS current_user, u.uuid, u.time
    FROM co_user u
    WHERE u.user = @name
    UNION
    SELECT u.id, u.user AS current_user, u.uuid, u.time
    FROM co_username_log ul
    JOIN co_user u ON u.uuid = ul.uuid
    WHERE ul.user = @name
)
SELECT DISTINCT id, current_user, uuid FROM name_history ORDER BY time DESC";
        var parameters = new DynamicParameters();
        parameters.Add("@name", name);
        return await QueryAsync(sql, parameters, MapUserResolution, cancellationToken).ConfigureAwait(false);
    }

    public Task<IReadOnlyList<BlockLogEntry>> GetBlocksAsync(BlockQueryParameters parameters, CancellationToken cancellationToken)
    {
        var sql = $"{UserCte}\nSELECT b.time, u.user AS user_name, w.world, b.x, b.y, b.z, b.action, b.type, mm.material, b.rolled_back\nFROM co_block b\nJOIN co_user u ON u.id = b.user\nJOIN co_world w ON w.id = b.wid\nLEFT JOIN co_material_map mm ON mm.id = b.type\nWHERE {BuildBaseFilter("b", "w", parameters.Base)}\n  AND b.type = COALESCE(@blockTypeId, b.type)\n  AND b.action = COALESCE(@blockAction, b.action)\nORDER BY b.time {ToSqlOrder(parameters.Base.SortDirection)}\nLIMIT @limit OFFSET @offset";

        var dapperParameters = BuildBaseParameters(parameters.Base);
        dapperParameters.Add("@blockTypeId", parameters.BlockTypeId);
        dapperParameters.Add("@blockAction", parameters.Action.HasValue ? (int?)ConvertBlockAction(parameters.Action.Value) : null);

        return QueryAsync(sql, dapperParameters, MapBlock, cancellationToken);
    }

    public Task<IReadOnlyList<ContainerLogEntry>> GetContainersAsync(ContainerQueryParameters parameters, CancellationToken cancellationToken)
    {
        var sql = $"{UserCte}\nSELECT c.time, u.user AS user_name, w.world, c.x, c.y, c.z, c.action, c.type, c.data, c.amount, c.metadata, c.rolled_back\nFROM co_container c\nJOIN co_user u ON u.id = c.user\nJOIN co_world w ON w.id = c.wid\nWHERE {BuildBaseFilter("c", "w", parameters.Base)}\nORDER BY c.time {ToSqlOrder(parameters.Base.SortDirection)}\nLIMIT @limit OFFSET @offset";

        var dapperParameters = BuildBaseParameters(parameters.Base);
        return QueryAsync(sql, dapperParameters, MapContainer, cancellationToken);
    }

    public Task<IReadOnlyList<ItemLogEntry>> GetItemsAsync(ItemQueryParameters parameters, CancellationToken cancellationToken)
    {
        var sql = $"{UserCte}\nSELECT i.time, u.user AS user_name, w.world, i.x, i.y, i.z, i.action, i.type, i.data, i.amount, i.rolled_back\nFROM co_item i\nJOIN co_user u ON u.id = i.user\nJOIN co_world w ON w.id = i.wid\nWHERE {BuildBaseFilter("i", "w", parameters.Base)}\nORDER BY i.time {ToSqlOrder(parameters.Base.SortDirection)}\nLIMIT @limit OFFSET @offset";

        var dapperParameters = BuildBaseParameters(parameters.Base);
        return QueryAsync(sql, dapperParameters, MapItem, cancellationToken);
    }

    public Task<IReadOnlyList<CommandLogEntry>> GetCommandsAsync(CommandQueryParameters parameters, CancellationToken cancellationToken)
    {
        var sql = $"{UserCte}\nSELECT c.time, u.user AS user_name, w.world, c.x, c.y, c.z, c.message\nFROM co_command c\nJOIN co_user u ON u.id = c.user\nJOIN co_world w ON w.id = c.wid\nWHERE {BuildBaseFilter("c", "w", parameters.Base)}\n  AND c.message LIKE COALESCE(@commandLike, c.message) COLLATE utf8mb4_general_ci\nORDER BY c.time {ToSqlOrder(parameters.Base.SortDirection)}\nLIMIT @limit OFFSET @offset";

        var dapperParameters = BuildBaseParameters(parameters.Base);
        dapperParameters.Add("@commandLike", parameters.CommandLike);
        return QueryAsync(sql, dapperParameters, MapCommand, cancellationToken);
    }

    public Task<IReadOnlyList<ChatLogEntry>> GetChatAsync(ChatQueryParameters parameters, CancellationToken cancellationToken)
    {
        var sql = $"{UserCte}\nSELECT ch.time, u.user AS user_name, w.world, ch.x, ch.y, ch.z, ch.message\nFROM co_chat ch\nJOIN co_user u ON u.id = ch.user\nJOIN co_world w ON w.id = ch.wid\nWHERE {BuildBaseFilter("ch", "w", parameters.Base)}\n  AND ch.message LIKE COALESCE(@messageLike, ch.message) COLLATE utf8mb4_general_ci\nORDER BY ch.time {ToSqlOrder(parameters.Base.SortDirection)}\nLIMIT @limit OFFSET @offset";

        var dapperParameters = BuildBaseParameters(parameters.Base);
        dapperParameters.Add("@messageLike", parameters.MessageLike);
        return QueryAsync(sql, dapperParameters, MapChat, cancellationToken);
    }

    public Task<IReadOnlyList<SessionLogEntry>> GetSessionsAsync(SessionQueryParameters parameters, CancellationToken cancellationToken)
    {
        var sql = $"{UserCte}\nSELECT s.time, u.user AS user_name, w.world, s.x, s.y, s.z, s.action\nFROM co_session s\nJOIN co_user u ON u.id = s.user\nJOIN co_world w ON w.id = s.wid\nWHERE {BuildBaseFilter("s", "w", parameters.Base)}\nORDER BY s.time {ToSqlOrder(parameters.Base.SortDirection)}\nLIMIT @limit OFFSET @offset";

        var dapperParameters = BuildBaseParameters(parameters.Base);
        return QueryAsync(sql, dapperParameters, MapSession, cancellationToken);
    }

    public Task<IReadOnlyList<SignLogEntry>> GetSignsAsync(SignQueryParameters parameters, CancellationToken cancellationToken)
    {
        var sql = $"{UserCte}\nSELECT sg.time, u.user AS user_name, w.world, sg.x, sg.y, sg.z, sg.action, sg.color, sg.glow, sg.line_1, sg.line_2, sg.line_3, sg.line_4, sg.line_5, sg.line_6, sg.line_7, sg.line_8\nFROM co_sign sg\nJOIN co_user u ON u.id = sg.user\nJOIN co_world w ON w.id = sg.wid\nWHERE {BuildBaseFilter("sg", "w", parameters.Base)}\nORDER BY sg.time {ToSqlOrder(parameters.Base.SortDirection)}\nLIMIT @limit OFFSET @offset";

        var dapperParameters = BuildBaseParameters(parameters.Base);
        return QueryAsync(sql, dapperParameters, MapSign, cancellationToken);
    }

    private async Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql,
        DynamicParameters parameters,
        Func<IDataRecord, T> projector,
        CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var stopwatch = Stopwatch.StartNew();
        await using var reader = await connection.ExecuteReaderAsync(sql, parameters, commandTimeout: (int)_options.CurrentValue.CommandTimeout.TotalSeconds).ConfigureAwait(false);

        var result = new List<T>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            result.Add(projector(reader));
        }

        stopwatch.Stop();
        if (stopwatch.Elapsed > TimeSpan.FromMilliseconds(500))
        {
            _logger.LogWarning("Slow SQL query detected ({ElapsedMilliseconds} ms): {Sql}", stopwatch.Elapsed.TotalMilliseconds, sql);
        }

        return result;
    }

    private static Func<IDataRecord, WorldInfo> MapWorld => record => new WorldInfo(record.GetInt32(0), record.GetString(1));

    private static Func<IDataRecord, UserSearchResult> MapUserSearch => record =>
        new UserSearchResult(record.GetInt32(0), record.GetString(1));

    private static Func<IDataRecord, UserResolution> MapUserResolution => record =>
        new UserResolution(record.GetInt32(0), record.GetString(1), record.IsDBNull(2) ? null : record.GetString(2));

    private BlockLogEntry MapBlock(IDataRecord record)
    {
        var action = record.GetInt32(6) == 0 ? BlockAction.Placed : BlockAction.Broken;
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(record.GetInt64(0));
        return new BlockLogEntry(
            record.GetInt64(0),
            timestamp,
            record.GetString(1),
            record.GetString(2),
            new Coordinates(record.GetInt32(3), record.GetInt32(4), record.GetInt32(5)),
            action,
            record.GetInt32(7),
            record.IsDBNull(8) ? null : record.GetString(8),
            record.GetBoolean(9));
    }

    private ContainerLogEntry MapContainer(IDataRecord record)
    {
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(record.GetInt64(0));
        var metadata = MetadataDocument.Empty;
        if (!record.IsDBNull(10))
        {
            var raw = (byte[])record.GetValue(10);
            metadata = _metadataDecoder.Decode(raw);
        }

        var action = record.GetInt32(6) == 0 ? ContainerAction.Put : ContainerAction.Took;
        return new ContainerLogEntry(
            record.GetInt64(0),
            timestamp,
            record.GetString(1),
            record.GetString(2),
            new Coordinates(record.GetInt32(3), record.GetInt32(4), record.GetInt32(5)),
            action,
            record.GetInt32(7),
            record.GetInt32(8),
            record.GetInt32(9),
            metadata,
            record.GetBoolean(11));
    }

    private ItemLogEntry MapItem(IDataRecord record)
    {
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(record.GetInt64(0));
        var actionValue = record.GetInt32(6);
        var action = actionValue switch
        {
            0 => ItemAction.Drop,
            1 => ItemAction.Pickup,
            _ => ItemAction.Unknown
        };

        return new ItemLogEntry(
            record.GetInt64(0),
            timestamp,
            record.GetString(1),
            record.GetString(2),
            new Coordinates(record.GetInt32(3), record.GetInt32(4), record.GetInt32(5)),
            action,
            record.GetInt32(7),
            record.GetInt32(8),
            record.GetInt32(9),
            record.GetBoolean(10));
    }

    private CommandLogEntry MapCommand(IDataRecord record)
    {
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(record.GetInt64(0));
        return new CommandLogEntry(
            record.GetInt64(0),
            timestamp,
            record.GetString(1),
            record.GetString(2),
            new Coordinates(record.GetInt32(3), record.GetInt32(4), record.GetInt32(5)),
            record.GetString(6));
    }

    private ChatLogEntry MapChat(IDataRecord record)
    {
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(record.GetInt64(0));
        return new ChatLogEntry(
            record.GetInt64(0),
            timestamp,
            record.GetString(1),
            record.GetString(2),
            new Coordinates(record.GetInt32(3), record.GetInt32(4), record.GetInt32(5)),
            record.GetString(6));
    }

    private SessionLogEntry MapSession(IDataRecord record)
    {
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(record.GetInt64(0));
        var action = record.GetInt32(6) == 0 ? SessionAction.Login : SessionAction.Logout;
        return new SessionLogEntry(
            record.GetInt64(0),
            timestamp,
            record.GetString(1),
            record.GetString(2),
            new Coordinates(record.GetInt32(3), record.GetInt32(4), record.GetInt32(5)),
            action);
    }

    private SignLogEntry MapSign(IDataRecord record)
    {
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(record.GetInt64(0));
        var lines = new List<string>(8);
        for (var i = 9; i <= 16; i++)
        {
            lines.Add(record.IsDBNull(i) ? string.Empty : record.GetString(i));
        }

        return new SignLogEntry(
            record.GetInt64(0),
            timestamp,
            record.GetString(1),
            record.GetString(2),
            new Coordinates(record.GetInt32(3), record.GetInt32(4), record.GetInt32(5)),
            record.GetString(6),
            record.IsDBNull(7) ? null : record.GetString(7),
            record.IsDBNull(8) ? null : record.GetString(8),
            lines);
    }

    private static int ConvertBlockAction(BlockAction action) => action switch
    {
        BlockAction.Placed => 0,
        BlockAction.Broken => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
    };

    private static string BuildBaseFilter(string tableAlias, string worldAlias, LogQueryParameters parameters)
    {
        return $"(COALESCE(@userExact, @userLike) IS NULL OR {tableAlias}.user IN (SELECT id FROM user_ids)) AND\n  {tableAlias}.time >= COALESCE(@from, 0) AND\n  {tableAlias}.time < COALESCE(@to, 9223372036854775807) AND\n  {tableAlias}.x BETWEEN COALESCE(@xMin, -2147483648) AND COALESCE(@xMax, 2147483647) AND\n  {tableAlias}.y BETWEEN COALESCE(@yMin, -2147483648) AND COALESCE(@yMax, 2147483647) AND\n  {tableAlias}.z BETWEEN COALESCE(@zMin, -2147483648) AND COALESCE(@zMax, 2147483647) AND\n  {worldAlias}.world = COALESCE(@world, {worldAlias}.world)";
    }

    private static DynamicParameters BuildBaseParameters(LogQueryParameters parameters)
    {
        var dp = new DynamicParameters();
        dp.Add("@userExact", parameters.UserExact);
        dp.Add("@userLike", parameters.UserLike);
        dp.Add("@world", parameters.World);
        dp.Add("@from", parameters.From);
        dp.Add("@to", parameters.To);
        dp.Add("@xMin", parameters.Coordinates.XMin);
        dp.Add("@xMax", parameters.Coordinates.XMax);
        dp.Add("@yMin", parameters.Coordinates.YMin);
        dp.Add("@yMax", parameters.Coordinates.YMax);
        dp.Add("@zMin", parameters.Coordinates.ZMin);
        dp.Add("@zMax", parameters.Coordinates.ZMax);
        dp.Add("@limit", parameters.Pagination.Limit);
        dp.Add("@offset", parameters.Pagination.Offset);
        return dp;
    }

    private static string ToSqlOrder(SortDirection direction) => direction == SortDirection.Ascending ? "ASC" : "DESC";
}
