namespace FinalProject.Application.Commands.User.ChangePassword;

public class ChangePasswordCommandValidator
{
    public static (bool IsValid, string ErrorMessage) Validate(ChangePasswordCommand command)
    {
        if (command.UserId <= 0)
            return (false, "Invalid user ID");

        if (string.IsNullOrWhiteSpace(command.CurrentPassword))
            return (false, "Current password is required");

        if (string.IsNullOrWhiteSpace(command.NewPassword))
            return (false, "New password is required");

        if (command.NewPassword.Length < 6)
            return (false, "New password must be at least 6 characters long");

        if (command.CurrentPassword == command.NewPassword)
            return (false, "New password must be different from current password");

        return (true, string.Empty);
    }
}