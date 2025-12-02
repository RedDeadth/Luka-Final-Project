namespace FinalProject.Application.DTOs.CampaignDtos;

public class CreateCampaignDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CampaignType { get; set; } = null!; // Tipo de campaña
    public decimal Budget { get; set; } // Presupuesto en Lukitas
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Schedule { get; set; } // Horarios de apertura y cierre
    public string? Location { get; set; } // Ubicación
    public string? ContactNumber { get; set; }
    public List<string>? ImageUrls { get; set; } // URLs de imágenes
}
