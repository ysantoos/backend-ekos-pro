using Infrastructure.Entity.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Entities = Domain.Service.Entities;

namespace Infrastructure.Entity.Data;

/// <summary>
/// Main database context for the application
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }
    public DbSet<Entities.CatalogBook> CatalogBooks => Set<Entities.CatalogBook>();
    public DbSet<Entities.LoanHistoryEntry> LoanHistoryEntries => Set<Entities.LoanHistoryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply base entity configuration (DateTime precision, etc.)
        modelBuilder.ApplyBaseEntityConfiguration();

        // Apply naming conventions
        modelBuilder.ApplyNamingConventions();

        // Apply cascade delete restrictions
        modelBuilder.ApplyCascadeRestrictions();

        // Seed initial data (if needed)
        // modelBuilder.SeedData();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Enable sensitive data logging in development
        // This is useful for debugging but should be disabled in production
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
#endif
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Updates audit fields (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy) before saving
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Service.Entities.BaseEntity && 
                   (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Domain.Service.Entities.BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                // TODO: Set CreatedBy from current user context
                // entity.CreatedBy = _currentUserService.UserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                // TODO: Set UpdatedBy from current user context
                // entity.UpdatedBy = _currentUserService.UserId;

                // Prevent modification of created fields
                entry.Property(nameof(Domain.Service.Entities.BaseEntity.CreatedAt)).IsModified = false;
                entry.Property(nameof(Domain.Service.Entities.BaseEntity.CreatedBy)).IsModified = false;
            }
        }
    }
}
