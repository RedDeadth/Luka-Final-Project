# 📋 INFORME TÉCNICO COMPLETO
## Sistema de Gestión de Lukitas - FinalProject

---

## 1. DESCRIPCIÓN GENERAL DEL PROYECTO

### 1.1 ¿Qué es?
Sistema de gestión de moneda virtual "Lukitas" para entornos educativos que permite:
- **Empresas**: Crear campañas promocionales y emitir lukitas
- **Estudiantes**: Inscribirse a campañas, recibir lukitas y comprar productos
- **Proveedores**: Vender productos y convertir lukitas a dinero real
- **Administradores**: Aprobar empresas, emitir lukitas y gestionar estadísticas

### 1.2 Tecnologías
| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| .NET | 9.0 | Framework principal |
| ASP.NET Core | 9.0 | API REST |
| Entity Framework Core | 9.0.10 | ORM |
| MySQL | 9.0.1 | Base de datos |
| Pomelo.EntityFrameworkCore.MySql | 9.0.0 | Proveedor MySQL para EF |
| MediatR | 12.4.1 | Implementación CQRS |
| JWT | 8.14.0 | Autenticación |
| Swashbuckle | 7.0.0 | Documentación Swagger |
| Hangfire.Core | 1.8.22 | Procesamiento en segundo plano |
| Hangfire.AspNetCore | 1.8.22 | Integración Hangfire con ASP.NET Core |
| Hangfire.MySqlStorage | 2.0.3 | Storage de Hangfire en MySQL |

---

## 2. ARQUITECTURA DEL PROYECTO

### 2.1 Clean Architecture (4 Capas)

```
┌─────────────────────────────────────────────────────────────┐
│                    FinalProject.API                         │
│  (Controladores V1/V2, Middlewares, Configuración)          │
│  Puerto: http://localhost:5140                              │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                FinalProject.Application                     │
│  (DTOs, Interfaces, Commands, Queries, Common)              │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  FinalProject.Domain                        │
│  (Entidades, Interfaces de Repositorios)                    │
└─────────────────────────────────────────────────────────────┘
                              ▲
                              │
┌─────────────────────────────────────────────────────────────┐
│               FinalProject.Infrastructure                   │
│  (Servicios, Repositorios, Handlers CQRS, DbContext)        │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Flujo de Dependencias
```
API → Application → Domain ← Infrastructure
```

---

## 3. CAPA: FinalProject.Domain

### 3.1 Entidades (16 total)

| Entidad | Tabla BD | Descripción |
|---------|----------|-------------|
| `User` | `users` | Usuarios del sistema |
| `Role` | `roles` | Roles del sistema |
| `Account` | `accounts` | Cuentas de lukitas |
| `Campaign` | `campaigns` | Campañas promocionales |
| `Product` | `products` | Productos de proveedores |
| `ProductType` | `product_types` | Tipos de productos |
| `Supplier` | `suppliers` | Proveedores |
| `SupplierType` | `supplier_types` | Tipos de proveedores |
| `Sale` | `sales` | Ventas realizadas |
| `SaleDetail` | `sale_details` | Detalle de ventas |
| `Transfer` | `transfers` | Transferencias |
| `Coupon` | `coupons` | Cupones de descuento |
| `MissionTemplate` | `mission_templates` | Catálogo de misiones |
| `UserMission` | `user_missions` | Misiones de usuarios |
| `Achievement` | `achievements` | Logros obtenidos |

### 3.2 Interfaces de Repositorios
- `IGenericRepository<T>`: Operaciones CRUD genéricas con paginación
- `IUnitOfWork`: Patrón Unit of Work para transacciones

---

## 4. CAPA: FinalProject.Application

### 4.1 Interfaces de Servicios (10 total)
| Interfaz | Propósito |
|----------|-----------|
| `IAuthService` | Autenticación y JWT |
| `IAdminService` | Funciones administrativas |
| `ICampaignService` | Gestión de campañas |
| `IStudentService` | Funciones de estudiantes |
| `IProductService` | Gestión de productos |
| `ISupplierService` | Balance de proveedores |
| `ISupplierManagementService` | CRUD de proveedores |
| `IMissionService` | Gestión de misiones |
| `ICouponService` | Gestión de cupones |
| `ITransferService` | Transferencias |

### 4.2 DTOs por Módulo
- **AuthDtos**: LoginRequestDto, LoginResponseDto
- **CampaignDtos**: CreateCampaignDto, CampaignResponseDto, EnrollCampaignDto
- **CompanyDtos**: CompanyProfileDto, CompanyApprovalDto
- **LukasDtos**: EmitLukasDto, LukasValueDto, UpdateLukasValueDto
- **ProductDtos**: CreateProductDto, ProductResponseDto, ProductPurchaseDto
- **MissionDtos**: AssignMissionDto, CompleteMissionDto, UserMissionResponseDto
- **CouponDtos**: CreateCouponDto, CouponResponseDto
- **TransferDtos**: CreateTransferDto, TransferResponseDto
- **SupplierDtos**: CreateSupplierDto, SupplierResponseDto
- **StatisticsDtos**: SystemStatisticsDto, ActivityLogDto

### 4.3 Common (CQRS Base)
```
FinalProject.Application/Common/
├── ICommand.cs          # Interfaz base para Commands
├── IQuery.cs            # Interfaz base para Queries
├── ICommandHandler.cs   # Interfaz base para Command Handlers
├── IQueryHandler.cs     # Interfaz base para Query Handlers
└── Result.cs            # Clase para encapsular respuestas
```

### 4.4 Features (Commands y Queries)
```
FinalProject.Application/Features/
├── Auth/
│   └── LoginCommand.cs
├── Admin/
│   ├── AdminCommands.cs
│   └── AdminQueries.cs
├── Campaign/
│   ├── CampaignCommands.cs
│   └── CampaignQueries.cs
├── Product/
│   ├── ProductCommands.cs
│   └── ProductQueries.cs
├── Student/
│   ├── StudentCommands.cs
│   └── StudentQueries.cs
├── Coupon/
│   ├── CouponCommands.cs
│   └── CouponQueries.cs
├── Mission/
│   ├── MissionCommands.cs
│   └── MissionQueries.cs
├── Transfer/
│   ├── TransferCommands.cs
│   └── TransferQueries.cs
└── Supplier/
    ├── SupplierCommands.cs
    └── SupplierQueries.cs
