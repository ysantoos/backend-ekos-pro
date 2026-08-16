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

        // Soft-delete configuration
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Indicates whether the entity has been soft-deleted");

        builder.Property(e => e.DeletedAt)
            .IsRequired(false)
            .HasColumnType("datetime2")
            .HasComment("Timestamp when the entity was soft-deleted");

        builder.Property(e => e.DeletedBy)
            .HasMaxLength(256)
            .IsRequired(false)
            .HasComment("User who soft-deleted the entity");

        // Index for performance on common queries
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedAt");

        // Index to speed up queries filtering out deleted rows
        builder.HasIndex(e => e.IsDeleted)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_IsDeleted");
    }
}
