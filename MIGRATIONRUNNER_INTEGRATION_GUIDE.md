# 🔗 How MigrationRunner Integrates with Your Solution

**Understanding how MigrationRunner fits into your complete MyStartUpCompany system**

---

## 📊 Solution Architecture

```
┌──────────────────────────────────────────────────────────────────────────┐
│                    MyStartUpCompany .NET 10 Solution                     │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  src/MyStartUpCompany.Persistence/                             │   │
│  │  ├── DbContext.cs        (Entity Framework DbContext)           │   │
│  │  ├── Migrations/         (EF Core migration files)              │   │
│  │  │   ├── 20240101...001_InitialCreate.cs                        │   │
│  │  │   ├── 20240102...002_AddUserTable.cs                         │   │
│  │  │   └── ...                                                    │   │
│  │  └── Extensions.cs       (Migration helper methods)             │   │
│  │                                                                 │   │
│  │  🔑 This is the shared persistence layer                       │   │
│  │  Both API and MigrationRunner use these migrations             │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                          ↑↑↑ Shared ↑↑↑                                │
│          (MigrationRunner and API use same DbContext)                  │
│                          ↓↓↓       ↓↓↓                                │
│  ┌────────────────────────┐  ┌──────────────────────────────┐        │
│  │  src/MigrationRunner/  │  │  src/MyStartUpCompany.Api/   │        │
│  │                        │  │                              │        │
│  │  Program.cs ────────┐  │  │  Program.cs ────────┐        │        │
│  │  • Parse args       │  │  │  • Build WebApp     │        │        │
│  │  • Load config      │  │  │  • Setup DI         │        │        │
│  │  • Run migrations   │  │  │  • Configure auth   │        │        │
│  │  • Exit             │  │  │  • Listen on port   │        │        │
│  │                     │  │  │                     │        │        │
│  │ appsettings.json    │  │  │ appsettings.json    │        │        │
│  │ .Development.json   │  │  │ .Development.json   │        │        │
│  │ .Production.json    │  │  │ .Production.json    │        │        │
│  │                     │  │  │                     │        │        │
│  │ ✅ CONFIGURED       │  │  │ ✅ CONFIGURED       │        │        │
│  └────────────────────────┘  └──────────────────────────────┘        │
│                                                                        │
│  ┌──────────────────────────────────────────────────────────────┐    │
│  │  src/MyStartUpCompany.Worker/                               │    │
│  │  ├── Program.cs (BackgroundService for async tasks)         │    │
│  │  ├── appsettings.json/.Development.json/.Production.json    │    │
│  │  └── ✅ CONFIGURED                                           │    │
│  └──────────────────────────────────────────────────────────────┘    │
│                                                                        │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Deployment Sequence

### **Local Development**

```
1️⃣ Start LocalDB (or Docker SQL)
   $ sqllocaldb start mssqllocaldb

2️⃣ Run MigrationRunner (applies all pending migrations)
   $ dotnet run --project src/MigrationRunner
   → Migrations applied to LocalDB ✅

3️⃣ Start API (uses migrated database)
   $ dotnet run --project src/MyStartUpCompany.Api
   → API connects to database
   → Ready for local testing

4️⃣ Start Worker (optional, uses same database)
   $ dotnet run --project src/MyStartUpCompany.Worker
   → Worker processes background jobs
   → Uses same migrated database
```

---

### **Docker Local Development**

```
1️⃣ Start SQL Server in Docker
   $ docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password123!" `
	 -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

2️⃣ Run MigrationRunner in Docker
   $ docker build -f src/MigrationRunner/Dockerfile -t migration:latest .
   $ docker run --network host -e "ConnectionStrings__DefaultConnection=..." migration:latest
   → Migrations applied to Docker SQL ✅

3️⃣ Run API in Docker (linked to Docker SQL)
   $ docker build -f src/MyStartUpCompany.Api/Dockerfile -t api:latest .
   $ docker run --network host -e "ConnectionStrings__DefaultConnection=..." `
	 -p 5000:5000 api:latest
   → API serves on http://localhost:5000

4️⃣ Test locally
   $ curl http://localhost:5000/api/...
```

---

### **Azure Kubernetes Deployment (Recommended)**

