namespace FinalProject.Application.Commands.User.UpdateUser;

public class UpdateUserCommandValidator
{
    public static (bool IsValid, string ErrorMessage) Validate(UpdateUserCommand command)
    {
        if (command.UserId <= 0)
            return (false, "Invalid user ID");

        var dto = command.Dto;

        if (dto.FirstName != null && string.IsNullOrWhiteSpace(dto.FirstName))
            return (false, "First name cannot be empty");

        if (dto.LastName != null && string.IsNullOrWhiteSpace(dto.LastName))
            return (false, "Last name cannot be empty");

        return (true, string.Empty);
    }
}