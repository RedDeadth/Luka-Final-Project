using FinalProject.Application.Interfaces;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using FinalProject.Infrastructure.Repositories;
using FinalProject.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar servicios de negocio (implementaciones en Infrastructure)
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ISupplierService, SupplierService>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var serverVersion = new MySqlServerVersion(new Version(9, 0, 1));
        services.AddDbContext<LukitasDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
