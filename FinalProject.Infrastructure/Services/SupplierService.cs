using FinalProject.Application.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class SupplierService : ISupplierService
{
    private readonly LukitasDbContext _context;

    public SupplierService(LukitasDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetLukasBalanceAsync(int supplierId)
    {
        var supplierAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.UserId == supplierId && a.Status == "active");

        return supplierAccount?.Balance ?? 0;
    }

    public async Task<bool> ConvertLukasToRealMoneyAsync(int supplierId, decimal amount)
    {
        var supplierAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.UserId == supplierId && a.Status == "active");

        if (supplierAccount == null || supplierAccount.Balance < amount)
            return false;

        supplierAccount.Balance -= amount;
        
        var transfer = new Transfer
        {
            SourceAccountId = supplierAccount.Id,
            DestinationAccountId = null,
            TransferDate = DateOnly.FromDateTime(DateTime.Now),
            Amount = amount,
            Status = "completed"
        };

        _context.Transfers.Add(transfer);
        await _context.SaveChangesAsync();

        return true;
    }
}
