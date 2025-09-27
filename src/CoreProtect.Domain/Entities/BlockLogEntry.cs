using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Domain.Entities;

public enum BlockAction
{
    Placed,
    Broken
}

public sealed record BlockLogEntry(
    long Time,
    DateTimeOffset Timestamp,
    string User,
    string World,
    Coordinates Coordinates,
    BlockAction Action,
    int BlockTypeId,
    string? BlockTypeName,
    bool RolledBack);
