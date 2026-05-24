# Database Migration Strategy for MyStartUpCompany

## Overview

This document describes the **separated migration strategy** for MyStartUpCompany. Migrations are **never executed automatically** when API or Worker services start. Instead, they run as an **explicit, auditable step** before service deployment.

This approach provides:
- **Safety**: Schema changes don't happen automatically during service startup
- **Auditability**: Every migration run is controlled and logged
- **Flexibility**: Migrations can run on-demand, in CI/CD, or as a separate deployment step
- **Reliability**: API and Worker services start quickly without database schema overhead

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Deployment Flow                          │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  1. Database Migrations (EXPLICIT STEP)                     │
│     ↓                                                        │
│     - MigrationRunner console app                           │
│     - Checks pending migrations                             │
│     - Applies schema changes                                │
│     - Exits                                                 │
│                                                              │
│  2. Deploy API Service (STARTS CLEAN)                       │
│     ↓                                                        │
│     - No migration code runs                                │
│     - DbContext registers AppDbContext                      │
│     - Service ready in seconds                              │
│                                                              │
│  3. Deploy Worker Service (STARTS CLEAN)                    │
│     ↓                                                        │
│     - No migration code runs                                │
│     - HostedServices start immediately                      │
│     - Ready to process messages                             │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## Key Components

### 1. MigrationExtensions (`src/MyStartUpCompany.Persistence/Extensions/MigrationExtensions.cs`)

Centralized utilities for migration operations:

```csharp
// Apply all pending migrations
await serviceProvider.ApplyMigrationsAsync(logger);

// Get migration status
var pending = await serviceProvider.GetPendingMigrationsAsync();
var applied = await serviceProvider.GetAppliedMigrationsAsync();

// Health check
var isHealthy = await serviceProvider.IsDatabaseHealthyAsync(logger);

// Synchronous variants available too
serviceProvider.ApplyMigrations(logger);
var healthy = serviceProvider.IsDatabaseHealthy(logger);
```

**Features**:
- Async and sync methods (flexibility for different contexts)
- Comprehensive logging at each step
- Error handling and graceful failure
- Health checks for database readiness

### 2. MigrationRunner Console App (`src/MigrationRunner/`)

Standalone application for running migrations.

**Project Structure**:
```
src/MigrationRunner/
├── Program.cs              # Main entry point
├── MigrationRunner.csproj  # Project file
└── Dockerfile              # Container configuration
```

**Capabilities**:
- Standalone execution (no API/Worker needed)
- Command-line arguments support
- Environment variable configuration
- Connection string override
- List-only mode (see pending without applying)
- Detailed logging with debug mode

**Features**:
- Configuration hierarchy: `appsettings.json` → `appsettings.{env}.json` → env vars → CLI args
- Connection string masking for security (no passwords in logs)
- Color-coded console output
- Minimal dependencies (just EF Core and logging)

### 3. Migration Scripts

#### PowerShell Script (`scripts/migrate.ps1`)

**Usage**:
```powershell
# Development (default)
.\scripts\migrate.ps1

# Production
.\scripts\migrate.ps1 -Environment Production

# Override connection string
.\scripts\migrate.ps1 -ConnectionString "Server=myserver;Database=mydb;..."

# List pending migrations only
.\scripts\migrate.ps1 -ListOnly
```

**Features**:
- Parameter validation and help
- Color-coded output (Cyan/Yellow/Green/Red)
- Environment variable setting
- Error handling with exit codes

#### Bash Script (`scripts/migrate.sh`)

**Usage**:
```bash
# Development (default)
./scripts/migrate.sh

# Production
./scripts/migrate.sh production

# Override connection string
./scripts/migrate.sh --connection-string "Server=...;"

# List pending migrations only
./scripts/migrate.sh --list-migrations

# Help
./scripts/migrate.sh --help
```

**Features**:
- POSIX-compliant (works on Linux, macOS, WSL)
- Color-coded output using ANSI codes
- Argument parsing (positional + flags)
- Consistent error handling

### 4. Docker Compose for Migrations (`docker-compose.migrations.yml`)

Run migrations in a containerized environment.

**Services**:
- `mssql`: SQL Server database
- `migrations`: MigrationRunner container

**Usage**:
```bash
# Run migrations standalone
docker-compose -f docker-compose.migrations.yml up --exit-code-from migrations

# Run migrations and then start other services
docker-compose -f docker-compose.yml -f docker-compose.migrations.yml up migrations

# Interactive debugging
docker-compose -f docker-compose.migrations.yml run --rm migrations bash

# Clean up
docker-compose -f docker-compose.migrations.yml down -v
```

