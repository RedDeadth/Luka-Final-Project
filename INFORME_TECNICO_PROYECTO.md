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
| JWT | 8.14.0 | Autenticación |
| Swashbuckle | 7.0.0 | Documentación Swagger |

---

## 2. ARQUITECTURA DEL PROYECTO

### 2.1 Clean Architecture (4 Capas)

```
┌─────────────────────────────────────────────────────────────┐
│                    FinalProject.API                         │
│  (Controladores, Middlewares, Configuración)                │
│  Puerto: http://localhost:5140                              │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                FinalProject.Application                     │
│  (DTOs, Interfaces de Servicios, Commands)                  │
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
│  (Implementación de Servicios, Repositorios, DbContext)     │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Flujo de Dependencias
```
API → Application → Domain ← Infrastructure
```
- **API** depende de Application e Infrastructure
- **Application** depende de Domain
- **Infrastructure** depende de Domain y Application
- **Domain** no depende de nadie (núcleo)

---

## 3. CAPA: FinalProject.Domain

### 3.1 Propósito
Contiene las **entidades del negocio** y las **interfaces de repositorios**. Es el núcleo de la aplicación, sin dependencias externas.

### 3.2 Entidades (16 total)

| Entidad | Tabla BD | Descripción | Relaciones |
|---------|----------|-------------|------------|
| `User` | `users` | Usuarios del sistema (estudiantes, empresas, admin) | → Role, → Accounts, → Campaigns, → UserMissions |
| `Role` | `roles` | Roles del sistema (student=1, coordinator=2) | → Users |
| `Account` | `accounts` | Cuentas de lukitas de usuarios | → User, → Campaign, → Sales, → Transfers |
| `Campaign` | `campaigns` | Campañas promocionales de empresas | → User, → Accounts, → Coupons |
| `Product` | `products` | Productos de proveedores | → Supplier, → ProductType, → SaleDetails |
| `ProductType` | `product_types` | Tipos de productos | → Products |
| `Supplier` | `suppliers` | Proveedores de productos | → SupplierType, → Products, → Coupons |
| `SupplierType` | `supplier_types` | Tipos de proveedores | → Suppliers |
| `Sale` | `sales` | Ventas/compras realizadas | → Account, → SaleDetails, → UserMissions |
| `SaleDetail` | `sale_details` | Detalle de productos en venta | → Sale, → Product |
| `Transfer` | `transfers` | Transferencias entre cuentas | → SourceAccount, → DestinationAccount |
| `Coupon` | `coupons` | Cupones de descuento | → Campaign, → Supplier |
| `MissionTemplate` | `mission_templates` | Catálogo de misiones | → UserMissions |
| `UserMission` | `user_missions` | Misiones asignadas a usuarios | → User, → MissionTemplate, → Sale, → Achievements |
| `Achievement` | `achievements` | Logros obtenidos | → UserMission |

### 3.3 Interfaces de Repositorios

#### IGenericRepository<T>
```csharp
// Ubicación: FinalProject.Domain/Interfaces/IGenericRepository.cs
// Propósito: Operaciones CRUD genéricas con paginación obligatoria

Task<T?> GetByIdAsync(int id);                    // Obtener por ID
Task<PaginatedResult<T>> GetAllAsync(int page, int pageSize);  // Listar con paginación
Task<PaginatedResult<T>> FindAsync(predicate, page, pageSize); // Buscar con filtro
Task<T?> FirstOrDefaultAsync(predicate);          // Primer elemento que cumple
IQueryable<T> Query();                            // Query personalizada
IQueryable<T> Query(predicate);                   // Query con filtro
Task AddAsync(T entity);                          // Crear
void Update(T entity);                            // Actualizar
void Delete(T entity);                            // Eliminar
Task<bool> ExistsAsync(predicate);                // Verificar existencia
Task<int> CountAsync();                           // Contar todos
Task<int> CountAsync(predicate);                  // Contar con filtro
```

#### IUnitOfWork
```csharp
// Ubicación: FinalProject.Domain/Interfaces/IUnitOfWork.cs
// Propósito: Patrón Unit of Work para transacciones

IGenericRepository<User> Users { get; }
IGenericRepository<Role> Roles { get; }
// ... repositorios para todas las entidades

