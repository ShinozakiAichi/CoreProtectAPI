using System;
using CoreProtect.Api.Configuration;
using CoreProtect.Application.Common;

namespace CoreProtect.Api.Models;

public sealed class SignQueryRequest : LogQueryRequest
{
    public new SignQueryParameters Build(ApiSettings settings)
    {
        var baseParams = base.Build(settings);
        return new SignQueryParameters(baseParams);
    }
}
