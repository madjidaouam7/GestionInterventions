using GestionInterventions.Application.DTOs;
using GestionInterventions.Application.Features.Auth.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionInterventions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginDto loginDto,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(loginDto.Email, loginDto.Password);

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }
}
