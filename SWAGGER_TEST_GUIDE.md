# Guía de Pruebas en Swagger

## URL Swagger
```
https://luka-final-project-production.up.railway.app/swagger
```

---

## PASO 1: Login (Obtener Token)

1. Busca **POST /api/Auth/login**
2. Click "Try it out"
3. Pega este JSON:
```json
{
  "email": "juan@test.com",
  "password": "Test123!"
}
```
4. Click "Execute"
5. **COPIA EL TOKEN** de la respuesta

---

## PASO 2: Autorizar Swagger

1. Click en el botón **"Authorize"** (arriba a la derecha, ícono de candado)
2. En el campo escribe: `Bearer TU_TOKEN_AQUI`
3. Click "Authorize"
4. Click "Close"

Ahora puedes probar todos los endpoints.

---

## PRUEBAS POR MÓDULO

### AUTH
| Endpoint | Body/Params | Esperado |
|----------|-------------|----------|
| POST /api/Auth/login | `{"email":"juan@test.com","password":"Test123!"}` | Token JWT |
| POST /api/Auth/login | `{"email":"empresa@test.com","password":"Test123!"}` | Token (coordinator) |
| POST /api/Auth/login | `{"email":"admin@test.com","password":"Test123!"}` | Token (admin) |

---

### STUDENT (usar token de estudiante)
| Endpoint | Params | Esperado |
|----------|--------|----------|
| GET /api/Student/1/balance | studentId: 1 | Balance en Lukas |
| GET /api/Student/1/campaigns | studentId: 1, page: 1 | Lista de campañas |

---

### CAMPAIGNS
| Endpoint | Body/Params | Esperado |
|----------|-------------|----------|
| GET /api/Campaign/active | page: 1, pageSize: 20 | Lista campañas activas |
| GET /api/Campaign/1 | id: 1 | Detalle campaña |
| POST /api/Campaign/enroll | `{"campaignId":1,"studentId":1}` | Inscripción exitosa |

**Crear campaña (usar token de empresa):**
```json
{
  "name": "Nueva Campaña Test",
  "description": "Descripción de prueba",
  "campaignType": "promocion",
  "budget": 5000,
  "startDate": "2024-01-01",
  "endDate": "2025-12-31",
  "schedule": "Lunes a Viernes",
  "location": "Campus",
  "contactNumber": "123456",
  "imageUrls": []
}
```

---

### TRANSFERS
| Endpoint | Body/Params | Esperado |
|----------|-------------|----------|
| GET /api/Transfer/account/1 | accountId: 1 | Historial transferencias |
| POST /api/Transfer | `{"sourceAccountId":1,"destinationAccountId":2,"amount":10}` | Transferencia creada |

---

### PRODUCTS
| Endpoint | Body/Params | Esperado |
|----------|-------------|----------|
| GET /api/Product | page: 1 | Lista productos |
| GET /api/Product/1 | id: 1 | Detalle producto |
| GET /api/Product/supplier/1 | supplierId: 1 | Productos del proveedor |

**Crear producto:**
```json
{
  "supplierId": 1,
  "productTypeId": 1,
  "code": "TEST001",
  "name": "Producto de Prueba",
  "price": 25.50,
  "stock": 100
}
```

---

### SUPPLIERS
| Endpoint | Body/Params | Esperado |
|----------|-------------|----------|
| GET /api/SupplierManagement | page: 1 | Lista proveedores |
| GET /api/SupplierManagement/1 | id: 1 | Detalle proveedor |

**Crear proveedor:**
```json
{
  "supplierTypeId": 1,
  "name": "Proveedor Test",
  "email": "test@proveedor.com",
  "phone": "3001234567"
}
```

---

### MISSIONS
| Endpoint | Params | Esperado |
|----------|--------|----------|
| GET /api/Mission/user/1 | userId: 1 | Todas las misiones |
| GET /api/Mission/user/1/pending | userId: 1 | Misiones pendientes |
| GET /api/Mission/user/1/completed | userId: 1 | Misiones completadas |

**Asignar misión:**
```json
{
  "userId": 1,
  "missionId": 1
}
```

---

### COUPONS
| Endpoint | Params | Esperado |
|----------|--------|----------|
| GET /api/Coupon/campaign/1 | campaignId: 1 | Cupones de campaña |
| GET /api/Coupon/validate/BIENESTAR10 | code: BIENESTAR10 | Validación cupón |

**Crear cupón:**
```json
{
  "campaignId": 1,
  "supplierId": 1,
  "code": "TESTCUPON",
  "discountType": "percentage",
  "discountValue": 15,
  "expirationDate": "2025-12-31"
}
```

---

### ADMIN (usar token de admin)
| Endpoint | Body/Params | Esperado |
|----------|-------------|----------|
| GET /api/Admin/statistics | - | Estadísticas del sistema |
| GET /api/Admin/companies/pending | page: 1 | Empresas pendientes |
| GET /api/Admin/lukas/value | - | Valor actual del Luka |

**Emitir Lukas:**
```json
{
  "companyId": 2,
  "amount": 500,
  "reason": "Prueba de emisión"
}
```

**Actualizar valor Luka:**
```json
{
  "lukasToUsdRate": 0.15
}
```

---

### REPORTS (Descargan Excel)
| Endpoint | Params | Esperado |
|----------|--------|----------|
| GET /api/Report/transactions/excel | startDate, endDate (opcional) | Archivo .xlsx |
| GET /api/Report/sales/excel | startDate, endDate (opcional) | Archivo .xlsx |
| GET /api/Report/users/excel | - | Archivo .xlsx |

---

## USUARIOS DE PRUEBA

| Rol | Email | Password | Permisos |
|-----|-------|----------|----------|
| Estudiante | juan@test.com | Test123! | Ver balance, campañas, transferir |
| Empresa | empresa@test.com | Test123! | Crear campañas, ver reportes |
| Admin | admin@test.com | Test123! | Todo: estadísticas, emitir Lukas |

---

## CÓDIGOS DE RESPUESTA

| Código | Significado |
|--------|-------------|
| 200 | OK - Operación exitosa |
| 201 | Created - Recurso creado |
| 400 | Bad Request - Datos inválidos |
| 401 | Unauthorized - Token inválido o faltante |
| 404 | Not Found - Recurso no encontrado |
| 500 | Server Error - Error interno |

---

## TIPS

1. **Siempre haz login primero** y copia el token
2. **Autoriza Swagger** con el token antes de probar otros endpoints
3. Para probar como **empresa**, usa `empresa@test.com`
4. Para probar como **admin**, usa `admin@test.com`
5. Los reportes Excel se descargan directamente
