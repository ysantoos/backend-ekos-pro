using Infrastructure.Entity.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        // Register MediatR handlers from this assembly so handlers placed in Infrastructure are discovered
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}
