using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Domain.Entities;

public sealed record SignLogEntry(
    long Time,
    DateTimeOffset Timestamp,
    string User,
    string World,
    Coordinates Coordinates,
    string Action,
    string? Color,
    string? GlowingText,
    IReadOnlyList<string> Lines);