Task<int> SaveChangesAsync();           // Guardar cambios
Task BeginTransactionAsync();           // Iniciar transacción
Task CommitTransactionAsync();          // Confirmar transacción
Task RollbackTransactionAsync();        // Revertir transacción
```

### 3.4 PaginatedResult<T>
```csharp
// Propósito: Evitar cargar tablas completas en memoria
public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; }     // Datos de la página
    public int TotalCount { get; set; }           // Total de registros
    public int Page { get; set; }                 // Página actual
    public int PageSize { get; set; }             // Tamaño de página
    public int TotalPages { get; }                // Total de páginas
    public bool HasPreviousPage { get; }          // ¿Hay página anterior?
    public bool HasNextPage { get; }              // ¿Hay página siguiente?
}
```

---

## 4. CAPA: FinalProject.Application

### 4.1 Propósito
Define los **contratos de servicios** (interfaces) y los **DTOs** para transferencia de datos. No contiene lógica de negocio, solo definiciones.

### 4.2 Interfaces de Servicios (10 total)

| Interfaz | Ubicación | Propósito |
|----------|-----------|-----------|
| `IAuthService` | Interfaces/IAuthService.cs | Autenticación y JWT |
| `IAdminService` | Interfaces/IAdminService.cs | Funciones administrativas |
| `ICampaignService` | Interfaces/ICampaignService.cs | Gestión de campañas |
| `IStudentService` | Interfaces/IStudentService.cs | Funciones de estudiantes |
| `IProductService` | Interfaces/IProductService.cs | Gestión de productos |
| `ISupplierService` | Interfaces/ISupplierService.cs | Balance y conversión de proveedores |
| `ISupplierManagementService` | Interfaces/ISupplierManagementService.cs | CRUD de proveedores |
| `IMissionService` | Interfaces/IMissionService.cs | Gestión de misiones |
| `ICouponService` | Interfaces/ICouponService.cs | Gestión de cupones |
| `ITransferService` | Interfaces/ITransferService.cs | Transferencias entre cuentas |

### 4.3 DTOs (Data Transfer Objects)

#### ¿Qué son los DTOs?
Son clases simples que transportan datos entre capas. **Nunca exponen entidades del dominio directamente** a la API.

#### ¿Por qué usar DTOs?
1. **Seguridad**: No exponer campos sensibles (ej: password)
2. **Desacoplamiento**: Cambios en entidades no afectan la API
3. **Optimización**: Solo enviar datos necesarios
4. **Validación**: Estructura clara para requests/responses


#### DTOs por Módulo

##### AuthDtos (Autenticación)
| DTO | Tipo | Campos | Uso |
|-----|------|--------|-----|
| `LoginRequestDto` | Request | Email, Password | Enviar credenciales de login |
| `LoginResponseDto` | Response | UserId, Email, FirstName, LastName, Role, Token, Company, University | Respuesta con token JWT |

##### CampaignDtos (Campañas)
| DTO | Tipo | Campos | Uso |
|-----|------|--------|-----|
| `CreateCampaignDto` | Request | Name, Description, CampaignType, Budget, StartDate, EndDate, Schedule, Location, ContactNumber, ImageUrls | Crear nueva campaña |
| `CampaignResponseDto` | Response | Id, Name, Description, CampaignType, Budget, RemainingBudget, StartDate, EndDate, Schedule, Location, ContactNumber, ImageUrls, Active, EnrolledStudents, CompanyName | Datos de campaña |
| `EnrollCampaignDto` | Request | CampaignId, StudentId | Inscribir estudiante a campaña |

##### CompanyDtos (Empresas)
| DTO | Tipo | Campos | Uso |
|-----|------|--------|-----|
| `CompanyProfileDto` | Response | Id, CompanyName, Email, ContactPerson, Phone, Approved, LukasBalance, ActiveCampaigns | Perfil de empresa |
| `CompanyApprovalDto` | Request | CompanyId, Approved, Reason | Aprobar/rechazar empresa |

##### LukasDtos (Moneda Virtual)
| DTO | Tipo | Campos | Uso |
|-----|------|--------|-----|
| `EmitLukasDto` | Request | CompanyId, Amount, Reason | Emitir lukitas a empresa |
| `LukasValueDto` | Response | LukasToUsdRate, UsdToLukasRate, LastUpdated | Valor actual de lukitas |
| `UpdateLukasValueDto` | Request | LukasToUsdRate | Actualizar valor de lukitas |

##### ProductDtos (Productos)
| DTO | Tipo | Campos | Uso |
|-----|------|--------|-----|
| `CreateProductDto` | Request | SupplierId, ProductTypeId, Code, Name, Price, Stock | Crear producto |
| `ProductResponseDto` | Response | Id, SupplierId, SupplierName, ProductTypeId, ProductTypeName, Code, Name, Price, Stock, Status | Datos de producto |
| `ProductPurchaseDto` | Request | StudentId, SupplierId, Items[] | Comprar productos |
| `ProductItemDto` | Request | ProductId, Quantity | Item de compra |
| `UpdateProductDto` | Request | Name, Price, Stock, Status | Actualizar producto |

##### MissionDtos (Misiones)
| DTO | Tipo | Campos | Uso |
|-----|------|--------|-----|
| `AssignMissionDto` | Request | UserId, MissionId | Asignar misión a usuario |
| `CompleteMissionDto` | Request | UserMissionId, SaleId | Completar misión |
| `UserMissionResponseDto` | Response | Id, UserId, UserName, MissionId, MissionName, MissionDescription, RewardPoints, Completed, AssignmentDate, CompletionDate | Datos de misión de usuario |

##### CouponDtos (Cupones)
| DTO | Tipo | Campos | Uso |
|-----|------|--------|-----|
| `CreateCouponDto` | Request | CampaignId, SupplierId, Code, DiscountType, DiscountValue, ExpirationDate | Crear cupón |
| `CouponResponseDto` | Response | Id, CampaignId, CampaignName, SupplierId, SupplierName, Code, DiscountType, DiscountValue, ExpirationDate, Active | Datos de cupón |

##### TransferDtos (Transferencias)
| DTO | Tipo | Campos | Uso |
|-----|------|--------|-----|
| `CreateTransferDto` | Request | SourceAccountId, DestinationAccountId, Amount | Crear transferencia |
| `TransferResponseDto` | Response | Id, SourceAccountId, SourceAccountNumber, DestinationAccountId, DestinationAccountNumber, TransferDate, Amount, Status | Datos de transferencia |

##### SupplierDtos (Proveedores)
| DTO | Tipo | Campos | Uso |
|-----|------|--------|-----|
| `CreateSupplierDto` | Request | SupplierTypeId, Name, Email, Phone | Crear proveedor |
| `SupplierResponseDto` | Response | Id, SupplierTypeId, SupplierTypeName, Name, Email, Phone, Status, TotalProducts | Datos de proveedor |

##### StatisticsDtos (Estadísticas)
| DTO | Tipo | Campos | Uso |
|-----|------|--------|-----|
| `SystemStatisticsDto` | Response | TotalUsers, TotalStudents, TotalCompanies, TotalSuppliers, ActiveCampaigns, TotalLukasInCirculation, TotalLukasSpent, TotalTransactions, RecentActivity[] | Estadísticas del sistema |
| `ActivityLogDto` | Response | Timestamp, ActivityType, Description, UserEmail | Log de actividad |

---

## 5. CAPA: FinalProject.Infrastructure

### 5.1 Propósito
Implementa los **servicios de negocio**, **repositorios** y el **acceso a datos**. Contiene toda la lógica de persistencia.

### 5.2 DbContext (LukitasDbContext)
```csharp
// Ubicación: FinalProject.Infrastructure/Data/LukitasDbContext.cs
// Propósito: Configuración de Entity Framework Core

