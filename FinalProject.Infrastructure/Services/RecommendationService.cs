using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace FinalProject.Infrastructure.Services;

/// <summary>
/// Servicio de recomendaciones de productos usando ML.NET
/// Implementa filtrado colaborativo con Matrix Factorization
/// </summary>
public class RecommendationService : IRecommendationService
{
    private readonly LukitasDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly MLContext _mlContext;
    private ITransformer? _model;
    private PredictionEngine<ProductRating, ProductRatingPrediction>? _predictionEngine;

    public bool IsModelTrained => _model != null;

    public RecommendationService(LukitasDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _mlContext = new MLContext(seed: 42);
    }

    public async Task TrainModelAsync()
    {
        // Obtener datos de compras para entrenar el modelo
        var purchaseData = await _context.SaleDetails
            .Include(sd => sd.Sale)
            .ThenInclude(s => s!.Account)
            .Where(sd => sd.Sale != null && sd.Sale.Account != null)
            .Select(sd => new ProductRating
            {
                UserId = (float)(sd.Sale!.Account!.UserId ?? 0),
                ProductId = (float)(sd.ProductId ?? 0),
                Rating = sd.Quantity // Usamos cantidad como indicador de preferencia
            })
            .ToListAsync();

        if (purchaseData.Count < 5)
        {
            // Si no hay suficientes datos, crear datos sintéticos para demo
            purchaseData = GenerateSyntheticData();
        }

        var dataView = _mlContext.Data.LoadFromEnumerable(purchaseData);

        // Configurar el pipeline de ML con Matrix Factorization
        var options = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = "UserIdEncoded",
            MatrixRowIndexColumnName = "ProductIdEncoded",
            LabelColumnName = "Rating",
            NumberOfIterations = 20,
            ApproximationRank = 10,
            LearningRate = 0.1
        };

        var pipeline = _mlContext.Transforms.Conversion
            .MapValueToKey("UserIdEncoded", "UserId")
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("ProductIdEncoded", "ProductId"))
            .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(options));

        // Entrenar el modelo
        _model = pipeline.Fit(dataView);
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<ProductRating, ProductRatingPrediction>(_model);
    }

    public async Task<List<ProductRecommendationDto>> GetRecommendationsForUserAsync(int userId, int count = 5)
    {
        var recommendations = new List<ProductRecommendationDto>();

        // Si el modelo no está entrenado, entrenarlo
        if (!IsModelTrained)
        {
            await TrainModelAsync();
        }

        if (_predictionEngine == null)
        {
            return recommendations;
        }

        // Obtener productos que el usuario NO ha comprado
        var purchasedProductIds = await _context.SaleDetails
            .Include(sd => sd.Sale)
            .ThenInclude(s => s!.Account)
            .Where(sd => sd.Sale!.Account!.UserId == userId)
            .Select(sd => sd.ProductId)
            .Distinct()
            .ToListAsync();

        var availableProducts = await _context.Products
            .Include(p => p.Supplier)
            .Where(p => p.Status == "active" && !purchasedProductIds.Contains(p.Id))
            .ToListAsync();

        // Predecir score para cada producto
        foreach (var product in availableProducts)
        {
            var prediction = _predictionEngine.Predict(new ProductRating
            {
                UserId = userId,
                ProductId = product.Id
            });

            if (!float.IsNaN(prediction.Score) && prediction.Score > 0)
            {
                recommendations.Add(new ProductRecommendationDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    SupplierName = product.Supplier?.Name ?? "",
                    Score = prediction.Score,
                    Reason = GetRecommendationReason(prediction.Score)
                });
            }
        }

        // Ordenar por score y tomar los mejores
        return recommendations
            .OrderByDescending(r => r.Score)
            .Take(count)
            .ToList();
    }

    private string GetRecommendationReason(float score)
    {
        return score switch
        {
            > 3 => "Altamente recomendado basado en usuarios similares",
            > 2 => "Recomendado por patrones de compra similares",
            > 1 => "Podría interesarte según tu perfil",
            _ => "Sugerencia basada en tendencias"
        };
    }

    private List<ProductRating> GenerateSyntheticData()
    {
        // Datos sintéticos para demostración cuando no hay suficientes compras
        var data = new List<ProductRating>();
        var random = new Random(42);

        for (int userId = 1; userId <= 10; userId++)
        {
            for (int productId = 1; productId <= 20; productId++)
            {
                if (random.NextDouble() > 0.7) // 30% de probabilidad de compra
                {
                    data.Add(new ProductRating
                    {
                        UserId = userId,
                        ProductId = productId,
                        Rating = random.Next(1, 6)
                    });
                }
            }
        }

        return data;
    }
}

// Clases para ML.NET
public class ProductRating
{
    public float UserId { get; set; }
    public float ProductId { get; set; }
    public float Rating { get; set; }
}

public class ProductRatingPrediction
{
    public float Score { get; set; }
}
