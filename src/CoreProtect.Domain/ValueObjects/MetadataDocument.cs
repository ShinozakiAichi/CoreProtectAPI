namespace CoreProtect.Domain.ValueObjects;

public sealed record MetadataDocument(object? Json, string? RawHex, string? Base64)
{
    public static MetadataDocument Empty { get; } = new(null, null, null);
}
