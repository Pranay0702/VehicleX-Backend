using Microsoft.EntityFrameworkCore;
using VehicleX.API.Middleware;
using VehicleX.Application.Interfaces;
using VehicleX.Application.Services;
using VehicleX.Infrastructure.Data;
using VehicleX.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database Context — configured for Supabase Transaction Pooler
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(120);
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null);
        }));

// Dependency Injection — Repositories
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IPartRepository, PartRepository>();

// Dependency Injection — Services
builder.Services.AddScoped<IVendorService, VendorService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Global exception handler
app.UseGlobalExceptionMiddleware();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

Console.WriteLine(builder.Environment.EnvironmentName);

app.Run();