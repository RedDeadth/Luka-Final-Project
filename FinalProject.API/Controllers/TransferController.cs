using FinalProject.Application.DTOs.TransferDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferController : ControllerBase
{
    private readonly ITransferService _transferService;

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
    public async Task<IActionResult> GetTransfersByAccount(int accountId)
    {
        var transfers = await _transferService.GetTransfersByAccountAsync(accountId);
        return Ok(new { success = true, data = transfers });
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
