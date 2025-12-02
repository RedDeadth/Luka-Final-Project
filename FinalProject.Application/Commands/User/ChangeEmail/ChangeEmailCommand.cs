using FinalProject.Application.Common;
using MediatR;

namespace FinalProject.Application.Commands.User.ChangeEmail;

public record ChangeEmailCommand(int UserId, string NewEmail) : IRequest<Result<bool>>;