using FinalProject.Application.DTOs.CompanyDtos;
using FinalProject.Application.DTOs.LukasDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private const int MaxPageSize = 100;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("companies/pending")]
    public async Task<IActionResult> GetPendingCompanies([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _adminService.GetPendingCompanies();
        var totalCount = await query.CountAsync();
        var companies = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new { 
            success = true, 
            data = companies,
            pagination = new {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
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