```
DEPLOYMENT SEQUENCE:

1️⃣ Create Kubernetes Namespace
   $ kubectl create namespace mystartup

2️⃣ Store Secrets (connection strings, API keys, etc.)
   $ kubectl create secret generic app-secrets \
	 --from-literal=database-connection-string="Server=tcp:myserver..." \
	 -n mystartup

3️⃣ RUN MIGRATIONS FIRST (This is critical!)
   $ kubectl apply -f k8s/06-migration-job.yml

   This runs the MigrationRunner as a one-off Job:
   • Applies all pending migrations to Azure SQL
   • Exits when done
   • Pod doesn't restart

   $ kubectl logs job/migration-runner -n mystartup -f
   → Watch migration progress

   $ kubectl wait --for=condition=complete job/migration-runner -n mystartup
   → Wait for completion

4️⃣ THEN Deploy API (depends on migrations being done)
   $ kubectl apply -f k8s/04-api-deployment.yml
   → API Deployment uses same connection string
   → Connects to migrated database
   → Ready to serve requests

5️⃣ Deploy Worker (optional, uses same database)
   $ kubectl apply -f k8s/05-worker-deployment.yml
   → Worker StatefulSet processes background jobs
   → Uses same migrated database

6️⃣ Verify Everything
   $ kubectl get all -n mystartup
   → See all running resources

   $ kubectl logs deployment/api -n mystartup -f
   → Watch API startup

   $ curl https://myapp.example.com/api/...
   → Test the API
```

**IMPORTANT**: Step 3 (MigrationRunner) MUST complete before Steps 4-5!

---

## 🔑 Connection String Management

### **Local Development**

```
appsettings.json / appsettings.Development.json:
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyStartUpCompanyDb;..."
  }

✅ Shared across all services (API, Worker, MigrationRunner)
✅ Stored in source code (safe for development)
✅ LocalDB runs locally, no server needed
```

---

### **Azure Production**

```
Kubernetes Secrets (k8s/03-secrets.yml):
  database-connection-string: "Server=tcp:myserver.database.windows.net;..."

Each deployment references the secret:

  MigrationRunner Job:
	env:
	- name: ConnectionStrings__DefaultConnection
	  valueFrom:
		secretKeyRef:
		  name: app-secrets
		  key: database-connection-string

  API Deployment:
	env:
	- name: ConnectionStrings__DefaultConnection
	  valueFrom:
		secretKeyRef:
		  name: app-secrets
		  key: database-connection-string

  Worker Deployment:
	env:
	- name: ConnectionStrings__DefaultConnection
	  valueFrom:
		secretKeyRef:
		  name: app-secrets
		  key: database-connection-string

✅ All services use SAME connection string from Secret
✅ Never stored in source code
✅ Securely injected by Kubernetes
✅ Easy to rotate (update Secret, restart pods)
```

---

## 🗄️ Database Migrations Workflow

### **How Migrations Are Created**

```
1. Developer makes database schema change
   Example: Add new Users table

2. Add migration using EF Core CLI
   $ dotnet ef migrations add AddUsersTable `
	 --project src/MyStartUpCompany.Persistence

3. EF Core creates migration file
   Migration file saved to:
   src/MyStartUpCompany.Persistence/Migrations/20240115...AddUsersTable.cs

4. Add to source control
   $ git add src/MyStartUpCompany.Persistence/Migrations/
   $ git commit -m "Add migration: AddUsersTable"
   $ git push
```

### **How Migrations Are Applied**

```
LOCAL:
  1. Developer runs MigrationRunner locally
	 $ dotnet run --project src/MigrationRunner
  2. Reads all migration files from Migrations/ folder
  3. Applies any pending migrations to LocalDB
  4. Database is now up to date ✅

CI/CD (Pre-deployment):
  1. CI pipeline builds MigrationRunner
  2. Runs migrations against test database
  3. Verifies migrations work
  4. Marks build as ready for deployment

AZURE (During Deployment):
  1. Kubernetes Job runs MigrationRunner container
  2. Reads all migration files from container
  3. Applies any pending migrations to Azure SQL
  4. Database is now up to date ✅
  5. API and Worker pods can now start
```

### **Shared Migrations Across Services**

```
KEY POINT: All services use the SAME migrations!

  src/MyStartUpCompany.Persistence/
  └── Migrations/
	  ├── 001_InitialCreate.cs
	  ├── 002_AddUserTable.cs
	  └── 003_AddProductTable.cs

Used by:
  ✅ MigrationRunner (applies before deployment)
  ✅ API (uses DbContext on startup)
  ✅ Worker (uses DbContext for background jobs)

Example: If AddUserTable migration exists:
  → MigrationRunner applies it to database
  → API can read/write Users table
  → Worker can read/write Users table
  → All in sync! ✅
```

---

## 📋 Configuration Inheritance Model

### **All Services Follow Same Pattern**

```
Every service (API, Worker, MigrationRunner) has:

