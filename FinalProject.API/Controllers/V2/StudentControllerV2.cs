using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Features.Student;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/student")]
public class StudentControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{studentId}/campaigns")]
    public async Task<IActionResult> GetAvailableCampaigns(int studentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetAvailableCampaignsQuery(studentId, page, pageSize));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data.Data }) : BadRequest(new { message = result.Error });
    }

    [HttpGet("{studentId}/balance")]
    public async Task<IActionResult> GetStudentBalance(int studentId)
    {
        var result = await _mediator.Send(new GetStudentBalanceQuery(studentId));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data }) : NotFound(new { success = false, message = result.Error });
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseProducts([FromBody] ProductPurchaseDto dto)
    {
        var result = await _mediator.Send(new PurchaseProductsCommand(dto.StudentId, dto.Products));
        return result.IsSuccess ? Ok(new { success = true, message = "Purchase successful" }) : BadRequest(new { success = false, message = result.Error });
    }
}
