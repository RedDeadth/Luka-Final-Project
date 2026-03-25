using FinalProject.Application.DTOs.AuthDtos;
using FinalProject.Application.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/auth")]
public class AuthControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password));
        return result.IsSuccess ? Ok(result.Data) : Unauthorized(new { message = result.Error });
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        var result = await _mediator.Send(new ValidateTokenCommand(token));
        return result.IsSuccess ? Ok(new { message = "Token is valid" }) : Unauthorized(new { message = result.Error });
    }
}
