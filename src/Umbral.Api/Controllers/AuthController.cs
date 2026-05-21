using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbral.Application.Commands.Auth;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Auth;

namespace Umbral.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var command = new LoginCommand(request.Email, request.Password);
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (InvalidCredentialsException)
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }
    }
}
