namespace FinalProject.Application.DTOs.TransferDtos;

public class TransferResponseDto
{
    public int Id { get; set; }
    public int SourceAccountId { get; set; }
    public string SourceAccountNumber { get; set; } = null!;
    public int DestinationAccountId { get; set; }
    public string DestinationAccountNumber { get; set; } = null!;
    public DateOnly TransferDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
}
