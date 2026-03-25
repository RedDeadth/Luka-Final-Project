# Luka Final Project - Enterprise API

Advanced Virtual Currency Management System for Educational Ecosystems.
Built with .NET 9.0, PostgreSQL, Entity Framework Core, Hangfire, and Swagger.

## Software Architecture

This project strictly adheres to the Clean Architecture pattern, structured into four main layers, implementing CQRS (Command Query Responsibility Segregation) orchestrated via MediatR. Both HTTP endpoints and background jobs rely on specialized handlers to process business logic, decoupling the infrastructure from the core domain.

## Detailed Directory Structure

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
├── database_schema.sql        # Master database schema script
├── render.yaml                # Render.com CI/CD architecture blueprint
└── Dockerfile                 # Containerization instructions
```

## Deployment Instructions

### Prerequisites
- .NET 9.0 SDK or higher.
- PostgreSQL Server running on port 5432.
- `appsettings.json` accurately configured with your credentials.

### Configuration
Copy the sample environment template to define your connection string and JWT Key:
```bash
cp FinalProject.API/appsettings.example.json FinalProject.API/appsettings.json
```

### Database (Database-First Scaffolding)
This project is designed using the **Database-First (Scaffolding)** approach rather than Code-First Migrations.
To establish the database structure:
1. Execute the `database_schema.sql` master script directly in your PostgreSQL database console.
2. Entity Framework Core is designed to automatically map its configured DbSets against the previously constructed schema without altering it.

### Execution
Launch the API locally:
```bash
cd FinalProject.API
dotnet run
```
- **Swagger UI**: `http://localhost:5140/swagger`
- **Hangfire Dashboard**: `http://localhost:5140/hangfire`
