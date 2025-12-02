using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _productService.CreateProductAsync(dto);
            return Ok(new { success = true, data = product });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProducts().ToListAsync();
        return Ok(new { success = true, data = products });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(new { success = true, data = product });
        }
        catch (Exception ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("supplier/{supplierId}")]
    public async Task<IActionResult> GetProductsBySupplier(int supplierId)
    {
        var products = await _productService.GetProductsBySupplier(supplierId).ToListAsync();
        return Ok(new { success = true, data = products });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
    {
        var result = await _productService.UpdateProductAsync(id, dto);
        if (result)
            return Ok(new { success = true, message = "Product updated successfully" });
        
        return NotFound(new { success = false, message = "Product not found" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (result)
            return Ok(new { success = true, message = "Product deleted successfully" });
        
        return NotFound(new { success = false, message = "Product not found" });
    }

    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
    {
        var result = await _productService.UpdateStockAsync(id, quantity);
        if (result)
            return Ok(new { success = true, message = "Stock updated successfully" });
        
        return NotFound(new { success = false, message = "Product not found" });
    }
}
