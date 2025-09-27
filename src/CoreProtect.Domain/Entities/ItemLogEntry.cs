using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Domain.Entities;

public enum ItemAction
{
    Drop,
    Pickup,
    Unknown
}

public sealed record ItemLogEntry(
    long Time,
    DateTimeOffset Timestamp,
    string User,
    string World,
    Coordinates Coordinates,
    ItemAction Action,
    int ItemTypeId,
    int ItemData,
    int Amount,
    bool RolledBack);
