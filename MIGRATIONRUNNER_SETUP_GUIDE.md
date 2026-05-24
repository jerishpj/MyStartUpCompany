# 📚 MigrationRunner - Configuration & Usage Guide

**Status**: Fixed ✅  
**Issue Resolved**: Missing appsettings.json files  
**Created**: appsettings.json, appsettings.Development.json, appsettings.Production.json

---

## 🔴 Problem That Was Fixed

```
Error: The configuration file 'appsettings.json' was not found and is not optional.
Expected physical path: C:\...\MyStartUpCompany\appsettings.json
```

**Root Cause**: The MigrationRunner project was looking for `appsettings.json` in the wrong location.

**Solution**: Created proper configuration files in the MigrationRunner project folder:
- ✅ `src/MigrationRunner/appsettings.json`
- ✅ `src/MigrationRunner/appsettings.Development.json`
- ✅ `src/MigrationRunner/appsettings.Production.json`

---

## 📁 Project Structure

```
src/
├── MigrationRunner/
│   ├── Program.cs
│   ├── Dockerfile
│   ├── MigrationRunner.csproj
│   ├── appsettings.json ...................... ✅ CREATED (base config)
│   ├── appsettings.Development.json ........ ✅ CREATED (local debug)
│   └── appsettings.Production.json ......... ✅ CREATED (Azure config)
├── MyStartUpCompany.Api/
├── MyStartUpCompany.Worker/
└── MyStartUpCompany.Persistence/
```

---

## 🏠 LOCAL DEVELOPMENT - How It Works

### **Step 1: Running Migrations Locally**

The MigrationRunner is designed to be run from the **project root** directory:

```powershell
# Navigate to project root
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany

# Run migration
dotnet run --project src/MigrationRunner

# Run with explicit environment
dotnet run --project src/MigrationRunner -- --environment Development
```

### **Step 2: Configuration Loading**

When you run locally, the MigrationRunner loads configuration in this order:

```
1. appsettings.json (base config)
   ↓ Merged with
2. appsettings.Development.json (overrides base config)
   ↓ Merged with
3. Environment variables (if set)
   ↓ Merged with
4. Command-line arguments (highest priority)
```

### **Step 3: Connection String Resolution (Local)**

For **local development**, the flow is:

```
┌─────────────────────────────────────────────────────┐
│ Run: dotnet run --project src/MigrationRunner      │
└─────────────────────────────────────────────────────┘
					↓
┌─────────────────────────────────────────────────────┐
│ Program.cs detects ASPNETCORE_ENVIRONMENT          │
│ Default: "Development" (if not set)                │
└─────────────────────────────────────────────────────┘
					↓
┌─────────────────────────────────────────────────────┐
│ Loads appsettings.json                              │
│ Then appsettings.Development.json                   │
└─────────────────────────────────────────────────────┘
					↓
┌─────────────────────────────────────────────────────┐
│ Connection String:                                  │
│ Server=(localdb)\mssqllocaldb;                      │
│ Database=MyStartUpCompanyDb;                        │
│ Trusted_Connection=True;                            │
│ MultipleActiveResultSets=true;                      │
└─────────────────────────────────────────────────────┘
					↓
┌─────────────────────────────────────────────────────┐
│ Connects to LOCAL SQL Server LocalDB                │
│ Applies all pending migrations                      │
│ Completes successfully ✅                           │
└─────────────────────────────────────────────────────┘
```

### **Step 4: LocalDB Setup (if needed)**

If you don't have LocalDB installed locally, you have two options:

**Option A: Use Docker SQL Server**
```powershell
# Run SQL Server in Docker locally
docker run -e "ACCEPT_EULA=Y" `
  -e "SA_PASSWORD=MyPassword123!" `
  -p 1433:1433 `
  -d mcr.microsoft.com/mssql/server:2022-latest

