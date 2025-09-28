using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record EntityMapDto(int RowId, int? EntityId, string? EntityName)
{
    public static EntityMapDto FromDomain(EntityMapEntry entry) => new(entry.RowId, entry.EntityId, entry.EntityName);
}
