using FinalProject.Application.Common;
using FinalProject.Application.DTOs.MissionDtos;
using FinalProject.Application.Features.Coupon;
using MediatR;

namespace FinalProject.Application.Features.Mission;

public record GetUserMissionsQuery(int UserId, int Page, int PageSize) : IRequest<Result<PaginatedResult<UserMissionResponseDto>>>;
public record GetPendingMissionsQuery(int UserId, int Page, int PageSize) : IRequest<Result<PaginatedResult<UserMissionResponseDto>>>;
public record GetCompletedMissionsQuery(int UserId, int Page, int PageSize) : IRequest<Result<PaginatedResult<UserMissionResponseDto>>>;
