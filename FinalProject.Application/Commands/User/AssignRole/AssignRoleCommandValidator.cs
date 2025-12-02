namespace FinalProject.Application.Commands.User.AssignRole;

public class AssignRoleCommandValidator
{
    public static (bool IsValid, string ErrorMessage) Validate(AssignRoleCommand command)
    {
        if (command.UserId <= 0)
            return (false, "Invalid user ID");

        if (command.RoleId <= 0)
            return (false, "Invalid role ID");

        return (true, string.Empty);
    }
}