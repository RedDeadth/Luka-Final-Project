using FinalProject.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("transactions/excel")]
    public async Task<IActionResult> DownloadTransactionsReport(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var fileBytes = await _reportService.GenerateTransactionsReportAsync(startDate, endDate);
        var fileName = $"Reporte_Transferencias_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        
        return File(fileBytes, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            fileName);
    }

    [HttpGet("sales/excel")]
    public async Task<IActionResult> DownloadSalesReport(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var fileBytes = await _reportService.GenerateSalesReportAsync(startDate, endDate);
        var fileName = $"Reporte_Ventas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        
        return File(fileBytes, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            fileName);
    }

    [HttpGet("users/excel")]
    public async Task<IActionResult> DownloadUsersReport()
    {
        var fileBytes = await _reportService.GenerateUsersReportAsync();
        var fileName = $"Reporte_Usuarios_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        
        return File(fileBytes, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            fileName);
    }
}
