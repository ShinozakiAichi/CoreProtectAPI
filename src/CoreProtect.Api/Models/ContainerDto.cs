using CoreProtect.Domain.Entities;
using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Api.Models;

public sealed record ContainerDto(
    long Time,
    string Timestamp,
    string User,
    string World,
    int X,
    int Y,
    int Z,
    string Action,
    int ItemTypeId,
    string? ItemMaterial,
    int ItemData,
    int Amount,
    object? MetaJson,
    string? MetaRawHex,
    string? MetaBase64,
    bool RolledBack)
{
    public static ContainerDto FromDomain(ContainerLogEntry entry) => new(
        entry.Time,
        entry.Timestamp.ToUniversalTime().ToString("O"),
        entry.User,
        entry.World,
        entry.Coordinates.X,
        entry.Coordinates.Y,
        entry.Coordinates.Z,
        entry.Action == ContainerAction.Put ? "put" : "took",
        entry.ItemTypeId,
        entry.ItemMaterial,
        entry.ItemData,
        entry.Amount,
        entry.Metadata.Json,
        entry.Metadata.RawHex,
        entry.Metadata.Base64,
        entry.RolledBack);
}
