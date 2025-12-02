namespace FinalProject.Application.DTOs.StatisticsDtos;

public class SystemStatisticsDto
{
    public int TotalUsers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalCompanies { get; set; }
    public int TotalSuppliers { get; set; }
    public int ActiveCampaigns { get; set; }
    public decimal TotalLukasInCirculation { get; set; }
    public decimal TotalLukasSpent { get; set; }
    public int TotalTransactions { get; set; }
    public List<ActivityLogDto> RecentActivity { get; set; } = new();
}

public class ActivityLogDto
{
    public DateTime Timestamp { get; set; }
    public string ActivityType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
}
