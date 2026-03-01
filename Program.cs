using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

// JWT Authentication
var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
var jwtIssuer = configuration["Jwt:Issuer"] ?? "SimpleMarketplaceApi";
var jwtAudience = configuration["Jwt:Audience"] ?? "SimpleMarketplaceClients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactLocal");

// Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configurar endpoints de SignalR
app.MapHub<SimpleMarketplace.Api.Hubs.NotificacionesHub>("/notificaciones");

// Configurar puerto para Railway (comentado para desarrollo local)
// var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
// app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();