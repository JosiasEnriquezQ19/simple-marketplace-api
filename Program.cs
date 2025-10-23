using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    // Configuración CORS para desarrollo y producción
    options.AddPolicy("AllowReactLocal", policy => policy
        .SetIsOriginAllowed(_ => true) // Permite todos los orígenes temporalmente para debug
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

// DbContext
var configuration = builder.Configuration;
builder.Services.AddDbContext<SimpleMarketplace.Api.Data.ApplicationDbContext>(options =>
    options.UseMySql(configuration.GetConnectionString("DefaultConnection"), ServerVersion.Parse("8.0.33-mysql")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(SimpleMarketplace.Api.Mapping.MappingProfile));

// Auth service

builder.Services.AddScoped<SimpleMarketplace.Api.Services.IAuthService, SimpleMarketplace.Api.Services.AuthService>();

// Authentication removed for testing (endpoints are anonymous)

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactLocal");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapControllers();

// Configurar endpoints de SignalR
app.MapHub<SimpleMarketplace.Api.Hubs.NotificacionesHub>("/notificaciones");

// Configurar puerto para Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}