1. appsettings.json (base config)
   • Default logging: Information
   • Default connection: LocalDB
   • Shared settings

2. appsettings.Development.json (local override)
   • More verbose logging: Debug
   • LocalDB connection
   • Used locally

3. appsettings.Production.json (production override)
   • Less logging: Warning
   • Azure SQL template
   • Used in Azure/Kubernetes

Configuration Loading (same for all):
  Base → Environment-specific → Environment vars → Command-line args

Example:
  appsettings.json (Information level)
  + appsettings.Development.json (overrides to Debug level)
  + Environment variable ASPNETCORE_ENVIRONMENT=Development
  = RESULT: Debug logging to LocalDB
```

---

## 🚀 Typical Development Day

### **Morning: Local Development**

```powershell
# 1. Start database
sqllocaldb start mssqllocaldb

# 2. Apply latest migrations
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet run --project src/MigrationRunner

# 3. Start API
dotnet run --project src/MyStartUpCompany.Api

# 4. Start Worker (in another terminal)
dotnet run --project src/MyStartUpCompany.Worker

# 5. Develop and test locally
# Make changes → Build → Test → Repeat

# 6. Commit changes
git add src/
git commit -m "Add new feature..."
git push origin main
```

### **Before Lunch: Submit Pull Request**

```
Your PR includes:
✅ Feature implementation
✅ EF Core migrations (if database changes)
✅ Unit tests
✅ Integration tests

CI Pipeline runs:
✅ Builds MigrationRunner and test migration
✅ Builds API
✅ Builds Worker
✅ Runs unit tests
✅ Runs integration tests (with test migrations)
✅ All pass! ✅

Code review happens...
```

### **After Lunch: Deployment**

```
PR is merged to main

CD Pipeline runs:
✅ Build all services
✅ Run migrations on test database
✅ Build Docker images
✅ Push to ACR (Azure Container Registry)

Manual deployment (when ready):
$ kubectl apply -f k8s/06-migration-job.yml      # Run migrations
$ kubectl wait --for=condition=complete job/migration-runner
$ kubectl apply -f k8s/04-api-deployment.yml     # Deploy API
$ kubectl apply -f k8s/05-worker-deployment.yml  # Deploy Worker

Azure live! ✅

Check logs:
$ kubectl logs deployment/api -n mystartup
$ kubectl logs deployment/worker -n mystartup
```

---

## 🔍 Troubleshooting Integration Issues

### **API Can't Connect to Database**

```
Symptom: API starts but crashes with connection error

Root Cause: Migrations not run yet

Solution:
  1. Ensure MigrationRunner has run successfully
  2. Check connection string matches database server
  3. Check firewall allows connection
  4. Restart API pod

# In Kubernetes:
$ kubectl delete pod deployment/api -n mystartup
# Kubernetes automatically restarts pod
```

---

### **MigrationRunner Fails, API Never Starts**

```
Symptom: Migration job fails → API deployment stuck

Root Cause: Migrations failed, database not ready

Solution:
  1. Check migration logs
	 $ kubectl logs job/migration-runner -n mystartup

  2. Fix migration issue (usually SQL syntax)

  3. Don't start API until migration succeeds!
	 $ kubectl wait --for=condition=complete job/migration-runner
	 → Wait for completion before deploying API

  4. Create new migration if needed
	 $ dotnet ef migrations add FixName --project src/MyStartUpCompany.Persistence

  5. Commit and push, then retry deployment
```

---

### **Worker Can't Access New Table Added Today**

```
Symptom: Worker throws "table not found" error

Root Cause: Migration for new table not run yet

Solution:
  1. Create migration (if not already done)
	 $ dotnet ef migrations add AddNewTable --project src/MyStartUpCompany.Persistence

  2. Commit and push

  3. Run pipeline (applies migration to test DB)

  4. Deploy to production (MigrationRunner applies migration)

  5. Worker uses new table ✅

Sequence is important:
  Commit migration → Build → Test → Deploy migration → Deploy worker
```

---

## 📈 Scaling Considerations

### **Multiple MigrationRunner Instances**

```
⚠️ IMPORTANT: Never run multiple MigrationRunner instances at once!

Why?
  • Only ONE should apply migrations to database
  • Multiple instances can cause locks/conflicts
  • Migrations must be applied serially

Solution:
  ✅ Use Kubernetes Job (not Deployment)
  ✅ Job runs one pod at a time
  ✅ Pod completes and exits
  ✅ Never has multiple replicas

