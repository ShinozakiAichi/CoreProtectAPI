using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record SkullDto(int RowId, long? Time, string? Timestamp, string? Owner)
{
    public static SkullDto FromDomain(SkullEntry entry)
    {
        var timestamp = entry.Timestamp?.ToUniversalTime().ToString("O");
        return new SkullDto(entry.RowId, entry.Time, timestamp, entry.Owner);
    }
}
