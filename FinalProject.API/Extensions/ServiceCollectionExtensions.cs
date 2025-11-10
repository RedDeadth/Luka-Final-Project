using FinalProject.Application.Interfaces;
using FinalProject.Application.Services;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using FinalProject.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LukitasDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
