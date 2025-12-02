using FinalProject.Domain.Entities;
using FinalProject.Application.DTOs.TransferDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class TransferService : ITransferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly LukitasDbContext _context;

    public TransferService(IUnitOfWork unitOfWork, LukitasDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<TransferResponseDto> CreateTransferAsync(CreateTransferDto dto)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var sourceAccount = await _unitOfWork.Accounts.GetByIdAsync(dto.SourceAccountId);
            var destinationAccount = await _unitOfWork.Accounts.GetByIdAsync(dto.DestinationAccountId);

            if (sourceAccount == null || destinationAccount == null)
                throw new Exception("Account not found");

            if (sourceAccount.Balance < dto.Amount)
                throw new Exception("Insufficient balance");

            sourceAccount.Balance -= dto.Amount;
            destinationAccount.Balance += dto.Amount;

            _unitOfWork.Accounts.Update(sourceAccount);
            _unitOfWork.Accounts.Update(destinationAccount);

            var transfer = new Transfer
            {
                SourceAccountId = dto.SourceAccountId,
                DestinationAccountId = dto.DestinationAccountId,
                TransferDate = DateOnly.FromDateTime(DateTime.Now),
                Amount = dto.Amount,
                Status = "completed"
            };

            await _unitOfWork.Transfers.AddAsync(transfer);

            return new TransferResponseDto
            {
                Id = transfer.Id,
                SourceAccountId = transfer.SourceAccountId ?? 0,
                SourceAccountNumber = sourceAccount.AccountNumber,
                DestinationAccountId = transfer.DestinationAccountId ?? 0,
                DestinationAccountNumber = destinationAccount.AccountNumber,
                TransferDate = transfer.TransferDate,
                Amount = transfer.Amount,
                Status = transfer.Status ?? "pending"
            };
        });
    }

    public IQueryable<TransferResponseDto> GetTransfersByAccount(int accountId)
    {
        return _context.Transfers
            .Include(t => t.SourceAccount)
            .Include(t => t.DestinationAccount)
            .Where(t => t.SourceAccountId == accountId || t.DestinationAccountId == accountId)
            .Select(t => new TransferResponseDto
            {
                Id = t.Id,
                SourceAccountId = t.SourceAccountId ?? 0,
                SourceAccountNumber = t.SourceAccount!.AccountNumber,
                DestinationAccountId = t.DestinationAccountId ?? 0,
                DestinationAccountNumber = t.DestinationAccount!.AccountNumber,
                TransferDate = t.TransferDate,
                Amount = t.Amount,
                Status = t.Status ?? "pending"
            });
    }

    public async Task<TransferResponseDto> GetTransferByIdAsync(int id)
    {
        var transfer = await _unitOfWork.Transfers.FirstOrDefaultAsync(t => t.Id == id);
        if (transfer == null) throw new Exception("Transfer not found");

        var sourceAccount = await _unitOfWork.Accounts.GetByIdAsync(transfer.SourceAccountId ?? 0);
        var destAccount = await _unitOfWork.Accounts.GetByIdAsync(transfer.DestinationAccountId ?? 0);

        return new TransferResponseDto
        {
            Id = transfer.Id,
            SourceAccountId = transfer.SourceAccountId ?? 0,
            SourceAccountNumber = sourceAccount?.AccountNumber ?? "",
            DestinationAccountId = transfer.DestinationAccountId ?? 0,
            DestinationAccountNumber = destAccount?.AccountNumber ?? "",
            TransferDate = transfer.TransferDate,
            Amount = transfer.Amount,
            Status = transfer.Status ?? "pending"
        };
    }
}
