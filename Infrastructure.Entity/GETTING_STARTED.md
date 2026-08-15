# Getting Started with Entity Framework Code First

## Infrastructure Setup Complete! ✅

La infraestructura de Entity Framework Code First ya está configurada y lista para usar.

## Estructura Actual

```
Infrastructure.Entity/
├── Configurations/           ✅ Configuraciones de entidades
│   ├── BaseEntityConfiguration.cs (base para todas las configuraciones)
│   └── Examples/            (carpeta con ejemplos de referencia)
├── Constants/               ✅ Constantes de base de datos
│   └── DatabaseConstants.cs
├── Data/                    ✅ DbContext principal
│   └── ApplicationDbContext.cs
├── Extensions/              ✅ Métodos de extensión
│   ├── DbContextExtensions.cs
│   └── ModelBuilderExtensions.cs
├── Helpers/                 ✅ Helpers para migraciones
│   └── DatabaseHelper.cs
├── DependencyInjection.cs   ✅ Configuración de DI
├── README_CODEFIRST.md      📖 Documentación completa
└── MIGRATION_COMMANDS.md    📖 Referencias de comandos
```

## Características Implementadas

### ✅ ApplicationDbContext
- Actualización automática de campos de auditoría (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
- Descubrimiento automático de configuraciones (IEntityTypeConfiguration)
- Convenciones globales aplicadas (DateTime precision, naming, cascade deletes)
- Debug logging habilitado en desarrollo
- Protección de campos de auditoría contra modificación

### ✅ BaseEntityConfiguration<TEntity>
- Configuración base reutilizable para todas las entidades
- Configuración automática de campos de auditoría
- Índices pre-configurados
- Listo para heredar en configuraciones específicas

### ✅ DatabaseConstants
- Longitudes estándar para campos comunes
- Precisión de decimales configurada
- Constantes centralizadas para toda la solución

### ✅ DependencyInjection
- SQL Server configurado
- Retry on failure habilitado (3 intentos)
- Command timeout (60 segundos)
- QueryTrackingBehavior.NoTracking por defecto

### ✅ Extensions
- DbContextExtensions: utilidades para tracking de entidades
- ModelBuilderExtensions: convenciones globales y seed data

### ✅ DatabaseHelper
- Métodos para aplicar migraciones
- Verificar existencia de base de datos
- Obtener migraciones pendientes/aplicadas
- Eliminar base de datos (solo desarrollo)

## Próximos Pasos

### 1️⃣ Crear tu Primera Entidad

Crea tu entidad en `Domain.Service/Entities/`:

```csharp
// Domain.Service/Entities/Product.cs
namespace Domain.Service.Entities;

public class Product : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public string Code { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int Stock { get; set; }
	public string? Description { get; set; }
}
```

### 2️⃣ Crear la Configuración

Crea la configuración en `Infrastructure.Entity/Configurations/`:

```csharp
// Infrastructure.Entity/Configurations/ProductConfiguration.cs
using Domain.Service.Entities;
using Infrastructure.Entity.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Entity.Configurations;

public class ProductConfiguration : BaseEntityConfiguration<Product>
{
	public override void Configure(EntityTypeBuilder<Product> builder)
	{
		base.Configure(builder);

		builder.ToTable("Products");

		builder.Property(p => p.Name)
			.IsRequired()
			.HasMaxLength(DatabaseConstants.NameLength);

		builder.Property(p => p.Code)
			.IsRequired()
			.HasMaxLength(DatabaseConstants.CodeLength);

		builder.Property(p => p.Price)
			.IsRequired()
			.HasPrecision(DatabaseConstants.DecimalPrecision, DatabaseConstants.DecimalScale);

		builder.Property(p => p.Stock)
			.IsRequired()
			.HasDefaultValue(0);

		builder.Property(p => p.Description)
			.HasMaxLength(DatabaseConstants.DescriptionLength);

		builder.HasIndex(p => p.Code)
			.IsUnique()
			.HasDatabaseName("UX_Product_Code");

		builder.HasIndex(p => p.Name)
			.HasDatabaseName("IX_Product_Name");
	}
}
```

### 3️⃣ Agregar DbSet al ApplicationDbContext

```csharp
// Infrastructure.Entity/Data/ApplicationDbContext.cs
public DbSet<Product> Products => Set<Product>();
```

### 4️⃣ Crear la Primera Migración

```powershell
# Crear migración inicial
dotnet ef migrations add InitialCreate -p Infrastructure.Entity -s backend-ekos-pro

# Aplicar migración (crea la base de datos EkosProDb)
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro
```

### 5️⃣ Verificar la Base de Datos

La base de datos `EkosProDb` será creada automáticamente en:
- SQL Server LocalDB: `(localdb)\mssqllocaldb`
- Nombre: `EkosProDb` (o `EkosProDb_Dev` según tu connection string)

Puedes verificarla con:
- SQL Server Management Studio (SSMS)
- Azure Data Studio
- Visual Studio SQL Server Object Explorer

## Connection String

Verifica tu connection string en `backend-ekos-pro/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EkosProDb_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

## Comandos Útiles

```powershell
# Crear migración
dotnet ef migrations add <NombreMigracion> -p Infrastructure.Entity -s backend-ekos-pro

# Aplicar migraciones
dotnet ef database update -p Infrastructure.Entity -s backend-ekos-pro

# Listar migraciones
dotnet ef migrations list -p Infrastructure.Entity -s backend-ekos-pro

# Generar script SQL
dotnet ef migrations script -p Infrastructure.Entity -s backend-ekos-pro -o script.sql

# Eliminar última migración (no aplicada)
dotnet ef migrations remove -p Infrastructure.Entity -s backend-ekos-pro

# Eliminar base de datos
dotnet ef database drop -p Infrastructure.Entity -s backend-ekos-pro --force
```

## Ejemplos de Referencia

Revisa los archivos en `Infrastructure.Entity/Configurations/Examples/`:
- `CatalogBookConfigurationExample.cs`: Ejemplo de configuración básica
- `RelationshipConfigurationExamples.cs`: Ejemplos de relaciones (1-N, N-M, 1-1)

**Nota**: Estos archivos son solo de referencia y no afectan tu base de datos.

## Documentación Completa

📖 Para más detalles, consulta:
- `Infrastructure.Entity/README_CODEFIRST.md`: Documentación completa
- `Infrastructure.Entity/MIGRATION_COMMANDS.md`: Referencia de comandos

## Tips Importantes

1. ✅ **Siempre hereda de BaseEntity** para todas tus entidades
2. ✅ **Siempre hereda de BaseEntityConfiguration<T>** para todas tus configuraciones
3. ✅ **Usa DatabaseConstants** para longitudes y precisiones
4. ✅ **Revisa la migración generada** antes de aplicarla
5. ✅ **Prueba en desarrollo** antes de producción
6. ✅ **Nombres en inglés** para todo el código (según copilot-instructions.md)
7. ✅ **Un cambio lógico por migración** (no múltiples cambios no relacionados)

## Estado Actual

- ❌ Base de datos **NO creada** aún
- ❌ Migraciones **NO creadas** aún
- ✅ Infraestructura **LISTA** para usar
- ✅ Configuración **CORRECTA** y compilando
- ✅ Documentación **COMPLETA**

**¡Estás listo para crear tu primera entidad y migración!** 🚀

---

**Siguiente acción sugerida**: Crea tu primera entidad de negocio y genera la migración inicial.