public class LukitasDbContext : DbContext
{
    // DbSets para todas las entidades
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    // ... 15 DbSets más

    // Configuración de relaciones y mapeo en OnModelCreating()
}
```

### 5.3 Repositorios

#### GenericRepository<T>
```csharp
// Ubicación: FinalProject.Infrastructure/Repositories/GenericRepository.cs
// Propósito: Implementación del patrón Repository con paginación

// Características:
// - MaxPageSize = 100 (límite de seguridad)
// - Paginación obligatoria en GetAllAsync y FindAsync
// - Métodos Query() para consultas personalizadas
```

#### UnitOfWork
```csharp
// Ubicación: FinalProject.Infrastructure/Repositories/UnitOfWork.cs
// Propósito: Coordinar transacciones entre múltiples repositorios

// Características:
// - Instancia un GenericRepository para cada entidad
// - Manejo de transacciones (Begin, Commit, Rollback)
// - Implementa IDisposable para liberar recursos
```

### 5.4 Servicios de Negocio (10 total)

| Servicio | Ubicación | Implementa | Funcionalidades |
|----------|-----------|------------|-----------------|
| `AuthService` | Services/AuthService.cs | IAuthService | Login, validación JWT, generación de tokens |
| `AdminService` | Services/AdminService.cs | IAdminService | Aprobar empresas, estadísticas, emitir lukitas, gestionar valor |
| `CampaignService` | Services/CampaignService.cs | ICampaignService | CRUD campañas, inscripción de estudiantes |
| `StudentService` | Services/StudentService.cs | IStudentService | Ver campañas disponibles, balance, comprar productos |
| `ProductService` | Services/ProductService.cs | IProductService | CRUD productos, gestión de stock |
| `SupplierService` | Services/SupplierService.cs | ISupplierService | Balance de proveedor, conversión a dinero real |
| `SupplierManagementService` | Services/SupplierManagementService.cs | ISupplierManagementService | CRUD proveedores |
| `MissionService` | Services/MissionService.cs | IMissionService | Asignar/completar misiones, listar misiones |
| `CouponService` | Services/CouponService.cs | ICouponService | CRUD cupones, validación |
| `TransferService` | Services/TransferService.cs | ITransferService | Crear transferencias, historial |

---

## 6. CAPA: FinalProject.API

### 6.1 Propósito
Expone los **endpoints REST**, configura **middlewares** y maneja la **inyección de dependencias**.

### 6.2 Program.cs (Punto de Entrada)
```csharp
// Configuración del servidor
builder.WebHost.UseUrls("http://localhost:5140");

