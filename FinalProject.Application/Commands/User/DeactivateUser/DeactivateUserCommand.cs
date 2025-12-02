using FinalProject.Application.Common;
using MediatR;

namespace FinalProject.Application.Commands.User.DeactivateUser;

public record DeactivateUserCommand(int UserId) : IRequest<Result<bool>>;