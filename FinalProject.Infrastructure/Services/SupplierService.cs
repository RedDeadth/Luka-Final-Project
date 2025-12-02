using FinalProject.Domain.Entities;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;

namespace FinalProject.Infrastructure.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;

    public SupplierService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> GetLukasBalanceAsync(int supplierId)
    {
        var supplierAccount = await _unitOfWork.Accounts
            .FirstOrDefaultAsync(a => a.UserId == supplierId && a.Status == "active");

        return supplierAccount?.Balance ?? 0;
    }

    public async Task<bool> ConvertLukasToRealMoneyAsync(int supplierId, decimal amount)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var supplierAccount = await _unitOfWork.Accounts
                .FirstOrDefaultAsync(a => a.UserId == supplierId && a.Status == "active");

            if (supplierAccount == null || supplierAccount.Balance < amount)
                return false;

            supplierAccount.Balance -= amount;
            _unitOfWork.Accounts.Update(supplierAccount);
            
            var transfer = new Transfer
            {
                SourceAccountId = supplierAccount.Id,
                DestinationAccountId = null,
                TransferDate = DateOnly.FromDateTime(DateTime.Now),
                Amount = amount,
                Status = "completed"
            };

            await _unitOfWork.Transfers.AddAsync(transfer);

            return true;
        });
    }
}
