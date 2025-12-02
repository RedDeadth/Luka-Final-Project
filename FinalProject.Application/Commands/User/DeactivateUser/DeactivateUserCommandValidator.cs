namespace FinalProject.Application.Commands.User.DeactivateUser;

public class DeactivateUserCommandValidator
{
    public static (bool IsValid, string ErrorMessage) Validate(DeactivateUserCommand command)
    {
        if (command.UserId <= 0)
            return (false, "Invalid user ID");

        return (true, string.Empty);
    }
}