// Servicios registrados
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();      // Servicios de negocio
builder.Services.AddInfrastructureServices();   // DbContext, repositorios

// Pipeline de middlewares
app.UseMiddleware<ErrorHandlingMiddleware>();   // Manejo global de errores
app.UseMiddleware<LoggingMiddleware>();         // Logging de requests
app.UseMiddleware<JwtAuthMiddleware>();         // Autenticación JWT
app.MapControllers();                           // Mapeo de controladores
```

### 6.3 Middlewares

#### ErrorHandlingMiddleware
```csharp
// Ubicación: Middlewares/ErrorHandlingMiddleware.cs
// Propósito: Capturar excepciones no manejadas y retornar JSON estructurado

// Respuesta de error:
{
    "status": 500,
    "message": "An unexpected error occurred.",
    "details": "Mensaje de la excepción"
}
```

#### JwtAuthMiddleware
```csharp
// Ubicación: Middlewares/JwtAuthMiddleware.cs
// Propósito: Extraer claims del token JWT y adjuntarlos al contexto

// Extrae:
// - UserId (ClaimTypes.NameIdentifier)
// - Role (ClaimTypes.Role)
// Los adjunta a context.Items["User"]
```

#### LoggingMiddleware
```csharp
// Ubicación: Middlewares/LoggingMiddleware.cs
// Propósito: Registrar requests y responses en logs

// Registra:
// - Request: {method} {url}
// - Response: {statusCode}
```

### 6.4 Inyección de Dependencias
```csharp
// Ubicación: Extensions/ServiceCollectionExtensions.cs

// AddApplicationServices() - Servicios de negocio
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<ICampaignService, CampaignService>();
services.AddScoped<IStudentService, StudentService>();
// ... 7 servicios más

