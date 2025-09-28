using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record MaterialMapDto(int RowId, int? MaterialId, string? MaterialName)
{
    public static MaterialMapDto FromDomain(MaterialMapEntry entry) => new(entry.RowId, entry.MaterialId, entry.MaterialName);
}
