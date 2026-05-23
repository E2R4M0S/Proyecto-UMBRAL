using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Users;
using Umbral.Application.Queries.Users;

namespace Umbral.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin,Operator")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/users — paginated, filterable user catalog.
    /// Restricted to Admin and Operator roles.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/users/{id} — full user detail.
    /// Restricted to Admin and Operator roles.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