// AddInfrastructureServices() - Infraestructura
services.AddDbContext<LukitasDbContext>(options => 
    options.UseMySql(connectionString, serverVersion));
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
```


### 6.5 Controladores y Endpoints

#### AuthController
```
POST /api/auth/login          → Login con email/password, retorna JWT
POST /api/auth/validate       → Validar token JWT
```

#### AdminController
```
GET  /api/admin/companies/pending     → Listar empresas pendientes de aprobación (paginado)
POST /api/admin/companies/approve     → Aprobar/rechazar empresa
GET  /api/admin/statistics            → Obtener estadísticas del sistema
POST /api/admin/lukas/emit            → Emitir lukitas a empresa
GET  /api/admin/lukas/value           → Obtener valor actual de lukitas
PUT  /api/admin/lukas/value           → Actualizar valor de lukitas
```

#### CampaignController
```
POST /api/campaign/create                    → Crear campaña
GET  /api/campaign/active                    → Listar campañas activas (paginado)
GET  /api/campaign/{id}                      → Obtener campaña por ID
POST /api/campaign/enroll                    → Inscribir estudiante a campaña
GET  /api/campaign/company/{companyUserId}   → Campañas de una empresa (paginado)
```

#### StudentController
```
GET  /api/student/{studentId}/campaigns   → Campañas disponibles para inscripción (paginado)
GET  /api/student/{studentId}/balance     → Balance de lukitas del estudiante
POST /api/student/purchase                → Comprar productos con lukitas
```

#### ProductController
```
POST   /api/product                       → Crear producto
GET    /api/product                       → Listar todos los productos (paginado)
GET    /api/product/{id}                  → Obtener producto por ID
GET    /api/product/supplier/{supplierId} → Productos de un proveedor (paginado)
PUT    /api/product/{id}                  → Actualizar producto
DELETE /api/product/{id}                  → Eliminar producto (soft delete)
PATCH  /api/product/{id}/stock            → Actualizar stock
```

#### SupplierController
```
GET  /api/supplier/{supplierId}/balance   → Balance de lukitas del proveedor
POST /api/supplier/{supplierId}/convert   → Convertir lukitas a dinero real
```

#### SupplierManagementController
```
POST   /api/suppliermanagement            → Crear proveedor
GET    /api/suppliermanagement            → Listar proveedores (paginado)
GET    /api/suppliermanagement/{id}       → Obtener proveedor por ID
PUT    /api/suppliermanagement/{id}       → Actualizar proveedor
DELETE /api/suppliermanagement/{id}       → Eliminar proveedor (soft delete)
```

#### MissionController
```
POST /api/mission/assign                  → Asignar misión a usuario
POST /api/mission/complete                → Completar misión
GET  /api/mission/user/{userId}           → Todas las misiones del usuario (paginado)
GET  /api/mission/user/{userId}/pending   → Misiones pendientes (paginado)
GET  /api/mission/user/{userId}/completed → Misiones completadas (paginado)
```

#### CouponController
```
POST   /api/coupon                        → Crear cupón
GET    /api/coupon/code/{code}            → Obtener cupón por código
GET    /api/coupon/campaign/{campaignId}  → Cupones de una campaña (paginado)
GET    /api/coupon/supplier/{supplierId}  → Cupones de un proveedor (paginado)
GET    /api/coupon/validate/{code}        → Validar cupón
DELETE /api/coupon/{id}                   → Desactivar cupón
```

#### TransferController
```
POST /api/transfer                        → Crear transferencia
GET  /api/transfer/account/{accountId}    → Transferencias de una cuenta (paginado)
GET  /api/transfer/{id}                   → Obtener transferencia por ID
```

---

## 7. PATRONES DE DISEÑO IMPLEMENTADOS

### 7.1 Repository Pattern
```
¿Qué es?    Abstrae el acceso a datos detrás de una interfaz
¿Dónde?     IGenericRepository<T> + GenericRepository<T>
¿Por qué?   Desacoplar la lógica de negocio del acceso a datos
```

### 7.2 Unit of Work Pattern
```
¿Qué es?    Coordina múltiples operaciones en una sola transacción
¿Dónde?     IUnitOfWork + UnitOfWork
¿Por qué?   Garantizar consistencia en operaciones complejas
```

### 7.3 Dependency Injection
```
¿Qué es?    Inyectar dependencias en lugar de crearlas
¿Dónde?     ServiceCollectionExtensions.cs
¿Por qué?   Facilitar testing, desacoplamiento, mantenibilidad
```

### 7.4 DTO Pattern
```
¿Qué es?    Objetos para transferir datos entre capas
¿Dónde?     FinalProject.Application/DTOs/
¿Por qué?   No exponer entidades, controlar datos enviados/recibidos
```

### 7.5 Clean Architecture
```
¿Qué es?    Separación en capas con dependencias hacia el centro
¿Dónde?     4 proyectos: API, Application, Domain, Infrastructure
¿Por qué?   Mantenibilidad, testabilidad, independencia de frameworks
```

---

## 8. SEGURIDAD Y BUENAS PRÁCTICAS

### 8.1 Autenticación JWT
```csharp
// Configuración en appsettings.json
{
    "Jwt": {
        "Key": "YourSuperSecretKeyForJWTTokenGeneration123456",
        "Issuer": "FinalProjectAPI",
        "Audience": "FinalProjectClient"
    }
}

