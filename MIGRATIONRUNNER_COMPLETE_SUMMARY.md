# 🎯 MigrationRunner - Complete Setup Summary

**Date**: 2024  
**Status**: ✅ READY TO USE  
**Issue Fixed**: Missing appsettings.json configuration files

---

## 📋 Executive Summary

### **What Was Wrong**
```
Error: The configuration file 'appsettings.json' was not found and is not optional.
Expected physical path: C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany\appsettings.json
```

The `MigrationRunner` project was missing its configuration files, causing runtime failures.

### **What Was Fixed**
✅ Created `src/MigrationRunner/appsettings.json`  
✅ Created `src/MigrationRunner/appsettings.Development.json`  
✅ Created `src/MigrationRunner/appsettings.Production.json`  
✅ Updated `MigrationRunner.csproj` to copy config files to output  
✅ Created comprehensive documentation

### **How to Verify It Works**
```powershell
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet run --project src/MigrationRunner

# You should see:
# ========================================
# MyStartUpCompany Database Migration Tool
# ========================================
# Migration completed successfully!
```

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        MigrationRunner Project                          │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  src/MigrationRunner/                                                   │
│  ├── Program.cs                    (Main entry point)                  │
│  ├── MigrationRunner.csproj        (Project definition)                │
│  ├── Dockerfile                    (Docker image)                      │
│  ├── appsettings.json              (Base configuration) ✅             │
│  ├── appsettings.Development.json  (Local dev overrides) ✅            │
│  └── appsettings.Production.json   (Azure prod template) ✅            │
│                                                                         │
│  Runs as:                                                               │
│  • Console app (locally via dotnet run)                               │
│  • Docker container (docker run)                                       │
│  • Kubernetes Job (in Azure AKS)                                       │
│                                                                         │
│  Purpose:                                                               │
│  • Applies EF Core database migrations                                 │
│  • Separate from API/Worker services                                   │
│  • Runs before deploying other services                                │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 🏠 Local Development Workflow

### **Configuration Flow (Local)**

```
┌─ User runs: dotnet run --project src/MigrationRunner ─┐
│                                                        │
│  Program.cs → BuildConfiguration()                    │
│  Set ASPNETCORE_ENVIRONMENT = "Development"           │
│                                                        │
│  Load configuration in order (later overrides):        │
│  1. appsettings.json                                   │
│  2. appsettings.Development.json                       │
│  3. Environment variables                             │
│  4. Command-line arguments                            │
│                                                        │
│  Get ConnectionString:                                 │
│  "Server=(localdb)\mssqllocaldb;                       │
│   Database=MyStartUpCompanyDb;                         │
│   Trusted_Connection=True;"                            │
│                                                        │
│  Connect to LocalDB                                    │
│  ↓                                                     │
│  Apply pending migrations                             │
│  ↓                                                     │
│  Success! ✅                                           │
└────────────────────────────────────────────────────────┘
```

### **Configuration Files Used Locally**

**appsettings.json** (Base):
- Default logging: Information level
- Default connection: LocalDB
- Shared across all environments

**appsettings.Development.json** (Local Override):
- Debug logging (more verbose)
- Logs SQL queries for debugging
- LocalDB connection
- Applied on top of base when Environment=Development

### **How to Run Locally**

```powershell
# 1. Navigate to solution root
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany

# 2. Run migration
dotnet run --project src/MigrationRunner

# 3. For verbose debug output
$env:DEBUG_MIGRATIONS = "true"
dotnet run --project src/MigrationRunner

# 4. With specific connection string
dotnet run --project src/MigrationRunner -- `
  --connection-string "Server=myserver;Database=mydb;User Id=sa;Password=pwd;"

