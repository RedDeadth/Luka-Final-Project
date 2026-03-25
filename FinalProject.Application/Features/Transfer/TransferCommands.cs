using FinalProject.Application.Common;
using FinalProject.Application.DTOs.TransferDtos;
using MediatR;

namespace FinalProject.Application.Features.Transfer;

public record CreateTransferCommand(CreateTransferDto Dto) : IRequest<Result<TransferResponseDto>>;