**Key Features**:
- Health checks on database before running migrations
- `restart: no` ensures migrations exit when complete
- Shared network for coordination with API/Worker
- Volumes for SQL Server persistence

### 5. MigrationRunner Dockerfile (`src/MigrationRunner/Dockerfile`)

Multi-stage Docker build for the migration runner.

**Stages**:
1. **Build**: Uses SDK image, restores/builds/publishes
2. **Runtime**: Uses minimal Alpine runtime

**Security**:
- Non-root user (migrationuser)
- Minimal base image (Alpine)
- Essential tools only

## Usage Scenarios

### Scenario 1: Local Development with Scripts

```bash
# Run migrations before starting services
./scripts/migrate.sh development

# Then start API and Worker
dotnet run --project src/MyStartUpCompany.Api
dotnet run --project src/MyStartUpCompany.Worker
```

### Scenario 2: Docker Compose (Local)

```bash
# Run migrations first
docker-compose -f docker-compose.migrations.yml up --exit-code-from migrations

# Then start API and Worker in another terminal
docker-compose up api worker
```

### Scenario 3: CI/CD Pipeline (GitHub Actions)

```yaml
- name: Run Migrations
  run: |
	dotnet run --project src/MigrationRunner -- \
	  --environment Production \
	  --connection-string "${{ secrets.DATABASE_CONNECTION_STRING }}"

- name: Deploy API
  run: |
	# Deploy API service

- name: Deploy Worker
  run: |
	# Deploy Worker service
```

### Scenario 4: Azure Container Instances

```bash
# Run migration container
az container create \
  --resource-group mygroup \
  --name migrations \
  --image mystartupcompany-migrations:latest \
  --environment-variables \
	ASPNETCORE_ENVIRONMENT=Production \
	ConnectionStrings__DefaultConnection="Server=...;"

# Wait for completion, then deploy API/Worker
```

### Scenario 5: Kubernetes (AKS)

```yaml
apiVersion: batch/v1
kind: Job
metadata:
  name: mystartupcompany-migrations
spec:
  template:
	spec:
	  containers:
	  - name: migrations
		image: mystartupcompany-migrations:latest
		env:
		- name: ASPNETCORE_ENVIRONMENT
		  value: Production
		- name: ConnectionStrings__DefaultConnection
		  valueFrom:
			secretKeyRef:
			  name: db-secrets
			  key: connection-string
	  restartPolicy: Never
  backoffLimit: 3
```

## Running Migrations

### Via .NET CLI

```bash
# Development (reads from appsettings.json + appsettings.Development.json)
dotnet run --project src/MigrationRunner

# Production with environment override
dotnet run --project src/MigrationRunner -- --environment Production

# List pending migrations
dotnet run --project src/MigrationRunner -- --list-migrations
```

### Via PowerShell Script

```powershell
# Add script directory to PATH or run from scripts folder
cd scripts
.\migrate.ps1 -Environment Production
```

### Via Bash Script

```bash
# Make executable (first time only)
chmod +x scripts/migrate.sh

# Run
./scripts/migrate.sh production
```

### Via Docker

```bash
# Build migration image
docker build -t mystartupcompany-migrations:latest -f src/MigrationRunner/Dockerfile .

# Run migrations
docker run --rm \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Server=...;Database=...;" \
  --network host \
  mystartupcompany-migrations:latest
```

## Configuration

### Connection String

Set via (in order of precedence):
1. `--connection-string` CLI argument
2. Environment variable: `ConnectionStrings__DefaultConnection`
3. `appsettings.{Environment}.json`
4. `appsettings.json`

**Example**:
```bash
# CLI argument
dotnet run -- --connection-string "Server=.;Database=MyDb;Trusted_Connection=true;"

# Environment variable
export ConnectionStrings__DefaultConnection="Server=.;Database=MyDb;..."
dotnet run

# Env var with nested keys (replace dots with double underscores)
ConnectionStrings__DefaultConnection=...
```

### Environment

Set via:
1. `--environment` CLI argument
2. `ASPNETCORE_ENVIRONMENT` environment variable
3. Default: `Development`

```bash
# Via CLI
dotnet run -- --environment Production

# Via environment variable
export ASPNETCORE_ENVIRONMENT=Production
dotnet run
```

### Debug Mode

Enable detailed logging:
```bash
# Via environment variable
export DEBUG_MIGRATIONS=true
dotnet run
```

## Troubleshooting

### Problem: "Cannot connect to database"

