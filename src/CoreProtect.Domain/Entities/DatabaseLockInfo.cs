namespace CoreProtect.Domain.Entities;

public sealed record DatabaseLockInfo(int RowId, int? Status, long? Time, DateTimeOffset? Timestamp);
