using FinalProject.Application.DTOs.TransferDtos;
using FinalProject.Application.Features.Transfer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/[controller]")]
public class TransferControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;
    private const int MaxPageSize = 100;

    public TransferControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferDto dto)
    {
        var result = await _mediator.Send(new CreateTransferCommand(dto));
        return result.IsSuccess
            ? Ok(new { success = true, data = result.Data })
            : BadRequest(new { success = false, message = result.ErrorMessage });
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetTransfersByAccount(int accountId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(1, page);

        var result = await _mediator.Send(new GetTransfersByAccountQuery(accountId, page, pageSize));
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
    public async Task<IActionResult> GetTransferById(int id)
    {
        var result = await _mediator.Send(new GetTransferByIdQuery(id));
        return result.IsSuccess
            ? Ok(new { success = true, data = result.Data })
            : NotFound(new { success = false, message = result.ErrorMessage });
    }
}
