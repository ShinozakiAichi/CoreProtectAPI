namespace CoreProtect.Domain.Entities;

public sealed record UserRecord(int RowId, long? Time, DateTimeOffset? Timestamp, string? UserName, string? Uuid);
