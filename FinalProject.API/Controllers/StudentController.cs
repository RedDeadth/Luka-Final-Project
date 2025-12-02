using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("{studentId}/campaigns")]
    public async Task<IActionResult> GetAvailableCampaigns(int studentId)
    {
        var campaigns = await _studentService.GetAvailableCampaignsAsync(studentId);
        return Ok(new { success = true, data = campaigns });
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