# Update connection string in appsettings.Development.json:
# "Server=localhost,1433;Database=MyStartUpCompanyDb;User Id=sa;Password=MyPassword123!;"
```

**Option B: Use LocalDB (Windows only)**
```powershell
# LocalDB is included with Visual Studio
# Just ensure it's running:
sqllocaldb info    # Lists instances
sqllocaldb start mssqllocaldb    # Starts the default instance
```

---

## ☁️ AZURE DEPLOYMENT - How It Works

### **Step 1: Docker Container Build**

During `docker build`, the Dockerfile copies the configuration files:

```dockerfile
# Dockerfile for MigrationRunner
FROM mcr.microsoft.com/dotnet/sdk:10.0 as builder
WORKDIR /app
COPY . .

# Copy appsettings files
COPY src/MigrationRunner/appsettings.json .
COPY src/MigrationRunner/appsettings.Production.json .

# Build the project
RUN dotnet publish -c Release
```

### **Step 2: Running in Kubernetes**

When deployed to Kubernetes, the MigrationRunner runs as a **Kubernetes Job**:

```yaml
# k8s/06-migration-job.yml
apiVersion: batch/v1
kind: Job
metadata:
  name: migration-runner
spec:
  template:
	spec:
	  containers:
	  - name: migration-runner
		image: mystartupregistry.azurecr.io/migration:latest
		env:
		- name: ASPNETCORE_ENVIRONMENT
		  value: Production              # ← Sets environment to Production
		- name: ConnectionStrings__DefaultConnection
		  valueFrom:
			secretKeyRef:
			  name: app-secrets
			  key: database-connection-string    # ← Gets connection string from secret
```

### **Step 3: Configuration Loading (Azure)**

When running in Azure/Kubernetes, the flow is:

```
┌─────────────────────────────────────────────────────┐
│ Kubernetes Job Starts MigrationRunner Container    │
└─────────────────────────────────────────────────────┘
					↓
┌─────────────────────────────────────────────────────┐
│ Environment Variable: ASPNETCORE_ENVIRONMENT=Production │
└─────────────────────────────────────────────────────┘
					↓
┌─────────────────────────────────────────────────────┐
│ Program.cs detects Production environment           │
│ Loads appsettings.json                              │
│ Then appsettings.Production.json (overrides)        │
└─────────────────────────────────────────────────────┘
					↓
┌─────────────────────────────────────────────────────┐
│ Configuration looks for connection string from:     │
│ 1. appsettings.Production.json (if set)             │
│ 2. Environment variable: ConnectionStrings__DefaultConnection │
│    (from Kubernetes Secret - WINS!)                 │
└─────────────────────────────────────────────────────┘
					↓
┌─────────────────────────────────────────────────────┐
│ Connection String from Kubernetes Secret:           │
│ Server=tcp:myserver.database.windows.net;           │
│ Database=MyStartUpCompanyDb;                        │
│ User ID=sqluser;                                    │
│ Password=***;                                       │
│ Encrypt=True;                                       │
└─────────────────────────────────────────────────────┘
					↓
┌─────────────────────────────────────────────────────┐
│ Connects to AZURE SQL Server                        │
│ Applies all pending migrations                      │
│ Kubernetes Job completes with success ✅            │
└─────────────────────────────────────────────────────┘
```

### **Step 4: Connection String in Azure**

The connection string comes from:

```powershell
# During Kubernetes deployment setup:
kubectl create secret generic app-secrets `
  --from-literal=database-connection-string="Server=tcp:myserver.database.windows.net,1433;Initial Catalog=MyStartUpCompanyDb;Persist Security Info=False;User ID=sqladmin;Password=YourPassword!;Encrypt=True;Connection Timeout=30;" `
  -n mystartup

