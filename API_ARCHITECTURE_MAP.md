# 🗺️ Mapa de Arquitectura de APIs - Sistema Lukas

## 📊 Resumen General

**Total de APIs**: 46+ endpoints  
**Controladores**: 10  
**Servicios**: 10  
**DTOs**: 22+  
**Arquitectura**: Clean Architecture  

---

## 🏗️ Estructura de Capas (Clean Architecture Pragmática)

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer                             │
│  Controllers → Exponen endpoints HTTP                    │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│              Application Layer                           │
│  ✓ Interfaces → Contratos de servicios                  │
│  ✓ DTOs → Transferencia de datos                        │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│              Infrastructure Layer                        │
│  ✓ Services → Implementaciones de lógica de negocio     │
│  ✓ Repositories → Acceso a datos (opcional)             │
│  ✓ DbContext → Entity Framework Core                    │
└────────────────────┬────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────────────┐
│                Domain Layer                              │
│  ✓ Entities → Modelos de dominio puros                  │
│  ✓ Interfaces → Contratos de repositorios               │
└─────────────────────────────────────────────────────────┘
```

**Implementación Pragmática**: 
- Los **Services** están en Infrastructure y usan DbContext directamente
- No se usan Use Cases puros para evitar sobre-ingeniería
- Entity Framework Core ya proporciona abstracción de datos suficiente
- Esta arquitectura es más simple y adecuada para proyectos medianos

---

## 📋 APIs por Controlador

### 1. AuthController
**Ruta base**: `/api/auth`  
**Servicio**: `IAuthService` → `AuthService`  
**Ubicación**: `FinalProject.API/Controllers/AuthController.cs`

| Método | Endpoint | DTOs Entrada | DTOs Salida | Descripción |
|--------|----------|--------------|-------------|-------------|
| POST | `/login` | `LoginRequestDto` | `LoginResponseDto` | Autenticación de usuario |
| POST | `/register` | `RegisterDto` | - | Registro de nuevo usuario |

**DTOs Ubicación**: `FinalProject.Application/DTOs/AuthDtos/`
- `LoginRequestDto.cs`
- `LoginResponseDto.cs`

---

### 2. CampaignController
**Ruta base**: `/api/campaign`  
**Servicio**: `ICampaignService` → `CampaignService`  
**Ubicación**: `FinalProject.API/Controllers/CampaignController.cs`

| Método | Endpoint | DTOs Entrada | DTOs Salida | Descripción |
|--------|----------|--------------|-------------|-------------|
| POST | `/create?companyUserId={id}` | `CreateCampaignDto` | `CampaignResponseDto` | Crear nueva campaña |
| GET | `/active` | - | `List<CampaignResponseDto>` | Listar campañas activas |
| GET | `/{id}` | - | `CampaignResponseDto` | Obtener campaña por ID |
| POST | `/enroll` | `EnrollCampaignDto` | - | Inscribir estudiante |
| GET | `/company/{companyUserId}` | - | `List<CampaignResponseDto>` | Campañas de empresa |

**DTOs Ubicación**: `FinalProject.Application/DTOs/CampaignDtos/`
- `CreateCampaignDto.cs` - Nombre, tipo, presupuesto, horarios, ubicación, imágenes
- `CampaignResponseDto.cs` - Respuesta completa con datos de campaña
- `EnrollCampaignDto.cs` - CampaignId, StudentId

**Servicio Ubicación**: `FinalProject.Infrastructure/Services/CampaignService.cs`

---

### 3. StudentController
**Ruta base**: `/api/student`  
**Servicio**: `IStudentService` → `StudentService`  
**Ubicación**: `FinalProject.API/Controllers/StudentController.cs`

| Método | Endpoint | DTOs Entrada | DTOs Salida | Descripción |
|--------|----------|--------------|-------------|-------------|
| GET | `/{studentId}/campaigns` | - | `List<CampaignResponseDto>` | Campañas disponibles |
| GET | `/{studentId}/balance` | - | `decimal` | Balance de Lukas |
| POST | `/purchase` | `ProductPurchaseDto` | - | Comprar productos |

**DTOs Ubicación**: `FinalProject.Application/DTOs/ProductDtos/`
- `ProductPurchaseDto.cs` - StudentId, SupplierId, Items[]

**Servicio Ubicación**: `FinalProject.Infrastructure/Services/StudentService.cs`

---

### 4. AdminController
**Ruta base**: `/api/admin`  
**Servicio**: `IAdminService` → `AdminService`  
**Ubicación**: `FinalProject.API/Controllers/AdminController.cs`

| Método | Endpoint | DTOs Entrada | DTOs Salida | Descripción |
|--------|----------|--------------|-------------|-------------|
| GET | `/companies/pending` | - | `List<CompanyProfileDto>` | Empresas pendientes |
| POST | `/companies/approve` | `CompanyApprovalDto` | - | Aprobar/rechazar empresa |
| GET | `/statistics` | - | `SystemStatisticsDto` | Estadísticas del sistema |
| POST | `/lukas/emit` | `EmitLukasDto` | - | Emitir Lukas a empresa |
| GET | `/lukas/value` | - | `LukasValueDto` | Valor actual de Lukas |
| PUT | `/lukas/value` | `UpdateLukasValueDto` | - | Actualizar valor Lukas |

**DTOs Ubicación**: 
- `FinalProject.Application/DTOs/CompanyDtos/`
  - `CompanyApprovalDto.cs` - CompanyId, Approved, Reason
  - `CompanyProfileDto.cs` - Perfil completo de empresa
- `FinalProject.Application/DTOs/LukasDtos/`
  - `EmitLukasDto.cs` - CompanyId, Amount, Reason
  - `LukasValueDto.cs` - Tasas de conversión
  - `UpdateLukasValueDto.cs` - Nueva tasa
- `FinalProject.Application/DTOs/StatisticsDtos/`
  - `SystemStatisticsDto.cs` - Estadísticas completas

**Servicio Ubicación**: `FinalProject.Infrastructure/Services/AdminService.cs`

---

### 5. SupplierController
**Ruta base**: `/api/supplier`  
**Servicio**: `ISupplierService` → `SupplierService`  
**Ubicación**: `FinalProject.API/Controllers/SupplierController.cs`

| Método | Endpoint | DTOs Entrada | DTOs Salida | Descripción |
|--------|----------|--------------|-------------|-------------|
| GET | `/{supplierId}/balance` | - | `decimal` | Balance de Lukas |
| POST | `/{supplierId}/convert` | `decimal` | - | Convertir Lukas a dinero |

**Servicio Ubicación**: `FinalProject.Infrastructure/Services/SupplierService.cs`

---

### 6. ProductController
**Ruta base**: `/api/product`  
**Servicio**: `IProductService` → `ProductService`  
**Ubicación**: `FinalProject.API/Controllers/ProductController.cs`

| Método | Endpoint | DTOs Entrada | DTOs Salida | Descripción |
|--------|----------|--------------|-------------|-------------|
| POST | `/` | `CreateProductDto` | `ProductResponseDto` | Crear producto |
| GET | `/` | - | `List<ProductResponseDto>` | Listar todos |
| GET | `/{id}` | - | `ProductResponseDto` | Obtener por ID |
| GET | `/supplier/{supplierId}` | - | `List<ProductResponseDto>` | Productos por proveedor |
| PUT | `/{id}` | `UpdateProductDto` | - | Actualizar producto |
| DELETE | `/{id}` | - | - | Eliminar producto |
| PATCH | `/{id}/stock` | `int` | - | Actualizar stock |

**DTOs Ubicación**: `FinalProject.Application/DTOs/ProductDtos/`
- `CreateProductDto.cs` - SupplierId, ProductTypeId, Code, Name, Price, Stock
- `ProductResponseDto.cs` - Respuesta completa con nombres de proveedor y tipo
- `UpdateProductDto.cs` - Name, Price, Stock, Status

**Servicio Ubicación**: `FinalProject.Infrastructure/Services/ProductService.cs`

---

### 7. SupplierManagementController
**Ruta base**: `/api/suppliermanagement`  
**Servicio**: `ISupplierManagementService` → `SupplierManagementService`  
**Ubicación**: `FinalProject.API/Controllers/SupplierManagementController.cs`

| Método | Endpoint | DTOs Entrada | DTOs Salida | Descripción |
|--------|----------|--------------|-------------|-------------|
| POST | `/` | `CreateSupplierDto` | `SupplierResponseDto` | Crear proveedor |
| GET | `/` | - | `List<SupplierResponseDto>` | Listar todos |
| GET | `/{id}` | - | `SupplierResponseDto` | Obtener por ID |
| PUT | `/{id}` | `CreateSupplierDto` | - | Actualizar proveedor |
| DELETE | `/{id}` | - | - | Eliminar proveedor |

**DTOs Ubicación**: `FinalProject.Application/DTOs/SupplierDtos/`
- `CreateSupplierDto.cs` - SupplierTypeId, Name, Email, Phone
- `SupplierResponseDto.cs` - Respuesta completa con tipo y total de productos

**Servicio Ubicación**: `FinalProject.Infrastructure/Services/SupplierManagementService.cs`

---

### 8. MissionController
**Ruta base**: `/api/mission`  
**Servicio**: `IMissionService` → `MissionService`  
**Ubicación**: `FinalProject.API/Controllers/MissionController.cs`

| Método | Endpoint | DTOs Entrada | DTOs Salida | Descripción |
|--------|----------|--------------|-------------|-------------|
| POST | `/assign` | `AssignMissionDto` | - | Asignar misión |
| POST | `/complete` | `CompleteMissionDto` | - | Completar misión |
| GET | `/user/{userId}` | - | `List<UserMissionResponseDto>` | Todas las misiones |
| GET | `/user/{userId}/pending` | - | `List<UserMissionResponseDto>` | Misiones pendientes |
| GET | `/user/{userId}/completed` | - | `List<UserMissionResponseDto>` | Misiones completadas |

**DTOs Ubicación**: `FinalProject.Application/DTOs/MissionDtos/`
- `AssignMissionDto.cs` - UserId, MissionId
- `CompleteMissionDto.cs` - UserMissionId, SaleId
- `UserMissionResponseDto.cs` - Respuesta completa con datos de misión y recompensa

**Servicio Ubicación**: `FinalProject.Infrastructure/Services/MissionService.cs`

---

### 9. CouponController
**Ruta base**: `/api/coupon`  
**Servicio**: `ICouponService` → `CouponService`  
**Ubicación**: `FinalProject.API/Controllers/CouponController.cs`

| Método | Endpoint | DTOs Entrada | DTOs Salida | Descripción |
|--------|----------|--------------|-------------|-------------|
| POST | `/` | `CreateCouponDto` | `CouponResponseDto` | Crear cupón |
| GET | `/code/{code}` | - | `CouponResponseDto` | Obtener por código |
| GET | `/campaign/{campaignId}` | - | `List<CouponResponseDto>` | Cupones por campaña |
| GET | `/supplier/{supplierId}` | - | `List<CouponResponseDto>` | Cupones por proveedor |
| GET | `/validate/{code}` | - | `bool` | Validar cupón |
| DELETE | `/{id}` | - | - | Desactivar cupón |

**DTOs Ubicación**: `FinalProject.Application/DTOs/CouponDtos/`
- `CreateCouponDto.cs` - CampaignId, SupplierId, Code, DiscountType, DiscountValue, ExpirationDate
- `CouponResponseDto.cs` - Respuesta completa con nombres de campaña y proveedor

**Servicio Ubicación**: `FinalProject.Infrastructure/Services/CouponService.cs`

---

### 10. TransferController
**Ruta base**: `/api/transfer`  
**Servicio**: `ITransferService` → `TransferService`  
**Ubicación**: `FinalProject.API/Controllers/TransferController.cs`

| Método | Endpoint | DTOs Entrada | DTOs Salida | Descripción |
|--------|----------|--------------|-------------|-------------|
| POST | `/` | `CreateTransferDto` | `TransferResponseDto` | Crear transferencia |
| GET | `/account/{accountId}` | - | `List<TransferResponseDto>` | Transferencias por cuenta |
| GET | `/{id}` | - | `TransferResponseDto` | Obtener por ID |

**DTOs Ubicación**: `FinalProject.Application/DTOs/TransferDtos/`
- `CreateTransferDto.cs` - SourceAccountId, DestinationAccountId, Amount
- `TransferResponseDto.cs` - Respuesta completa con números de cuenta

**Servicio Ubicación**: `FinalProject.Infrastructure/Services/TransferService.cs`

---

## 🗂️ Organización de DTOs

```
FinalProject.Application/DTOs/
├── AuthDtos/
│   ├── LoginRequestDto.cs
│   └── LoginResponseDto.cs
├── CampaignDtos/
│   ├── CreateCampaignDto.cs
│   ├── CampaignResponseDto.cs
│   └── EnrollCampaignDto.cs
├── CompanyDtos/
│   ├── CompanyApprovalDto.cs
│   └── CompanyProfileDto.cs
├── CouponDtos/
│   ├── CreateCouponDto.cs
│   └── CouponResponseDto.cs
├── LukasDtos/
│   ├── EmitLukasDto.cs
│   ├── LukasValueDto.cs
│   └── UpdateLukasValueDto.cs
├── MissionDtos/
│   ├── AssignMissionDto.cs
│   ├── CompleteMissionDto.cs
│   └── UserMissionResponseDto.cs
├── ProductDtos/
│   ├── CreateProductDto.cs
│   ├── ProductResponseDto.cs
│   ├── UpdateProductDto.cs
│   └── ProductPurchaseDto.cs
├── StatisticsDtos/
│   └── SystemStatisticsDto.cs
├── SupplierDtos/
│   ├── CreateSupplierDto.cs
│   └── SupplierResponseDto.cs
├── TransferDtos/
│   ├── CreateTransferDto.cs
│   └── TransferResponseDto.cs
└── UserDtos/
    └── (DTOs de usuario)
