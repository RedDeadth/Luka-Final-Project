using FinalProject.Application.Common;
using FinalProject.Application.DTOs.TransferDtos;
using FinalProject.Application.Features.Coupon;
using MediatR;

namespace FinalProject.Application.Features.Transfer;

// Queries
public record GetTransferByIdQuery(int Id) : IRequest<Result<TransferResponseDto>>;
public record GetTransfersByAccountQuery(int AccountId, int Page, int PageSize) : IRequest<Result<PaginatedResult<TransferResponseDto>>>;
