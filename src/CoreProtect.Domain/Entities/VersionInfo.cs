namespace CoreProtect.Domain.Entities;

public sealed record VersionInfo(int RowId, long? Time, DateTimeOffset? Timestamp, string? Version);
