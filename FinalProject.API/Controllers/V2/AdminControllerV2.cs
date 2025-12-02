using FinalProject.Application.DTOs.CompanyDtos;
using FinalProject.Application.DTOs.LukasDtos;
using FinalProject.Application.Features.Admin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

/// <summary>
/// Controlador de administración usando CQRS con MediatR
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("companies/pending")]
    public async Task<IActionResult> GetPendingCompanies([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetPendingCompaniesQuery(page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, data = result.Data!.Data, pagination = result.Data.Pagination });
    }

    [HttpPost("companies/approve")]
    public async Task<IActionResult> ApproveCompany([FromBody] CompanyApprovalDto dto)
    {
        var command = new ApproveCompanyCommand(dto.CompanyId, dto.Approved, dto.Reason);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, message = "Company approval status updated" });
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetSystemStatistics()
    {
        var query = new GetSystemStatisticsQuery();
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, data = result.Data });
    }

    [HttpPost("lukas/emit")]
    public async Task<IActionResult> EmitLukas([FromBody] EmitLukasDto dto)
    {
        var command = new EmitLukasCommand(dto.CompanyId, dto.Amount, dto.Reason);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, message = "Lukas emitted successfully" });
    }
}
