using FinalProject.Application.DTOs.MissionDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MissionController : ControllerBase
{
    private readonly IMissionService _missionService;

    public MissionController(IMissionService missionService)
    {
        _missionService = missionService;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignMission([FromBody] AssignMissionDto dto)
    {
        var result = await _missionService.AssignMissionAsync(dto);
        if (result)
            return Ok(new { success = true, message = "Mission assigned successfully" });
        
        return BadRequest(new { success = false, message = "Failed to assign mission" });
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteMission([FromBody] CompleteMissionDto dto)
    {
        var result = await _missionService.CompleteMissionAsync(dto);
        if (result)
            return Ok(new { success = true, message = "Mission completed successfully" });
        
        return BadRequest(new { success = false, message = "Failed to complete mission" });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserMissions(int userId)
    {
        var missions = await _missionService.GetUserMissions(userId).ToListAsync();
        return Ok(new { success = true, data = missions });
    }

    [HttpGet("user/{userId}/pending")]
    public async Task<IActionResult> GetPendingMissions(int userId)
    {
        var missions = await _missionService.GetPendingMissions(userId).ToListAsync();
        return Ok(new { success = true, data = missions });
    }

    [HttpGet("user/{userId}/completed")]
    public async Task<IActionResult> GetCompletedMissions(int userId)
    {
        var missions = await _missionService.GetCompletedMissions(userId).ToListAsync();
        return Ok(new { success = true, data = missions });
    }
}
