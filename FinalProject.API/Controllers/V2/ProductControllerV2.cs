using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Features.Product;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

/// <summary>
/// Controlador de productos usando CQRS con MediatR
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        var command = new CreateProductCommand(
            dto.SupplierId,
            dto.ProductTypeId,
            dto.Code,
            dto.Name,
            dto.Price,
            dto.Stock
        );

        var result = await _mediator.Send(command);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return StatusCode(201, new { success = true, data = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetAllProductsQuery(page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, data = result.Data!.Data, pagination = result.Data.Pagination });
    }
}
