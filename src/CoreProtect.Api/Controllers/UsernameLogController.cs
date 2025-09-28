using System.Collections.Generic;
using System.Linq;
using CoreProtect.Api.Configuration;
using CoreProtect.Api.Models;
using CoreProtect.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CoreProtect.Api.Controllers;

[ApiController]
[Route("v1/username-log")]
public sealed class UsernameLogController : ControllerBase
{
    private readonly ICoreProtectQueryService _service;
    private readonly IOptions<ApiSettings> _settings;

    public UsernameLogController(ICoreProtectQueryService service, IOptions<ApiSettings> settings)
    {
        _service = service;
        _settings = settings;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsernameLogDto>>> GetAsync([FromQuery] SimplePaginationRequest request, CancellationToken cancellationToken)
    {
        var pagination = request.Build(_settings.Value);
        var entries = await _service.GetUsernameLogAsync(pagination, cancellationToken).ConfigureAwait(false);
        return Ok(entries.Select(UsernameLogDto.FromDomain));
    }
}
