using CoreProtect.Application.Abstractions;
using CoreProtect.Application.Common;
using CoreProtect.Domain.Entities;

namespace CoreProtect.Application.Services;

public sealed class CoreProtectQueryService : ICoreProtectQueryService
{
    private const int UserSearchLimit = 50;
    private readonly ICoreProtectReadRepository _repository;

    public CoreProtectQueryService(ICoreProtectReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<WorldInfo>> GetWorldsAsync(CancellationToken cancellationToken) =>
        _repository.GetWorldsAsync(cancellationToken);

    public Task<IReadOnlyList<UserSearchResult>> SearchUsersAsync(string term, CancellationToken cancellationToken)
    {
        return _repository.SearchUsersAsync(term, UserSearchLimit, cancellationToken);
    }

    public Task<IReadOnlyList<UserResolution>> ResolveUserAsync(string name, CancellationToken cancellationToken)
    {
        return _repository.ResolveUserAsync(name, cancellationToken);
    }

    public Task<IReadOnlyList<BlockLogEntry>> GetBlocksAsync(BlockQueryParameters parameters, CancellationToken cancellationToken) =>
        _repository.GetBlocksAsync(parameters, cancellationToken);

    public Task<IReadOnlyList<ContainerLogEntry>> GetContainersAsync(ContainerQueryParameters parameters, CancellationToken cancellationToken) =>
        _repository.GetContainersAsync(parameters, cancellationToken);

    public Task<IReadOnlyList<ItemLogEntry>> GetItemsAsync(ItemQueryParameters parameters, CancellationToken cancellationToken) =>
        _repository.GetItemsAsync(parameters, cancellationToken);

    public Task<IReadOnlyList<CommandLogEntry>> GetCommandsAsync(CommandQueryParameters parameters, CancellationToken cancellationToken) =>
        _repository.GetCommandsAsync(parameters, cancellationToken);

    public Task<IReadOnlyList<ChatLogEntry>> GetChatAsync(ChatQueryParameters parameters, CancellationToken cancellationToken) =>
        _repository.GetChatAsync(parameters, cancellationToken);

    public Task<IReadOnlyList<SessionLogEntry>> GetSessionsAsync(SessionQueryParameters parameters, CancellationToken cancellationToken) =>
        _repository.GetSessionsAsync(parameters, cancellationToken);

    public Task<IReadOnlyList<SignLogEntry>> GetSignsAsync(SignQueryParameters parameters, CancellationToken cancellationToken) =>
        _repository.GetSignsAsync(parameters, cancellationToken);

    public Task<IReadOnlyList<ArtMapEntry>> GetArtMapAsync(Pagination pagination, CancellationToken cancellationToken) =>
        _repository.GetArtMapAsync(pagination, cancellationToken);

    public Task<IReadOnlyList<BlockDataMapEntry>> GetBlockDataMapAsync(Pagination pagination, CancellationToken cancellationToken) =>
        _repository.GetBlockDataMapAsync(pagination, cancellationToken);

    public Task<IReadOnlyList<EntityMapEntry>> GetEntityMapAsync(Pagination pagination, CancellationToken cancellationToken) =>
        _repository.GetEntityMapAsync(pagination, cancellationToken);

    public Task<IReadOnlyList<MaterialMapEntry>> GetMaterialMapAsync(Pagination pagination, CancellationToken cancellationToken) =>
        _repository.GetMaterialMapAsync(pagination, cancellationToken);

    public Task<IReadOnlyList<DatabaseLockInfo>> GetDatabaseLocksAsync(CancellationToken cancellationToken) =>
        _repository.GetDatabaseLocksAsync(cancellationToken);

    public Task<IReadOnlyList<EntitySnapshot>> GetEntitiesAsync(TimeRangeQueryParameters parameters, CancellationToken cancellationToken) =>
        _repository.GetEntitiesAsync(parameters, cancellationToken);

    public Task<IReadOnlyList<SkullEntry>> GetSkullsAsync(TimeRangeQueryParameters parameters, CancellationToken cancellationToken) =>
        _repository.GetSkullsAsync(parameters, cancellationToken);

    public Task<IReadOnlyList<UserRecord>> GetUsersAsync(Pagination pagination, CancellationToken cancellationToken) =>
        _repository.GetUsersAsync(pagination, cancellationToken);

    public Task<IReadOnlyList<UsernameLogEntry>> GetUsernameLogAsync(Pagination pagination, CancellationToken cancellationToken) =>
        _repository.GetUsernameLogAsync(pagination, cancellationToken);

    public Task<IReadOnlyList<VersionInfo>> GetVersionsAsync(CancellationToken cancellationToken) =>
        _repository.GetVersionsAsync(cancellationToken);
}