```

---

## 5. CAPA: FinalProject.Infrastructure

### 5.1 Servicios de Negocio (10 total)
| Servicio | Implementa |
|----------|------------|
| `AuthService` | IAuthService |
| `AdminService` | IAdminService |
| `CampaignService` | ICampaignService |
| `StudentService` | IStudentService |
| `ProductService` | IProductService |
| `SupplierService` | ISupplierService |
| `SupplierManagementService` | ISupplierManagementService |
| `MissionService` | IMissionService |
| `CouponService` | ICouponService |
| `TransferService` | ITransferService |

### 5.2 Handlers CQRS (MediatR)
```
FinalProject.Infrastructure/Handlers/
├── Auth/
│   └── LoginCommandHandler.cs
├── Admin/
│   └── AdminHandlers.cs
├── Campaign/
│   ├── CampaignCommandHandlers.cs
│   └── CampaignQueryHandlers.cs
├── Product/
│   └── ProductHandlers.cs
├── Student/
│   └── StudentHandlers.cs
├── Coupon/
│   ├── CouponCommandHandlers.cs
│   └── CouponQueryHandlers.cs
├── Mission/
│   ├── MissionCommandHandlers.cs
│   └── MissionQueryHandlers.cs
├── Transfer/
│   ├── TransferCommandHandlers.cs
│   └── TransferQueryHandlers.cs
└── Supplier/
    ├── SupplierCommandHandlers.cs
    └── SupplierQueryHandlers.cs
