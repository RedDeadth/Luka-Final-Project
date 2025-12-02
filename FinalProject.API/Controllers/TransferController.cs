using FinalProject.Application.DTOs.TransferDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferController : ControllerBase
{
    private readonly ITransferService _transferService;
    private const int MaxPageSize = 100;

    public TransferController(ITransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferDto dto)
    {
        try
        {
            var transfer = await _transferService.CreateTransferAsync(dto);
            return Ok(new { success = true, data = transfer });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetTransfersByAccount(int accountId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _transferService.GetTransfersByAccount(accountId);
        var totalCount = await query.CountAsync();
        var transfers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new { 
            success = true, 
            data = transfers,
            pagination = new {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransferById(int id)
    {
        try
        {
            var transfer = await _transferService.GetTransferByIdAsync(id);
            return Ok(new { success = true, data = transfer });
        }
        catch (Exception ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }
}
