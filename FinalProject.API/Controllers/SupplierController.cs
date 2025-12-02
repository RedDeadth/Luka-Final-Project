using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet("{supplierId}/balance")]
    public async Task<IActionResult> GetLukasBalance(int supplierId)
    {
        var balance = await _supplierService.GetLukasBalanceAsync(supplierId);
        return Ok(new { success = true, balance });
    }

    [HttpPost("{supplierId}/convert")]
    public async Task<IActionResult> ConvertLukasToRealMoney(int supplierId, [FromBody] decimal amount)
    {
        var result = await _supplierService.ConvertLukasToRealMoneyAsync(supplierId, amount);
        if (result)
            return Ok(new { success = true, message = "Lukas converted successfully" });
        
        return BadRequest(new { success = false, message = "Failed to convert Lukas" });
    }
}
