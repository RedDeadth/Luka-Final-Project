using FinalProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    /// <summary>
    /// Obtiene recomendaciones de productos personalizadas para un usuario
    /// usando Machine Learning (filtrado colaborativo con ML.NET)
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetRecommendations(int userId, [FromQuery] int count = 5)
    {
        try
        {
            var recommendations = await _recommendationService.GetRecommendationsForUserAsync(userId, count);
            
            return Ok(new
            {
                success = true,
                data = recommendations,
                metadata = new
                {
                    userId,
                    count = recommendations.Count,
                    modelTrained = _recommendationService.IsModelTrained,
                    algorithm = "Matrix Factorization (ML.NET)"
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Entrena el modelo de recomendaciones con los datos actuales
    /// </summary>
    [HttpPost("train")]
    public async Task<IActionResult> TrainModel()
    {
        try
        {
            await _recommendationService.TrainModelAsync();
            
            return Ok(new
            {
                success = true,
                message = "Modelo de IA entrenado exitosamente",
                algorithm = "Matrix Factorization",
                framework = "ML.NET 3.0"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el estado del modelo de IA
    /// </summary>
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            success = true,
            data = new
            {
                modelTrained = _recommendationService.IsModelTrained,
                algorithm = "Matrix Factorization (Collaborative Filtering)",
                framework = "ML.NET 3.0",
                description = "Sistema de recomendaciones basado en patrones de compra de usuarios similares"
            }
        });
    }
}
