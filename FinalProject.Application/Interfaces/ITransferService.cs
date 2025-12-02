using FinalProject.Application.DTOs.TransferDtos;

namespace FinalProject.Application.Interfaces;

public interface ITransferService
{
    Task<TransferResponseDto> CreateTransferAsync(CreateTransferDto dto);
    IQueryable<TransferResponseDto> GetTransfersByAccount(int accountId);
    Task<TransferResponseDto> GetTransferByIdAsync(int id);
}
