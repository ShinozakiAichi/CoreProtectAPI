using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record UserResolutionDto(int Id, string CurrentUser, string? Uuid)
{
    public static UserResolutionDto FromDomain(UserResolution resolution) => new(resolution.Id, resolution.CurrentUser, resolution.Uuid);
}
