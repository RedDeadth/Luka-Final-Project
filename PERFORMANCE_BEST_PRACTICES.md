# ⚡ Mejores Prácticas de Performance - Entity Framework Core

## 🎯 Regla de Oro: Filtrar en Base de Datos, No en Memoria

---

## ❌ MAL: Filtrado en Memoria

```csharp
// ❌ INCORRECTO: Trae TODOS los datos a memoria y luego filtra
public async Task<List<Product>> GetActiveProducts()
{
    var allProducts = await _context.Products.ToListAsync(); // Trae TODO
    return allProducts.Where(p => p.Status == "active").ToList(); // Filtra en memoria
}
```

**Problemas**:
- Consume mucha memoria
- Trae datos innecesarios de la BD
- Lento con tablas grandes
- Desperdicia ancho de banda

---

## ✅ BIEN: Filtrado en Base de Datos

```csharp
// ✅ CORRECTO: Filtra en la base de datos
public async Task<List<Product>> GetActiveProducts()
{
    return await _context.Products
        .Where(p => p.Status == "active") // Filtrado en SQL
        .ToListAsync(); // Solo trae datos filtrados
}
```

**Ventajas**:
- Solo trae datos necesarios
- Usa índices de la BD
- Rápido incluso con millones de registros
- Menos memoria consumida

---

## 📊 Ejemplos del Proyecto

### ✅ Ejemplo 1: CampaignService (CORRECTO)

```csharp
public async Task<List<CampaignResponseDto>> GetActiveCampaignsAsync()
{
    return await _context.Campaigns
        .Where(c => c.Active == true && c.EndDate >= DateOnly.FromDateTime(DateTime.Now))
        .Include(c => c.User)
        .Include(c => c.Accounts)
        .Select(c => new CampaignResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            // ... más propiedades
        })
        .ToListAsync(); // ✓ Filtrado ANTES de ToListAsync
}
```

**SQL Generado**:
```sql
SELECT c.Id, c.Name, ...
FROM Campaigns c
WHERE c.Active = 1 AND c.EndDate >= @p0
```

---

### ✅ Ejemplo 2: StudentService con Subconsulta (CORRECTO)

```csharp
public async Task<List<CampaignResponseDto>> GetAvailableCampaignsAsync(int studentId)
{
    // Subconsulta - NO se ejecuta hasta que se use
    var enrolledCampaignIds = _context.Accounts
        .Where(a => a.UserId == studentId)
        .Select(a => a.CampaignId);

    // Todo se ejecuta en una sola query SQL
    return await _context.Campaigns
        .Where(c => c.Active == true && 
                   !enrolledCampaignIds.Contains(c.Id)) // ✓ Subconsulta en SQL
        .ToListAsync();
}
```

**SQL Generado**:
```sql
SELECT c.*
FROM Campaigns c
WHERE c.Active = 1 
  AND c.Id NOT IN (
    SELECT a.CampaignId 
    FROM Accounts a 
    WHERE a.UserId = @studentId
  )
```

---

### ❌ Ejemplo 3: Anti-patrón (EVITAR)

```csharp
// ❌ NO HACER ESTO
public async Task<List<CampaignResponseDto>> GetAvailableCampaignsAsync(int studentId)
{
    // Trae TODOS los IDs a memoria
    var enrolledCampaignIds = await _context.Accounts
        .Where(a => a.UserId == studentId)
        .Select(a => a.CampaignId)
        .ToListAsync(); // ❌ ToListAsync aquí es problemático

    // Luego filtra en memoria
    return await _context.Campaigns
        .Where(c => !enrolledCampaignIds.Contains(c.Id)) // Puede no optimizarse bien
        .ToListAsync();
}
```

---

## 🔍 Cómo Identificar Problemas

### 1. Buscar patrones problemáticos:

```csharp
// ❌ Patrón 1: ToList() seguido de Where
var list = await _context.Table.ToListAsync();
var filtered = list.Where(x => x.Condition);

// ❌ Patrón 2: ToList() seguido de Select
var list = await _context.Table.ToListAsync();
var mapped = list.Select(x => new Dto { ... });

// ❌ Patrón 3: ToList() seguido de OrderBy
var list = await _context.Table.ToListAsync();
var sorted = list.OrderBy(x => x.Property);
```

### 2. Verificar el orden de operaciones:

