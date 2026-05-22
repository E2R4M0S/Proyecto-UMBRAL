using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbral.Application.Commands.Missions;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Missions;
using Umbral.Application.Queries.Missions;

namespace Umbral.Api.Controllers;

[ApiController]
[Route("api/missions")]
[Authorize]
public class MissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/missions — paginated, filterable mission catalog.
    /// Accessible to any authenticated user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? difficulty = null,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null)
    {
        var query = new GetMissionsQuery(page, pageSize, difficulty, status, search);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/missions/{id} — full mission detail.
    /// Accessible to any authenticated user.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetMissionByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/missions — create a new mission.
    /// Restricted to Admin role only.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateMissionRequest request)
    {
        try
        {
            var command = new CreateMissionCommand(request.Title, request.Description, request.Difficulty, request.TimeMinutes);
            var response = await _mediator.Send(command);
            return Created(string.Empty, response);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
