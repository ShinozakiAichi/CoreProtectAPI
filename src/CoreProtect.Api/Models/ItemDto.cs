using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record ItemDto(
    long Time,
    string Timestamp,
    string User,
    string World,
    int X,
    int Y,
    int Z,
    string Action,
    int ItemTypeId,
    int ItemData,
    int Amount,
    bool RolledBack)
{
    public static ItemDto FromDomain(ItemLogEntry entry)
    {
        var action = entry.Action switch
        {
            ItemAction.Drop => "drop",
            ItemAction.Pickup => "pickup",
            _ => entry.Action.ToString().ToLowerInvariant()
        };

        return new ItemDto(
            entry.Time,
            entry.Timestamp.ToUniversalTime().ToString("O"),
            entry.User,
            entry.World,
            entry.Coordinates.X,
            entry.Coordinates.Y,
            entry.Coordinates.Z,
            action,
            entry.ItemTypeId,
            entry.ItemData,
            entry.Amount,
            entry.RolledBack);
    }
}
