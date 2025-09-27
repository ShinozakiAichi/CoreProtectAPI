using System.Collections.Generic;
using System.Linq;
using CoreProtect.Api.Exceptions;
using CoreProtect.Api.Models;
using CoreProtect.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreProtect.Api.Controllers;

[ApiController]
[Route("v1/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ICoreProtectQueryService _service;

    public UsersController(ICoreProtectQueryService service)
    {
        _service = service;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserSearchDto>>> SearchAsync([FromQuery(Name = "name")] string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Query parameter 'name' is required.");
        }

        var results = await _service.SearchUsersAsync(name.Trim(), cancellationToken).ConfigureAwait(false);
        return Ok(results.Select(UserSearchDto.FromDomain));
    }

    [HttpGet("resolve")]
    public async Task<ActionResult<IEnumerable<UserResolutionDto>>> ResolveAsync([FromQuery(Name = "name")] string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Query parameter 'name' is required.");
        }

        var results = await _service.ResolveUserAsync(name.Trim(), cancellationToken).ConfigureAwait(false);
        return Ok(results.Select(UserResolutionDto.FromDomain));
    }
}