using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private const int MaxPageSize = 100;

    public CampaignController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDto dto, [FromQuery] int companyUserId)
    {
        try
        {
            var campaign = await _campaignService.CreateCampaignAsync(companyUserId, dto);
            return Ok(new { success = true, data = campaign });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCampaigns([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _campaignService.GetActiveCampaigns();
        var totalCount = await query.CountAsync();
        var campaigns = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new { 
            success = true, 
            data = campaigns,
            pagination = new {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCampaignById(int id)
    {
        try
        {
            var campaign = await _campaignService.GetCampaignByIdAsync(id);
            return Ok(new { success = true, data = campaign });
        }
        catch (Exception ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollCampaignDto dto)
    {
        var result = await _campaignService.EnrollStudentAsync(dto);
        if (result)
            return Ok(new { success = true, message = "Student enrolled successfully" });
        
        return BadRequest(new { success = false, message = "Failed to enroll student" });
    }

    [HttpGet("company/{companyUserId}")]
    public async Task<IActionResult> GetCompanyCampaigns(int companyUserId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _campaignService.GetCompanyCampaigns(companyUserId);
        var totalCount = await query.CountAsync();
        var campaigns = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new { 
            success = true, 
            data = campaigns,
            pagination = new {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }
}
