# 🪙 Sistema Lukitas - Moneda Virtual Educativa

Sistema de moneda virtual para entornos educativos que permite a estudiantes ganar y gastar "Lukas" en campañas universitarias.

## 📋 Tabla de Contenidos

- [Tecnologías](#-tecnologías)
- [Arquitectura](#-arquitectura)
- [Patrones Implementados](#-patrones-implementados)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Instalación](#-instalación)
- [API Endpoints](#-api-endpoints)
- [Rúbrica de Evaluación](#-rúbrica-de-evaluación)

---

## 🛠 Tecnologías

| Componente | Tecnología |
|------------|------------|
| Backend | .NET 9, ASP.NET Core |
| Base de Datos | MySQL 9.2 |
| ORM | Entity Framework Core 9 |
| Autenticación | JWT (JSON Web Tokens) |
| IA/ML | ML.NET 3.0 |
| Reportes | ClosedXML |
| Jobs | Hangfire |
| Deploy | Railway, Docker |

---

## 🏗 Arquitectura

El proyecto implementa **Arquitectura Hexagonal (Ports & Adapters)** con 4 capas bien definidas:

```
┌─────────────────────────────────────────────────────────────┐
│                    FinalProject.API                          │
│              (Adaptadores Primarios - Controllers)           │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│                FinalProject.Application                      │
│                (Puertos - Interfaces, DTOs)                  │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│                  FinalProject.Domain                         │
│            (Núcleo - Entidades, Interfaces)                  │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│              FinalProject.Infrastructure                     │
│     (Adaptadores Secundarios - Repositorios, Servicios)      │
└─────────────────────────────────────────────────────────────┘
```

### Flujo de Dependencias

- **Domain** → Sin dependencias externas (núcleo puro)
- **Application** → Depende solo de Domain
- **Infrastructure** → Depende de Domain y Application
- **API** → Depende de Application e Infrastructure

---

## 📐 Patrones Implementados

### 1. Repository Pattern

Abstracción del acceso a datos mediante interfaz genérica:

```csharp
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<PaginatedResult<T>> GetAllAsync(int page, int pageSize);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> Query();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

**Ubicación:** `FinalProject.Domain/Interfaces/IGenericRepository.cs`

### 2. Unit of Work Pattern

Coordina múltiples repositorios en una sola transacción:

```csharp
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<Account> Accounts { get; }
    IGenericRepository<Campaign> Campaigns { get; }
    IGenericRepository<Transfer> Transfers { get; }
    // ... más repositorios
    
    Task<int> SaveChangesAsync();
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
}
```

**Uso en servicios:**
```csharp
public async Task<TransferResponseDto> CreateTransferAsync(CreateTransferDto dto)
{
    return await _unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var source = await _unitOfWork.Accounts.GetByIdAsync(dto.SourceAccountId);
        var dest = await _unitOfWork.Accounts.GetByIdAsync(dto.DestinationAccountId);
        
        source.Balance -= dto.Amount;
        dest.Balance += dto.Amount;
        
        _unitOfWork.Accounts.Update(source);
        _unitOfWork.Accounts.Update(dest);
        
        return result;
    });
}
```

**Ubicación:** `FinalProject.Domain/Interfaces/IUnitOfWork.cs`

### 3. Middlewares

#### JwtAuthMiddleware
Valida la firma del token JWT, verifica issuer, audience y expiración.

```csharp
var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
```

#### ErrorHandlingMiddleware
Maneja excepciones y retorna respuestas JSON consistentes:

```csharp
var (statusCode, message) = exception switch
{
    UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
    KeyNotFoundException => (HttpStatusCode.NotFound, "Not found"),
    _ => (HttpStatusCode.InternalServerError, "Error")
};
```

#### LoggingMiddleware
Registra cada request con ID único y tiempo de respuesta:

```
[abc123] Request: POST /api/Transfer from 192.168.1.1
[abc123] Response: 200 in 45ms
```

**Ubicación:** `FinalProject.API/Middlewares/`

### 4. CQRS (Command Query Responsibility Segregation)

Separación de comandos y consultas usando MediatR:

```csharp
// Command
public record CreateCampaignCommand(...) : IRequest<Result<CampaignResponseDto>>;

// Query
public record GetActiveCampaignsQuery(int Page, int PageSize) : IRequest<Result<PaginatedResponse>>;
```

**Ubicación:** `FinalProject.Application/Features/`

---

## 🤖 Inteligencia Artificial (ML.NET)

Sistema de recomendaciones de productos usando **ML.NET 3.0** con algoritmo de **Matrix Factorization** (Filtrado Colaborativo).

### Endpoints

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Recommendation/user/{userId}` | Recomendaciones personalizadas |
| POST | `/api/Recommendation/train` | Entrenar modelo |
| GET | `/api/Recommendation/status` | Estado del modelo |

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

**Ubicación:** `FinalProject.Infrastructure/Services/RecommendationService.cs`

---

## 📁 Estructura del Proyecto

```
Luka-Final-Project/
├── FinalProject.API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── CampaignController.cs
│   │   ├── TransferController.cs
│   │   ├── RecommendationController.cs    # IA
│   │   └── ...
│   ├── Middlewares/
│   │   ├── JwtAuthMiddleware.cs
│   │   ├── ErrorHandlingMiddleware.cs
│   │   └── LoggingMiddleware.cs
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs
│   └── Program.cs
│
├── FinalProject.Application/
│   ├── DTOs/
│   │   ├── AuthDtos/
│   │   ├── CampaignDtos/
│   │   ├── TransferDtos/
│   │   └── ...
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── ICampaignService.cs
│   │   ├── IRecommendationService.cs      # IA
│   │   └── ...
│   ├── Features/                          # CQRS
│   │   ├── Auth/
│   │   ├── Campaign/
│   │   └── ...
│   └── Common/
│
├── FinalProject.Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Campaign.cs
│   │   ├── Account.cs
│   │   ├── Transfer.cs
│   │   └── ...
│   └── Interfaces/
│       ├── IGenericRepository.cs
│       └── IUnitOfWork.cs
│
├── FinalProject.Infrastructure/
│   ├── Data/
│   │   └── LukitasDbContext.cs
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   └── UnitOfWork.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── TransferService.cs
│   │   ├── RecommendationService.cs       # ML.NET
│   │   ├── ReportService.cs               # Excel
│   │   └── ...
│   ├── Handlers/                          # MediatR
│   └── Jobs/                              # Hangfire
│
├── ARQUITECTURA.md
├── Dockerfile
└── README.md
```

---

## 🚀 Instalación

### Prerrequisitos

- .NET 9 SDK
- MySQL 9.2
- Git

### Pasos

1. **Clonar repositorio**
```bash
git clone https://github.com/RedDeadth/Luka-Final-Project.git
cd Luka-Final-Project
```

2. **Configurar base de datos**

Editar `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=lukitas_db;User=root;Password=tu_password;"
  }
}
```

3. **Restaurar paquetes y ejecutar**
```bash
dotnet restore
dotnet run --project FinalProject.API
```

4. **Acceder a Swagger**
```
http://localhost:5140/swagger
```

---

## 📡 API Endpoints

### Autenticación
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/Auth/login` | Iniciar sesión |
| POST | `/api/Auth/register` | Registrar usuario |

### Campañas
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Campaign/active` | Campañas activas |
| POST | `/api/Campaign` | Crear campaña |
| POST | `/api/Campaign/enroll` | Inscribir estudiante |

### Transferencias
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/Transfer` | Crear transferencia |
| GET | `/api/Transfer/account/{id}` | Transferencias por cuenta |

### Productos
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Product` | Listar productos |
| POST | `/api/Product` | Crear producto |
| PUT | `/api/Product/{id}` | Actualizar producto |

### Reportes (Excel)
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Report/transactions/excel` | Reporte de transferencias |
| GET | `/api/Report/sales/excel` | Reporte de ventas |
| GET | `/api/Report/users/excel` | Reporte de usuarios |

### IA - Recomendaciones
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/Recommendation/user/{id}` | Recomendaciones personalizadas |
| POST | `/api/Recommendation/train` | Entrenar modelo ML |
| GET | `/api/Recommendation/status` | Estado del modelo |

---



---

## 👥 Autor

Proyecto Final - Sistema de Moneda Virtual Educativa

---

## 📄 Licencia

Este proyecto es de uso educativo.