// Token incluye:
// - UserId (ClaimTypes.NameIdentifier)
// - Email (ClaimTypes.Email)
// - Role (ClaimTypes.Role)
// - Expiración: 8 horas
```

### 8.2 Paginación Obligatoria
```csharp
// Todos los endpoints que retornan listas tienen paginación
// Parámetros: ?page=1&pageSize=20
// Límite máximo: 100 registros por página

// Respuesta incluye metadatos:
{
    "success": true,
    "data": [...],
    "pagination": {
        "page": 1,
        "pageSize": 20,
        "totalCount": 150,
        "totalPages": 8
    }
}
```

### 8.3 Manejo de Errores
```csharp
// ErrorHandlingMiddleware captura todas las excepciones
// Retorna JSON estructurado con status 500
// Registra errores en logs
```

### 8.4 Transacciones
```csharp
// Operaciones complejas usan transacciones
// Ejemplo: StudentService.PurchaseProductsAsync()
using var transaction = await _context.Database.BeginTransactionAsync();
try {
    // Operaciones...
    await transaction.CommitAsync();
} catch {
    await transaction.RollbackAsync();
    throw;
}
```

---

## 9. BASE DE DATOS

### 9.1 Configuración
```json
// appsettings.json
{
    "ConnectionStrings": {
        "DefaultConnection": "server=127.0.0.1;port=3306;database=lukitas_db;user=root;password=123456789"
    }
}
```

### 9.2 Diagrama de Relaciones (Simplificado)
```
users ─────┬──────────────────────────────────────────────────┐
           │                                                   │
           ▼                                                   ▼
       accounts ◄──────────────────────────────────────── campaigns
           │                                                   │
           ▼                                                   ▼
        sales ◄─────────────────────────────────────────── coupons
           │                                                   │
           ▼                                                   │
     sale_details ◄─── products ◄─── suppliers ◄──────────────┘
```

### 9.3 Índices Importantes
- `users.email` (UNIQUE)
- `users.student_code` (UNIQUE)
- `accounts.account_number` (UNIQUE)
- `products.code` (UNIQUE)
- `coupons.code` (UNIQUE)
- `campaigns.start_date, end_date` (INDEX)
- `sales.sale_date` (INDEX)

---

## 10. FLUJOS DE NEGOCIO PRINCIPALES

### 10.1 Flujo de Compra de Productos
```
1. Estudiante llama POST /api/student/purchase
2. StudentService.PurchaseProductsAsync():
   a. Inicia transacción
   b. Verifica balance del estudiante
   c. Verifica stock de productos
   d. Calcula total
   e. Crea registro en Sales
   f. Crea registros en SaleDetails
   g. Descuenta balance del estudiante
   h. Aumenta balance del proveedor
   i. Actualiza stock de productos
   j. Commit transacción
3. Retorna éxito o error
```

### 10.2 Flujo de Inscripción a Campaña
```
1. Estudiante llama POST /api/campaign/enroll
2. CampaignService.EnrollStudentAsync():
   a. Verifica que estudiante y campaña existan
   b. Verifica que no esté ya inscrito
   c. Crea nueva Account vinculada a la campaña
   d. Guarda cambios
