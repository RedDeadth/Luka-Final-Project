using FinalProject.Application.DTOs.MissionDtos;
using FinalProject.Application.Features.Mission;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/mission")]
public class MissionControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;

    public MissionControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignMission([FromBody] AssignMissionDto dto)
    {
        var result = await _mediator.Send(new AssignMissionCommand(dto));
        return result.IsSuccess ? Ok(new { success = true, message = "Mission assigned successfully" }) : BadRequest(new { success = false, message = result.Error });
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteMission([FromBody] CompleteMissionDto dto)
    {
        var result = await _mediator.Send(new CompleteMissionCommand(dto));
        return result.IsSuccess ? Ok(new { success = true, message = "Mission completed successfully" }) : BadRequest(new { success = false, message = result.Error });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserMissions(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetUserMissionsQuery(userId, page, pageSize));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data.Data }) : BadRequest(new { message = result.Error });
    }

    [HttpGet("user/{userId}/pending")]
    public async Task<IActionResult> GetPendingMissions(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetPendingMissionsQuery(userId, page, pageSize));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data.Data }) : BadRequest(new { message = result.Error });
    }

    [HttpGet("user/{userId}/completed")]
    public async Task<IActionResult> GetCompletedMissions(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetCompletedMissionsQuery(userId, page, pageSize));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data.Data }) : BadRequest(new { message = result.Error });
    }
}
