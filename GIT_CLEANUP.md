# 🧹 Git History Cleanup - Remover Credenciales Expuestas

## ⚠️ SITUACIÓN CRÍTICA

Las credenciales de Azure SQL fueron subidas accidentalmente a GitHub en los commits anteriores en:
- `appsettings.json`
- `appsettings.Development.json`

**IMPORTANTE**: Aunque hayas eliminado las credenciales en el último commit, **siguen existiendo en el historial de Git** y son accesibles públicamente.

---

## 🚨 Pasos Urgentes (Hacer Ahora)

### 1. Cambiar Credenciales Inmediatamente

**⚠️ ESTO ES CRÍTICO - HAZLO PRIMERO**

Las credenciales expuestas son:
- **Server**: `ekospro-sql.database.windows.net`
- **Usuario**: `administrador`
- **Password**: `oPR0qz78`

#### Cambiar Password en Azure Portal:

1. Ve a [Azure Portal](https://portal.azure.com)
2. Navega a: **SQL Server** → `ekospro-sql`
3. Click en **"Settings"** → **"Reset password"**
4. Ingresa un **nuevo password seguro**
5. Guarda los cambios

#### Actualizar User Secrets con el Nuevo Password:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=ekospro-sql.database.windows.net;Database=EkosProDb;User Id=administrador;Password=NUEVO_PASSWORD_AQUI;Encrypt=true;TrustServerCertificate=false;MultipleActiveResultSets=true" -p backend-ekos-pro
```

---

## 🔧 Limpiar Historial de Git

### Opción 1: Force Push (Recomendado si trabajas solo)

Esta opción **reescribe el historial de Git** y elimina las credenciales de todos los commits.

⚠️ **ADVERTENCIA**: Esto afectará a todos los que hayan clonado el repositorio.

```powershell
# 1. Asegúrate de estar en la rama master
git checkout master

# 2. Crear un nuevo commit con los archivos limpios
git add .gitignore
git add backend-ekos-pro/appsettings.json
git add backend-ekos-pro/appsettings.Development.json
git add USER_SECRETS_GUIDE.md
git add GIT_CLEANUP.md
git commit -m "security: Remove sensitive credentials and implement User Secrets"

# 3. Ver el historial para identificar commits con credenciales
git log --oneline

# 4. Opción A: Reset hard al commit ANTES de agregar credenciales
# (Encuentra el hash del último commit SIN credenciales)
git reset --hard <COMMIT_HASH_SIN_CREDENCIALES>

# 5. Cherry-pick commits buenos si es necesario
git cherry-pick <HASH_DE_COMMIT_BUENO>

# 6. Force push (PELIGROSO - solo si estás seguro)
git push origin master --force
```

### Opción 2: BFG Repo-Cleaner (Más Seguro y Rápido)

BFG es una herramienta específica para limpiar historiales de Git.

#### Instalación:

**Windows (usando Chocolatey):**
```powershell
choco install bfg-repo-cleaner
```

**O descarga directa:**
1. Descarga desde: https://rtyley.github.io/bfg-repo-cleaner/
2. Requiere Java instalado

#### Uso:

```powershell
# 1. Hacer un backup del repositorio
cd C:\Users\yeiso\Documents\VIVO\Projects\.NET\
git clone --mirror https://github.com/ysantoos/backend-ekos-pro backend-ekos-pro-backup.git

# 2. Crear archivo con credenciales a remover
# Crear archivo: passwords.txt con el contenido:
# oPR0qz78
# ekospro-sql.database.windows.net
# administrador

# 3. Ejecutar BFG
cd backend-ekos-pro-backup.git
bfg --replace-text passwords.txt

# 4. Limpiar y comprimir
git reflog expire --expire=now --all && git gc --prune=now --aggressive

# 5. Force push
git push --force
```

### Opción 3: Git Filter-Branch (Manual pero Efectivo)

```powershell
# 1. Backup primero
git clone https://github.com/ysantoos/backend-ekos-pro backend-ekos-pro-backup

# 2. Filtrar historial para remover archivos sensibles
git filter-branch --force --index-filter \
  "git rm --cached --ignore-unmatch backend-ekos-pro/appsettings.json backend-ekos-pro/appsettings.Development.json" \
  --prune-empty --tag-name-filter cat -- --all

# 3. Force push
git push origin --force --all
git push origin --force --tags
```

### Opción 4: Empezar de Cero (Nuclear Option)

Si el repositorio es nuevo y no tiene mucha historia importante:

```powershell
# 1. Eliminar repositorio en GitHub
# Ve a GitHub → Settings → Delete this repository

# 2. Crear repositorio nuevo en GitHub

# 3. Reinicializar Git localmente
cd C:\Users\yeiso\Documents\VIVO\Projects\.NET\backend-ekos-pro
Remove-Item -Recurse -Force .git

git init
git add .
git commit -m "Initial commit with secure configuration"
git branch -M master
git remote add origin https://github.com/ysantoos/backend-ekos-pro
git push -u origin master --force
```

---

## 📋 Checklist Post-Limpieza

Después de limpiar el historial:

- [ ] Password de Azure SQL cambiado
- [ ] User Secrets configurados localmente
- [ ] `.gitignore` actualizado
- [ ] `appsettings.json` sin credenciales
- [ ] `appsettings.Development.json` sin credenciales
- [ ] Historial de Git limpio (verificar en GitHub)
- [ ] Nuevo commit pushed con archivos seguros
- [ ] Equipo notificado del cambio (si aplica)

---

## 🔍 Verificar que el Historial está Limpio

```powershell
# Buscar "password" en todo el historial
git log -p -S "oPR0qz78"

# Buscar en todos los archivos del historial
git log -p --all -- backend-ekos-pro/appsettings.json

# Si no devuelve nada o no muestra el password, está limpio ✅
```

---

## 🛡️ Prevención Futura

### Git Hooks (Pre-commit)

Crear archivo `.git/hooks/pre-commit`:

```bash
#!/bin/sh

# Buscar patrones sensibles antes de commit
if git diff --cached | grep -E "Password=|pwd=|secret="; then
	echo "❌ ERROR: Posible credencial detectada en el commit"
	echo "Revisa los archivos staged antes de commitear"
	exit 1
fi

exit 0
```

```powershell
# Hacerlo ejecutable (en Git Bash)
chmod +x .git/hooks/pre-commit
```

### GitHub Secret Scanning

GitHub automáticamente escanea credenciales expuestas. Revisa:
- GitHub Repository → Security → Secret scanning alerts

---

## 📚 Recursos Adicionales

- [Removing sensitive data from GitHub](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/removing-sensitive-data-from-a-repository)
- [BFG Repo-Cleaner](https://rtyley.github.io/bfg-repo-cleaner/)
- [Git Filter-Branch](https://git-scm.com/docs/git-filter-branch)
- [GitHub Secret Scanning](https://docs.github.com/en/code-security/secret-scanning)

---

## 🆘 Si Necesitas Ayuda

Si trabajas en equipo:
1. **Notifica al equipo** antes de hacer force push
2. **Coordina** para que todos hagan `git pull --force` después
3. **Documenta** los cambios en un README o Wiki

---

**Creado**: 2026-08-15  
**Propósito**: Guía de emergencia para limpiar credenciales expuestas  
**Estado**: ⚠️ ACCIÓN REQUERIDA