```csharp
// ✅ CORRECTO: Where → Select → ToListAsync
await _context.Table
    .Where(x => x.Condition)
    .Select(x => new Dto { ... })
    .ToListAsync();

// ❌ INCORRECTO: ToListAsync → Where → Select
(await _context.Table.ToListAsync())
    .Where(x => x.Condition)
    .Select(x => new Dto { ... });
```

---

## 📈 Optimizaciones Aplicadas en el Proyecto

### 1. ProductService
```csharp
// ✅ Filtrado en BD
return await _context.Products
    .Where(p => p.Status == "active") // SQL WHERE
    .Include(p => p.Supplier)
    .Select(p => new ProductResponseDto { ... })
    .ToListAsync();
```

### 2. MissionService
```csharp
// ✅ Filtrado en BD
return await _context.UserMissions
    .Where(um => um.UserId == userId && um.Completed == false) // SQL WHERE
    .Include(um => um.Mission)
    .Select(um => new UserMissionResponseDto { ... })
    .ToListAsync();
```

### 3. CouponService
```csharp
// ✅ Filtrado en BD
return await _context.Coupons
    .Where(c => c.CampaignId == campaignId && c.Active == true) // SQL WHERE
    .Include(c => c.Campaign)
    .Select(c => new CouponResponseDto { ... })
    .ToListAsync();
```

---

## 🎯 Reglas a Seguir

### 1. Orden de Operaciones LINQ

```csharp
// ✅ Orden correcto:
_context.Table
    .Where(...)      // 1. Filtrar
    .Include(...)    // 2. Cargar relaciones
    .OrderBy(...)    // 3. Ordenar
    .Select(...)     // 4. Proyectar
    .ToListAsync()   // 5. Ejecutar query
```

### 2. Usar Proyecciones (Select)

```csharp
// ✅ MEJOR: Solo trae columnas necesarias
return await _context.Products
    .Select(p => new ProductDto 
    { 
        Id = p.Id, 
        Name = p.Name 
    })
    .ToListAsync();

// ❌ PEOR: Trae todas las columnas
return await _context.Products
    .ToListAsync();
```

### 3. Evitar N+1 Queries

```csharp
// ❌ N+1 Problem
var products = await _context.Products.ToListAsync();
foreach (var product in products)
{
    var supplier = await _context.Suppliers.FindAsync(product.SupplierId);
}

// ✅ Solución: Include
var products = await _context.Products
    .Include(p => p.Supplier)
    .ToListAsync();
```

---

## 🔧 Herramientas para Verificar Performance

### 1. Logging de SQL

En `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### 2. Ver SQL Generado

```csharp
var query = _context.Products
    .Where(p => p.Status == "active");

var sql = query.ToQueryString(); // Ver SQL generado
Console.WriteLine(sql);
```

### 3. Usar AsNoTracking para Lectura

```csharp
// ✅ Para queries de solo lectura
return await _context.Products
    .AsNoTracking() // Más rápido, no trackea cambios
    .Where(p => p.Status == "active")
    .ToListAsync();
```

---

## 📊 Comparación de Performance

### Escenario: 10,000 productos, buscar 100 activos

| Método | Tiempo | Memoria | SQL Queries |
|--------|--------|---------|-------------|
| ❌ ToList() → Where | 500ms | 50MB | 1 query grande |
| ✅ Where → ToList() | 50ms | 5MB | 1 query optimizada |

**Diferencia**: 10x más rápido, 10x menos memoria

---

## ✅ Checklist de Optimización

- [ ] Todos los `Where` están ANTES de `ToListAsync()`
- [ ] Todos los `Select` están ANTES de `ToListAsync()`
- [ ] Todos los `OrderBy` están ANTES de `ToListAsync()`
- [ ] Se usan `Include()` para evitar N+1
- [ ] Se usan proyecciones (`Select`) para traer solo datos necesarios
- [ ] Se usa `AsNoTracking()` para queries de solo lectura
- [ ] Las subconsultas se mantienen como `IQueryable` hasta el final

---

## 🎓 Resumen

**Regla Simple**: 
> Todo lo que puedas hacer en SQL, hazlo en SQL. 
> Solo trae a memoria lo que realmente necesitas.

**Patrón a seguir**:
```csharp
return await _context.Table
    .Where(...)      // Filtrar en BD
    .Include(...)    // Cargar relaciones
    .Select(...)     // Proyectar solo lo necesario
    .ToListAsync();  // Ejecutar y traer a memoria
```

---

**Estado del Proyecto**: ✅ Todos los servicios optimizados correctamente
