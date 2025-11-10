namespace FinalProject.Application.DTOs.AuthDtos;

public record LoginResponseDto(
    int UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    string Token,
    string? Company,
    string? University
);
