using FinalProject.Application.Common;
using FinalProject.Application.DTOs.UserDtos;
using MediatR;

namespace FinalProject.Application.Commands.User.UpdateUser;

public record UpdateUserCommand(int UserId, UpdateUserDto Dto) : IRequest<Result<UserDto>>;