```

---

## 6. CAPA: FinalProject.API

### 6.1 Controladores V1 (Tradicionales)
| Controlador | Endpoints |
|-------------|-----------|
| AuthController | 2 |
| AdminController | 6 |
| CampaignController | 5 |
| StudentController | 3 |
| ProductController | 7 |
| SupplierController | 2 |
| SupplierManagementController | 5 |
| MissionController | 5 |
| CouponController | 6 |
| TransferController | 3 |

### 6.2 Controladores V2 (CQRS con MediatR)
```
FinalProject.API/Controllers/V2/
├── AuthControllerV2.cs           # POST /api/v2/auth/login
├── AdminControllerV2.cs          # 4 endpoints
├── CampaignControllerV2.cs       # 5 endpoints
├── ProductControllerV2.cs        # 2 endpoints
├── StudentControllerV2.cs        # 3 endpoints
├── CouponControllerV2.cs         # 7 endpoints
├── MissionControllerV2.cs        # 5 endpoints
├── TransferControllerV2.cs       # 3 endpoints
└── SupplierManagementControllerV2.cs  # 5 endpoints
```

### 6.3 Endpoints V2 Completos

#### Auth V2
```
POST /api/v2/auth/login
```

#### Admin V2
```
GET  /api/v2/admin/companies/pending
POST /api/v2/admin/companies/approve
GET  /api/v2/admin/statistics
POST /api/v2/admin/lukas/emit
```

#### Campaign V2
```
POST /api/v2/campaign/create
GET  /api/v2/campaign/active
GET  /api/v2/campaign/{id}
POST /api/v2/campaign/enroll
GET  /api/v2/campaign/company/{companyUserId}
```

#### Product V2
```
POST /api/v2/product
GET  /api/v2/product
```

#### Student V2
```
GET  /api/v2/student/{studentId}/campaigns
GET  /api/v2/student/{studentId}/balance
POST /api/v2/student/purchase
```

#### Coupon V2
```
POST   /api/v2/coupon
GET    /api/v2/coupon/code/{code}
GET    /api/v2/coupon/campaign/{campaignId}
GET    /api/v2/coupon/supplier/{supplierId}
GET    /api/v2/coupon/validate/{code}
DELETE /api/v2/coupon/{id}
```

#### Mission V2
```
POST /api/v2/mission/assign
POST /api/v2/mission/complete
GET  /api/v2/mission/user/{userId}
GET  /api/v2/mission/user/{userId}/pending
GET  /api/v2/mission/user/{userId}/completed
```

#### Transfer V2
```
POST /api/v2/transfer
GET  /api/v2/transfer/account/{accountId}
GET  /api/v2/transfer/{id}
```

#### Supplier V2
```
POST   /api/v2/suppliermanagement
GET    /api/v2/suppliermanagement
GET    /api/v2/suppliermanagement/{id}
PUT    /api/v2/suppliermanagement/{id}
DELETE /api/v2/suppliermanagement/{id}
```

---

## 7. PATRÓN CQRS CON MEDIATR

### 7.1 Descripción
CQRS (Command Query Responsibility Segregation) separa las operaciones de lectura (Queries) de las de escritura (Commands).

### 7.2 Clase Result<T>
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool Success => IsSuccess;
    public T? Data { get; }
    public string? ErrorMessage { get; }
    public string? Error => ErrorMessage;
    public int? StatusCode { get; }

    public static Result<T> Ok(T data);
    public static Result<T> Failure(string error, int statusCode = 400);
    public static Result<T> NotFound(string error);
}
```

### 7.3 Ejemplo de Command
```csharp
// Definición
public record CreateCouponCommand(CreateCouponDto Dto) 
    : IRequest<Result<CouponResponseDto>>;

// Handler
public class CreateCouponCommandHandler 
    : IRequestHandler<CreateCouponCommand, Result<CouponResponseDto>>
{
    public async Task<Result<CouponResponseDto>> Handle(
        CreateCouponCommand request, CancellationToken ct)
    {
        var coupon = await _couponService.CreateCouponAsync(request.Dto);
        return Result<CouponResponseDto>.Ok(coupon);
    }
}

// Uso en Controller
[HttpPost]
public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto dto)
{
    var result = await _mediator.Send(new CreateCouponCommand(dto));
    return result.IsSuccess
        ? Ok(new { success = true, data = result.Data })
        : BadRequest(new { success = false, message = result.ErrorMessage });
}
```

### 7.4 Ejemplo de Query con Paginación
```csharp
// Definición
public record GetCouponsByCampaignQuery(int CampaignId, int Page, int PageSize) 
    : IRequest<Result<PaginatedResult<CouponResponseDto>>>;

// Handler
public class GetCouponsByCampaignQueryHandler 
    : IRequestHandler<GetCouponsByCampaignQuery, Result<PaginatedResult<CouponResponseDto>>>
{
    public async Task<Result<PaginatedResult<CouponResponseDto>>> Handle(
        GetCouponsByCampaignQuery request, CancellationToken ct)
    {
        var query = _couponService.GetCouponsByCampaign(request.CampaignId);
        var totalCount = await query.CountAsync(ct);
        var data = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return Result<PaginatedResult<CouponResponseDto>>.Ok(
            new PaginatedResult<CouponResponseDto>
            {
                Data = data,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
    }
}
```

