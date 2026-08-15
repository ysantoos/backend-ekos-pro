# Entity Framework Core - Migration Quick Reference

## Installation

```bash
# Install EF Core Tools globally (one time)
dotnet tool install --global dotnet-ef

# Update to latest version
dotnet tool update --global dotnet-ef

# Verify installation
dotnet ef --version
```

## Common Commands

### Create Migration
```bash
# Basic migration
dotnet ef migrations add InitialCreate -p Infrastructure.Entity -s backend-ekos-pro

# With output directory
dotnet ef migrations add AddCatalogBooks -p Infrastructure.Entity -s backend-ekos-pro -o Data/Migrations

# With context name (if multiple contexts)
dotnet ef migrations add AddUsers -p Infrastructure.Entity -s backend-ekos-pro --context ApplicationDbContext
```

### Apply Migrations
```bash
# Update database to latest migration
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro

# Update to specific migration
dotnet ef database update AddCatalogBooks -p Infrastructure.Entity -s backend-ekos-pro

# Revert all migrations
dotnet ef database update 0 -p Infrastructure.Entity -s backend-ekos-pro
```

### Remove Migration
```bash
# Remove last migration (not applied to database)
dotnet ef migrations remove -p Infrastructure.Entity -s backend-ekos-pro

# Force remove (use with caution)
dotnet ef migrations remove -p Infrastructure.Entity -s backend-ekos-pro --force
```

### List Migrations
```bash
# List all migrations
dotnet ef migrations list -p Infrastructure.Entity -s backend-ekos-pro

# Show connection string without connecting
dotnet ef migrations list -p Infrastructure.Entity -s backend-ekos-pro --no-connect
```

### Generate SQL Script
```bash
# Generate script for all migrations
dotnet ef migrations script -p Infrastructure.Entity -s backend-ekos-pro -o migration.sql

# Generate script from specific migration
dotnet ef migrations script AddCatalogBooks -p Infrastructure.Entity -s backend-ekos-pro -o update.sql

# Generate script between two migrations
dotnet ef migrations script AddUsers AddOrders -p Infrastructure.Entity -s backend-ekos-pro -o update.sql

# Idempotent script (can be run multiple times)
dotnet ef migrations script -p Infrastructure.Entity -s backend-ekos-pro -o migration.sql --idempotent
```

### Database Operations
```bash
# Drop database
dotnet ef database drop -p Infrastructure.Entity -s backend-ekos-pro

# Drop database without confirmation
dotnet ef database drop -p Infrastructure.Entity -s backend-ekos-pro --force

# Get database info
dotnet ef dbcontext info -p Infrastructure.Entity -s backend-ekos-pro

# Scaffold from existing database (reverse engineering)
dotnet ef dbcontext scaffold "Server=(localdb)\\mssqllocaldb;Database=ExistingDb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -p Infrastructure.Entity -s backend-ekos-pro -o Entities -c ExistingDbContext
```

## Workflow Examples

### Starting Fresh
```bash
# 1. Create initial migration
dotnet ef migrations add InitialCreate -p Infrastructure.Entity -s backend-ekos-pro

# 2. Create database and apply migration
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro
```

### Adding New Entity
```bash
# 1. Create entity class in Domain.Service/Entities
# 2. Create configuration in Infrastructure.Entity/Configurations
# 3. Add DbSet to ApplicationDbContext
# 4. Create migration
dotnet ef migrations add AddCatalogBook -p Infrastructure.Entity -s backend-ekos-pro

# 5. Review generated migration
# 6. Apply migration
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro
```

### Modifying Existing Entity
```bash
# 1. Modify entity or configuration
# 2. Create migration
dotnet ef migrations add UpdateCatalogBook -p Infrastructure.Entity -s backend-ekos-pro

# 3. Review migration to ensure changes are correct
# 4. Apply migration
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro
```

### Reverting Changes
```bash
# 1. Revert to previous migration
dotnet ef database update PreviousMigrationName -p Infrastructure.Entity -s backend-ekos-pro

# 2. Remove the unwanted migration
dotnet ef migrations remove -p Infrastructure.Entity -s backend-ekos-pro
```

