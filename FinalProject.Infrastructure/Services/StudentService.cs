using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly LukitasDbContext _context;

    public StudentService(LukitasDbContext context)
    {
        _context = context;
    }

    public IQueryable<CampaignResponseDto> GetAvailableCampaigns(int studentId)
    {
        // Subconsulta para obtener IDs de campañas inscritas
        var enrolledCampaignIds = _context.Accounts
            .Where(a => a.UserId == studentId)
            .Select(a => a.CampaignId);

        // Retorna IQueryable para que el filtrado se haga en BD
        return _context.Campaigns
            .Where(c => c.Active == true && 
                       c.EndDate >= DateOnly.FromDateTime(DateTime.Now) &&
                       !enrolledCampaignIds.Contains(c.Id))
            .Include(c => c.User)
            .Include(c => c.Accounts)
            .Select(c => new CampaignResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CampaignType = "General",
                Budget = c.Budget ?? 0,
                RemainingBudget = c.Budget ?? 0,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Schedule = c.Schedule,
                Location = c.Location,
                ContactNumber = c.ContactNumber,
                Active = c.Active ?? true,
                EnrolledStudents = c.Accounts.Count,
                CompanyName = c.User!.Company
            });
    }

    public async Task<decimal> GetLukasBalanceAsync(int studentId)
    {
        var totalBalance = await _context.Accounts
            .Where(a => a.UserId == studentId && a.Status == "active")
            .SumAsync(a => a.Balance ?? 0);

        return totalBalance;
    }

    public async Task<bool> PurchaseProductsAsync(ProductPurchaseDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var studentBalance = await GetLukasBalanceAsync(dto.StudentId);
            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                    throw new Exception($"Product {item.ProductId} not available");

                total += product.Price * item.Quantity;
            }

            if (studentBalance < total)
                throw new Exception("Insufficient Lukas balance");

            var studentAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == dto.StudentId && a.Status == "active");

            if (studentAccount == null)
                throw new Exception("Student account not found");

            var sale = new Sale
            {
                AccountId = studentAccount.Id,
                SaleDate = DateTime.Now,
                Total = total,
                Status = "completed"
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                
                var saleDetail = new SaleDetail
                {
                    SaleId = sale.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product!.Price,
                    Subtotal = product.Price * item.Quantity
                };

                _context.SaleDetails.Add(saleDetail);
                product.Stock -= item.Quantity;
            }

            studentAccount.Balance -= total;

            var supplierAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == dto.SupplierId && a.Status == "active");

            if (supplierAccount != null)
            {
                supplierAccount.Balance += total;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
