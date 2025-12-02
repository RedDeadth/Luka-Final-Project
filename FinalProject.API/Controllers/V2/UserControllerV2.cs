using FinalProject.Application.Commands.User.ActivateUser;
using FinalProject.Application.Commands.User.AssignRole;
using FinalProject.Application.Commands.User.ChangeEmail;
using FinalProject.Application.Commands.User.ChangePassword;
using FinalProject.Application.Commands.User.CreateUser;
using FinalProject.Application.Commands.User.DeactivateUser;
using FinalProject.Application.Commands.User.DeleteUser;
using FinalProject.Application.Commands.User.UpdateUser;
using FinalProject.Application.DTOs.UserDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/[controller]")]
public class UserControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;

    public UserControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        var validation = CreateUserCommandValidator.Validate(new CreateUserCommand(dto));
        if (!validation.IsValid)
            return BadRequest(new { success = false, message = validation.ErrorMessage });

        var result = await _mediator.Send(new CreateUserCommand(dto));

        return result.IsSuccess
            ? StatusCode(201, new { success = true, data = result.Data })
            : BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Update user information
    /// </summary>
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto dto)
    {
        var validation = UpdateUserCommandValidator.Validate(new UpdateUserCommand(userId, dto));
        if (!validation.IsValid)
            return BadRequest(new { success = false, message = validation.ErrorMessage });

        var result = await _mediator.Send(new UpdateUserCommand(userId, dto));

        return result.IsSuccess
            ? Ok(new { success = true, data = result.Data })
            : result.StatusCode == 404
                ? NotFound(new { success = false, message = result.ErrorMessage })
                : BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var validation = DeleteUserCommandValidator.Validate(new DeleteUserCommand(userId));
        if (!validation.IsValid)
            return BadRequest(new { success = false, message = validation.ErrorMessage });

        var result = await _mediator.Send(new DeleteUserCommand(userId));

        return result.IsSuccess
            ? Ok(new { success = true, message = "User deleted successfully" })
            : result.StatusCode == 404
                ? NotFound(new { success = false, message = result.ErrorMessage })
                : BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Activate a user
    /// </summary>
    [HttpPost("{userId}/activate")]
    public async Task<IActionResult> ActivateUser(int userId)
    {
        var validation = ActivateUserCommandValidator.Validate(new ActivateUserCommand(userId));
        if (!validation.IsValid)
            return BadRequest(new { success = false, message = validation.ErrorMessage });

        var result = await _mediator.Send(new ActivateUserCommand(userId));

        return result.IsSuccess
            ? Ok(new { success = true, message = "User activated successfully" })
            : result.StatusCode == 404
                ? NotFound(new { success = false, message = result.ErrorMessage })
                : BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Deactivate a user
    /// </summary>
    [HttpPost("{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int userId)
    {
        var validation = DeactivateUserCommandValidator.Validate(new DeactivateUserCommand(userId));
        if (!validation.IsValid)
            return BadRequest(new { success = false, message = validation.ErrorMessage });

        var result = await _mediator.Send(new DeactivateUserCommand(userId));

        return result.IsSuccess
            ? Ok(new { success = true, message = "User deactivated successfully" })
            : result.StatusCode == 404
                ? NotFound(new { success = false, message = result.ErrorMessage })
                : BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Change user email
    /// </summary>
    [HttpPut("{userId}/email")]
    public async Task<IActionResult> ChangeEmail(int userId, [FromBody] ChangeEmailDto dto)
    {
        var validation = ChangeEmailCommandValidator.Validate(new ChangeEmailCommand(userId, dto.NewEmail));
        if (!validation.IsValid)
            return BadRequest(new { success = false, message = validation.ErrorMessage });

        var result = await _mediator.Send(new ChangeEmailCommand(userId, dto.NewEmail));

        return result.IsSuccess
            ? Ok(new { success = true, message = "Email changed successfully" })
            : result.StatusCode == 404
                ? NotFound(new { success = false, message = result.ErrorMessage })
                : BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPut("{userId}/password")]
    public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordDto dto)
    {
        var validation = ChangePasswordCommandValidator.Validate(
            new ChangePasswordCommand(userId, dto.CurrentPassword, dto.NewPassword));

        if (!validation.IsValid)
            return BadRequest(new { success = false, message = validation.ErrorMessage });

        var result = await _mediator.Send(
            new ChangePasswordCommand(userId, dto.CurrentPassword, dto.NewPassword));

        return result.IsSuccess
            ? Ok(new { success = true, message = "Password changed successfully" })
            : result.StatusCode == 404
                ? NotFound(new { success = false, message = result.ErrorMessage })
                : result.StatusCode == 401
                    ? Unauthorized(new { success = false, message = result.ErrorMessage })
                    : BadRequest(new { success = false, message = result.ErrorMessage });
    }

    /// <summary>
    /// Assign role to user
    /// </summary>
    [HttpPut("{userId}/role")]
    public async Task<IActionResult> AssignRole(int userId, [FromBody] AssignRoleDto dto)
    {
        var validation = AssignRoleCommandValidator.Validate(new AssignRoleCommand(userId, dto.RoleId));
        if (!validation.IsValid)
            return BadRequest(new { success = false, message = validation.ErrorMessage });

        var result = await _mediator.Send(new AssignRoleCommand(userId, dto.RoleId));

        return result.IsSuccess
            ? Ok(new { success = true, message = "Role assigned successfully" })
            : result.StatusCode == 404
                ? NotFound(new { success = false, message = result.ErrorMessage })
                : BadRequest(new { success = false, message = result.ErrorMessage });
    }
}

// DTOs for request bodies
public class ChangeEmailDto
{
    public string NewEmail { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class AssignRoleDto
{
    public int RoleId { get; set; }
}
