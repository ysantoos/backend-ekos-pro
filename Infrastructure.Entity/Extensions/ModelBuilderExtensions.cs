using Domain.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Entity.Extensions;

/// <summary>
/// Extension methods for ModelBuilder
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies global query filters to all entities inheriting from BaseEntity
    /// Example: Soft delete filter
    /// </summary>
    public static void ApplyBaseEntityConfiguration(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Configure precision for DateTime fields to avoid SQL Server precision issues
                var dateTimeProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

                foreach (var property in dateTimeProperties)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasColumnType("datetime2");
                }

                // Apply global query filter to exclude soft-deleted entities by default
                var method = typeof(ModelBuilderExtensions)
                    .GetMethod(nameof(ApplySoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(null, new object[] { modelBuilder });
            }
        }
    }

    private static void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    /// <summary>
    /// Applies naming conventions to all tables (snake_case, PascalCase, etc.)
    /// </summary>
    public static void ApplyNamingConventions(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Use PascalCase for table names (default)
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(tableName);
            }

            // Configure all string properties to use Unicode (nvarchar) by default
            var stringProperties = entity.ClrType.GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            foreach (var property in stringProperties)
            {
                var propertyBuilder = modelBuilder.Entity(entity.ClrType)
                    .Property(property.Name);

                // Only apply if no max length is already configured
                if (!property.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.MaxLengthAttribute), false).Any())
                {
                    propertyBuilder.HasMaxLength(500); // Default max length
                }
            }
        }
    }

    /// <summary>
    /// Applies cascade delete restrictions
    /// </summary>
    public static void ApplyCascadeRestrictions(this ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            // Change all cascade deletes to restrict
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    /// <summary>
    /// Seeds initial data for development/testing
    /// </summary>
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        // Add seed data here if needed
        // Example:
        // modelBuilder.Entity<Role>().HasData(
        //     new Role { Id = Guid.NewGuid(), Name = "Admin" }
        // );
    }
}