---

## 8. HANGFIRE - PROCESAMIENTO EN SEGUNDO PLANO

### 8.1 Descripción
Hangfire es una librería para procesamiento de tareas en segundo plano en aplicaciones .NET. Permite ejecutar jobs de forma automática, recurrente o bajo demanda sin necesidad de servicios externos.

### 8.2 Configuración
Hangfire está configurado en el proyecto siguiendo la arquitectura hexagonal:

**Ubicación**: `FinalProject.API/Extensions/ServiceCollectionExtensions.cs`

```csharp
public static IServiceCollection AddHangfireServices(
    this IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    // Configurar Hangfire con MySQL Storage
    services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseStorage(new MySqlStorage(connectionString, options)));

    // Servidor Hangfire con 5 workers
    services.AddHangfireServer(options =>
    {
        options.WorkerCount = 5;
        options.ServerName = "FinalProject-HangfireServer";
    });

    // Registrar Jobs
    services.AddScoped<ExpireCouponsJob>();
    services.AddScoped<DataCleanupJob>();
    services.AddScoped<DailyStatisticsJob>();

    return services;
}
```

### 8.3 Jobs Implementados

**Ubicación**: `FinalProject.Infrastructure/Jobs/`

#### 1. ExpireCouponsJob
- **Propósito**: Expirar cupones vencidos automáticamente
- **Frecuencia**: Diariamente a las 00:00 UTC
- **Lógica**:
  - Busca cupones con `ExpirationDate < hoy` y `Active == true`
  - Marca los cupones como `Active = false`
  - Registra logs con cantidad de cupones expirados

#### 2. DataCleanupJob
- **Propósito**: Limpiar datos antiguos del sistema
- **Frecuencia**: Semanalmente los domingos a las 02:00 UTC
- **Lógica**:
  - Elimina cupones vencidos mayores a 6 meses
  - Mantiene la base de datos limpia
  - Registro de logs con total de registros eliminados

#### 3. DailyStatisticsJob
- **Propósito**: Generar estadísticas diarias del sistema
- **Frecuencia**: Diariamente a las 23:00 UTC
- **Métricas**:
  - Ventas del día
  - Transferencias del día
  - Total de Lukitas en el sistema
  - Campañas activas
  - Total de usuarios
- **Salida**: Logs estructurados para auditoría

### 8.4 Configuración de Jobs Recurrentes

**Ubicación**: `FinalProject.API/Extensions/HangfireJobsConfiguration.cs`

```csharp
public static void ConfigureRecurringJobs()
{
    RecurringJob.AddOrUpdate<ExpireCouponsJob>(
        "expire-coupons-daily",
        job => job.ExecuteAsync(),
        Cron.Daily(0, 0),
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

    RecurringJob.AddOrUpdate<DataCleanupJob>(
        "data-cleanup-weekly",
        job => job.ExecuteAsync(),
        Cron.Weekly(DayOfWeek.Sunday, 2, 0),
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

    RecurringJob.AddOrUpdate<DailyStatisticsJob>(
        "daily-statistics",
        job => job.ExecuteAsync(),
        Cron.Daily(23, 0),
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
}
```

### 8.5 Dashboard de Hangfire

**URL**: http://localhost:5140/hangfire (solo en desarrollo)

El dashboard permite:
- Ver jobs en ejecución
- Monitorear trabajos programados
- Ver historial de ejecuciones
- Reintentarjobs fallidos manualmente
- Ver servidores activos

**Seguridad**: En desarrollo el acceso es libre, en producción se debe implementar autenticación (filtro `HangfireAuthorizationFilter` en `FinalProject.API/Filters/`).

### 8.6 Configuración en Program.cs

**Ubicación**: `FinalProject.API/Program.cs`

```csharp
// Registro de servicios
builder.Services.AddHangfireServices(builder.Configuration);

// Configuración del middleware y dashboard
app.UseHangfireDashboard("/hangfire", options);

// Configuración de jobs recurrentes al iniciar
HangfireJobsConfiguration.ConfigureRecurringJobs();
```

### 8.7 Tablas de Base de Datos

Hangfire crea automáticamente las siguientes tablas en MySQL (con prefijo `Hangfire`):
- `Hangfire.Job`: Jobs almacenados
- `Hangfire.State`: Estados de jobs
- `Hangfire.Set`: Conjuntos de jobs
- `Hangfire.Counter`: Contadores
- `Hangfire.Hash`: Hashes
- `Hangfire.List`: Listas
- `Hangfire.AggregatedCounter`: Contadores agregados
- `Hangfire.Server`: Servidores activos
- `Hangfire.JobParameter`: Parámetros de jobs
- `Hangfire.JobQueue`: Cola de jobs

