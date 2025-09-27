using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record ChatDto(
    long Time,
    string Timestamp,
    string User,
    string World,
    int X,
    int Y,
    int Z,
    string Message)
{
    public static ChatDto FromDomain(ChatLogEntry entry) => new(
        entry.Time,
        entry.Timestamp.ToUniversalTime().ToString("O"),
        entry.User,
        entry.World,
        entry.Coordinates.X,
        entry.Coordinates.Y,
        entry.Coordinates.Z,
        entry.Message);
}
