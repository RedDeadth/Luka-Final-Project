using FinalProject.Application.DTOs.AuthDtos;

namespace FinalProject.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<bool> ValidateTokenAsync(string token);
    string GenerateToken(int userId, string email, string role);
}
