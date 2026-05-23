using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbral.Application.Commands.Operators;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Operators;

namespace Umbral.Api.Controllers;

[ApiController]
[Route("api/operators")]
[Authorize(Roles = "Admin")]
public class OperatorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OperatorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOperatorRequest request)
    {
        try
        {
            var command = new CreateOperatorCommand(request.Name, request.Email, request.Password);
            var response = await _mediator.Send(command);
            return Created(string.Empty, response);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            var command = new DeactivateOperatorCommand(id);
            await _mediator.Send(command);
            return Ok(new { message = "Operator deactivated successfully." });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
