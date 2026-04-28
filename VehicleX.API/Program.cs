using Microsoft.AspNetCore.Mvc;
using VehicleX.API.Common;
using VehicleX.API.Middleware;
using VehicleX.Application;
using VehicleX.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(modelState => modelState.Value?.Errors.Count > 0)
                .SelectMany(modelState => modelState.Value!.Errors)
                .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "Invalid input provided."
                    : error.ErrorMessage)
                .Distinct()
                .ToList();

            var response = ApiResponse<object>.Failure("Validation failed.", errors);
            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
