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
    private const int MaxPageSize = 100;

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
    public async Task<IActionResult> GetUserMissions(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _missionService.GetUserMissions(userId);
        var totalCount = await query.CountAsync();
        var missions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new { 
            success = true, 
            data = missions,
            pagination = new {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }

    [HttpGet("user/{userId}/pending")]
    public async Task<IActionResult> GetPendingMissions(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _missionService.GetPendingMissions(userId);
        var totalCount = await query.CountAsync();
        var missions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new { 
            success = true, 
            data = missions,
            pagination = new {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }

    [HttpGet("user/{userId}/completed")]
    public async Task<IActionResult> GetCompletedMissions(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _missionService.GetCompletedMissions(userId);
        var totalCount = await query.CountAsync();
        var missions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new { 
            success = true, 
            data = missions,
            pagination = new {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }
}
