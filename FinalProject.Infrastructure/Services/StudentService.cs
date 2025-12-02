using FinalProject.Domain.Entities;
using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.DTOs.ProductDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly LukitasDbContext _context;

    public StudentService(IUnitOfWork unitOfWork, LukitasDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public IQueryable<CampaignResponseDto> GetAvailableCampaigns(int studentId)
    {
        var enrolledCampaignIds = _context.Accounts
            .Where(a => a.UserId == studentId)
            .Select(a => a.CampaignId);

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
        var accounts = _unitOfWork.Accounts.Query(a => a.UserId == studentId && a.Status == "active");
        var totalBalance = await accounts.SumAsync(a => a.Balance ?? 0);
        return totalBalance;
    }

    public async Task<bool> PurchaseProductsAsync(ProductPurchaseDto dto)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var studentBalance = await GetLukasBalanceAsync(dto.StudentId);
            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                    throw new Exception($"Product {item.ProductId} not available");

                total += product.Price * item.Quantity;
            }

            if (studentBalance < total)
                throw new Exception("Insufficient Lukas balance");

            var studentAccount = await _unitOfWork.Accounts
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

            await _unitOfWork.Sales.AddAsync(sale);

            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                
                var saleDetail = new SaleDetail
                {
                    SaleId = sale.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product!.Price,
                    Subtotal = product.Price * item.Quantity
                };

                await _unitOfWork.SaleDetails.AddAsync(saleDetail);
                product.Stock -= item.Quantity;
                _unitOfWork.Products.Update(product);
            }

            studentAccount.Balance -= total;
            _unitOfWork.Accounts.Update(studentAccount);

            var supplierAccount = await _unitOfWork.Accounts
                .FirstOrDefaultAsync(a => a.UserId == dto.SupplierId && a.Status == "active");

            if (supplierAccount != null)
            {
                supplierAccount.Balance += total;
                _unitOfWork.Accounts.Update(supplierAccount);
            }

            return true;
        });
    }
}
