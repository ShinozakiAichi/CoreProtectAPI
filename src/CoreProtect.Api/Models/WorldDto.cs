using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record WorldDto(int Id, string World)
{
    public static WorldDto FromDomain(WorldInfo info) => new(info.Id, info.Name);
}
