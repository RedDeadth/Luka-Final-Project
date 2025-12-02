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
    public async Task<IActionResult> GetActiveCampaigns()
    {
        var campaigns = await _campaignService.GetActiveCampaigns().ToListAsync();
        return Ok(new { success = true, data = campaigns });
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
    public async Task<IActionResult> GetCompanyCampaigns(int companyUserId)
    {
        var campaigns = await _campaignService.GetCompanyCampaigns(companyUserId).ToListAsync();
        return Ok(new { success = true, data = campaigns });
    }
}
