using FinalProject.Application.DTOs.SupplierDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierManagementController : ControllerBase
{
    private readonly ISupplierManagementService _supplierService;
    private const int MaxPageSize = 100;

    public SupplierManagementController(ISupplierManagementService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto dto)
    {
        try
        {
            var supplier = await _supplierService.CreateSupplierAsync(dto);
            return Ok(new { success = true, data = supplier });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSuppliers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);
        
        var query = _supplierService.GetAllSuppliers();
        var totalCount = await query.CountAsync();
        var suppliers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return Ok(new { 
            success = true, 
            data = suppliers,
            pagination = new {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplierById(int id)
    {
        try
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            return Ok(new { success = true, data = supplier });
        }
        catch (Exception ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] CreateSupplierDto dto)
    {
        var result = await _supplierService.UpdateSupplierAsync(id, dto);
        if (result)
            return Ok(new { success = true, message = "Supplier updated successfully" });
        
        return NotFound(new { success = false, message = "Supplier not found" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var result = await _supplierService.DeleteSupplierAsync(id);
        if (result)
            return Ok(new { success = true, message = "Supplier deleted successfully" });
        
        return NotFound(new { success = false, message = "Supplier not found" });
    }
}