### Production Deployment
```bash
# 1. Generate idempotent SQL script
dotnet ef migrations script -p Infrastructure.Entity -s backend-ekos-pro -o production-migration.sql --idempotent

# 2. Review the script
# 3. Run script on production database using SQL Server Management Studio or Azure portal
```

## PowerShell Scripts

### Create and Apply Migration
```powershell
# Save as: create-migration.ps1
param([string]$MigrationName)

if ([string]::IsNullOrEmpty($MigrationName)) {
	Write-Host "Usage: .\create-migration.ps1 <MigrationName>"
	exit 1
}

Write-Host "Creating migration: $MigrationName" -ForegroundColor Green
dotnet ef migrations add $MigrationName -p Infrastructure.Entity -s backend-ekos-pro

if ($LASTEXITCODE -eq 0) {
	Write-Host "Migration created successfully!" -ForegroundColor Green
	Write-Host "Apply migration? (Y/N)" -ForegroundColor Yellow
	$response = Read-Host
	if ($response -eq 'Y' -or $response -eq 'y') {
		dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro
		Write-Host "Migration applied!" -ForegroundColor Green
	}
}
```

### Reset Database
```powershell
# Save as: reset-database.ps1
Write-Host "WARNING: This will drop the database and recreate it!" -ForegroundColor Red
Write-Host "Are you sure? (type 'yes' to confirm)" -ForegroundColor Yellow
$response = Read-Host

if ($response -eq 'yes') {
	Write-Host "Dropping database..." -ForegroundColor Yellow
	dotnet ef database drop -p Infrastructure.Entity -s backend-ekos-pro --force

	Write-Host "Applying migrations..." -ForegroundColor Yellow
	dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro

	Write-Host "Database reset complete!" -ForegroundColor Green
} else {
	Write-Host "Operation cancelled" -ForegroundColor Yellow
}
```

## Troubleshooting

### Build Failed
```bash
# Clean and rebuild
dotnet clean
dotnet build

# Then try migration again
dotnet ef migrations add MigrationName -p Infrastructure.Entity -s backend-ekos-pro
```

### Migration Already Applied
```bash
# List applied migrations
dotnet ef migrations list -p Infrastructure.Entity -s backend-ekos-pro

# Revert to previous
dotnet ef database update PreviousMigrationName -p Infrastructure.Entity -s backend-ekos-pro

# Remove the migration
dotnet ef migrations remove -p Infrastructure.Entity -s backend-ekos-pro
```

### Connection Error
```bash
# Check connection string in appsettings.json
# Verify SQL Server is running
# Test with:
dotnet ef dbcontext info -p Infrastructure.Entity -s backend-ekos-pro
```

### Startup Project Error
```bash
# Always specify both -p (project) and -s (startup) parameters
dotnet ef migrations add Name -p Infrastructure.Entity -s backend-ekos-pro
```

## Environment-Specific Commands

### Development
```bash
# Use development settings
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro --environment Development
```

### Staging
```bash
# Use staging settings
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro --environment Staging
```

### Production
```bash
# Generate script only (don't apply directly)
dotnet ef migrations script -p Infrastructure.Entity -s backend-ekos-pro --environment Production -o prod-migration.sql --idempotent
```

## Tips

1. **Always review migrations** before applying them
2. **Use meaningful names** for migrations (e.g., `AddUserTable`, `UpdateBookPriceColumn`)
3. **Test in development** before applying to production
4. **Keep migrations small** - one logical change per migration
5. **Never modify** applied migrations
6. **Use --idempotent scripts** for production deployments
7. **Backup database** before applying migrations in production
8. **Version control** all migration files

## Quick Reference

```bash
# Most used commands (save these!)

# Create migration
dotnet ef migrations add <Name> -p Infrastructure.Entity -s backend-ekos-pro

# Apply migration
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro

# Remove last migration
dotnet ef migrations remove -p Infrastructure.Entity -s backend-ekos-pro

# List migrations
dotnet ef migrations list -p Infrastructure.Entity -s backend-ekos-pro

# Generate SQL script
dotnet ef migrations script -p Infrastructure.Entity -s backend-ekos-pro -o script.sql --idempotent

# Drop database
dotnet ef database drop -p Infrastructure.Entity -s backend-ekos-pro --force
```
