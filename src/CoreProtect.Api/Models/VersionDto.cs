using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record VersionDto(int RowId, long? Time, string? Timestamp, string? Version)
{
    public static VersionDto FromDomain(VersionInfo info)
    {
        var timestamp = info.Timestamp?.ToUniversalTime().ToString("O");
        return new VersionDto(info.RowId, info.Time, timestamp, info.Version);
    }
}
