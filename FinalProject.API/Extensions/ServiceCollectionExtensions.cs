using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using FinalProject.Infrastructure.Repositories;
using FinalProject.Infrastructure.Services;
using FinalProject.Infrastructure.Jobs;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.MySql;

namespace FinalProject.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar MediatR - handlers están en Infrastructure
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(FinalProject.Application.Common.ICommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(FinalProject.Infrastructure.Handlers.Auth.LoginCommandHandler).Assembly);
        });
        
        // Registrar servicios de negocio (implementaciones en Infrastructure) - mantener para compatibilidad con V1
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISupplierManagementService, SupplierManagementService>();
        services.AddScoped<IMissionService, MissionService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<ITransferService, TransferService>();

        // Registrar servicio de hashing de contraseñas
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var serverVersion = new MySqlServerVersion(new Version(9, 2, 0));
        services.AddDbContext<LukitasDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }

    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Configurar Hangfire con MySQL Storage
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(new MySqlStorage(
                connectionString,
                new MySqlStorageOptions
                {
                    TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    DashboardJobListLimit = 50000,
                    TransactionTimeout = TimeSpan.FromMinutes(1),
                    TablesPrefix = "Hangfire"
                }
            )));

        // Agregar el servidor de Hangfire
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
            options.ServerName = "FinalProject-HangfireServer";
        });

        // Registrar Jobs como servicios
        services.AddScoped<ExpireCouponsJob>();
        services.AddScoped<DataCleanupJob>();
        services.AddScoped<DailyStatisticsJob>();

        return services;
    }
}
