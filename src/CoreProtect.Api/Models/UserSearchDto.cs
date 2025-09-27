using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record UserSearchDto(int Id, string User)
{
    public static UserSearchDto FromDomain(UserSearchResult result) => new(result.Id, result.UserName);
}