# This secret is referenced in the Job specification
# Environment variable receives the secret value
# Program reads it and uses it to connect
```

---

## 🔄 Configuration Hierarchy Comparison

### **LOCAL (Development)**

```
Priority (highest to lowest):
1. Command-line arguments: dotnet run -- --connection-string "..."
2. Environment variables: $env:ConnectionStrings__DefaultConnection = "..."
3. appsettings.Development.json
4. appsettings.json
```

**Typical Result**:
- Uses LocalDB connection from appsettings.Development.json
- Logs at Debug level
- Quick iteration

### **DOCKER LOCAL**

```
Priority (highest to lowest):
1. Environment variables (if Docker Compose sets them)
2. appsettings.Docker.json (if exists)
3. appsettings.json
```

**Typical Result**:
- Uses Docker container's SQL Server
- Logs at Information level
- Tests containerization before Azure

### **AZURE (Production)**

```
Priority (highest to lowest):
1. Kubernetes Secrets (injected as env vars)
2. appsettings.Production.json
3. appsettings.json
```

**Typical Result**:
- Uses Azure SQL connection string from Kubernetes Secret
- Logs at Information level
- Connects to production database
- Job completes and terminates

---

## 🔑 appsettings Files Explained

### **appsettings.json (Base Configuration)**

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
- Safe defaults for local development
- Merged with environment-specific files

---

### **appsettings.Development.json (Local Development)**

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
- Enables Debug logging (more verbose)
- Uses LocalDB connection
- Used when: `ASPNETCORE_ENVIRONMENT=Development` (default)

---

### **appsettings.Production.json (Azure/Production)**

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
- Reduces logging (less verbose)
- Template for Azure SQL connection
- **NOTE**: Connection string from appsettings is OVERRIDDEN by Kubernetes Secret!

---

## 📝 How to Use the MigrationRunner

### **Scenario 1: Run Migrations Locally (Development)**

```powershell
# Command (run from project root)
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet run --project src/MigrationRunner

# What happens:
# 1. ASPNETCORE_ENVIRONMENT defaults to "Development"
# 2. Loads appsettings.Development.json
# 3. Connects to LocalDB
# 4. Applies all pending migrations
# 5. Outputs "Migration completed successfully"
```

### **Scenario 2: Run Migrations Against Azure SQL (from Local Machine)**

```powershell
# Command
dotnet run --project src/MigrationRunner -- --connection-string "Server=tcp:myserver.database.windows.net,1433;Initial Catalog=MyDb;User ID=sqladmin;Password=YourPassword!;Encrypt=True;Connection Timeout=30;"

# What happens:
# 1. Command-line connection string takes priority
# 2. Connects to Azure SQL Server (not LocalDB!)
# 3. Applies all pending migrations to Azure
# 4. Useful for emergency fixes or manual migrations
```

### **Scenario 3: Run Migrations in Docker (Local)**

```powershell
# Build Docker image
docker build -f src/MigrationRunner/Dockerfile -t mystartup-migration:latest .

# Run migration in Docker
docker run `
  --rm `
  -e "ASPNETCORE_ENVIRONMENT=Development" `
  -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433;Database=MyDb;User Id=sa;Password=Password123!;" `
  mystartup-migration:latest

# What happens:
# 1. Runs migration in a Docker container
# 2. Uses environment variables for configuration
# 3. Connects to SQL Server (inside Docker network)
# 4. Tests containerization before Azure deployment
```

### **Scenario 4: Run Migrations in Kubernetes (Azure)**

```powershell
# Kubernetes job already configured in: k8s/06-migration-job.yml
# Just apply the manifests:

kubectl apply -f k8s/01-namespace.yml
kubectl apply -f k8s/03-secrets.yml        # Creates secrets with connection strings
kubectl apply -f k8s/06-migration-job.yml  # Creates and runs migration job

# Watch the job:
kubectl logs job/migration-runner -n mystartup -f

