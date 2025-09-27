using System.Collections.Generic;
using System.Linq;
using CoreProtect.Api.Configuration;
using CoreProtect.Api.Models;
using CoreProtect.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CoreProtect.Api.Controllers;

[ApiController]
[Route("v1/signs")]
public sealed class SignsController : ControllerBase
{
    private readonly ICoreProtectQueryService _service;
    private readonly IOptions<ApiSettings> _settings;

    public SignsController(ICoreProtectQueryService service, IOptions<ApiSettings> settings)
    {
        _service = service;
        _settings = settings;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SignDto>>> GetAsync([FromQuery] SignQueryRequest request, CancellationToken cancellationToken)
    {
        var parameters = request.Build(_settings.Value);
        var entries = await _service.GetSignsAsync(parameters, cancellationToken).ConfigureAwait(false);
        return Ok(entries.Select(SignDto.FromDomain));
    }
}