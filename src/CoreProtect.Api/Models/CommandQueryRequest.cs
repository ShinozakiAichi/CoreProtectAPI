using System;
using CoreProtect.Api.Configuration;
using CoreProtect.Application.Common;

namespace CoreProtect.Api.Models;

public sealed class CommandQueryRequest : LogQueryRequest
{
    public string? Command { get; init; }

    public new CommandQueryParameters Build(ApiSettings settings)
    {
        var baseParams = base.Build(settings);
        var commandLike = string.IsNullOrWhiteSpace(Command) ? null : $"%{Command.Trim()}%";
        return new CommandQueryParameters(baseParams, commandLike);
    }
}
