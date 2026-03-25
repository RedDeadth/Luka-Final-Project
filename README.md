<div align="center">
  <h1>💰 Luka Final Project - Enterprise API</h1>
  <p><b>Advanced Virtual Currency Management System for Educational Ecosystems</b></p>
  
  [![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
  [![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
  [![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white)](https://www.mysql.com/)
  [![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)
</div>

---

## 📖 Visión General
**Luka** es un ecosistema financiero virtual diseñado bajo estrictos estándares corporativos para entornos estudiantiles. La API actúa como un orquestador central que permite a empresas emitir moneda (`Lukitas`), a estudiantes realizar compras y cumplir misiones mediante Arquitectura Limpia, y a los proveedores transaccionar en tiempo real.

## 🏗️ Arquitectura de Software
Este proyecto se rige fielmente bajo el patrón **Clean Architecture** estructurado en 4 capas profundas, implementando segregación **CQRS** (Command Query Responsibility Segregation) orquestado a través de **MediatR**.

* **`FinalProject.Domain`**: Entidades core del negocio (Users, Products, Transfers).
* **`FinalProject.Application`**: DTOs, Interfaces, y todos los `Commands/Queries` del sistema.
* **`FinalProject.Infrastructure`**: DbContext (Entity Framework Core), Repositorios, CQRS Handlers y Jobs en segundo plano (Hangfire).
* **`FinalProject.API`**: Exposición REST estandarizada V2.

## 🚀 Características Premium
- **Background Jobs (Hangfire):** El sistema autolimpia cupones expirados a medianoche y genera estadísticas semanales de auditoría sin colgar el hilo principal.
- **Seguridad Inquebrantable:** Endpoints asegurados por `JWT` (JSON Web Tokens) estructurados con Claims y Roles granulares.
- **MediatR en todas sus venas:** Todos los endpoints HTTP son 100% inertes; únicamente disparan `.Send()` hacia los Handlers de Infrastructure.
- **Paginación Universal:** Ningún registro de base de datos colapsa en memoria gracias a los `GenericRepositories` paginados obligatorios.

---

## 🛠️ Instrucciones de Despliegue Local

### 1. Requisitos Previos
* **.NET 9.0 SDK** o superior.
* Servidor **MySQL** corriendo en el puerto 3306.
* Archivos confidenciales (`appsettings.json`) apropiadamente configurados con tus credenciales.

### 2. Configurar el Entorno
Copia la estructura del entorno seguro renombrando la plantilla:
```bash
cp FinalProject.API/appsettings.example.json FinalProject.API/appsettings.json
```
*(No olvides inyectar en este nuevo archivo tu frase semilla secreta JWT y acceso a la Base de Datos).*

### 3. Migraciones y Base de Datos
Aplicar el esquema inicial en Entity Framework:
```bash
cd FinalProject.Infrastructure
dotnet ef database update --startup-project ../FinalProject.API
```

### 4. Ejecución (Modo Desarrollo)
Lanza la API con Swagger de manera nativa:
```bash
cd FinalProject.API
dotnet run
```
> La interfaz interactiva **Swagger UI** estará disponible en: `http://localhost:5140/swagger`
> El Dashboard de control de **Hangfire** estará en: `http://localhost:5140/hangfire`

---
*Arquitecto y Desarrollador: **RedDeadth**.*
