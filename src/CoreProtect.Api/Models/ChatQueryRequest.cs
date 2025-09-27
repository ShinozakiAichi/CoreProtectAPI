using System;
using CoreProtect.Api.Configuration;
using CoreProtect.Application.Common;

namespace CoreProtect.Api.Models;

public sealed class ChatQueryRequest : LogQueryRequest
{
    public string? Message { get; init; }

    public new ChatQueryParameters Build(ApiSettings settings)
    {
        var baseParams = base.Build(settings);
        var messageLike = string.IsNullOrWhiteSpace(Message) ? null : $"%{Message.Trim()}%";
        return new ChatQueryParameters(baseParams, messageLike);
    }
}
