using FinalProject.API.Extensions;
using FinalProject.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// En desarrollo usa puerto 5140, en producción usa variable de entorno PORT
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://localhost:5140");
}

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// CORS para React - incluye dominios de producción
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", 
                "http://localhost:5173",
                "http://localhost:8080",
                "http://127.0.0.1:5173",
                "https://*.railway.app",
                "https://*.up.railway.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Hangfire solo en desarrollo
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHangfireServices(builder.Configuration);
}
                                                                                                                                                                                                                                                                                        
var app = builder.Build();

// Swagger disponible en todos los entornos
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinalProject API V1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowReact");

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<JwtAuthMiddleware>();

// Hangfire solo en desarrollo
if (app.Environment.IsDevelopment())
{
    // Hangfire Dashboard y Jobs solo en desarrollo
    // app.UseHangfireDashboard("/hangfire");
    // HangfireJobsConfiguration.ConfigureRecurringJobs();
}

app.MapControllers();
app.MapGet("/", () => "API is running");

app.Run();
