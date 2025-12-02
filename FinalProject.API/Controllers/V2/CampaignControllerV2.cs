using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.Features.Campaign;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers.V2;

/// <summary>
/// Controlador de campañas usando CQRS con MediatR
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDto dto, [FromQuery] int companyUserId)
    {
        var command = new CreateCampaignCommand(
            companyUserId,
            dto.Name,
            dto.Description,
            dto.CampaignType,
            dto.Budget,
            dto.StartDate,
            dto.EndDate,
            dto.Schedule,
            dto.Location,
            dto.ContactNumber,
            dto.ImageUrls
        );

        var result = await _mediator.Send(command);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return StatusCode(201, new { success = true, data = result.Data });
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCampaigns([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetActiveCampaignsQuery(page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, data = result.Data!.Data, pagination = result.Data.Pagination });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCampaignById(int id)
    {
        var query = new GetCampaignByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 404, new { success = false, message = result.Error });

        return Ok(new { success = true, data = result.Data });
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollCampaignDto dto)
    {
        var command = new EnrollStudentCommand(dto.CampaignId, dto.StudentId);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, message = "Student enrolled successfully" });
    }

    [HttpGet("company/{companyUserId}")]
    public async Task<IActionResult> GetCompanyCampaigns(int companyUserId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetCompanyCampaignsQuery(companyUserId, page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(result.StatusCode ?? 400, new { success = false, message = result.Error });

        return Ok(new { success = true, data = result.Data!.Data, pagination = result.Data.Pagination });
    }
}
