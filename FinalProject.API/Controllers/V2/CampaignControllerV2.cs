using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.Features.Campaign;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

[ApiController]
[Route("api/v2/campaign")]
public class CampaignControllerV2 : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignControllerV2(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDto dto)
    {
        var result = await _mediator.Send(new CreateCampaignCommand(dto.Name, dto.Description, dto.CompanyId, dto.Budget, dto.StartDate, dto.EndDate, dto.RewardPerStudent));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data }) : BadRequest(new { success = false, message = result.Error });
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCampaigns([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetActiveCampaignsQuery(page, pageSize));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data.Data }) : BadRequest(new { message = result.Error });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCampaignById(int id)
    {
        var result = await _mediator.Send(new GetCampaignByIdQuery(id));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data }) : NotFound(new { success = false, message = result.Error });
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollCampaignDto dto)
    {
        var result = await _mediator.Send(new EnrollStudentCommand(dto.CampaignId, dto.StudentId));
        return result.IsSuccess ? Ok(new { success = true, message = "Enrolled successfully" }) : BadRequest(new { success = false, message = result.Error });
    }

    [HttpGet("company/{companyUserId}")]
    public async Task<IActionResult> GetCompanyCampaigns(int companyUserId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetCompanyCampaignsQuery(companyUserId, page, pageSize));
        return result.IsSuccess ? Ok(new { success = true, data = result.Data.Data }) : BadRequest(new { message = result.Error });
    }
}
