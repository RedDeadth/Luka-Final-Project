using FinalProject.Application.Common;
using FinalProject.Application.DTOs.TransferDtos;
using FinalProject.Application.Features.Coupon;
using FinalProject.Application.Features.Transfer;
using FinalProject.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Handlers.Transfer;

public class GetTransferByIdQueryHandler : IRequestHandler<GetTransferByIdQuery, Result<TransferResponseDto>>
{
    private readonly ITransferService _transferService;

    public GetTransferByIdQueryHandler(ITransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<Result<TransferResponseDto>> Handle(GetTransferByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var transfer = await _transferService.GetTransferByIdAsync(request.Id);
            return Result<TransferResponseDto>.Ok(transfer);
        }
        catch (Exception ex)
        {
            return Result<TransferResponseDto>.Failure(ex.Message);
        }
    }
}

public class GetTransfersByAccountQueryHandler : IRequestHandler<GetTransfersByAccountQuery, Result<PaginatedResult<TransferResponseDto>>>
{
    private readonly ITransferService _transferService;

    public GetTransfersByAccountQueryHandler(ITransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<Result<PaginatedResult<TransferResponseDto>>> Handle(GetTransfersByAccountQuery request, CancellationToken cancellationToken)
    {
        var query = _transferService.GetTransfersByAccount(request.AccountId);
        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<TransferResponseDto>>.Ok(new PaginatedResult<TransferResponseDto>
        {
            Data = data,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}