3. Estudiante puede recibir lukitas en esa cuenta
```

### 10.3 Flujo de Emisión de Lukitas
```
1. Admin llama POST /api/admin/lukas/emit
2. AdminService.EmitLukasAsync():
   a. Verifica que la empresa exista
   b. Busca o crea cuenta de la empresa
   c. Aumenta balance de la cuenta
   d. Guarda cambios
3. Empresa puede usar lukitas en campañas
```

---

## 11. ESTRUCTURA DE ARCHIVOS

```
FinalProject/
├── FinalProject.sln
├── FinalProject.API/
│   ├── Controllers/
│   │   ├── AdminController.cs
│   │   ├── AuthController.cs
│   │   ├── CampaignController.cs
│   │   ├── CouponController.cs
│   │   ├── MissionController.cs
│   │   ├── ProductController.cs
│   │   ├── StudentController.cs
│   │   ├── SupplierController.cs
│   │   ├── SupplierManagementController.cs
│   │   └── TransferController.cs
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs
│   ├── Middlewares/
│   │   ├── ErrorHandlingMiddleware.cs
│   │   ├── JwtAuthMiddleware.cs
│   │   └── LoggingMiddleware.cs
│   ├── Program.cs
│   └── appsettings.json
├── FinalProject.Application/
│   ├── DTOs/
│   │   ├── AuthDtos/
│   │   ├── CampaignDtos/
│   │   ├── CompanyDtos/
│   │   ├── CouponDtos/
│   │   ├── LukasDtos/
│   │   ├── MissionDtos/
│   │   ├── ProductDtos/
│   │   ├── StatisticsDtos/
│   │   ├── SupplierDtos/
│   │   ├── TransferDtos/
│   │   └── UserDtos/
│   └── Interfaces/
│       ├── IAdminService.cs
│       ├── IAuthService.cs
│       ├── ICampaignService.cs
│       ├── ICouponService.cs
│       ├── IMissionService.cs
│       ├── IProductService.cs
│       ├── IStudentService.cs
│       ├── ISupplierManagementService.cs
│       ├── ISupplierService.cs
│       └── ITransferService.cs
├── FinalProject.Domain/
│   ├── Entities/
│   │   ├── Account.cs
│   │   ├── Achievement.cs
│   │   ├── Campaign.cs
│   │   ├── Coupon.cs
│   │   ├── MissionTemplate.cs
│   │   ├── Product.cs
│   │   ├── ProductType.cs
│   │   ├── Role.cs
│   │   ├── Sale.cs
│   │   ├── SaleDetail.cs
│   │   ├── Supplier.cs
│   │   ├── SupplierType.cs
│   │   ├── Transfer.cs
│   │   ├── User.cs
│   │   └── UserMission.cs
│   └── Interfaces/
│       ├── IGenericRepository.cs
│       └── IUnitOfWork.cs
└── FinalProject.Infrastructure/
    ├── Data/
    │   └── LukitasDbContext.cs
    ├── Repositories/
    │   ├── GenericRepository.cs
    │   └── UnitOfWork.cs
    └── Services/
        ├── AdminService.cs
        ├── AuthService.cs
        ├── CampaignService.cs
        ├── CouponService.cs
        ├── MissionService.cs
        ├── ProductService.cs
        ├── StudentService.cs
        ├── SupplierManagementService.cs
        ├── SupplierService.cs
        └── TransferService.cs
```

---

## 12. CÓMO EJECUTAR EL PROYECTO

### 12.1 Requisitos
- .NET 9.0 SDK
- MySQL 9.0+
- IDE (Visual Studio 2022 / Rider)

### 12.2 Configuración
1. Crear base de datos `lukitas_db` en MySQL
2. Ejecutar scripts SQL de setup
3. Configurar connection string en `appsettings.json`

### 12.3 Ejecución
```bash
cd FinalProject.API
dotnet run
```

### 12.4 Acceso
- API: http://localhost:5140
- Swagger: http://localhost:5140/swagger

---

## 13. RESUMEN DE CUMPLIMIENTO

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
| CQRS + MediatR | ❌ No implementado |
| Hangfire | ❌ No implementado |
| ClosedXML (Excel) | ❌ No implementado |

---

*Documento generado el 2 de Diciembre de 2025*
