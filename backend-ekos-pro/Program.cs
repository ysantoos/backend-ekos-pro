using backend_ekos_pro.Middleware;
using Domain.Service;
using Infrastructure.Entity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure and Domain layers
builder.Services.AddInfrastructureEntity(builder.Configuration);
builder.Services.AddDomainService();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ekos Pro API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at app's root
    });
}

// Global exception handling middleware (must be first)
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

app.MapControllers();

app.Run();
