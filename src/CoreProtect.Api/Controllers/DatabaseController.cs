using System.Collections.Generic;
using System.Linq;
using CoreProtect.Api.Models;
using CoreProtect.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreProtect.Api.Controllers;

[ApiController]
[Route("v1/database")]
public sealed class DatabaseController : ControllerBase
{
    private readonly ICoreProtectQueryService _service;

    public DatabaseController(ICoreProtectQueryService service)
    {
        _service = service;
    }

    [HttpGet("locks")]
    public async Task<ActionResult<IEnumerable<DatabaseLockDto>>> GetLocksAsync(CancellationToken cancellationToken)
    {
        var locks = await _service.GetDatabaseLocksAsync(cancellationToken).ConfigureAwait(false);
        return Ok(locks.Select(DatabaseLockDto.FromDomain));
    }

    [HttpGet("versions")]
    public async Task<ActionResult<IEnumerable<VersionDto>>> GetVersionsAsync(CancellationToken cancellationToken)
    {
        var versions = await _service.GetVersionsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(versions.Select(VersionDto.FromDomain));
    }
}