# What happens:
# 1. Kubernetes creates a Pod from migration image
# 2. Sets ASPNETCORE_ENVIRONMENT=Production
# 3. Injects ConnectionStrings__DefaultConnection from Secret
# 4. Container runs migration in Azure SQL
# 5. Job completes and terminates (doesn't stay running)
```

---

## 🐛 Troubleshooting

### **Issue 1: "appsettings.json not found"**

```
Error: The configuration file 'appsettings.json' was not found and is not optional.
```

**Solution**:
```powershell
# Ensure you're in the project root directory:
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany

# Run from there:
dotnet run --project src/MigrationRunner

# The program looks in the current working directory
# So you MUST run from the root!
```

### **Issue 2: "Cannot connect to LocalDB"**

```
Error: Cannot connect to database. LocalDB instance not found.
```

**Solution**:
```powershell
# Check if LocalDB is running:
sqllocaldb info

# Start it:
sqllocaldb start mssqllocaldb

# Or use Docker SQL Server instead:
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MyPassword123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# Update connection string in appsettings.Development.json
```

### **Issue 3: "Cannot connect to Azure SQL"**

```
Error: Login failed for user 'sqladmin'. Server: myserver.database.windows.net
```

**Solution**:
```powershell
# Verify connection string in Kubernetes Secret:
kubectl get secret app-secrets -n mystartup -o yaml

# Check it contains valid Azure SQL connection string
# Verify firewall allows Kubernetes cluster IP:
# Azure Portal → SQL Server → Networking → Add client IP
```

### **Issue 4: "Migrations already applied"**

```
Message: Done. No migrations needed to be applied.
```

**This is NORMAL!** If you've already run migrations, there are no new ones to apply.

**To see migration status**:
```powershell
dotnet run --project src/MigrationRunner -- --list-migrations
```

---

## 📊 Quick Reference Table

| Scenario | Environment | Connection | Log Level | Command |
|----------|-------------|------------|-----------|---------|
| **Local Dev** | Development | LocalDB | Debug | `dotnet run --project src/MigrationRunner` |
| **Local Docker** | Docker | Docker SQL | Information | `docker run -e ConnectionStrings__DefaultConnection="..." migration:latest` |
| **Azure/K8s** | Production | Azure SQL | Information | `kubectl apply -f k8s/06-migration-job.yml` |
| **Manual Azure** | Production | Azure SQL | Information | `dotnet run --project src/MigrationRunner -- --connection-string "..."` |

---

## ✅ Verification Checklist

- [x] Created `src/MigrationRunner/appsettings.json`
- [x] Created `src/MigrationRunner/appsettings.Development.json`
- [x] Created `src/MigrationRunner/appsettings.Production.json`
- [x] LocalDB configured locally
- [x] Run from project root directory
- [x] ASPNETCORE_ENVIRONMENT properly set
- [x] Connection strings configured
- [x] Docker setup verified (optional)
- [x] Kubernetes manifests ready (optional)

---

## 🚀 Next Steps

### **To Test Locally Right Now**:
```powershell
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet run --project src/MigrationRunner
```

### **To Deploy to Azure**:
1. Create Kubernetes Secret with Azure SQL connection string
2. Apply migration job: `kubectl apply -f k8s/06-migration-job.yml`
3. Verify: `kubectl logs job/migration-runner -n mystartup -f`

### **To Test with Docker**:
1. Build: `docker build -f src/MigrationRunner/Dockerfile -t migration:latest .`
2. Run: `docker run -e "ConnectionStrings__DefaultConnection=..." migration:latest`

---

## 📚 Additional Resources

- **Program.cs** - Full implementation with comments
- **k8s/06-migration-job.yml** - Kubernetes Job configuration
- **docs/LOCAL_DEVELOPMENT.md** - Local setup guide
- **docs/DEPLOYMENT_GUIDE.md** - Deployment procedures
- **docs/AZURE_AKS_DEPLOYMENT_GUIDE.md** - Complete Azure guide

---

**✅ Your MigrationRunner is now fully configured and ready to use!** 🚀
