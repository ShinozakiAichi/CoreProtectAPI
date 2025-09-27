using CoreProtect.Domain.Entities;

namespace CoreProtect.Application.Common;

public sealed record BlockQueryParameters(
    LogQueryParameters Base,
    int? BlockTypeId,
    BlockAction? Action);
