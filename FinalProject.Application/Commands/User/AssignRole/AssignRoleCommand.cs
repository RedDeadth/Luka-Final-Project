using FinalProject.Application.Common;
using MediatR;

namespace FinalProject.Application.Commands.User.AssignRole;

public record AssignRoleCommand(int UserId, int RoleId) : IRequest<Result<bool>>;