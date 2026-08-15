using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Entity.Extensions;

/// <summary>
/// Extension methods for DbContext operations
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Detaches all tracked entities from the context
    /// </summary>
    public static void DetachAllEntities(this DbContext context)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Detached)
            .ToList();

        foreach (var entry in entries)
        {
            entry.State = EntityState.Detached;
        }
    }

    /// <summary>
    /// Gets all tracked entities of a specific type
    /// </summary>
    public static IEnumerable<TEntity> GetTrackedEntities<TEntity>(this DbContext context)
        where TEntity : class
    {
        return context.ChangeTracker.Entries<TEntity>()
            .Select(e => e.Entity);
    }

    /// <summary>
    /// Checks if an entity is being tracked
    /// </summary>
    public static bool IsTracked<TEntity>(this DbContext context, TEntity entity)
        where TEntity : class
    {
        return context.Entry(entity).State != EntityState.Detached;
    }
}
