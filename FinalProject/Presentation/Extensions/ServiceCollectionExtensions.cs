namespace FinalProject.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar servicios de aplicación aquí
        // Ejemplo: services.AddScoped<IMyService, MyService>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Registrar repositorios y servicios de infraestructura aquí
        // Ejemplo: services.AddScoped<IMyRepository, MyRepository>();
        return services;
    }
}
