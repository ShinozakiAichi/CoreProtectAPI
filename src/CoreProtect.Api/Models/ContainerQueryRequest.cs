using System;
using CoreProtect.Api.Configuration;
using CoreProtect.Application.Common;

namespace CoreProtect.Api.Models;

public sealed class ContainerQueryRequest : LogQueryRequest
{
    public new ContainerQueryParameters Build(ApiSettings settings)
    {
        var baseParams = base.Build(settings);
        return new ContainerQueryParameters(baseParams);
    }
}
