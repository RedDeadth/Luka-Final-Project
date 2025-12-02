namespace FinalProject.Application.Commands.User.DeleteUser;

public class DeleteUserCommandValidator
{
    public static (bool IsValid, string ErrorMessage) Validate(DeleteUserCommand command)
    {
        if (command.UserId <= 0)
            return (false, "Invalid user ID");

        return (true, string.Empty);
    }
}