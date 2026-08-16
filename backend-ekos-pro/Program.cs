using backend_ekos_pro.Middleware;
using Domain.Service;
using Infrastructure.Entity;

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
    app.UseSwaggerUI();
}


// Global exception handling middleware (must be first)
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

// Enable CORS using the configured policy
app.UseCors("DefaultCorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
