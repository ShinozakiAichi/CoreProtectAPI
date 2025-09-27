using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record SessionDto(
    long Time,
    string Timestamp,
    string User,
    string World,
    int X,
    int Y,
    int Z,
    string Action)
{
    public static SessionDto FromDomain(SessionLogEntry entry) => new(
        entry.Time,
        entry.Timestamp.ToUniversalTime().ToString("O"),
        entry.User,
        entry.World,
        entry.Coordinates.X,
        entry.Coordinates.Y,
        entry.Coordinates.Z,
        entry.Action == SessionAction.Login ? "login" : "logout");
}
