using FinalProject.Application.Common;
using MediatR;

namespace FinalProject.Application.Commands.User.DeleteUser;

public record DeleteUserCommand(int UserId) : IRequest<Result<bool>>;