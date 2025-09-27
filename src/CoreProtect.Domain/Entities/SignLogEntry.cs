using CoreProtect.Domain.ValueObjects;

namespace CoreProtect.Domain.Entities;

public enum SignAction
{
    Create,
    Remove,
    Unknown
}

public enum SignFace
{
    Front,
    Back
}

public enum SignGlowState
{
    None,
    Front,
    Back,
    Both
}

public sealed record SignLogEntry(
    long Time,
    DateTimeOffset Timestamp,
    string User,
    string World,
    Coordinates Coordinates,
    SignAction Action,
    string? Color,
    string? GlowingText,
    IReadOnlyList<string> Lines)
{
    public string? SecondaryColor { get; init; }
    public SignGlowState GlowState { get; init; } = SignGlowState.None;
    public SignFace Face { get; init; } = SignFace.Front;
    public bool IsWaxed { get; init; }
}
