using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinalProject.Application.DTOs.AuthDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinalProject.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => 
            u.Email == request.Email && u.Active == true);

        if (user == null)
            return null;

        if (!_passwordHasher.VerifyPassword(request.Password, user.Password))
            return null;

        var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId ?? 0);
        if (role == null)
            return null;

        var token = GenerateToken(user.Id, user.Email, role.Name);

        return new LoginResponseDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            role.Name,
            token,
            user.Company,
            user.University
        );
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
                      ?? _configuration["Jwt:Key"]
                      ?? throw new InvalidOperationException("JWT Key not configured");

            var key = Encoding.UTF8.GetBytes(jwtKey);

            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                         ?? _configuration["Jwt:Issuer"];
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                           ?? _configuration["Jwt:Audience"];

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GenerateToken(int userId, string email, string role)
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
                  ?? _configuration["Jwt:Key"]
                  ?? throw new InvalidOperationException("JWT Key not configured");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                     ?? _configuration["Jwt:Issuer"];
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                       ?? _configuration["Jwt:Audience"];

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
