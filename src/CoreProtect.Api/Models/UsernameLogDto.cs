using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record UsernameLogDto(int RowId, long? Time, string? Timestamp, string? Uuid, string? UserName, string? CurrentUser)
{
    public static UsernameLogDto FromDomain(UsernameLogEntry entry)
    {
        var timestamp = entry.Timestamp?.ToUniversalTime().ToString("O");
        return new UsernameLogDto(entry.RowId, entry.Time, timestamp, entry.Uuid, entry.UserName, entry.CurrentUser);
    }
}