**Causes**:
- Database server not running
- Wrong connection string
- Firewall blocking connection
- Wrong credentials

**Solutions**:
```bash
# Verify connection string
dotnet run -- --connection-string "Server=localhost;Database=test;..."

# Check database server is running
docker ps | grep mssql

# Try connecting manually (SQL Tools installed)
sqlcmd -S localhost -U sa -P "password" -Q "SELECT 1"
```

### Problem: "No pending migrations"

**Cause**: All migrations have been applied.

**Solution**: Normal. Database is up-to-date. Proceed with service deployment.

```bash
# Verify applied migrations
dotnet run -- --list-migrations
```

### Problem: Migration fails with "Timeout"

**Causes**:
- Database server slow to respond
- Large migration taking long
- Network latency

**Solutions**:
```bash
# Increase timeout (if supported)
ConnectionStrings__DefaultConnection="Server=...;Connection Timeout=60;..."

# Run with debug logging
DEBUG_MIGRATIONS=true dotnet run
```

### Problem: "User 'X' lacks permission"

**Cause**: SQL Server user doesn't have required permissions.

**Solution**:
```sql
-- Grant permissions (example for sa user)
GRANT CREATE TABLE, ALTER, DROP, CREATE INDEX to [sa];
GRANT VIEW DEFINITION to [sa];
```

## Best Practices

### 1. **Always Run Migrations Before Services**
- Include migration step in deployment pipeline
- Don't rely on services to migrate on startup
- Makes schema changes explicit and auditable

### 2. **Test Migrations in Non-Production First**
```bash
# Test in Development first
./scripts/migrate.sh development

# Then Production
./scripts/migrate.sh production
```

### 3. **Use Version Control for Migrations**
- Commit `*.cs` migration files to git
- Track schema evolution
- Enables rollback if needed (via down migrations)

### 4. **Monitor Migration Execution**
```bash
# Enable debug mode
DEBUG_MIGRATIONS=true ./scripts/migrate.sh production

# Capture logs for audit trail
dotnet run -- --environment Production > migration.log 2>&1
```

### 5. **Use Read-Only Connection for Health Checks**
- Separate read-only account for health checks
- Production accounts for migrations only

### 6. **Implement Backup Before Migrations**
```bash
# Backup SQL Server database
docker exec mystartupcompany-db \
  /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa \
  -Q "BACKUP DATABASE MyStartUpCompanyDb TO DISK='/var/opt/mssql/backup/db.bak'"

# Then run migrations
./scripts/migrate.sh production
```

### 7. **Separate Development and Production**
```bash
# Development uses local SQLExpress
./scripts/migrate.sh development

# Production uses managed SQL Server
./scripts/migrate.sh production --connection-string "Server=myserver.database.windows.net;..."
```

## API and Worker Startup Paths

Both services **do NOT run migrations** on startup. They only register the DbContext:

### API (`src/MyStartUpCompany.Api/Program.cs`)
```csharp
// Services are registered
builder.Services.AddAppDatabase(builder.Configuration, builder.Environment);

// App runs without any migration calls
var app = builder.Build();
app.Run();
```

### Worker (`src/MyStartUpCompany.Worker/Program.cs`)
```csharp
// Services are registered
builder.Services.AddAppDatabase(builder.Configuration, builder.Environment);

// Host runs without any migration calls
var host = builder.Build();
host.Run();
```

## Migration File Structure

Migrations are stored in `src/MyStartUpCompany.Persistence/Migrations/`:

```
Migrations/
├── 20240101000000_InitialCreate.cs
├── 20240102000000_AddCompanyTable.cs
├── 20240103000000_AddIndexes.cs
└── ...
```

EF Core manages these automatically when you run:
```bash
dotnet ef migrations add <MigrationName> --project src/MyStartUpCompany.Persistence
```

## Next Steps / Future Enhancements

1. **Add migration rollback support** (with down migrations)
2. **Create Azure SQL migration runner task** (DevOps pipeline)
3. **Implement pre-migration backup automation**
4. **Add migration validation/test suite**
5. **Create Terraform/Bicep for Azure SQL infrastructure**
6. **Implement canary deployment** (test migrations on replica first)

## Summary

The separated migration strategy ensures:
- ✅ Migrations never run automatically
- ✅ API and Worker start quickly
- ✅ Schema changes are explicit and auditable
- ✅ Easy to integrate with CI/CD and containers
- ✅ Production-safe deployment approach
- ✅ Flexible execution (CLI, scripts, Docker, Kubernetes)

This is the **recommended approach** for production deployments.
