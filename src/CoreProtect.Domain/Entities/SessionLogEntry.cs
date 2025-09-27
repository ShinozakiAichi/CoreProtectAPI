using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Domain.Entities;

public enum SessionAction
{
    Login,
    Logout
}

public sealed record SessionLogEntry(
    long Time,
    DateTimeOffset Timestamp,
    string User,
    string World,
    Coordinates Coordinates,
    SessionAction Action);
