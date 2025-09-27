using System;
using CoreProtect.Api.Configuration;
using CoreProtect.Application.Common;

namespace CoreProtect.Api.Models;

public sealed class SessionQueryRequest : LogQueryRequest
{
    public SessionQueryParameters Build(ApiSettings settings)
    {
        var baseParams = base.Build(settings);
        return new SessionQueryParameters(baseParams);
    }
}
