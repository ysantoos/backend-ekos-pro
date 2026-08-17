using Infrastructure.Entity.Data;
using Infrastructure.Entity.Options;
using Infrastructure.Entity.Services;
using Domain.Service.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace Infrastructure.Entity;

/// <summary>
/// Dependency injection configuration for Infrastructure.Entity layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Infrastructure.Entity services
    /// </summary>
    public static IServiceCollection AddInfrastructureEntity(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    // Assembly for migrations
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);

                    // Enable retry on failure (transient fault handling)
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    // Command timeout
                    sqlOptions.CommandTimeout(60);
                });

            // Enable query splitting for better performance with include operations
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        // Register repositories here when needed
        // services.AddScoped<IBookRepository, BookRepository>();

        // Bind Gemini options (stored under section "Gemini" in appsettings)
        var geminiOptions = configuration.GetSection("Gemini").Get<GeminiOptions>() ?? new GeminiOptions();
        services.AddSingleton(geminiOptions);

        // Register a simple HttpClient-backed GeminiTextGenerationService.
        // We create a dedicated HttpClient per service instance to avoid requiring IHttpClientFactory in this project.
        services.AddScoped<ITextGenerationService>(sp =>
        {
            var opts = sp.GetRequiredService<GeminiOptions>();
            var client = new HttpClient
            {
                BaseAddress = new Uri(opts.BaseUrl),
                Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds)
            };
            if (!string.IsNullOrWhiteSpace(opts.ApiKey))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", opts.ApiKey);

            return new GeminiTextGenerationService(client, Microsoft.Extensions.Options.Options.Create(opts));
        });

        // Register MediatR handlers from this assembly so handlers placed in Infrastructure are discovered
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}
