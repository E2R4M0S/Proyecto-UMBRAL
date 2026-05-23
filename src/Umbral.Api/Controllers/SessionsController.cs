using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbral.Application.Commands.Sessions;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Sessions;
using Umbral.Application.Queries.Sessions;

namespace Umbral.Api.Controllers;

[ApiController]
[Route("api/sessions")]
[Authorize(Roles = "Operator")]
public class SessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/sessions — paginated, filterable session list.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? missionId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var query = new GetSessionsQuery(page, pageSize, missionId, status, fromDate, toDate);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/sessions/active — list active and preparing sessions.
    /// Must be declared BEFORE the {id:guid} route to avoid conflicts.
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var result = await _mediator.Send(new GetActiveSessionsQuery());
        return Ok(result);
    }

    /// <summary>
    /// GET /api/sessions/{id} — full session detail with mission info and teams.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var query = new GetSessionByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/sessions — create a new session with PIN.
    /// Restricted to Operator role only.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSessionRequest request)
    {
        try
        {
            var command = new CreateSessionCommand(request.Name, request.MissionId);
            var response = await _mediator.Send(command);
            return Created(string.Empty, response);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage });
            return BadRequest(new { message = "Validation failed.", errors });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message, param = ex.ParamName });
        }
    }
}
