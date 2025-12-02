using FinalProject.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinalProject.Application.Common;
using FinalProject.Application.DTOs.AuthDtos;
using FinalProject.Application.Features.Auth;
using FinalProject.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinalProject.Infrastructure.Handlers.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<LoginResponseDto>.Fail("Email and password are required");

        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Active == true);
        if (user == null || request.Password != user.Password)
            return Result<LoginResponseDto>.Unauthorized("Invalid credentials");

        var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId ?? 0);
        if (role == null)
            return Result<LoginResponseDto>.Fail("User role not found");

        var token = GenerateToken(user.Id, user.Email, role.Name);
        var response = new LoginResponseDto(user.Id, user.Email, user.FirstName, user.LastName, role.Name, token, user.Company, user.University);

        return Result<LoginResponseDto>.Ok(response);
    }

    private string GenerateToken(int userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
