using FinalProject.Application.Common;
using MediatR;

namespace FinalProject.Application.Commands.User.ActivateUser;

public record ActivateUserCommand(int UserId) : IRequest<Result<bool>>;