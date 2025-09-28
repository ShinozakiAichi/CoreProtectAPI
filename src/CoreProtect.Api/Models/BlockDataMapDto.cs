using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record BlockDataMapDto(int RowId, int? DataId, string? DataValue)
{
    public static BlockDataMapDto FromDomain(BlockDataMapEntry entry) => new(entry.RowId, entry.DataId, entry.DataValue);
}
