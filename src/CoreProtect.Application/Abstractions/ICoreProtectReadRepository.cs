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
}
