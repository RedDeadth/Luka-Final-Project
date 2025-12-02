using FinalProject.Application.DTOs.CompanyDtos;
using FinalProject.Application.DTOs.LukasDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("companies/pending")]
    public async Task<IActionResult> GetPendingCompanies()
    {
        var companies = await _adminService.GetPendingCompaniesAsync();
        return Ok(new { success = true, data = companies });
    }

    [HttpPost("companies/approve")]
    public async Task<IActionResult> ApproveCompany([FromBody] CompanyApprovalDto dto)
    {
        var result = await _adminService.ApproveCompanyAsync(dto);
        if (result)
            return Ok(new { success = true, message = "Company approval status updated" });
        
        return BadRequest(new { success = false, message = "Failed to update company status" });
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetSystemStatistics()
    {
        var stats = await _adminService.GetSystemStatisticsAsync();
        return Ok(new { success = true, data = stats });
    }

    [HttpPost("lukas/emit")]
    public async Task<IActionResult> EmitLukas([FromBody] EmitLukasDto dto)
    {
        var result = await _adminService.EmitLukasAsync(dto);
        if (result)
            return Ok(new { success = true, message = "Lukas emitted successfully" });
        
        return BadRequest(new { success = false, message = "Failed to emit Lukas" });
    }

    [HttpGet("lukas/value")]
    public async Task<IActionResult> GetLukasValue()
    {
        var value = await _adminService.GetLukasValueAsync();
        return Ok(new { success = true, data = value });
    }

    [HttpPut("lukas/value")]
    public async Task<IActionResult> UpdateLukasValue([FromBody] UpdateLukasValueDto dto)
    {
        var result = await _adminService.UpdateLukasValueAsync(dto);
        if (result)
            return Ok(new { success = true, message = "Lukas value updated successfully" });
        
        return BadRequest(new { success = false, message = "Failed to update Lukas value" });
    }
}
