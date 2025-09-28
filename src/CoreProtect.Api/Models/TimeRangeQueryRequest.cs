using CoreProtect.Api.Configuration;
using CoreProtect.Api.Exceptions;
using CoreProtect.Application.Common;

namespace CoreProtect.Api.Models;

public sealed class TimeRangeQueryRequest : SimplePaginationRequest
{
    public long? From { get; init; }
    public long? To { get; init; }

    protected override void ValidateSpecific()
    {
        base.ValidateSpecific();
        if (From.HasValue && To.HasValue && From > To)
        {
            throw new ValidationException("Parameter 'from' must be less than or equal to 'to'.");
        }
    }

    public new TimeRangeQueryParameters Build(ApiSettings settings)
    {
        var pagination = base.BuildPagination(settings);
        return new TimeRangeQueryParameters(From, To, pagination);
    }
}
