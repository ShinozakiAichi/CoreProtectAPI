using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Domain.Entities;

public sealed record CommandLogEntry(
    long Time,
    DateTimeOffset Timestamp,
    string User,
    string World,
    Coordinates Coordinates,
    string Command);
