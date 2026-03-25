using FinalProject.Application.DTOs.CompanyDtos;
using FinalProject.Application.DTOs.LukasDtos;
using FinalProject.Application.Features.Admin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/admin")]
public class AdminControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("companies/pending")]
    public async Task<IActionResult> GetPendingCompanies([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetPendingCompaniesQuery(page, pageSize));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data.Data, pagination = new { page, pageSize, totalCount = result.Data.TotalCount, totalPages = (int)Math.Ceiling(result.Data.TotalCount / (double)pageSize) } }) : BadRequest(new { message = result.Error });
    }

    [HttpPost("companies/approve")]
    public async Task<IActionResult> ApproveCompany([FromBody] CompanyApprovalDto dto)
    {
        var result = await _mediator.Send(new ApproveCompanyCommand(dto.CompanyId, dto.Approved, dto.Reason));
        return result.IsSuccess ? Ok(new { success = true, message = "Company approval status updated" }) : BadRequest(new { success = false, message = result.Error });
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetSystemStatistics()
    {
        var result = await _mediator.Send(new GetSystemStatisticsQuery());
        return result.IsSuccess ? Ok(new { success = true, data = result.Data }) : BadRequest(new { message = result.Error });
    }

    [HttpPost("lukas/emit")]
    public async Task<IActionResult> EmitLukas([FromBody] EmitLukasDto dto)
    {
        var result = await _mediator.Send(new EmitLukasCommand(dto.CompanyId, dto.Amount, dto.Reason));
        return result.IsSuccess ? Ok(new { success = true, message = "Lukas emitted successfully" }) : BadRequest(new { success = false, message = result.Error });
    }
}
