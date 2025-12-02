namespace FinalProject.Application.Commands.User.ActivateUser;

public class ActivateUserCommandValidator
{
    public static (bool IsValid, string ErrorMessage) Validate(ActivateUserCommand command)
    {
        if (command.UserId <= 0)
            return (false, "Invalid user ID");

        return (true, string.Empty);
    }
}