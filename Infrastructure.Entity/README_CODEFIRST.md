# Entity Framework Code First - Infrastructure Setup

## Overview

This document describes the Entity Framework Code First setup for the Ekos Pro application.

## Structure

```
Infrastructure.Entity/
├── Configurations/          # Entity configurations using IEntityTypeConfiguration
│   └── BaseEntityConfiguration.cs
├── Constants/              # Database constants (lengths, precision, etc.)
│   └── DatabaseConstants.cs
├── Data/                   # DbContext
│   └── ApplicationDbContext.cs
├── Extensions/             # Extension methods
│   ├── DbContextExtensions.cs
│   └── ModelBuilderExtensions.cs
├── Helpers/               # Helper classes
│   └── DatabaseHelper.cs
└── DependencyInjection.cs
```

## Key Features

### 1. BaseEntityConfiguration
All entity configurations should inherit from `BaseEntityConfiguration<TEntity>` which provides:
- Primary key configuration
- Audit fields configuration (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
- Common indexes

### 2. ApplicationDbContext
The main DbContext with:
- Automatic audit field updates
- Configuration discovery from assembly
- Cascade delete restrictions
- DateTime precision handling
- Development-only sensitive data logging

### 3. Database Constants
Centralized constants for:
- String lengths
- Decimal precision
- Schema names
- Common field sizes

### 4. Extensions
- `DbContextExtensions`: Helper methods for entity tracking
- `ModelBuilderExtensions`: Global configurations and conventions

### 5. DatabaseHelper
Utility methods for:
- Applying migrations
- Checking database existence
- Getting pending/applied migrations
- Database deletion (development only)

## Creating a New Entity

### Step 1: Create the Entity in Domain.Service

```csharp
// Domain.Service/Entities/CatalogBook.cs
using Domain.Service.Entities;

namespace Domain.Service.Entities;

public class CatalogBook : BaseEntity
{
	public string Title { get; set; } = string.Empty;
	public string Author { get; set; } = string.Empty;
	public string ISBN { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int StockQuantity { get; set; }
	public string? Description { get; set; }
}
```

### Step 2: Create the Configuration in Infrastructure.Entity

```csharp
// Infrastructure.Entity/Configurations/CatalogBookConfiguration.cs
using Domain.Service.Entities;
using Infrastructure.Entity.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Entity.Configurations;

public class CatalogBookConfiguration : BaseEntityConfiguration<CatalogBook>
{
	public override void Configure(EntityTypeBuilder<CatalogBook> builder)
	{
		// Call base configuration first
		base.Configure(builder);

		// Table name
		builder.ToTable("CatalogBooks");

		// Properties
		builder.Property(b => b.Title)
			.IsRequired()
			.HasMaxLength(DatabaseConstants.NameLength);

		builder.Property(b => b.Author)
			.IsRequired()
			.HasMaxLength(DatabaseConstants.NameLength);

		builder.Property(b => b.ISBN)
			.IsRequired()
			.HasMaxLength(20);

		builder.Property(b => b.Price)
			.IsRequired()
			.HasPrecision(DatabaseConstants.DecimalPrecision, DatabaseConstants.DecimalScale);

		builder.Property(b => b.StockQuantity)
			.IsRequired()
			.HasDefaultValue(0);

		builder.Property(b => b.Description)
			.HasMaxLength(DatabaseConstants.DescriptionLength);

		// Indexes
		builder.HasIndex(b => b.ISBN)
			.IsUnique()
			.HasDatabaseName("IX_CatalogBook_ISBN");

		builder.HasIndex(b => b.Title)
			.HasDatabaseName("IX_CatalogBook_Title");

		// Example of composite index
		// builder.HasIndex(b => new { b.Author, b.Title })
		//     .HasDatabaseName("IX_CatalogBook_Author_Title");
	}
}
```

### Step 3: Add DbSet to ApplicationDbContext

```csharp
// Infrastructure.Entity/Data/ApplicationDbContext.cs
public DbSet<CatalogBook> CatalogBooks => Set<CatalogBook>();
```

### Step 4: Create and Apply Migration

```bash
# Create migration
dotnet ef migrations add AddCatalogBook -p Infrastructure.Entity -s backend-ekos-pro

# Apply migration
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro
```

## Migration Commands

### Prerequisites
```bash
# Install EF Core tools globally (one time only)
dotnet tool install --global dotnet-ef

# Update to latest version
dotnet tool update --global dotnet-ef
```

### Common Commands

```bash
# Create a new migration
dotnet ef migrations add <MigrationName> -p Infrastructure.Entity -s backend-ekos-pro

# Apply migrations to database
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro

# Revert last migration
dotnet ef database update <PreviousMigrationName> -p Infrastructure.Entity -s backend-ekos-pro

# Remove last migration (if not applied)
dotnet ef migrations remove -p Infrastructure.Entity -s backend-ekos-pro

# List all migrations
dotnet ef migrations list -p Infrastructure.Entity -s backend-ekos-pro

# Generate SQL script from migrations
dotnet ef migrations script -p Infrastructure.Entity -s backend-ekos-pro -o migration.sql

# Generate SQL script for specific migration range
dotnet ef migrations script <FromMigration> <ToMigration> -p Infrastructure.Entity -s backend-ekos-pro -o migration.sql

# Drop database (development only)
dotnet ef database drop -p Infrastructure.Entity -s backend-ekos-pro

# View DbContext info
dotnet ef dbcontext info -p Infrastructure.Entity -s backend-ekos-pro
```

## Relationship Examples

### One-to-Many

```csharp
// In Author entity
public ICollection<Book> Books { get; set; } = new List<Book>();

// In Book entity
public Guid AuthorId { get; set; }
public Author Author { get; set; } = null!;

// In BookConfiguration
builder.HasOne(b => b.Author)
	.WithMany(a => a.Books)
	.HasForeignKey(b => b.AuthorId)
	.OnDelete(DeleteBehavior.Restrict);
```

### Many-to-Many

```csharp
// In Book entity
public ICollection<Category> Categories { get; set; } = new List<Category>();

// In Category entity
public ICollection<Book> Books { get; set; } = new List<Book>();

// In BookConfiguration
builder.HasMany(b => b.Categories)
	.WithMany(c => c.Books)
	.UsingEntity<Dictionary<string, object>>(
		"BookCategory",
		j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
		j => j.HasOne<Book>().WithMany().HasForeignKey("BookId"));
```

### One-to-One

```csharp
// In User entity
public UserProfile Profile { get; set; } = null!;

// In UserProfile entity
public Guid UserId { get; set; }
public User User { get; set; } = null!;

// In UserProfileConfiguration
builder.HasOne(p => p.User)
	.WithOne(u => u.Profile)
	.HasForeignKey<UserProfile>(p => p.UserId)
	.OnDelete(DeleteBehavior.Cascade);
```

## Best Practices

### 1. Use Fluent API Over Data Annotations
❌ Don't:
```csharp
[Required]
[MaxLength(100)]
public string Title { get; set; }
```

✅ Do:
```csharp
builder.Property(b => b.Title)
	.IsRequired()
	.HasMaxLength(100);
```

### 2. Always Use IEntityTypeConfiguration
❌ Don't configure in OnModelCreating directly

✅ Do create separate configuration classes

### 3. Use Constants for Field Lengths
```csharp
builder.Property(b => b.Title)
	.HasMaxLength(DatabaseConstants.NameLength);
```

### 4. Add Meaningful Indexes
```csharp
// Single column index
builder.HasIndex(b => b.Email)
	.IsUnique();

// Composite index
builder.HasIndex(b => new { b.LastName, b.FirstName });
```

### 5. Configure Delete Behavior
```csharp
builder.HasOne(b => b.Author)
	.WithMany(a => a.Books)
	.HasForeignKey(b => b.AuthorId)
	.OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
```

### 6. Use Value Converters for Enums
```csharp
builder.Property(b => b.Status)
	.HasConversion<string>(); // Store as string instead of int
```

## Testing Migrations

### 1. Create Test Migration
```bash
dotnet ef migrations add TestMigration -p Infrastructure.Entity -s backend-ekos-pro
```

### 2. Review Generated Migration
Check the file in `Infrastructure.Entity/Migrations/`

### 3. Apply to Development Database
```bash
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro
```

### 4. Verify in SQL Server
Check tables, columns, indexes, and constraints

### 5. Rollback if Needed
```bash
dotnet ef migrations remove -p Infrastructure.Entity -s backend-ekos-pro
```

## Connection Strings

### Development (appsettings.Development.json)
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EkosProDb_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### Production (appsettings.json)
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EkosProDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### Azure SQL (Production)
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=EkosProDb;Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

## Troubleshooting

### Migration already applied
```bash
# List migrations
dotnet ef migrations list -p Infrastructure.Entity -s backend-ekos-pro

# Remove from history
dotnet ef database update <PreviousMigration> -p Infrastructure.Entity -s backend-ekos-pro

# Remove migration file
dotnet ef migrations remove -p Infrastructure.Entity -s backend-ekos-pro
```

### Build failed before migration
```bash
# Build the solution first
dotnet build

# Then create migration
dotnet ef migrations add <Name> -p Infrastructure.Entity -s backend-ekos-pro
```

### Connection string not found
Verify connection string in appsettings.json and ensure the startup project is correct.

## Next Steps

1. Create your first entity in Domain.Service
2. Create its configuration in Infrastructure.Entity
3. Add DbSet to ApplicationDbContext
4. Create and apply migration
5. Test with a simple CRUD operation

---

**Remember**: Always follow the architecture guidelines in `copilot-instructions.md`