# 5. List available migrations
dotnet run --project src/MigrationRunner -- --list-migrations
```

---

## ☁️ Azure/Kubernetes Deployment Workflow

### **Configuration Flow (Azure)**

```
┌─ Kubernetes Job Starts ─┐
│                         │
│  Pod initialized        │
│  Container starts       │
│                         │
│  Environment variables set:
│  • ASPNETCORE_ENVIRONMENT=Production        (from Job spec)
│  • ConnectionStrings__DefaultConnection=... (from Secret)
│                         │
│  Program.cs → BuildConfiguration()
│  Set ASPNETCORE_ENVIRONMENT = "Production"
│                         │
│  Load configuration in order (later overrides):
│  1. appsettings.json
│  2. appsettings.Production.json
│  3. Environment variables (WINS!)
│  4. Command-line arguments
│                         │
│  Get ConnectionString from Kubernetes Secret:
│  "Server=tcp:myserver.database.windows.net;
│   Database=MyStartUpCompanyDb;
│   User ID=sqladmin;
│   Password=***;
│   Encrypt=True;"
│                         │
│  Connect to Azure SQL Server
│  ↓
│  Apply pending migrations
│  ↓
│  Success! ✅
│  ↓
│  Container exits
│  Job terminates
└─────────────────────────────────────────────┘
```

### **Configuration Files Used in Azure**

**appsettings.json** (Base):
- Default logging: Information level
- Fallback connection (overridden by Secret)

**appsettings.Production.json** (Production Override):
- Information logging (not verbose)
- Template Azure SQL connection string
- **IMPORTANT**: Overridden by Kubernetes Secret!

**Kubernetes Secret** (Actual Config):
- Contains real Azure SQL connection string
- Injected as environment variable `ConnectionStrings__DefaultConnection`
- Takes highest priority
- Never stored in source code

### **How to Deploy to Azure**

```powershell
# 1. Create Kubernetes Secret with Azure SQL connection string
# (Do this once during initial setup)
kubectl create secret generic app-secrets `
  --from-literal=database-connection-string="Server=tcp:myserver.database.windows.net,1433;Initial Catalog=MyStartUpCompanyDb;Persist Security Info=False;User ID=sqladmin;Password=YourPassword!;Encrypt=True;Connection Timeout=30;" `
  -n mystartup

# 2. Apply Kubernetes Job manifest
kubectl apply -f k8s/06-migration-job.yml -n mystartup

# 3. Watch migration progress
kubectl logs job/migration-runner -n mystartup -f

# 4. Verify success
kubectl get job migration-runner -n mystartup
# STATUS should be "1/1" (one pod completed successfully)
```

---

## 🔧 Configuration File Details

### **appsettings.json** (Base Configuration)

```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.EntityFrameworkCore": "Warning",
	  "Microsoft.EntityFrameworkCore.Database.Command": "Information"
	}
  },
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyStartUpCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
  }
}
```

**Purpose**: 
- Base configuration for all environments
- Safe defaults (LocalDB)
- Merged with environment-specific overrides

**When Used**:
- Always loaded first
- Overridden by environment-specific JSON files
- Overridden by environment variables
- Overridden by command-line arguments

---

### **appsettings.Development.json** (Local Development)

```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Debug",
	  "Microsoft.EntityFrameworkCore": "Debug",
	  "Microsoft.EntityFrameworkCore.Database.Command": "Debug"
	}
  },
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyStartUpCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
  }
}
```

**Purpose**:
- Override base config for local development
- Enable debug logging to see SQL queries
- LocalDB connection string

**When Used**:
- When `ASPNETCORE_ENVIRONMENT=Development` (default)
- Applied after appsettings.json

**What It Changes**:
- Logging level from Information → Debug
- More detailed SQL query logging

---

### **appsettings.Production.json** (Azure Production)

```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.EntityFrameworkCore": "Warning",
	  "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
	}
  },
  "ConnectionStrings": {
	"DefaultConnection": "Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=YOUR_DATABASE;Persist Security Info=False;User ID=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;"
  }
}
```

**Purpose**:
- Override base config for production
- Reduce logging verbosity
- Template for Azure SQL connection

**When Used**:
- When `ASPNETCORE_ENVIRONMENT=Production` (in Kubernetes)
- Applied after appsettings.json

**What It Changes**:
- Logging level back to Information
- Reduces EF Core logging
- **Connection string is replaced by Kubernetes Secret in Azure!**

**Important Note**: The connection string in this file is just a template. In Kubernetes, the real connection string comes from the Secret and is injected as `ConnectionStrings__DefaultConnection` environment variable.

---

## 📝 Project File Configuration

### **MigrationRunner.csproj Update**

```xml
<!-- Copy appsettings files to output directory -->
<ItemGroup>
  <None Update="appsettings.json" CopyToOutputDirectory="PreserveNewest" />
  <None Update="appsettings.Development.json" CopyToOutputDirectory="PreserveNewest" />
  <None Update="appsettings.Production.json" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

