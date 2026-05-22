using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbral.Application.Commands.Teams;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Teams;

namespace Umbral.Api.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize(Roles = "Operator")]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// POST /api/teams — create a new team with leader and optional members.
    /// Restricted to Operator role only.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeamRequest request)
    {
        try
        {
            var command = new CreateTeamCommand(
                request.Name,
                request.Description,
                request.LeaderId,
                request.MemberIds);

            var response = await _mediator.Send(command);
            return Created(string.Empty, response);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
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