### 8.8 Archivos Modificados/Creados

#### Archivos Modificados:
1. `FinalProject.API/FinalProject.API.csproj` - Agregados paquetes NuGet
2. `FinalProject.API/Extensions/ServiceCollectionExtensions.cs` - Método `AddHangfireServices()`
3. `FinalProject.API/Program.cs` - Configuración de Hangfire y dashboard

#### Archivos Creados:
4. `FinalProject.API/Extensions/HangfireJobsConfiguration.cs` - Configuración de jobs recurrentes
5. `FinalProject.API/Filters/HangfireAuthorizationFilter.cs` - Filtro de autorización
6. `FinalProject.Infrastructure/Jobs/ExpireCouponsJob.cs` - Job de expiración de cupones
7. `FinalProject.Infrastructure/Jobs/DataCleanupJob.cs` - Job de limpieza de datos
8. `FinalProject.Infrastructure/Jobs/DailyStatisticsJob.cs` - Job de estadísticas diarias

---

## 9. PATRONES DE DISEÑO IMPLEMENTADOS

| Patrón | Ubicación | Propósito |
|--------|-----------|-----------|
| Repository | IGenericRepository + GenericRepository | Abstrae acceso a datos |
| Unit of Work | IUnitOfWork + UnitOfWork | Coordina transacciones |
| CQRS | Features/ + Handlers/ | Separa lecturas de escrituras |
| Mediator | MediatR | Desacopla requests de handlers |
| DTO | Application/DTOs/ | Transferencia de datos |
| Dependency Injection | ServiceCollectionExtensions | Inyección de dependencias |
| Clean Architecture | 4 proyectos | Separación de responsabilidades |
| Background Jobs | Hangfire | Tareas programadas y en segundo plano |

---

## 10. SEGURIDAD Y BUENAS PRÁCTICAS

### 9.1 Autenticación JWT
- Token con expiración de 8 horas
- Claims: UserId, Email, Role

### 9.2 Paginación Obligatoria
- Límite máximo: 100 registros por página
- Todos los endpoints de listado incluyen paginación

### 9.3 Manejo de Errores
- ErrorHandlingMiddleware captura excepciones
- Respuestas JSON estructuradas

### 9.4 Transacciones
- Operaciones complejas usan transacciones
- Rollback automático en caso de error

---

## 11. CÓMO EJECUTAR

### Requisitos
- .NET 9.0 SDK
- MySQL 9.0+

### Ejecución
```bash
cd FinalProject.API
dotnet run
```

### Acceso
- API: http://localhost:5140
- Swagger: http://localhost:5140/swagger
- **Hangfire Dashboard**: http://localhost:5140/hangfire

---

## 12. RESUMEN DE CUMPLIMIENTO

| Requisito | Estado |
|-----------|--------|
| Clean Architecture | ✅ Implementado |
| Repository Pattern | ✅ Implementado |
| Unit of Work | ✅ Implementado |
| Repositorio Genérico | ✅ Implementado |
| Inyección de Dependencias | ✅ Implementado |
| LINQ | ✅ Usado extensivamente |
| Paginación (sin ToList peligrosos) | ✅ Implementado |
| JWT Authentication | ✅ Implementado |
| DTOs | ✅ Implementado |
| Swagger | ✅ Implementado |
| **CQRS + MediatR** | ✅ **Implementado** |
| **Hangfire** | ✅ **Implementado** |
| ClosedXML (Excel) | ❌ No implementado |

---

## 13. ESTADÍSTICAS DEL PROYECTO

| Métrica | Cantidad |
|---------|----------|
| Entidades | 16 |
| Servicios | 10 |
| Controladores V1 | 10 |
| Controladores V2 (CQRS) | 9 |
| Commands | 15 |
| Queries | 18 |
| Handlers | 33 |
| DTOs | 30+ |
| Endpoints totales | 70+ |
| **Jobs de Hangfire** | **3** |
| **Middlewares** | **3** |
| **Filtros** | **1** |

---

*Documento actualizado el 2 de Diciembre de 2025*
*Último commit: feat(Hangfire): Implementación completa de Hangfire para background jobs*
