using CoreProtect.Domain.Entities;
using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Api.Models;

public sealed record BlockDto(
    long Time,
    string Timestamp,
    string User,
    string World,
    int X,
    int Y,
    int Z,
    string Action,
    int BlockTypeId,
    string? BlockTypeName,
    bool RolledBack)
{
    public static BlockDto FromDomain(BlockLogEntry entry) => new(
        entry.Time,
        entry.Timestamp.ToUniversalTime().ToString("O"),
        entry.User,
        entry.World,
        entry.Coordinates.X,
        entry.Coordinates.Y,
        entry.Coordinates.Z,
        entry.Action == BlockAction.Placed ? "placed" : "broken",
        entry.BlockTypeId,
        entry.BlockTypeName,
        entry.RolledBack);
}
