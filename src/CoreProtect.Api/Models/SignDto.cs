using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record SignDto(
    long Time,
    string Timestamp,
    string User,
    string World,
    int X,
    int Y,
    int Z,
    string Action,
    string? Color,
    string? GlowingText,
    IReadOnlyList<string> Lines)
{
    public static SignDto FromDomain(SignLogEntry entry) => new(
        entry.Time,
        entry.Timestamp.ToUniversalTime().ToString("O"),
        entry.User,
        entry.World,
        entry.Coordinates.X,
        entry.Coordinates.Y,
        entry.Coordinates.Z,
        ConvertAction(entry.Action),
        entry.Color,
        entry.GlowingText,
        entry.Lines);

    private static string ConvertAction(SignAction action) => action switch
    {
        SignAction.Create => "create",
        SignAction.Remove => "remove",
        _ => "unknown"
    };
}
