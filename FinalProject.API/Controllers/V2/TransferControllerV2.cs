using FinalProject.Application.DTOs.TransferDtos;
using FinalProject.Application.Features.Transfer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/transfer")]
public class TransferControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;

    public TransferControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferDto dto)
    {
        var result = await _mediator.Send(new CreateTransferCommand(dto));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data }) : BadRequest(new { success = false, message = result.Error });
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetTransfersByAccount(int accountId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetTransfersByAccountQuery(accountId, page, pageSize));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data.Data }) : BadRequest(new { message = result.Error });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransferById(int id)
    {
        var result = await _mediator.Send(new GetTransferByIdQuery(id));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data }) : NotFound(new { success = false, message = result.Error });
    }
}
