using CoreProtect.Application.Common;
using CoreProtect.Domain.Entities;

namespace CoreProtect.Application.Abstractions;

public interface ICoreProtectReadRepository
{
    Task<IReadOnlyList<WorldInfo>> GetWorldsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<UserSearchResult>> SearchUsersAsync(string term, int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserResolution>> ResolveUserAsync(string name, CancellationToken cancellationToken);
    Task<IReadOnlyList<BlockLogEntry>> GetBlocksAsync(BlockQueryParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<ContainerLogEntry>> GetContainersAsync(ContainerQueryParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<ItemLogEntry>> GetItemsAsync(ItemQueryParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<CommandLogEntry>> GetCommandsAsync(CommandQueryParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<ChatLogEntry>> GetChatAsync(ChatQueryParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<SessionLogEntry>> GetSessionsAsync(SessionQueryParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<SignLogEntry>> GetSignsAsync(SignQueryParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<ArtMapEntry>> GetArtMapAsync(Pagination pagination, CancellationToken cancellationToken);
    Task<IReadOnlyList<BlockDataMapEntry>> GetBlockDataMapAsync(Pagination pagination, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntityMapEntry>> GetEntityMapAsync(Pagination pagination, CancellationToken cancellationToken);
    Task<IReadOnlyList<MaterialMapEntry>> GetMaterialMapAsync(Pagination pagination, CancellationToken cancellationToken);
    Task<IReadOnlyList<DatabaseLockInfo>> GetDatabaseLocksAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<EntitySnapshot>> GetEntitiesAsync(TimeRangeQueryParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<SkullEntry>> GetSkullsAsync(TimeRangeQueryParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserRecord>> GetUsersAsync(Pagination pagination, CancellationToken cancellationToken);
    Task<IReadOnlyList<UsernameLogEntry>> GetUsernameLogAsync(Pagination pagination, CancellationToken cancellationToken);
    Task<IReadOnlyList<VersionInfo>> GetVersionsAsync(CancellationToken cancellationToken);
}
