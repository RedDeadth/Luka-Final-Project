using System.Text.RegularExpressions;

namespace FinalProject.Application.Commands.User.ChangeEmail;

public class ChangeEmailCommandValidator
{
    public static (bool IsValid, string ErrorMessage) Validate(ChangeEmailCommand command)
    {
        if (command.UserId <= 0)
            return (false, "Invalid user ID");

        if (string.IsNullOrWhiteSpace(command.NewEmail))
            return (false, "New email is required");

        if (!IsValidEmail(command.NewEmail))
            return (false, "Invalid email format");

        return (true, string.Empty);
    }

    private static bool IsValidEmail(string email)
    {
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }
}