using Microsoft.EntityFrameworkCore;
using VehicleX.API.Middleware;
using VehicleX.Application.Interfaces;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;
using VehicleX.Application.Services;
using VehicleX.Infrastructure.Data;
using VehicleX.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection — Repositories
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IPartRepository, PartRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISalesInvoiceRepository, SalesInvoiceRepository>();
builder.Services.AddScoped<IPurchaseInvoiceRepository, PurchaseInvoiceRepository>();

// Dependency Injection — Services
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IFinancialReportService, FinancialReportService>();
builder.Services.AddScoped<ICustomerReportService, CustomerReportService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

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
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Applying database migrations...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying database migrations.");
    }
}

Console.WriteLine(builder.Environment.EnvironmentName);

app.Run();