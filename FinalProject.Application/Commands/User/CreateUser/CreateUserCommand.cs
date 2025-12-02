using FinalProject.Application.Common;
using FinalProject.Application.DTOs.UserDtos;
using MediatR;

namespace FinalProject.Application.Commands.User.CreateUser;

public record CreateUserCommand(CreateUserDto Dto) : IRequest<Result<UserDto>>;
