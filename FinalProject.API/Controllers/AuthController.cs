using FinalProject.Application.DTOs.AuthDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Email and password are required" });

        var response = await _authService.LoginAsync(request);

        if (response == null)
            return Unauthorized(new { message = "Invalid credentials" });

        return Ok(response);
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        var isValid = await _authService.ValidateTokenAsync(token);
        
        if (!isValid)
            return Unauthorized(new { message = "Invalid token" });

        return Ok(new { message = "Token is valid" });
    }
}
