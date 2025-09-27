using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Domain.Entities;

public enum ContainerAction
{
    Put,
    Took
}

public sealed record ContainerLogEntry(
    long Time,
    DateTimeOffset Timestamp,
    string User,
    string World,
    Coordinates Coordinates,
    ContainerAction Action,
    int ItemTypeId,
    int ItemData,
    int Amount,
    MetadataDocument Metadata,
    bool RolledBack);
