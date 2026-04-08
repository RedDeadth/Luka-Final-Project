# Luka Final Project - Enterprise API
[English](#english) | [Español](#español)

## English
Virtual currency system (Lukitas) for educational ecosystems — companies issue currency, students purchase and complete missions.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)

### Software Architecture
This project is structured around the Clean Architecture pattern with four main layers, implementing CQRS orchestrated via MediatR to decouple infrastructure from the core domain.

### Detailed Directory Structure
```text
├── FinalProject.API
│   ├── Controllers/V2         # CQRS API Endpoints (Admin, Auth, Campaigns, etc.)
│   ├── Extensions             # Service Registrations (Hangfire, MediatR, DbContext)
│   ├── Filters                # API Filters (e.g., Hangfire Authorization)
│   ├── Middlewares            # Global Error Handling & JWT Middleware
│   ├── appsettings.json       # Local configuration secrets
│   └── Program.cs             # Application Entry Point
├── FinalProject.Application
│   ├── Common                 # Base CQRS Interfaces (ICommand, IQuery, Result)
│   ├── DTOs                   # Data Transfer Objects
│   ├── Features               # MediatR Commands and Queries definitions
│   └── Interfaces             # Legacy Service Contracts
├── FinalProject.Domain
│   ├── Entities               # Core Database Models (Account, Campaign, Coupon...)
│   └── Interfaces             # Core Repository Contracts (IUnitOfWork, IGenericRepository)
├── FinalProject.Infrastructure
│   ├── Data                   # Entity Framework DbContext (LukitasDbContext)
│   ├── Handlers               # MediatR CQRS Handlers implementation
│   ├── Jobs                   # Hangfire Background Tasks (DailyStatistics, ExpireCoupons)
│   ├── Repositories           # Generic Repository Implementations
│   └── Services               # Legacy Infrastructure Services
├── database_schema.sql        # Database schema script
├── render.yaml                # Render.com CI/CD architecture blueprint
└── Dockerfile                 # Containerization instructions
```

### Deployment Instructions

#### Prerequisites
- .NET 9.0 SDK or higher.
- PostgreSQL Server running on port 5432.
- `appsettings.json` accurately configured with your credentials.

#### Configuration
Copy the sample environment template to define your connection string and JWT Key:
```bash
cp FinalProject.API/appsettings.example.json FinalProject.API/appsettings.json
```

#### Database (Database-First Scaffolding)
This project is designed using the **Database-First (Scaffolding)** approach rather than Code-First Migrations.
To establish the database structure:
1. Execute the `database_schema.sql` script directly in your PostgreSQL database console.
2. Entity Framework Core is designed to automatically map its configured DbSets against the previously constructed schema without altering it.

#### Execution
Launch the API locally:
```bash
cd FinalProject.API
dotnet run
```
- **Swagger UI**: `http://localhost:5140/swagger`
- **Hangfire Dashboard**: `http://localhost:5140/hangfire`

---

## Español
Sistema de moneda virtual (Lukitas) para ecosistemas educativos — las empresas emiten moneda, los estudiantes compran y completan misiones.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)

### Arquitectura de Software
Este proyecto está estructurado en base al patrón de Clean Architecture con cuatro capas principales, implementando CQRS orquestado vía MediatR para desacoplar la infraestructura del dominio central.

### Estructura Detallada de Directorios
```text
├── FinalProject.API
│   ├── Controllers/V2         # Endpoints de la API CQRS (Admin, Auth, Campañas, etc.)
│   ├── Extensions             # Registro de Servicios (Hangfire, MediatR, DbContext)
│   ├── Filters                # Filtros de API (ej., Autorización de Hangfire)
│   ├── Middlewares            # Manejo Global de Errores y Middleware de JWT
│   ├── appsettings.json       # Secretos de configuración local
│   └── Program.cs             # Punto de entrada de la aplicación
├── FinalProject.Application
│   ├── Common                 # Interfaces base de CQRS (ICommand, IQuery, Result)
│   ├── DTOs                   # Objetos de Transferencia de Datos
│   ├── Features               # Definiciones de Comandos y Consultas de MediatR
│   └── Interfaces             # Contratos de Servicios Legacy
├── FinalProject.Domain
│   ├── Entities               # Modelos de Base de Datos (Cuenta, Campaña, Cupón...)
│   └── Interfaces             # Contratos de Repositorio (IUnitOfWork, IGenericRepository)
├── FinalProject.Infrastructure
│   ├── Data                   # Entity Framework DbContext (LukitasDbContext)
│   ├── Handlers               # Implementación de Handlers de MediatR para CQRS
│   ├── Jobs                   # Tareas en Segundo Plano de Hangfire (Estadísticas, Expiración)
│   ├── Repositories           # Implementaciones de Repositorios Genéricos
│   └── Services               # Servicios Legacy de Infraestructura
├── database_schema.sql        # Script del esquema de la base de datos
├── render.yaml                # Diseño de arquitectura CI/CD para Render.com
└── Dockerfile                 # Instrucciones de contenedorización (Docker)
```

### Instrucciones de Despliegue

#### Requisitos Previos
- SDK de .NET 9.0 o superior.
- Servidor PostgreSQL corriendo en el puerto 5432.
- `appsettings.json` correctamente configurado con tus credenciales.

#### Configuración
Copia la plantilla de ejemplo para definir tu cadena de conexión y clave JWT:
```bash
cp FinalProject.API/appsettings.example.json FinalProject.API/appsettings.json
```

#### Base de Datos (Database-First Scaffolding)
Este proyecto está diseñado usando el enfoque **Database-First (Scaffolding)** en lugar de migraciones Code-First.
Para establecer la estructura de la base de datos:
1. Ejecuta el script `database_schema.sql` directamente en tu consola de PostgreSQL.
2. Entity Framework Core está diseñado para mapear automáticamente sus DbSets configurados contra el esquema construido previamente sin alterarlo.

#### Ejecución
Inicia la API localmente:
```bash
cd FinalProject.API
dotnet run
```
- **Swagger UI**: `http://localhost:5140/swagger`
- **Hangfire Dashboard**: `http://localhost:5140/hangfire`
