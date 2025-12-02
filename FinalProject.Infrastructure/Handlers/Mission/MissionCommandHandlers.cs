using FinalProject.Domain.Entities;
using FinalProject.Application.Common;
using FinalProject.Application.Features.Mission;
using FinalProject.Application.Interfaces;
using MediatR;

namespace FinalProject.Infrastructure.Handlers.Mission;

public class AssignMissionCommandHandler : IRequestHandler<AssignMissionCommand, Result<bool>>
{
    private readonly IMissionService _missionService;

    public AssignMissionCommandHandler(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public async Task<Result<bool>> Handle(AssignMissionCommand request, CancellationToken cancellationToken)
    {
        var result = await _missionService.AssignMissionAsync(request.Dto);
        return result 
            ? Result<bool>.Ok(true) 
            : Result<bool>.Failure("Failed to assign mission");
    }
}

public class CompleteMissionCommandHandler : IRequestHandler<CompleteMissionCommand, Result<bool>>
{
    private readonly IMissionService _missionService;

    public CompleteMissionCommandHandler(IMissionService missionService)
    {
        _missionService = missionService;
    }

    public async Task<Result<bool>> Handle(CompleteMissionCommand request, CancellationToken cancellationToken)
    {
        var result = await _missionService.CompleteMissionAsync(request.Dto);
        return result 
            ? Result<bool>.Ok(true) 
            : Result<bool>.Failure("Failed to complete mission");
    }
}