**Purpose**:
- Ensures configuration files are copied when building
- During `dotnet run` → files copied to bin/Debug/net10.0/
- During `docker build` → files copied to output
- During `dotnet publish` → files included in release

**Why This Matters**:
- Without this, `dotnet run` wouldn't find appsettings.json
- Configuration files must be in the working directory
- This makes the project more portable

---

## 🔄 Configuration Priority Hierarchy

### **Local Development**
```
Priority (highest to lowest):
1. Command-line: dotnet run -- --connection-string "..."
2. Environment variables: $env:ConnectionStrings__DefaultConnection
3. appsettings.Development.json (if ASPNETCORE_ENVIRONMENT=Development)
4. appsettings.json
```

### **Azure/Kubernetes**
```
Priority (highest to lowest):
1. Kubernetes Secret (injected as ConnectionStrings__DefaultConnection)
2. Command-line arguments (rarely used in production)
3. appsettings.Production.json
4. appsettings.json
```

---

## 🐛 Troubleshooting Guide

### **Problem: "appsettings.json not found"**

**Cause**: Running from wrong directory

**Solution**:
```powershell
# WRONG - runs from wrong directory
cd src/MigrationRunner
dotnet run

# CORRECT - run from solution root
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet run --project src/MigrationRunner
```

---

### **Problem: "Cannot connect to LocalDB"**

**Cause**: LocalDB not running or not installed

**Solution Option 1** - Start LocalDB:
```powershell
sqllocaldb info                              # List instances
sqllocaldb start mssqllocaldb                # Start default instance
```

**Solution Option 2** - Use Docker SQL Server:
```powershell
# Start SQL Server in Docker
docker run -e "ACCEPT_EULA=Y" `
  -e "SA_PASSWORD=YourPassword123!" `
  -p 1433:1433 -d `
  mcr.microsoft.com/mssql/server:2022-latest

# Run migration with Docker connection
dotnet run --project src/MigrationRunner -- `
  --connection-string "Server=localhost,1433;Database=MyStartUpCompanyDb;User Id=sa;Password=YourPassword123!;"
```

---

### **Problem: "No migrations needed" (after first run)**

**Cause**: Migrations already applied

**Solution**: This is NORMAL behavior!

```powershell
# Check migration status
dotnet run --project src/MigrationRunner -- --list-migrations

# If all migrations show as applied, nothing more to do
# Next run will be fast (no changes needed)
```

---

### **Problem: "Cannot connect to Azure SQL"**

**Cause**: Connection string invalid or firewall blocking

**Solutions**:

1. **Verify connection string in Secret**:
```powershell
kubectl get secret app-secrets -n mystartup -o yaml
# Check the database-connection-string value
```

2. **Check firewall allows pod IP**:
```powershell
# Get pod IP
kubectl get pod -n mystartup -l app=migration-runner

# Add IP to Azure SQL firewall
# Portal → SQL Server → Networking → Add IP
```

3. **Test connection locally first**:
```powershell
dotnet run --project src/MigrationRunner -- `
  --connection-string "Server=tcp:myserver.database.windows.net,1433;..."
```

---

### **Problem: "Deployment fails but no migration logs"**

**Cause**: Job failed before logging

**Solution**:
```powershell
# Check pod status
kubectl get pod -n mystartup

# Get detailed pod info
kubectl describe pod migration-runner-xxxxx -n mystartup

# Check all logs including errors
kubectl logs job/migration-runner -n mystartup --all-containers=true

