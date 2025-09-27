using System.Collections.Generic;
using System.Linq;
using CoreProtect.Api.Models;
using CoreProtect.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreProtect.Api.Controllers;

[ApiController]
[Route("v1/worlds")]
public sealed class WorldsController : ControllerBase
{
    private readonly ICoreProtectQueryService _service;

    public WorldsController(ICoreProtectQueryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorldDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var worlds = await _service.GetWorldsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(worlds.Select(WorldDto.FromDomain));
    }
}