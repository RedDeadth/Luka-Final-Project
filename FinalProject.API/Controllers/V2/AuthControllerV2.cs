using FinalProject.Application.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

/// <summary>
/// Controlador de autenticación usando CQRS con MediatR
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
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
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, data = result.Data });
    }
}

public record LoginRequest(string Email, string Password);
