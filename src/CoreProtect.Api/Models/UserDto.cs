using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record UserDto(int RowId, long? Time, string? Timestamp, string? UserName, string? Uuid)
{
    public static UserDto FromDomain(UserRecord record)
    {
        var timestamp = record.Timestamp?.ToUniversalTime().ToString("O");
        return new UserDto(record.RowId, record.Time, timestamp, record.UserName, record.Uuid);
    }
}
