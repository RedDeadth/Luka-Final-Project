# Arquitectura del Sistema Lukitas

## Visión General

Sistema de moneda virtual educativa que permite a estudiantes ganar y gastar "Lukas" en campañas universitarias.

```
┌─────────────────────────────────────────────────────────────────┐
│                        FRONTEND                                  │
│                    React + Vite + TypeScript                     │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ HTTP/REST + JWT
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                        BACKEND API                               │
│                    .NET 9 + ASP.NET Core                         │
│                    + ML.NET (IA)                                 │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ Entity Framework Core
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                        BASE DE DATOS                             │
│                    MySQL 9.2                                     │
└─────────────────────────────────────────────────────────────────┘
```

## Arquitectura Hexagonal (Ports & Adapters)

```
                    ┌─────────────────────────────────────┐
                    │         ADAPTADORES PRIMARIOS       │
                    │     (FinalProject.API/Controllers)  │
                    │   HTTP REST → Casos de Uso          │
                    └─────────────────┬───────────────────┘
                                      │
                    ┌─────────────────▼───────────────────┐
                    │              PUERTOS                │
                    │   (FinalProject.Application)        │
                    │   Interfaces: IAuthService,         │
                    │   IRecommendationService (IA)       │
                    └─────────────────┬───────────────────┘
                                      │
                    ┌─────────────────▼───────────────────┐
                    │         NÚCLEO DE DOMINIO           │
                    │      (FinalProject.Domain)          │
                    │   Entities: User, Campaign, etc.    │
                    │   Interfaces: IUnitOfWork           │
                    └─────────────────┬───────────────────┘
                                      │
                    ┌─────────────────▼───────────────────┐
                    │      ADAPTADORES SECUNDARIOS        │
                    │   (FinalProject.Infrastructure)     │
                    │   UnitOfWork, Services, ML.NET      │
                    └─────────────────────────────────────┘
```

## Integración de Inteligencia Artificial (ML.NET)

El sistema incluye un módulo de IA para recomendaciones de productos usando **ML.NET 3.0**.

### Algoritmo: Matrix Factorization (Filtrado Colaborativo)

```csharp
public interface IRecommendationService
{
    Task<List<ProductRecommendationDto>> GetRecommendationsForUserAsync(int userId, int count = 5);
    Task TrainModelAsync();
    bool IsModelTrained { get; }
}
```

### Funcionamiento

1. **Recolección de datos**: Se obtienen los datos de compras (SaleDetails) de la base de datos
2. **Entrenamiento**: El modelo aprende patrones de compra usando Matrix Factorization
3. **Predicción**: Para cada usuario, predice qué productos le podrían interesar basándose en usuarios con patrones similares

### Endpoints de IA

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Recommendation/user/{userId}` | Obtiene recomendaciones personalizadas |
| POST | `/api/Recommendation/train` | Entrena el modelo con datos actuales |
| GET | `/api/Recommendation/status` | Estado del modelo de IA |

### Ejemplo de Respuesta

```json
{
  "success": true,
  "data": [
    {
      "productId": 26,
      "productName": "Galletas Surtidas",
      "price": 4.00,
      "score": 1.55,
      "reason": "Podría interesarte según tu perfil"
    }
  ],
  "metadata": {
    "algorithm": "Matrix Factorization (ML.NET)"
  }
}
```

## Patrón Repository + Unit of Work

```csharp
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<Account> Accounts { get; }
    // ... más repositorios
    
    Task<int> SaveChangesAsync();
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
}
```

## Middlewares

- **JwtAuthMiddleware**: Valida firma del token JWT
- **ErrorHandlingMiddleware**: Maneja excepciones
- **LoggingMiddleware**: Request ID + tiempo de respuesta

## Estructura de Carpetas

```
Luka-Final-Project/
├── FinalProject.API/
│   ├── Controllers/
│   │   ├── RecommendationController.cs  # IA
│   │   └── ...
│   ├── Middlewares/
│   └── Extensions/
│
├── FinalProject.Application/
│   ├── Interfaces/
│   │   ├── IRecommendationService.cs    # IA
│   │   └── ...
│   └── DTOs/
│
├── FinalProject.Domain/
│   ├── Entities/
│   └── Interfaces/
│
├── FinalProject.Infrastructure/
│   ├── Services/
│   │   ├── RecommendationService.cs     # ML.NET
│   │   └── ...
│   ├── Repositories/
│   └── Data/
│
└── ARQUITECTURA.md
```

## Tecnologías

| Componente | Tecnología |
|------------|------------|
| Backend | .NET 9, ASP.NET Core |
| IA/ML | ML.NET 3.0, Matrix Factorization |
| ORM | Entity Framework Core 9 |
| Base de Datos | MySQL 9.2 |
| Autenticación | JWT |
| Reportes | ClosedXML |
| Deploy | Railway, Docker |

## Patrones Implementados

| Patrón | Ubicación |
|--------|-----------|
| Arquitectura Hexagonal | Todo el proyecto |
| Repository | Domain + Infrastructure |
| Unit of Work | Domain + Infrastructure |
| CQRS | Application/Features |
| Dependency Injection | API/Extensions |
| Machine Learning | Infrastructure/Services |
