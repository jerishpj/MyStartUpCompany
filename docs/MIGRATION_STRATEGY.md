# Database Migration Strategy

This guide explains how database migrations are managed in MyStartUpCompany using a dedicated migration runner.

## Overview

Database migrations are handled explicitly through **MigrationRunner**, a separate console application. This approach ensures:

- ✅ **Safety** - Schema changes are explicit, not automatic
- ✅ **Auditability** - Every migration run is logged and controlled
- ✅ **Flexibility** - Migrations can run on-demand, in CI/CD, or as separate deployment step
- ✅ **Reliability** - Services start quickly without schema overhead

## Quick Start

```powershell
# Apply pending migrations
dotnet run --project src/MigrationRunner

# Preview migrations without applying
dotnet run --project src/MigrationRunner -- --list

# Run with specific environment
dotnet run --project src/MigrationRunner -- --environment Production

# Use custom connection string
dotnet run --project src/MigrationRunner -- --connection-string "Server=localhost;Database=MyDb;..."
```

## Running Migrations

### Using PowerShell Script

```powershell
# Development (default)
.\scripts\migrate.ps1

# Production
.\scripts\migrate.ps1 -Environment Production

# List pending migrations only
.\scripts\migrate.ps1 -ListOnly
```

### Using Bash Script (Linux/macOS)

```bash
# Development (default)
./scripts/migrate.sh

# Production
./scripts/migrate.sh production

# List pending migrations only
./scripts/migrate.sh --list-migrations
```

### Using Docker

```bash
# Run migrations in container
docker-compose -f docker-compose.migrations.yml up --exit-code-from migrations
```

## Creating Migrations

When you modify Entity Framework models:

### Step 1: Update Your Models

Edit entity classes in `src/MyStartUpCompany.Persistence/Entities/`

### Step 2: Create Migration

```powershell
cd src/MyStartUpCompany.Persistence
dotnet ef migrations add YourMigrationName
```

### Step 3: Review Migration

Always review the generated migration in `Migrations/YYYYMMDDhhmmss_YourMigrationName.cs`

### Step 4: Apply Migration

```powershell
dotnet run --project src/MigrationRunner
```

### Step 5: Commit and Push

```bash
git add src/MyStartUpCompany.Persistence/Migrations/
git commit -m "Add migration: YourMigrationName"
```

## Configuration Hierarchy

Configuration priority (highest to lowest):

1. Command-line arguments
2. Environment variables
3. User Secrets (local only)
4. appsettings.{Environment}.json
5. appsettings.json

## Local Development Workflow

```powershell
# 1. Clone repository
git clone https://github.com/jerishpj/MyStartUpCompany.git
cd MyStartUpCompany

# 2. Build solution
dotnet build

# 3. Apply migrations
dotnet run --project src/MigrationRunner

# 4. Run application
dotnet run --project src/MyStartUpCompany.Api
```

## CI/CD Integration

### GitHub Actions

```yaml
- name: Run Migrations
  env:
    ConnectionStrings__DefaultConnection: ${{ secrets.DB_CONNECTION_STRING }}
  run: |
    dotnet run --project src/MigrationRunner -- --environment Production
```

### Azure DevOps

```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Migrations'
  inputs:
    command: 'run'
    projects: 'src/MigrationRunner/MigrationRunner.csproj'
  env:
    ConnectionStrings__DefaultConnection: $(DB_CONNECTION_STRING)
```

## Troubleshooting

### "Connection string not found"

```powershell
# Check appsettings files
cat src/MigrationRunner/appsettings.json

# Check environment variables
$env:ConnectionStrings__DefaultConnection

# Provide via command line
dotnet run --project src/MigrationRunner -- --connection-string "Server=localhost;Database=MyDb;..."
```

### "Database connection failed"

Verify SQL Server is running and connection string is correct.

### "Pending migrations exist"

List and apply pending migrations:

```powershell
dotnet run --project src/MigrationRunner -- --list
dotnet run --project src/MigrationRunner
```

### "Cannot connect to localhost,1433"

If using LocalDB, ensure it's created and started:

```powershell
sqllocaldb create "MyStartUpCompanyDb"
sqllocaldb start "MyStartUpCompanyDb"
```

## Best Practices

- ✅ Always test migrations locally before deployment
- ✅ Use `--list` to preview before applying
- ✅ Review migration file before committing
- ✅ Backup database before production deployment
- ✅ Have a rollback plan
- ✅ Run migrations as separate deployment step

## Related Documentation

- [Local Development Guide](LOCAL_DEVELOPMENT.md) - Full setup instructions
- [Containerization Guide](CONTAINERIZATION_GUIDE.md) - Running in Docker
- [Deployment Guide](DEPLOYMENT_GUIDE.md) - Production deployment
- [Architecture & Design](ARCHITECTURE.md) - Technical decisions
