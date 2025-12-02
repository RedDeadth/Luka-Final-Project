using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;
    private const int MaxPageSize = 100;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("{studentId}/campaigns")]
    public async Task<IActionResult> GetAvailableCampaigns(int studentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _studentService.GetAvailableCampaigns(studentId);
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

    [HttpGet("{studentId}/balance")]
    public async Task<IActionResult> GetLukasBalance(int studentId)
    {
        var balance = await _studentService.GetLukasBalanceAsync(studentId);
        return Ok(new { success = true, balance });
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseProducts([FromBody] ProductPurchaseDto dto)
    {
        try
        {
            var result = await _studentService.PurchaseProductsAsync(dto);
            return Ok(new { success = true, message = "Purchase completed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
