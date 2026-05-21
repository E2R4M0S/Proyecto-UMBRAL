using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbral.Application.Commands.Missions;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Missions;

namespace Umbral.Api.Controllers;

[ApiController]
[Route("api/missions")]
[Authorize(Roles = "Admin")]
public class MissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
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
