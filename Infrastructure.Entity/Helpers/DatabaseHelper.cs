using Infrastructure.Entity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Entity.Helpers;

/// <summary>
/// Helper class for database migrations and initialization
/// </summary>
public static class DatabaseHelper
{
    /// <summary>
    /// Applies pending migrations and creates the database if it doesn't exist
    /// </summary>
    public static async Task MigrateDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetService<ILogger<ApplicationDbContext>>();

        try
        {
            logger?.LogInformation("Starting database migration");

            // Create database if it doesn't exist and apply migrations
            await context.Database.MigrateAsync();

            logger?.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }

    /// <summary>
    /// Checks if the database exists
    /// </summary>
    public static async Task<bool> DatabaseExistsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Database.CanConnectAsync();
    }

    /// <summary>
    /// Gets pending migrations
    /// </summary>
    public static async Task<IEnumerable<string>> GetPendingMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Database.GetPendingMigrationsAsync();
    }

    /// <summary>
    /// Gets applied migrations
    /// </summary>
    public static async Task<IEnumerable<string>> GetAppliedMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Database.GetAppliedMigrationsAsync();
    }

    /// <summary>
    /// Deletes the database (USE WITH CAUTION - only for development)
    /// </summary>
    public static async Task DeleteDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetService<ILogger<ApplicationDbContext>>();

        logger?.LogWarning("Deleting database");
        await context.Database.EnsureDeletedAsync();
        logger?.LogWarning("Database deleted");
    }
}
