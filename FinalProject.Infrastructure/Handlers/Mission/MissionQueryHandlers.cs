using FinalProject.Application.Common;
using FinalProject.Application.DTOs.MissionDtos;
using FinalProject.Application.Features.Coupon;
using FinalProject.Application.Features.Mission;
using FinalProject.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Handlers.Mission;

public class GetUserMissionsQueryHandler : IRequestHandler<GetUserMissionsQuery, Result<PaginatedResult<UserMissionResponseDto>>>
{
    private readonly IMissionService _missionService;

    public GetUserMissionsQueryHandler(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public async Task<Result<PaginatedResult<UserMissionResponseDto>>> Handle(GetUserMissionsQuery request, CancellationToken cancellationToken)
    {
        var query = _missionService.GetUserMissions(request.UserId);
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<UserMissionResponseDto>>.Ok(new PaginatedResult<UserMissionResponseDto>
        {
            Data = data,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}

public class GetPendingMissionsQueryHandler : IRequestHandler<GetPendingMissionsQuery, Result<PaginatedResult<UserMissionResponseDto>>>
{
    private readonly IMissionService _missionService;

    public GetPendingMissionsQueryHandler(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public async Task<Result<PaginatedResult<UserMissionResponseDto>>> Handle(GetPendingMissionsQuery request, CancellationToken cancellationToken)
    {
        var query = _missionService.GetPendingMissions(request.UserId);
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<UserMissionResponseDto>>.Ok(new PaginatedResult<UserMissionResponseDto>
        {
            Data = data,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}

public class GetCompletedMissionsQueryHandler : IRequestHandler<GetCompletedMissionsQuery, Result<PaginatedResult<UserMissionResponseDto>>>
{
    private readonly IMissionService _missionService;

    public GetCompletedMissionsQueryHandler(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public async Task<Result<PaginatedResult<UserMissionResponseDto>>> Handle(GetCompletedMissionsQuery request, CancellationToken cancellationToken)
    {
        var query = _missionService.GetCompletedMissions(request.UserId);
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<UserMissionResponseDto>>.Ok(new PaginatedResult<UserMissionResponseDto>
        {
            Data = data,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}
