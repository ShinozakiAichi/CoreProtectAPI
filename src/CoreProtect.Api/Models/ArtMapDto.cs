using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record ArtMapDto(int RowId, int? ArtId, string? ArtName)
{
    public static ArtMapDto FromDomain(ArtMapEntry entry) => new(entry.RowId, entry.ArtId, entry.ArtName);
}
