using FinalProject.Domain.Entities;
using FinalProject.Application.Common;
using FinalProject.Application.DTOs.CampaignDtos;
using FinalProject.Application.Features.Student;
using FinalProject.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Handlers.Student;

public class GetAvailableCampaignsQueryHandler : IRequestHandler<GetAvailableCampaignsQuery, Result<PaginatedResponse<CampaignResponseDto>>>
{
    private readonly LukitasDbContext _context;
    private const int MaxPageSize = 100;
    public GetAvailableCampaignsQueryHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<PaginatedResponse<CampaignResponseDto>>> Handle(GetAvailableCampaignsQuery request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        var page = Math.Max(1, request.Page);

        var enrolledIds = _context.Accounts.Where(a => a.UserId == request.StudentId).Select(a => a.CampaignId);
        var query = _context.Campaigns.Where(c => c.Active == true && c.EndDate >= DateOnly.FromDateTime(DateTime.Now) && !enrolledIds.Contains(c.Id));
        var totalCount = await query.CountAsync(cancellationToken);

        var campaigns = await query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => new CampaignResponseDto
            {
                Id = c.Id, Name = c.Name, Description = c.Description, CampaignType = "General",
                Budget = c.Budget ?? 0, RemainingBudget = c.Budget ?? 0, StartDate = c.StartDate,
                EndDate = c.EndDate, Schedule = c.Schedule, Location = c.Location,
                ContactNumber = c.ContactNumber, Active = c.Active ?? true,
                EnrolledStudents = c.Accounts.Count, CompanyName = c.User!.Company
            }).ToListAsync(cancellationToken);

        return Result<PaginatedResponse<CampaignResponseDto>>.Ok(new PaginatedResponse<CampaignResponseDto>
        {
            Data = campaigns,
            Pagination = new PaginationInfo { Page = page, PageSize = pageSize, TotalCount = totalCount, TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize) }
        });
    }
}

public class GetStudentBalanceQueryHandler : IRequestHandler<GetStudentBalanceQuery, Result<decimal>>
{
    private readonly LukitasDbContext _context;
    public GetStudentBalanceQueryHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<decimal>> Handle(GetStudentBalanceQuery request, CancellationToken cancellationToken)
    {
        var balance = await _context.Accounts.Where(a => a.UserId == request.StudentId && a.Status == "active").SumAsync(a => a.Balance ?? 0, cancellationToken);
        return Result<decimal>.Ok(balance);
    }
}

public class PurchaseProductsCommandHandler : IRequestHandler<PurchaseProductsCommand, Result<bool>>
{
    private readonly LukitasDbContext _context;
    public PurchaseProductsCommandHandler(LukitasDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(PurchaseProductsCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var balance = await _context.Accounts.Where(a => a.UserId == request.StudentId && a.Status == "active").SumAsync(a => a.Balance ?? 0, cancellationToken);
            decimal total = 0;

            foreach (var item in request.Items)
            {
                var product = await _context.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);
                if (product == null || product.Stock < item.Quantity) return Result<bool>.Fail($"Product {item.ProductId} not available");
                total += product.Price * item.Quantity;
            }

            if (balance < total) return Result<bool>.Fail("Insufficient balance");

            var studentAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == request.StudentId && a.Status == "active", cancellationToken);
            if (studentAccount == null) return Result<bool>.NotFound("Student account not found");

            var sale = new Sale { AccountId = studentAccount.Id, SaleDate = DateTime.Now, Total = total, Status = "completed" };
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync(cancellationToken);

            foreach (var item in request.Items)
            {
                var product = await _context.Products.FindAsync(new object[] { item.ProductId }, cancellationToken);
                _context.SaleDetails.Add(new SaleDetail { SaleId = sale.Id, ProductId = item.ProductId, Quantity = item.Quantity, UnitPrice = product!.Price, Subtotal = product.Price * item.Quantity });
                product.Stock -= item.Quantity;
            }

            studentAccount.Balance -= total;
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<bool>.Fail(ex.Message);
        }
    }
}
