using FinalProject.Domain.Entities;
using ClosedXML.Excel;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class ReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly LukitasDbContext _context;

    public ReportService(IUnitOfWork unitOfWork, LukitasDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<byte[]> GenerateTransactionsReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Transfers
            .Include(t => t.SourceAccount).ThenInclude(a => a!.User)
            .Include(t => t.DestinationAccount).ThenInclude(a => a!.User)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.TransferDate >= DateOnly.FromDateTime(startDate.Value));
        if (endDate.HasValue)
            query = query.Where(t => t.TransferDate <= DateOnly.FromDateTime(endDate.Value));

        var transfers = await query.OrderByDescending(t => t.TransferDate).ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Transferencias");

        worksheet.Cell("A1").Value = "REPORTE DE TRANSFERENCIAS";
        worksheet.Range("A1:F1").Merge();
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.DarkBlue;
        worksheet.Cell("A1").Style.Font.FontColor = XLColor.White;

        worksheet.Cell("A2").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        worksheet.Range("A2:F2").Merge();

        var headers = new[] { "ID", "Fecha", "Origen", "Destino", "Monto", "Estado" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(4, i + 1).Value = headers[i];
            worksheet.Cell(4, i + 1).Style.Font.Bold = true;
            worksheet.Cell(4, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Cell(4, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        int row = 5;
        foreach (var t in transfers)
        {
            worksheet.Cell(row, 1).Value = t.Id;
            worksheet.Cell(row, 2).Value = t.TransferDate.ToString();
            worksheet.Cell(row, 3).Value = t.SourceAccount?.User?.Email ?? "N/A";
            worksheet.Cell(row, 4).Value = t.DestinationAccount?.User?.Email ?? "N/A";
            worksheet.Cell(row, 5).Value = t.Amount;
            worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 6).Value = t.Status;
            
            if (t.Status == "completed")
                worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.Green;
            else
                worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.Orange;

            row++;
        }

        worksheet.Cell(row + 1, 4).Value = "TOTAL:";
        worksheet.Cell(row + 1, 4).Style.Font.Bold = true;
        worksheet.Cell(row + 1, 5).FormulaA1 = $"=SUM(E5:E{row - 1})";
        worksheet.Cell(row + 1, 5).Style.Font.Bold = true;
        worksheet.Cell(row + 1, 5).Style.NumberFormat.Format = "#,##0.00";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerateSalesReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Sales
            .Include(s => s.Account).ThenInclude(a => a!.User)
            .Include(s => s.SaleDetails).ThenInclude(sd => sd.Product)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(s => s.SaleDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(s => s.SaleDate <= endDate.Value);

        var sales = await query.OrderByDescending(s => s.SaleDate).ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ventas");

        worksheet.Cell("A1").Value = "REPORTE DE VENTAS";
        worksheet.Range("A1:F1").Merge();
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.DarkGreen;
        worksheet.Cell("A1").Style.Font.FontColor = XLColor.White;

        worksheet.Cell("A2").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        worksheet.Range("A2:F2").Merge();

        var headers = new[] { "ID", "Fecha", "Cliente", "Total", "Estado", "Productos" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(4, i + 1).Value = headers[i];
            worksheet.Cell(4, i + 1).Style.Font.Bold = true;
            worksheet.Cell(4, i + 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
            worksheet.Cell(4, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        int row = 5;
        foreach (var s in sales)
        {
            worksheet.Cell(row, 1).Value = s.Id;
            worksheet.Cell(row, 2).Value = s.SaleDate?.ToString("dd/MM/yyyy HH:mm") ?? "";
            worksheet.Cell(row, 3).Value = s.Account?.User?.Email ?? "N/A";
            worksheet.Cell(row, 4).Value = s.Total;
            worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 5).Value = s.Status;
            worksheet.Cell(row, 6).Value = s.SaleDetails?.Count ?? 0;
            row++;
        }

        worksheet.Cell(row + 1, 3).Value = "TOTAL VENTAS:";
        worksheet.Cell(row + 1, 3).Style.Font.Bold = true;
        worksheet.Cell(row + 1, 4).FormulaA1 = $"=SUM(D5:D{row - 1})";
        worksheet.Cell(row + 1, 4).Style.Font.Bold = true;
        worksheet.Cell(row + 1, 4).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(row + 1, 4).Style.Fill.BackgroundColor = XLColor.Yellow;

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerateUsersReportAsync()
    {
        var users = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Accounts)
            .OrderBy(u => u.RoleId)
            .ThenBy(u => u.LastName)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Usuarios");

        worksheet.Cell("A1").Value = "REPORTE DE USUARIOS";
        worksheet.Range("A1:H1").Merge();
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.DarkOrange;
        worksheet.Cell("A1").Style.Font.FontColor = XLColor.White;

        worksheet.Cell("A2").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        worksheet.Range("A2:H2").Merge();

        var headers = new[] { "ID", "Nombre", "Apellido", "Email", "Rol", "Empresa/Universidad", "Balance", "Estado" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(4, i + 1).Value = headers[i];
            worksheet.Cell(4, i + 1).Style.Font.Bold = true;
            worksheet.Cell(4, i + 1).Style.Fill.BackgroundColor = XLColor.LightSalmon;
            worksheet.Cell(4, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        int row = 5;
        foreach (var u in users)
        {
            worksheet.Cell(row, 1).Value = u.Id;
            worksheet.Cell(row, 2).Value = u.FirstName;
            worksheet.Cell(row, 3).Value = u.LastName;
            worksheet.Cell(row, 4).Value = u.Email;
            worksheet.Cell(row, 5).Value = u.Role?.Name ?? "N/A";
            worksheet.Cell(row, 6).Value = u.RoleId == 1 ? u.University : u.Company;
            worksheet.Cell(row, 7).Value = u.Accounts?.Where(a => a.Status == "active").Sum(a => a.Balance ?? 0) ?? 0;
            worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 8).Value = u.Active == true ? "Activo" : "Inactivo";
            
            var roleColor = u.RoleId switch
            {
                1 => XLColor.LightBlue,
                2 => XLColor.LightGreen,
                3 => XLColor.LightYellow,
                _ => XLColor.White
            };
            worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = roleColor;
            
            row++;
        }

        row += 2;
        worksheet.Cell(row, 1).Value = "RESUMEN";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row + 1, 1).Value = "Total Estudiantes:";
        worksheet.Cell(row + 1, 2).Value = users.Count(u => u.RoleId == 1);
        worksheet.Cell(row + 2, 1).Value = "Total Empresas:";
        worksheet.Cell(row + 2, 2).Value = users.Count(u => u.RoleId == 2);
        worksheet.Cell(row + 3, 1).Value = "Total Admins:";
        worksheet.Cell(row + 3, 2).Value = users.Count(u => u.RoleId == 3);

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
