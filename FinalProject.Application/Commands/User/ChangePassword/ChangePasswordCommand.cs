using FinalProject.Application.Common;
using MediatR;

namespace FinalProject.Application.Commands.User.ChangePassword;

public record ChangePasswordCommand(int UserId, string CurrentPassword, string NewPassword) : IRequest<Result<bool>>;