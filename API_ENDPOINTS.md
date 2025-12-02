# API Endpoints - Lukitas

## Base URL
```
https://luka-final-project-production.up.railway.app
```

## Autenticación
Todos los endpoints (excepto login) requieren header:
```
Authorization: Bearer {token}
```

---

## AUTH

### Login
```
POST /api/Auth/login
Body: { "email": "string", "password": "string" }
Response: { id, email, firstName, lastName, role, token, company, university }
```

### Validar Token
```
POST /api/Auth/validate
Body: "token_string"
```

---

## ESTUDIANTE

### Ver Balance
```
GET /api/Student/{studentId}/balance
Response: { success, data: { balance, accountNumber } }
```

### Ver Campañas Inscritas
```
GET /api/Student/{studentId}/campaigns?page=1&pageSize=20
```

### Realizar Compra
```
POST /api/Student/purchase
Body: {
  "studentId": 1,
  "supplierId": 2,
  "items": [{ "productId": 1, "quantity": 2 }]
}
```

---

## CAMPAÑAS

### Listar Activas
```
GET /api/Campaign/active?page=1&pageSize=20
```

### Obtener por ID
```
GET /api/Campaign/{id}
```

### Crear (Coordinator)
```
POST /api/Campaign/create?userId={coordinatorId}
Body: {
  "name": "string",
  "description": "string",
  "campaignType": "string",
  "budget": 1000,
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "schedule": "string",
  "location": "string",
  "contactNumber": "string",
  "imageUrls": []
}
```

### Inscribirse
```
POST /api/Campaign/enroll
Body: { "campaignId": 1, "studentId": 1 }
```

### Campañas de Empresa
```
GET /api/Campaign/company/{companyUserId}?page=1&pageSize=20
```

---

## TRANSFERENCIAS

### Crear Transferencia
```
POST /api/Transfer
Body: {
  "sourceAccountId": 1,
  "destinationAccountId": 2,
  "amount": 50.00
}
```

### Historial por Cuenta
```
GET /api/Transfer/account/{accountId}?page=1&pageSize=20
```

### Obtener por ID
```
GET /api/Transfer/{id}
```

---

## PRODUCTOS

### Listar
```
GET /api/Product?page=1&pageSize=20
```

### Obtener por ID
```
GET /api/Product/{id}
```

### Crear
```
POST /api/Product
Body: {
  "supplierId": 1,
  "productTypeId": 1,
  "code": "PROD001",
  "name": "string",
  "price": 10.50,
  "stock": 100
}
```

### Actualizar
```
PUT /api/Product/{id}
Body: { "name": "string", "price": 10.50, "stock": 100, "status": "active" }
```

### Eliminar
```
DELETE /api/Product/{id}
```

### Por Proveedor
```
GET /api/Product/supplier/{supplierId}?page=1&pageSize=20
```

---

## PROVEEDORES

### Listar
```
GET /api/SupplierManagement?page=1&pageSize=20
```

### Obtener por ID
```
GET /api/SupplierManagement/{id}
```

### Crear
```
POST /api/SupplierManagement
Body: { "supplierTypeId": 1, "name": "string", "email": "string", "phone": "string" }
```

### Actualizar
```
PUT /api/SupplierManagement/{id}
Body: { "name": "string", "email": "string", "phone": "string", "status": "active" }
```

### Eliminar
```
DELETE /api/SupplierManagement/{id}
```

---

## MISIONES

### Listar por Usuario
```
GET /api/Mission/user/{userId}?page=1&pageSize=20
```

### Pendientes
```
GET /api/Mission/user/{userId}/pending?page=1&pageSize=20
```

### Completadas
```
GET /api/Mission/user/{userId}/completed?page=1&pageSize=20
```

### Asignar
```
POST /api/Mission/assign
Body: { "userId": 1, "missionId": 1 }
```

### Completar
```
POST /api/Mission/complete
Body: { "userMissionId": 1, "saleId": 1 }
```

---

## CUPONES

### Buscar por Código
```
GET /api/Coupon/code/{code}
```

### Por Campaña
```
GET /api/Coupon/campaign/{campaignId}?page=1&pageSize=20
```

### Por Proveedor
```
GET /api/Coupon/supplier/{supplierId}?page=1&pageSize=20
```

### Validar
```
GET /api/Coupon/validate/{code}
```

### Crear
```
POST /api/Coupon
Body: {
  "campaignId": 1,
  "supplierId": 1,
  "code": "CUPON10",
  "discountType": "percentage",
  "discountValue": 10.00,
  "expirationDate": "2024-12-31"
}
```

### Eliminar
```
DELETE /api/Coupon/{id}
```

---

## ADMIN

### Estadísticas
```
GET /api/Admin/statistics
Response: {
  totalUsers, totalStudents, totalCompanies, totalSuppliers,
  activeCampaigns, totalLukasInCirculation, totalLukasSpent,
  totalTransactions, recentActivity
}
```

### Empresas Pendientes
```
GET /api/Admin/companies/pending?page=1&pageSize=20
```

### Aprobar Empresa
```
POST /api/Admin/companies/approve
Body: { "companyId": 1, "approved": true, "reason": "string" }
```

### Valor del Luka
```
GET /api/Admin/lukas/value
PUT /api/Admin/lukas/value
Body: { "lukasToUsdRate": 0.50 }
```

### Emitir Lukas
```
POST /api/Admin/lukas/emit
Body: { "companyId": 1, "amount": 1000, "reason": "string" }
```

---

## REPORTES EXCEL

### Transferencias
```
GET /api/Report/transactions/excel?startDate=2024-01-01&endDate=2024-12-31
Response: Archivo .xlsx
```

### Ventas
```
GET /api/Report/sales/excel?startDate=2024-01-01&endDate=2024-12-31
Response: Archivo .xlsx
```

### Usuarios
```
GET /api/Report/users/excel
Response: Archivo .xlsx
```

---

## Usuarios de Prueba

| Rol | Email | Password |
|-----|-------|----------|
| Estudiante | juan@test.com | Test123! |
| Empresa | empresa@test.com | Test123! |
| Admin | admin@test.com | Test123! |

---

## Formato de Respuesta

```json
{
  "success": true,
  "data": { ... },
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 100,
    "totalPages": 5
  }
}
```
