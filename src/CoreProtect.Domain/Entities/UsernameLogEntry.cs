namespace CoreProtect.Domain.Entities;

public sealed record UsernameLogEntry(
    int RowId,
    long? Time,
    DateTimeOffset? Timestamp,
    string? Uuid,
    string? UserName,
    string? CurrentUser);
