using Hangfire.Dashboard;

namespace FinalProject.API.Middlewares;

/// <summary>
/// Filtro de autorización para el Dashboard de Hangfire
/// En desarrollo: permite acceso sin restricciones
/// En producción: debería validar roles de administrador
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // En desarrollo permitir acceso sin autenticación
        // En producción, validar que el usuario sea administrador
        // var httpContext = context.GetHttpContext();
        // return httpContext.User.IsInRole("Admin");

        return true; // Permitir acceso en desarrollo
    }
}
