using FinalProject.Application.Common;
using FinalProject.Application.DTOs.TransferDtos;
using FinalProject.Application.Features.Transfer;
using FinalProject.Application.Interfaces;
using MediatR;

namespace FinalProject.Infrastructure.Handlers.Transfer;

public class CreateTransferCommandHandler : IRequestHandler<CreateTransferCommand, Result<TransferResponseDto>>
{
    private readonly ITransferService _transferService;

    public CreateTransferCommandHandler(ITransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<Result<TransferResponseDto>> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transfer = await _transferService.CreateTransferAsync(request.Dto);
            return Result<TransferResponseDto>.Ok(transfer);
        }
        catch (Exception ex)
        {
            return Result<TransferResponseDto>.Failure(ex.Message);
        }
    }
}
