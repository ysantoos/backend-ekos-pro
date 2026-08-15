using Domain.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Entity.Configurations;

/// <summary>
/// Base configuration class for entities inheriting from BaseEntity.
/// Provides common configuration for audit fields.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Primary Key
        builder.HasKey(e => e.Id);

        // Id configuration
        builder.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        // Audit fields configuration
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(256)
            .IsRequired(false);

        // Index for performance on common queries
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedAt");
    }
}
