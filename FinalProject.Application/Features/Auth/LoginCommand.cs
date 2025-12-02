using FinalProject.Application.Common;
using FinalProject.Application.DTOs.AuthDtos;

namespace FinalProject.Application.Features.Auth;

public record LoginCommand(string Email, string Password) : ICommand<Result<LoginResponseDto>>;
public record ValidateTokenCommand(string Token) : ICommand<Result<bool>>;
