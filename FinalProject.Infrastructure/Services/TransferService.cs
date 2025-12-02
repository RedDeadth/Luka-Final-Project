using FinalProject.Application.DTOs.TransferDtos;
using FinalProject.Application.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Services;

public class TransferService : ITransferService
{
    private readonly LukitasDbContext _context;

    public TransferService(LukitasDbContext context)
    {
        _context = context;
    }

    public async Task<TransferResponseDto> CreateTransferAsync(CreateTransferDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var sourceAccount = await _context.Accounts.FindAsync(dto.SourceAccountId);
            var destinationAccount = await _context.Accounts.FindAsync(dto.DestinationAccountId);

            if (sourceAccount == null || destinationAccount == null)
                throw new Exception("Account not found");

            if (sourceAccount.Balance < dto.Amount)
                throw new Exception("Insufficient balance");

            sourceAccount.Balance -= dto.Amount;
            destinationAccount.Balance += dto.Amount;

            var transfer = new Transfer
            {
                SourceAccountId = dto.SourceAccountId,
                DestinationAccountId = dto.DestinationAccountId,
                TransferDate = DateOnly.FromDateTime(DateTime.Now),
                Amount = dto.Amount,
                Status = "completed"
            };

            _context.Transfers.Add(transfer);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetTransferByIdAsync(transfer.Id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
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
        var transfer = await _context.Transfers
            .Include(t => t.SourceAccount)
            .Include(t => t.DestinationAccount)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transfer == null) throw new Exception("Transfer not found");

        return new TransferResponseDto
        {
            Id = transfer.Id,
            SourceAccountId = transfer.SourceAccountId ?? 0,
            SourceAccountNumber = transfer.SourceAccount!.AccountNumber,
            DestinationAccountId = transfer.DestinationAccountId ?? 0,
            DestinationAccountNumber = transfer.DestinationAccount!.AccountNumber,
            TransferDate = transfer.TransferDate,
            Amount = transfer.Amount,
            Status = transfer.Status ?? "pending"
        };
    }
}
