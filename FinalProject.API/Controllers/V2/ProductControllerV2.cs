using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Features.Product;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/product")]
public class ProductControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        var result = await _mediator.Send(new CreateProductCommand(dto.SupplierId, dto.Name, dto.Description, dto.PriceLukas, dto.Stock, dto.ProductTypeId));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data }) : BadRequest(new { success = false, message = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetAllProductsQuery(page, pageSize));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data.Data }) : BadRequest(new { message = result.Error });
    }
}
