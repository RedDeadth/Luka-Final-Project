using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Features.Student;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

/// <summary>
/// Controlador de estudiantes usando CQRS con MediatR
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{studentId}/campaigns")]
    public async Task<IActionResult> GetAvailableCampaigns(int studentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetAvailableCampaignsQuery(studentId, page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, data = result.Data!.Data, pagination = result.Data.Pagination });
    }

    [HttpGet("{studentId}/balance")]
    public async Task<IActionResult> GetLukasBalance(int studentId)
    {
        var query = new GetStudentBalanceQuery(studentId);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, balance = result.Data });
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseProducts([FromBody] ProductPurchaseDto dto)
    {
        var command = new PurchaseProductsCommand(dto.StudentId, dto.SupplierId, dto.Items);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, message = "Purchase completed successfully" });
    }
}
