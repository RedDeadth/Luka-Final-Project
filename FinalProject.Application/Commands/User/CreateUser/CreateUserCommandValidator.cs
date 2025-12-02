using System.Text.RegularExpressions;

namespace FinalProject.Application.Commands.User.CreateUser;

public class CreateUserCommandValidator
{
    public static (bool IsValid, string ErrorMessage) Validate(CreateUserCommand command)
    {
        var dto = command.Dto;

        if (string.IsNullOrWhiteSpace(dto.FirstName))
            return (false, "First name is required");

        if (string.IsNullOrWhiteSpace(dto.LastName))
            return (false, "Last name is required");

        if (string.IsNullOrWhiteSpace(dto.Email))
            return (false, "Email is required");

        if (!IsValidEmail(dto.Email))
            return (false, "Invalid email format");

        if (string.IsNullOrWhiteSpace(dto.Password))
            return (false, "Password is required");

        if (dto.Password.Length < 6)
            return (false, "Password must be at least 6 characters long");

        if (string.IsNullOrWhiteSpace(dto.Role))
            return (false, "Role is required");

        return (true, string.Empty);
    }

    private static bool IsValidEmail(string email)
    {
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }
}