using FinalProject.Application.DTOs.SupplierDtos;
using FinalProject.Application.Features.Supplier;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/suppliermanagement")]
public class SupplierManagementControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;

    public SupplierManagementControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto dto)
    {
        var result = await _mediator.Send(new CreateSupplierCommand(dto));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data }) : BadRequest(new { success = false, message = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSuppliers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetAllSuppliersQuery(page, pageSize));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data.Data }) : BadRequest(new { message = result.Error });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplierById(int id)
    {
        var result = await _mediator.Send(new GetSupplierByIdQuery(id));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data }) : NotFound(new { success = false, message = result.Error });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] CreateSupplierDto dto)
    {
        var result = await _mediator.Send(new UpdateSupplierCommand(id, dto));
        return result.IsSuccess ? Ok(new { success = true, message = "Supplier updated successfully" }) : BadRequest(new { success = false, message = result.Error });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var result = await _mediator.Send(new DeleteSupplierCommand(id));
        return result.IsSuccess ? Ok(new { success = true, message = "Supplier deleted successfully" }) : NotFound(new { success = false, message = result.Error });
    }
}
