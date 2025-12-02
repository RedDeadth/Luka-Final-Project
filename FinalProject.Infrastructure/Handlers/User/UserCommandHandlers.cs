using FinalProject.Domain.Entities;
using FinalProject.Application.Commands.User.ActivateUser;
using FinalProject.Application.Commands.User.AssignRole;
using FinalProject.Application.Commands.User.ChangeEmail;
using FinalProject.Application.Commands.User.ChangePassword;
using FinalProject.Application.Commands.User.CreateUser;
using FinalProject.Application.Commands.User.DeactivateUser;
using FinalProject.Application.Commands.User.DeleteUser;
using FinalProject.Application.Commands.User.UpdateUser;
using FinalProject.Application.Common;
using FinalProject.Application.DTOs.UserDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Handlers.User;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;

            // Check if email already exists
            var existingUser = await _unitOfWork.Users.Query(u => u.Email == dto.Email)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUser != null)
                return Result<UserDto>.Failure("User with this email already exists");

            // Get role by name
            var role = await _unitOfWork.Roles.Query(r => r.Name == dto.Role)
                .FirstOrDefaultAsync(cancellationToken);

            if (role == null)
                return Result<UserDto>.Failure("Invalid role specified");

            var user = new Domain.Entities.User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = _passwordHasher.HashPassword(dto.Password),
                RoleId = role.Id,
                Active = true,
                Company = string.Empty,
                University = string.Empty
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleName = role.Name,
                Active = user.Active ?? true
            };

            return Result<UserDto>.Created(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure(ex.Message);
        }
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.Query(u => u.Id == request.UserId)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return Result<UserDto>.NotFound("User not found");

            if (request.Dto.FirstName != null)
                user.FirstName = request.Dto.FirstName;

            if (request.Dto.LastName != null)
                user.LastName = request.Dto.LastName;

            if (request.Dto.Company != null)
                user.Company = request.Dto.Company;

            if (request.Dto.University != null)
                user.University = request.Dto.University;

            if (request.Dto.Active.HasValue)
                user.Active = request.Dto.Active.Value;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleName = user.Role?.Name ?? string.Empty,
                Active = user.Active ?? true
            };

            return Result<UserDto>.Ok(userDto);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure(ex.Message);
        }
    }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

            if (user == null)
                return Result<bool>.NotFound("User not found");

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

            if (user == null)
                return Result<bool>.NotFound("User not found");

            user.Active = true;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

            if (user == null)
                return Result<bool>.NotFound("User not found");

            user.Active = false;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}

public class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ChangeEmailCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

            if (user == null)
                return Result<bool>.NotFound("User not found");

            var emailExists = await _unitOfWork.Users.Query(u => u.Email == request.NewEmail && u.Id != request.UserId)
                .AnyAsync(cancellationToken);

            if (emailExists)
                return Result<bool>.Failure("Email already in use by another user");

            user.Email = request.NewEmail;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

            if (user == null)
                return Result<bool>.NotFound("User not found");

            if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.Password))
                return Result<bool>.Failure("Current password is incorrect", 401);

            user.Password = _passwordHasher.HashPassword(request.NewPassword);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);

            if (user == null)
                return Result<bool>.NotFound("User not found");

            var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);

            if (role == null)
                return Result<bool>.NotFound("Role not found");

            user.RoleId = request.RoleId;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