```

---

## 🔗 Flujo de Datos Completo

### Ejemplo: Crear una Campaña

```
1. HTTP Request
   POST /api/campaign/create?companyUserId=2
   Body: CreateCampaignDto
   ↓
2. CampaignController
   Ubicación: FinalProject.API/Controllers/CampaignController.cs
   Recibe: CreateCampaignDto
   ↓
3. ICampaignService (Interface)
   Ubicación: FinalProject.Application/Interfaces/ICampaignService.cs
   Método: CreateCampaignAsync(int userId, CreateCampaignDto dto)
   ↓
4. CampaignService (Implementation)
   Ubicación: FinalProject.Infrastructure/Services/CampaignService.cs
   - Usa: LukitasDbContext (EF Core)
   - Crea: Campaign entity (Domain)
   - Retorna: CampaignResponseDto
   ↓
5. HTTP Response
   Body: CampaignResponseDto
```

---

## 📊 Mapa de Dependencias

### Controllers → Services
```
AuthController → IAuthService
CampaignController → ICampaignService
StudentController → IStudentService
AdminController → IAdminService
SupplierController → ISupplierService
ProductController → IProductService
SupplierManagementController → ISupplierManagementService
MissionController → IMissionService
CouponController → ICouponService
TransferController → ITransferService
```

### Services → DbContext
```
Todos los servicios en Infrastructure usan:
- LukitasDbContext (Entity Framework Core)
- DbSet<Entity> para acceso a datos
- Include() para cargar relaciones
- Transacciones para operaciones complejas
```

### DTOs → Entities
```
CreateCampaignDto → Campaign (Entity)
ProductPurchaseDto → Sale + SaleDetail (Entities)
CreateTransferDto → Transfer (Entity)
AssignMissionDto → UserMission (Entity)
CreateCouponDto → Coupon (Entity)
```

---

## 🎯 Patrones Implementados

### 1. Service Layer Pattern
**Ubicación**: `FinalProject.Infrastructure/Services/`

Servicios que encapsulan lógica de negocio:
- `AuthService` - Autenticación y autorización
- `CampaignService` - Operaciones de campañas
- `StudentService` - Operaciones de estudiantes
- `AdminService` - Operaciones administrativas
- `ProductService` - Gestión de productos
- `SupplierService` - Operaciones de proveedores
- `MissionService` - Gestión de misiones
- `CouponService` - Gestión de cupones
- `TransferService` - Transferencias de Lukas

**Características**:
- Implementan interfaces definidas en Application
- Encapsulan lógica de negocio compleja
- Usan Entity Framework Core directamente
- Manejan transacciones y validaciones

**Ejemplo de uso**:
```csharp
// En el Controller
public class CampaignController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    
    public CampaignController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateCampaign(CreateCampaignDto dto)
    {
        var result = await _campaignService.CreateCampaignAsync(userId, dto);
        return Ok(result);
    }
}
```

### 2. Repository Pattern
- `IGenericRepository<T>` en Domain
- `GenericRepository<T>` en Infrastructure
- `IUnitOfWork` para transacciones

**Nota**: Los Services usan DbContext directamente en lugar de repositorios para simplicidad.

### 3. DTO Pattern
- Separación entre entidades y DTOs
- DTOs para entrada (Create, Update)
- DTOs para salida (Response)

### 4. Dependency Injection
- Configurado en `ServiceCollectionExtensions.cs`
- Lifetime: Scoped para servicios y Use Cases
- Interfaces → Implementaciones

---

## 🤔 Arquitectura de Servicios (Service Layer Pattern)

### ¿Por qué Services en lugar de Use Cases puros?

En Clean Architecture pura, los **Use Cases** deberían estar en Application y usar **Repositorios** 
para acceder a datos. Sin embargo, este proyecto usa **Services** en Infrastructure por razones prácticas:

**Ventajas de la implementación actual**:
1. ✓ **Simplicidad**: Entity Framework Core ya proporciona abstracción de datos
2. ✓ **Menos código**: No necesitamos crear repositorios para cada entidad
3. ✓ **Pragmatismo**: Para proyectos medianos, Services son suficientes
4. ✓ **Mantenibilidad**: Menos capas = más fácil de mantener

### Services en Infrastructure

**Ubicación**: `FinalProject.Infrastructure/Services/`

Cada servicio encapsula la lógica de negocio de un dominio:
- `CampaignService` - Operaciones de campañas
- `StudentService` - Operaciones de estudiantes  
- `AdminService` - Operaciones administrativas
- `ProductService` - Operaciones de productos
- `MissionService` - Operaciones de misiones
- `CouponService` - Operaciones de cupones
- `TransferService` - Operaciones de transferencias

**Características**:
- Implementan interfaces definidas en Application
- Usan DbContext directamente (EF Core)
- Encapsulan lógica de negocio
- Manejan transacciones
- Validan reglas de negocio

### ¿Cuándo migrar a Use Cases puros?

Considera usar Use Cases puros cuando:
- El proyecto crece significativamente
- Necesitas cambiar de ORM
- Requieres alta testabilidad con mocks
- Múltiples equipos trabajan en paralelo
- Necesitas reutilizar lógica en diferentes contextos

**Para implementar Use Cases puros necesitarías**:
1. Crear repositorios para cada entidad
2. Mover Services a Application como Use Cases
3. Hacer que Use Cases dependan de repositorios, no de DbContext
4. Implementar repositorios en Infrastructure

---

## 🔍 Cómo Encontrar Componentes

### Para agregar un nuevo endpoint:

1. **Crear DTO** en `FinalProject.Application/DTOs/[Categoria]Dtos/`
2. **Agregar método a Interface** en `FinalProject.Application/Interfaces/I[Nombre]Service.cs`
3. **Implementar en Service** en `FinalProject.Infrastructure/Services/[Nombre]Service.cs`
4. **Agregar endpoint en Controller** en `FinalProject.API/Controllers/[Nombre]Controller.cs`
5. **Registrar servicio** en `FinalProject.API/Extensions/ServiceCollectionExtensions.cs` (si es nuevo)

### Para modificar lógica de negocio:

1. Ir a `FinalProject.Infrastructure/Services/[Nombre]Service.cs`
2. Modificar el método correspondiente
3. Si cambia la firma, actualizar la interface en Application

### Para cambiar estructura de datos:

1. Modificar DTO en `FinalProject.Application/DTOs/`
2. Actualizar mapeo en el Service correspondiente

---

## 📈 Estadísticas del Sistema

```
Total de Archivos Creados: 60+
├── Controllers: 10
├── Services: 10
├── Interfaces: 10
├── DTOs: 22+
└── Entities: 16

Líneas de Código: ~3,500+
Endpoints HTTP: 46+
Métodos de Servicio: 50+
```

---

## ✅ Verificación de Clean Architecture

### ✓ Domain Layer
- Sin dependencias externas
- Solo entidades puras
- Interfaces de repositorio

### ✓ Application Layer
- Solo depende de Domain
- Contiene interfaces de servicios
- Contiene DTOs

### ✓ Infrastructure Layer
- Depende de Application y Domain
- Implementa servicios
- Implementa repositorios
- Acceso a base de datos

### ✓ API Layer
- Depende de todas las capas
- Solo controladores
- Configuración de DI
- Middlewares

---

## 🚀 Acceso a Swagger

```
URL: http://localhost:5140/swagger
```

En Swagger encontrarás todos los endpoints organizados por controlador con:
- Descripción de parámetros
- Ejemplos de request/response
- Posibilidad de probar directamente

---

**Última actualización**: Sistema completo con 46+ APIs REST
