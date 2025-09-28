using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record DatabaseLockDto(int RowId, int? Status, long? Time, string? Timestamp, bool? IsLocked)
{
    public static DatabaseLockDto FromDomain(DatabaseLockInfo info)
    {
        var timestamp = info.Timestamp?.ToUniversalTime().ToString("O");
        var isLocked = info.Status is null ? (bool?)null : info.Status != 0;
        return new DatabaseLockDto(info.RowId, info.Status, info.Time, timestamp, isLocked);
    }
}
