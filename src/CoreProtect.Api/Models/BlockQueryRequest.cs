using System;
using CoreProtect.Api.Configuration;
using CoreProtect.Api.Exceptions;
using CoreProtect.Application.Common;
using CoreProtect.Domain.Entities;

namespace CoreProtect.Api.Models;

public sealed class BlockQueryRequest : LogQueryRequest
{
    public int? BlockTypeId { get; init; }
    public string? Action { get; init; }

    public BlockQueryParameters Build(ApiSettings settings)
    {
        var baseParams = base.Build(settings);
        BlockAction? action = null;
        if (!string.IsNullOrWhiteSpace(Action))
        {
            action = Action.ToLowerInvariant() switch
            {
                "placed" => BlockAction.Placed,
                "broken" => BlockAction.Broken,
                _ => throw new ValidationException("Block action must be either 'placed' or 'broken'.")
            };
        }

        return new BlockQueryParameters(baseParams, BlockTypeId, action);
    }
}
