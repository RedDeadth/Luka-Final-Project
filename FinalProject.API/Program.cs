using FinalProject.API.Extensions;
using FinalProject.API.Middlewares;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5140");

builder.Services.AddControllers();
// builder.Services.AddOpenApi(); // removed in favor of Swashbuckle
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHangfireServices(builder.Configuration);
                                                                                                                                                                                                                                                                                        
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi(); // removed in favor of Swashbuckle
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinalProject API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

// app.UseHttpsRedirection(); // Disabled for local testing

app.UseMiddleware<JwtAuthMiddleware>();

// Configurar Hangfire Dashboard (opcional - solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        DashboardTitle = "Lukitas - Hangfire Dashboard",
        Authorization = new[] { new HangfireAuthorizationFilter() }
    });
}

// Configurar Jobs Recurrentes
HangfireJobsConfiguration.ConfigureRecurringJobs();

app.MapControllers();
app.MapGet("/", () => "API is running");

app.Run();
