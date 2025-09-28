namespace CoreProtect.Domain.Entities;

public sealed record SkullEntry(int RowId, long? Time, DateTimeOffset? Timestamp, string? Owner);
