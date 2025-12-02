using FinalProject.Application.Common;
using FinalProject.Application.DTOs.MissionDtos;
using MediatR;

namespace FinalProject.Application.Features.Mission;

// Commands
public record AssignMissionCommand(AssignMissionDto Dto) : IRequest<Result<bool>>;
public record CompleteMissionCommand(CompleteMissionDto Dto) : IRequest<Result<bool>>;
