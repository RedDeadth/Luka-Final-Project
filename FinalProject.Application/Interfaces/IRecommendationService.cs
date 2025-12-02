using FinalProject.Application.DTOs.ProductDtos;

namespace FinalProject.Application.Interfaces;

public interface IRecommendationService
{
    /// <summary>
    /// Obtiene recomendaciones de productos para un usuario basado en su historial de compras
    /// usando Machine Learning (filtrado colaborativo).
    /// </summary>
    Task<List<ProductRecommendationDto>> GetRecommendationsForUserAsync(int userId, int count = 5);
    
    /// <summary>
    /// Entrena el modelo de recomendaciones con los datos actuales de compras.
    /// </summary>
    Task TrainModelAsync();
    
    /// <summary>
    /// Indica si el modelo está entrenado y listo para hacer predicciones.
    /// </summary>
    bool IsModelTrained { get; }
}

public class ProductRecommendationDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public float Score { get; set; }
    public string Reason { get; set; } = string.Empty;
}
