namespace FinalProject.Application.DTOs.TransferDtos;

public class CreateTransferDto
{
    public int SourceAccountId { get; set; }
    public int DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
}