# Check previous pod logs (if crashed)
kubectl logs job/migration-runner -n mystartup --previous
```

---

## 📚 Documentation Files

| File | Purpose | Audience |
|------|---------|----------|
| **MIGRATIONRUNNER_QUICK_START.md** | 3-step setup guide | Everyone (START HERE) |
| **MIGRATIONRUNNER_SETUP_GUIDE.md** | Comprehensive guide | Developers & DevOps |
| **MIGRATIONRUNNER_COMPLETE_SUMMARY.md** | This file | Architecture overview |
| **k8s/06-migration-job.yml** | Kubernetes Job definition | DevOps |
| **k8s/03-secrets.yml** | Secret configuration | DevOps |
| **src/MigrationRunner/Dockerfile** | Docker image | DevOps |

---

## ✅ Verification Checklist

Before running migrations, verify:

```powershell
# 1. Configuration files exist
Test-Path "src/MigrationRunner/appsettings.json"                    # True
Test-Path "src/MigrationRunner/appsettings.Development.json"        # True
Test-Path "src/MigrationRunner/appsettings.Production.json"         # True

# 2. Project builds successfully
dotnet build --project src/MigrationRunner                          # Success

# 3. Can run locally
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet run --project src/MigrationRunner                            # Migrations complete

# 4. Docker image builds (optional)
docker build -f src/MigrationRunner/Dockerfile -t migration:latest  # Success

# 5. Kubernetes deployment ready (optional)
kubectl apply -f k8s/06-migration-job.yml --dry-run=client          # Valid
```

---

## 🚀 Quick Reference

### **Local Development**
```powershell
dotnet run --project src/MigrationRunner
```

### **With Custom Connection String**
```powershell
dotnet run --project src/MigrationRunner -- --connection-string "..."
```

### **Docker Local**
```powershell
docker build -f src/MigrationRunner/Dockerfile -t migration:latest
docker run -e "ASPNETCORE_ENVIRONMENT=Development" migration:latest
```

### **Kubernetes Azure**
```powershell
kubectl apply -f k8s/06-migration-job.yml
kubectl logs job/migration-runner -n mystartup -f
```

---

## 📊 Configuration Matrix

| Scenario | Environment | Config File | Connection | Logging |
|----------|-------------|-------------|-----------|---------|
| Local Dev | Development | appsettings.Development.json | LocalDB | Debug |
| Local Production Test | Production | appsettings.Production.json | LocalDB (override) | Information |
| Docker Local | Development | appsettings.Development.json | Docker SQL | Debug |
| Azure/K8s | Production | appsettings.Production.json | Azure SQL (Secret) | Information |

---

## 🎯 Next Steps

### **Immediate** (Do This First)
1. ✅ Configuration files created
2. ⏭️ Run locally: `dotnet run --project src/MigrationRunner`
3. ⏭️ Verify: See "Migration completed successfully!"

### **Short-term** (This Week)
1. ⏭️ Test with Docker: `docker build -f src/MigrationRunner/Dockerfile -t migration:latest`
2. ⏭️ Verify Azure SQL connection works

### **Medium-term** (Before Deploy)
1. ⏭️ Create Kubernetes Secret with real Azure SQL connection string
2. ⏭️ Deploy migration job: `kubectl apply -f k8s/06-migration-job.yml`
3. ⏭️ Verify migrations apply successfully in Azure

---

## 💡 Key Takeaways

✅ **MigrationRunner** is a **standalone application**  
✅ Separate from API and Worker services  
✅ Runs **before** deploying other services  
✅ Uses **same EF Core** migrations as API  

**Locally**: Connects to LocalDB, debug logging  
**In Azure**: Connects to Azure SQL, prod logging  

**Configuration Priority**:  
Kubernetes Secret > Environment Variables > appsettings.{Environment}.json > appsettings.json

---

**🎉 Your MigrationRunner is now fully configured and documented!**

**Ready to run?**
```powershell
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet run --project src/MigrationRunner
```

---

*For quick start, see: MIGRATIONRUNNER_QUICK_START.md*  
*For comprehensive guide, see: MIGRATIONRUNNER_SETUP_GUIDE.md*
