using System.Collections.Generic;
using System.Linq;
using CoreProtect.Api.Configuration;
using CoreProtect.Api.Models;
using CoreProtect.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CoreProtect.Api.Controllers;

[ApiController]
[Route("v1/maps")]
public sealed class MapsController : ControllerBase
{
    private readonly ICoreProtectQueryService _service;
    private readonly IOptions<ApiSettings> _settings;

    public MapsController(ICoreProtectQueryService service, IOptions<ApiSettings> settings)
    {
        _service = service;
        _settings = settings;
    }

    [HttpGet("art")]
    public async Task<ActionResult<IEnumerable<ArtMapDto>>> GetArtAsync([FromQuery] SimplePaginationRequest request, CancellationToken cancellationToken)
    {
        var pagination = request.Build(_settings.Value);
        var entries = await _service.GetArtMapAsync(pagination, cancellationToken).ConfigureAwait(false);
        return Ok(entries.Select(ArtMapDto.FromDomain));
    }

    [HttpGet("blockdata")]
    public async Task<ActionResult<IEnumerable<BlockDataMapDto>>> GetBlockDataAsync([FromQuery] SimplePaginationRequest request, CancellationToken cancellationToken)
    {
        var pagination = request.Build(_settings.Value);
        var entries = await _service.GetBlockDataMapAsync(pagination, cancellationToken).ConfigureAwait(false);
        return Ok(entries.Select(BlockDataMapDto.FromDomain));
    }

    [HttpGet("entities")]
    public async Task<ActionResult<IEnumerable<EntityMapDto>>> GetEntityMapAsync([FromQuery] SimplePaginationRequest request, CancellationToken cancellationToken)
    {
        var pagination = request.Build(_settings.Value);
        var entries = await _service.GetEntityMapAsync(pagination, cancellationToken).ConfigureAwait(false);
        return Ok(entries.Select(EntityMapDto.FromDomain));
    }

    [HttpGet("materials")]
    public async Task<ActionResult<IEnumerable<MaterialMapDto>>> GetMaterialMapAsync([FromQuery] SimplePaginationRequest request, CancellationToken cancellationToken)
    {
        var pagination = request.Build(_settings.Value);
        var entries = await _service.GetMaterialMapAsync(pagination, cancellationToken).ConfigureAwait(false);
        return Ok(entries.Select(MaterialMapDto.FromDomain));
    }
}
