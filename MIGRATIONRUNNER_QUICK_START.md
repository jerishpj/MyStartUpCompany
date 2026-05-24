# 🚀 MigrationRunner - Quick Start Guide

**Status**: ✅ FIXED  
**Files Created**: 3 appsettings configuration files  
**Ready to Use**: YES

---

## 🔧 What Was Fixed

**Problem**: 
```
Error: The configuration file 'appsettings.json' was not found and is not optional.
```

**Solution**: Created three configuration files:
- ✅ `src/MigrationRunner/appsettings.json` - Base configuration
- ✅ `src/MigrationRunner/appsettings.Development.json` - Local development
- ✅ `src/MigrationRunner/appsettings.Production.json` - Azure/Production

---

## ⚡ Quick Start (3 Steps)

### **Step 1: Open PowerShell**

```powershell
# Navigate to project root
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
```

### **Step 2: Run Migrations Locally**

```powershell
# Run with default Development environment
dotnet run --project src/MigrationRunner

# OR with explicit environment
dotnet run --project src/MigrationRunner -- --environment Development
```

### **Step 3: See the Result**

```
Output should show:
========================================
MyStartUpCompany Database Migration Tool
========================================
Starting database migrations...
Migration 1: ...
Migration 2: ...
Migration completed successfully!
```

---

## 🏠 How It Works Locally

```
Your Command:
  dotnet run --project src/MigrationRunner
		   ↓
Program detects environment: Development
		   ↓
Loads configuration from:
  1. appsettings.json (base)
  2. appsettings.Development.json (overrides)
		   ↓
Gets connection string:
  Server=(localdb)\mssqllocaldb;
  Database=MyStartUpCompanyDb;
		   ↓
Connects to LOCAL SQL Server LocalDB
		   ↓
Applies all pending database migrations
		   ↓
Completes successfully ✅
```

---

## ☁️ How It Works in Azure

```
Kubernetes runs:
  Job: migration-runner
		   ↓
Environment variable:
  ASPNETCORE_ENVIRONMENT=Production
		   ↓
Loads configuration from:
  1. appsettings.json (base)
  2. appsettings.Production.json (overrides)
		   ↓
Gets connection string from KUBERNETES SECRET:
  Server=tcp:myserver.database.windows.net;
  Database=MyStartUpCompanyDb;
  User ID=sqladmin;
  Password=***;
		   ↓
Connects to AZURE SQL Server
		   ↓
Applies all pending database migrations
		   ↓
Job completes and terminates ✅
```

---

## 📋 Configuration Files

### **appsettings.json** (Base Config)
```json
{
  "Logging": { "LogLevel": { "Default": "Information" } },
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyStartUpCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
  }
}
```
**Use**: Fallback for all environments

### **appsettings.Development.json** (Local Dev)
```json
{
  "Logging": { "LogLevel": { "Default": "Debug" } },
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyStartUpCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
  }
}
```
**Use**: When running locally (ASPNETCORE_ENVIRONMENT=Development)

### **appsettings.Production.json** (Azure)
```json
{
  "Logging": { "LogLevel": { "Default": "Information" } },
  "ConnectionStrings": {
	"DefaultConnection": "Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=YOUR_DATABASE;User ID=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;..."
  }
}
```
**Use**: When running in Kubernetes/Azure (ASPNETCORE_ENVIRONMENT=Production)

---

## 🔑 Key Points

### **Local Development**
- ✅ Runs from project root directory
- ✅ Uses LocalDB (SQL Server Developer Edition on Windows)
- ✅ Debug logging for development
- ✅ Quick iteration

```powershell
# How to run:
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet run --project src/MigrationRunner
```

---

### **Azure Deployment**
- ✅ Runs as Kubernetes Job
- ✅ Uses Azure SQL Server connection
- ✅ Connection string comes from Kubernetes Secret
- ✅ Information-level logging
- ✅ Job completes and terminates

```yaml
# How it runs in k8s:
spec:
  containers:
  - name: migration-runner
	env:
	- name: ASPNETCORE_ENVIRONMENT
	  value: Production
	- name: ConnectionStrings__DefaultConnection
	  valueFrom:
		secretKeyRef:
		  name: app-secrets
		  key: database-connection-string
```

---

## 🧪 Test Scenarios

### **Scenario 1: Run Migrations Locally**
```powershell
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet run --project src/MigrationRunner

# Expected: Migrations apply successfully to LocalDB
```

### **Scenario 2: Run Against Docker SQL Server**
```powershell
# First, start SQL Server in Docker:
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" `
  -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# Then run migration with custom connection string:
dotnet run --project src/MigrationRunner -- `
  --connection-string "Server=localhost,1433;Database=MyStartUpCompanyDb;User Id=sa;Password=YourPassword123!;"

# Expected: Connects to Docker SQL Server and applies migrations
```

### **Scenario 3: Test with Different Environment**
```powershell
# Set environment and run:
$env:ASPNETCORE_ENVIRONMENT = "Production"
dotnet run --project src/MigrationRunner

# Expected: Loads appsettings.Production.json
```

### **Scenario 4: Run in Docker Container**
```powershell
# Build image:
docker build -f src/MigrationRunner/Dockerfile -t migration:latest .

# Run migration:
docker run -e "ASPNETCORE_ENVIRONMENT=Development" `
  -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433;..." `
  migration:latest

# Expected: Migration runs inside container
```

---

## 🎯 Common Commands

```powershell
# Run migration locally (default Development)
dotnet run --project src/MigrationRunner

# Run migration with explicit environment
dotnet run --project src/MigrationRunner -- --environment Development

# Run migration with custom connection string
dotnet run --project src/MigrationRunner -- `
  --connection-string "Server=myserver;Database=mydb;User Id=sa;Password=pwd;"

# List available migrations (check before running)
dotnet run --project src/MigrationRunner -- --list-migrations

# Run in Docker
docker build -f src/MigrationRunner/Dockerfile -t migration:latest .
docker run -e "ASPNETCORE_ENVIRONMENT=Development" migration:latest

# Run in Kubernetes
kubectl apply -f k8s/06-migration-job.yml
kubectl logs job/migration-runner -n mystartup -f
```

---

## ✅ Verification Checklist

Run this to verify everything works:

```powershell
# 1. Check files exist
Test-Path "src/MigrationRunner/appsettings.json"                    # Should be True
Test-Path "src/MigrationRunner/appsettings.Development.json"        # Should be True
Test-Path "src/MigrationRunner/appsettings.Production.json"         # Should be True

# 2. Build the project
dotnet build --project src/MigrationRunner

# 3. Run migrations locally
dotnet run --project src/MigrationRunner

# 4. Check result
# If you see "Migration completed successfully!" → All good! ✅
```

---

## 📚 For More Details

See the comprehensive guide: **MIGRATIONRUNNER_SETUP_GUIDE.md**

---

## 🚀 Next Steps

1. ✅ Configuration files created
2. ⏭️ Run migrations locally: `dotnet run --project src/MigrationRunner`
3. ⏭️ Test with Docker: `docker build -f src/MigrationRunner/Dockerfile -t migration:latest .`
4. ⏭️ Deploy to Azure: `kubectl apply -f k8s/06-migration-job.yml`

---

**Your MigrationRunner is ready to use! 🎉**
