namespace FinalProject.Application.DTOs.AuthDtos;

public record LoginRequestDto(
    string Email,
    string Password
);
