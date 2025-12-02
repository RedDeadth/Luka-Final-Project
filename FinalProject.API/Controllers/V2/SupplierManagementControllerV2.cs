using FinalProject.Application.DTOs.SupplierDtos;
using FinalProject.Application.Features.Supplier;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/[controller]")]
public class SupplierManagementControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;
    private const int MaxPageSize = 100;

    public SupplierManagementControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto dto)
    {
        var result = await _mediator.Send(new CreateSupplierCommand(dto));
        return result.IsSuccess
            ? Ok(new { success = true, data = result.Data })
            : BadRequest(new { success = false, message = result.ErrorMessage });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSuppliers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);

        var result = await _mediator.Send(new GetAllSuppliersQuery(page, pageSize));
        return Ok(new
        {
            success = true,
            data = result.Data!.Data,
            pagination = new
            {
                page = result.Data.Page,
                pageSize = result.Data.PageSize,
                totalCount = result.Data.TotalCount,
                totalPages = result.Data.TotalPages
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplierById(int id)
    {
        var result = await _mediator.Send(new GetSupplierByIdQuery(id));
        return result.IsSuccess
            ? Ok(new { success = true, data = result.Data })
            : NotFound(new { success = false, message = result.ErrorMessage });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] CreateSupplierDto dto)
    {
        var result = await _mediator.Send(new UpdateSupplierCommand(id, dto));
        return result.IsSuccess
            ? Ok(new { success = true, message = "Supplier updated successfully" })
            : NotFound(new { success = false, message = result.ErrorMessage });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var result = await _mediator.Send(new DeleteSupplierCommand(id));
        return result.IsSuccess
            ? Ok(new { success = true, message = "Supplier deleted successfully" })
            : NotFound(new { success = false, message = result.ErrorMessage });
    }
}
