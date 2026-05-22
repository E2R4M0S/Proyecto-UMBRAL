using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbral.Application.Commands.Sessions;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Sessions;

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
