using backend_ekos_pro.Middleware;
using Domain.Service;
using Infrastructure.Entity;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
// Configure CORS for front-end requests (adjust origins as needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // front-end origin
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ekos Pro API",
        Version = "v1",
        Description = "API documentation for Ekos Pro"
    });

    // Include XML comments from this assembly and referenced projects (if available)
    var basePath = AppContext.BaseDirectory;
    var assemblies = new[]
    {
        Assembly.GetExecutingAssembly().GetName().Name,
        "Domain.Service",
        "Infrastructure.Entity"
    };

    foreach (var asmName in assemblies)
    {
        try
        {
            var xmlFile = Path.Combine(basePath, asmName + ".xml");
            if (File.Exists(xmlFile))
            {
                options.IncludeXmlComments(xmlFile);
            }
        }
        catch
        {
            // Ignore missing XML files
        }
    }
});

// Add Infrastructure and Domain layers
builder.Services.AddInfrastructureEntity(builder.Configuration);
builder.Services.AddDomainService();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ekos Pro API v1");
    options.RoutePrefix = string.Empty; // Swagger UI at app's root
    options.DocumentTitle = "Ekos Pro API Docs";
    options.DefaultModelsExpandDepth(-1); // hide schema definitions by default
});

// Global exception handling middleware (must be first)
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

// Enable CORS using the configured policy
app.UseCors("DefaultCorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
