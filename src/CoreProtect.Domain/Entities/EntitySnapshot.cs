namespace CoreProtect.Domain.Entities;

public sealed record EntitySnapshot(int RowId, long? Time, DateTimeOffset? Timestamp, byte[] Data);
