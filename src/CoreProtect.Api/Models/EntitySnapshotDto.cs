using System;
using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed record EntitySnapshotDto(int RowId, long? Time, string? Timestamp, string DataBase64, int DataLength)
{
    public static EntitySnapshotDto FromDomain(EntitySnapshot snapshot)
    {
        var timestamp = snapshot.Timestamp?.ToUniversalTime().ToString("O");
        var data = snapshot.Data ?? Array.Empty<byte>();
        var base64 = data.Length == 0 ? string.Empty : Convert.ToBase64String(data);
        return new EntitySnapshotDto(snapshot.RowId, snapshot.Time, timestamp, base64, data.Length);
    }
}
