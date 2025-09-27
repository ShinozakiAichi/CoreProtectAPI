using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Domain.Entities;

public sealed record ChatLogEntry(
    long Time,
    DateTimeOffset Timestamp,
    string User,
    string World,
    Coordinates Coordinates,
    string Message);
