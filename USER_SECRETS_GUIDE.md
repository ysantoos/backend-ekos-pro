# 🔒 Configuration Security - User Secrets

## ⚠️ IMPORTANT: Credenciales NO deben estar en appsettings.json

Este proyecto usa **User Secrets** para almacenar información sensible de forma segura **solo en tu máquina local**.

---

## 📋 ¿Qué son User Secrets?

User Secrets son un mecanismo de .NET para almacenar datos sensibles (como connection strings, API keys, passwords) **fuera del código fuente** y **fuera del control de versiones** (Git).

### Beneficios:
- ✅ **Seguro**: Las credenciales NO se suben a GitHub
- ✅ **Local**: Solo en tu máquina
- ✅ **Fácil**: Se integra automáticamente con la configuración de .NET
- ✅ **Por desarrollador**: Cada desarrollador tiene sus propias credenciales

---

## 🚀 Setup Inicial (Ya Configurado)

El proyecto ya está configurado con User Secrets. El `UserSecretsId` está en el archivo `.csproj`:

```xml
<UserSecretsId>6064e52e-ed73-4119-b374-fb3478f0385f</UserSecretsId>
```

---

## 🔧 Cómo Configurar Tus Credenciales

### Opción 1: Usando la CLI (Recomendado)

```powershell
# Navegar al directorio del proyecto API
cd backend-ekos-pro

# Agregar el connection string de Azure SQL
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=ekospro-sql.database.windows.net;Database=EkosProDb;User Id=TU_USUARIO;Password=TU_PASSWORD;Encrypt=true;TrustServerCertificate=false;MultipleActiveResultSets=true"
```

### Opción 2: Usando Visual Studio

1. **Click derecho** en el proyecto `backend-ekos-pro` en el Solution Explorer
2. Seleccionar **"Manage User Secrets"**
3. Agregar el JSON:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=ekospro-sql.database.windows.net;Database=EkosProDb;User Id=TU_USUARIO;Password=TU_PASSWORD;Encrypt=true;TrustServerCertificate=false;MultipleActiveResultSets=true"
  }
}
```

4. Guardar y cerrar

---

## 📂 Ubicación de los Secrets

Los User Secrets se almacenan en:

**Windows:**
```
%APPDATA%\Microsoft\UserSecrets\6064e52e-ed73-4119-b374-fb3478f0385f\secrets.json
```

**macOS/Linux:**
```
~/.microsoft/usersecrets/6064e52e-ed73-4119-b374-fb3478f0385f/secrets.json
```

---

## 🔍 Ver Tus Secrets Configurados

```powershell
# Listar todos los secrets
dotnet user-secrets list -p backend-ekos-pro

# Ver un secret específico
dotnet user-secrets get "ConnectionStrings:DefaultConnection" -p backend-ekos-pro
```

---

## 🗑️ Eliminar Secrets

```powershell
# Eliminar un secret específico
dotnet user-secrets remove "ConnectionStrings:DefaultConnection" -p backend-ekos-pro

# Eliminar TODOS los secrets
dotnet user-secrets clear -p backend-ekos-pro
```

---

## 🎯 ¿Cómo Funciona?

1. **Desarrollo Local**: .NET lee automáticamente los User Secrets cuando ejecutas la app en modo `Development`
2. **appsettings.json**: Contiene valores por defecto (sin credenciales)
3. **appsettings.Development.json**: Configuraciones de desarrollo (sin credenciales)
4. **User Secrets**: Sobrescribe los valores anteriores con tus credenciales privadas

### Orden de Prioridad (de menor a mayor):

```
appsettings.json 
  → appsettings.{Environment}.json 
	→ User Secrets 
	  → Environment Variables 
		→ Command Line Args
```

---

## 📝 Ejemplo de Configuración Completa

### appsettings.json (Público - En Git)
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=(local);Database=EkosProDb;Trusted_Connection=true"
  }
}
```

### secrets.json (Privado - NO en Git)
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=ekospro-sql.database.windows.net;Database=EkosProDb;User Id=administrador;Password=oPR0qz78;Encrypt=true"
  }
}
```

**Resultado Final**: La app usa el connection string de `secrets.json` en desarrollo.

---

## 🌐 Configuración para Producción

User Secrets **SOLO funcionan en desarrollo**. Para producción:

### Azure App Service
Usar **Application Settings** en Azure Portal:
1. Azure Portal → App Service → Configuration → Application settings
2. Agregar: `ConnectionStrings__DefaultConnection` = `tu-connection-string`

### Docker / Kubernetes
Usar **Environment Variables** o **Secrets Management Systems**:
- Docker: `.env` files o `docker-compose.yml` environment
- Kubernetes: Secrets y ConfigMaps
- Azure Key Vault: Para secretos seguros en producción

---

## ✅ Checklist de Seguridad

- [x] User Secrets configurado
- [x] `.gitignore` actualizado para ignorar `appsettings.*.json`
- [x] `appsettings.json` sin credenciales
- [x] `appsettings.Development.json` sin credenciales
- [ ] Credenciales removidas del historial de Git (ver siguiente sección)
- [ ] Password de Azure SQL cambiado (recomendado)

---

## 🚨 ¿Qué Hacer Si Ya Subiste Credenciales?

### 1. Cambiar Password Inmediatamente

Conectarse a Azure Portal y cambiar el password del servidor SQL:
```
Azure Portal → SQL Server → Settings → Reset Password
```

### 2. Limpiar Historial de Git

Ver `GIT_CLEANUP.md` para instrucciones de cómo limpiar el historial.

---

## 📚 Documentación Oficial

- [User Secrets en ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Configuration in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/)

---

## 🆘 Troubleshooting

### Error: "Connection string not found"

**Solución**: Verifica que los User Secrets están configurados:
```powershell
dotnet user-secrets list -p backend-ekos-pro
```

### Error: "Cannot connect to database"

**Solución**: Verifica tu IP está permitida en el Firewall de Azure SQL:
```
Azure Portal → SQL Server → Networking → Firewall rules
```

### User Secrets no se cargan

**Solución**: Asegúrate de estar en modo `Development`:
```json
// launchSettings.json
"environmentVariables": {
  "ASPNETCORE_ENVIRONMENT": "Development"
}
```

---

## 👥 Para Nuevos Desarrolladores

Cuando un nuevo desarrollador clone el repositorio:

1. **Pedir credenciales** al líder del equipo
2. **Configurar User Secrets**:
   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "CREDENCIALES_PROPORCIONADAS"
   ```
3. **Verificar**:
   ```powershell
   dotnet user-secrets list -p backend-ekos-pro
   ```
4. **Ejecutar el proyecto** - debería funcionar sin necesidad de modificar archivos

---

**Última actualización**: 2026-08-15  
**Proyecto**: backend-ekos-pro  
**UserSecretsId**: `6064e52e-ed73-4119-b374-fb3478f0385f`