k8s/06-migration-job.yml:
  apiVersion: batch/v1
  kind: Job              # ← Job, not Deployment!
  spec:
	parallelism: 1      # ← One at a time
	completions: 1      # ← Complete once
```

### **Multiple API Instances**

```
✅ FINE to scale API to multiple replicas

Why?
  • API is read/write to database
  • Database handles concurrent connections
  • Scale as needed for load

k8s/04-api-deployment.yml:
  spec:
	replicas: 3         # ← Multiple OK
	template:
	  spec:
		containers:
		- name: api
		  env:
		  - name: ConnectionStrings__DefaultConnection
			valueFrom:
			  secretKeyRef:
				name: app-secrets
				key: database-connection-string
		  # All replicas share same connection string
		  # Database handles concurrency
```

---

## ✅ Deployment Checklist

Before deploying to Azure:

```
MigrationRunner:
  ☐ Configuration files exist (appsettings.json, etc.)
  ☐ MigrationRunner.csproj has CopyToOutputDirectory
  ☐ All migrations in src/MyStartUpCompany.Persistence/Migrations/
  ☐ Test migration locally: dotnet run --project src/MigrationRunner
  ☐ Test in Docker: docker build -f src/MigrationRunner/Dockerfile
  ☐ k8s/06-migration-job.yml configured correctly
  ☐ ASPNETCORE_ENVIRONMENT=Production set in Job
  ☐ ConnectionStrings__DefaultConnection from Secret

API:
  ☐ Configuration files exist (appsettings.json, etc.)
  ☐ Depends on Persistence project (has DbContext)
  ☐ Startup uses migrations (or runs them on startup)
  ☐ Test locally: dotnet run --project src/MyStartUpCompany.Api
  ☐ Test in Docker: docker build -f src/MyStartUpCompany.Api/Dockerfile
  ☐ k8s/04-api-deployment.yml configured correctly
  ☐ ASPNETCORE_ENVIRONMENT=Production set in Deployment
  ☐ ConnectionStrings__DefaultConnection from Secret
  ☐ Startup probe configured (waits for DB ready)

Worker:
  ☐ Configuration files exist (appsettings.json, etc.)
  ☐ Extends BackgroundService
  ☐ Depends on Persistence project (has DbContext)
  ☐ Test locally: dotnet run --project src/MyStartUpCompany.Worker
  ☐ Test in Docker: docker build -f src/MyStartUpCompany.Worker/Dockerfile
  ☐ k8s/05-worker-deployment.yml configured correctly
  ☐ ASPNETCORE_ENVIRONMENT=Production set in StatefulSet
  ☐ ConnectionStrings__DefaultConnection from Secret

Kubernetes:
  ☐ Namespace created: kubectl create namespace mystartup
  ☐ Secret created: kubectl create secret generic app-secrets
  ☐ Migration job succeeds: kubectl apply -f k8s/06-migration-job.yml
  ☐ API deployment succeeds: kubectl apply -f k8s/04-api-deployment.yml
  ☐ Worker deployment succeeds: kubectl apply -f k8s/05-worker-deployment.yml
  ☐ All pods running: kubectl get pods -n mystartup
  ☐ API accessible: kubectl port-forward deployment/api 5000:5000
  ☐ Health checks passing: kubectl describe pod -n mystartup
```

---

## 📚 Related Documentation

| File | Purpose |
|------|---------|
| MIGRATIONRUNNER_QUICK_START.md | How to run MigrationRunner |
| MIGRATIONRUNNER_SETUP_GUIDE.md | MigrationRunner reference |
| docs/LOCAL_DEVELOPMENT.md | Local setup for all services |
| docs/AZURE_AKS_DEPLOYMENT_GUIDE.md | Full Azure deployment |
| k8s/06-migration-job.yml | MigrationRunner Kubernetes Job |
| k8s/04-api-deployment.yml | API Kubernetes Deployment |
| k8s/05-worker-deployment.yml | Worker Kubernetes StatefulSet |

---

## 🎯 Key Takeaways

✅ **MigrationRunner** is a **separate service** that runs BEFORE API and Worker  
✅ All services share **same DbContext** and **same migrations**  
✅ Migrations must be applied **once** to database before other services start  
✅ **Kubernetes Job** ensures MigrationRunner runs serially (not multiple at once)  
✅ **Secrets** store production connection strings (never in code)  
✅ **Configuration hierarchy**: base → environment → env vars → secrets  

---

**Ready to understand the complete flow? You now have MigrationRunner, API, and Worker all properly configured! 🚀**
