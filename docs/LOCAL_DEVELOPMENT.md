# Local Development Guide

This guide covers setting up your development environment, running services locally, and common development tasks.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Visual Studio 2026 Community](https://visualstudio.microsoft.com/) or VS Code
- [SQL Server 2022 (LocalDB or Docker)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Git and PowerShell

## Quick Start (5 Minutes)

```powershell
# 1. Clone repository
git clone https://github.com/jerishpj/MyStartUpCompany.git
cd MyStartUpCompany

# 2. Build solution
dotnet build

# 3. Apply migrations
dotnet run --project src/MigrationRunner

# 4. Run API
dotnet run --project src/MyStartUpCompany.Api
```

## Complete Setup

### Step 1: Clone Repository

```powershell
git clone https://github.com/jerishpj/MyStartUpCompany.git
cd MyStartUpCompany
```

### Step 2: Initialize Database Migrations

Before running the application, apply all pending migrations:

```powershell
dotnet run --project src/MigrationRunner
```

This ensures your local database schema is up-to-date.

**Preview migrations without applying:**
```powershell
dotnet run --project src/MigrationRunner -- --list
```

### Step 3: Configure Local Secrets (If Needed)

If your application uses Azure Service Bus or other external services:

```powershell
cd src/MyStartUpCompany.Worker

# Initialize User Secrets
dotnet user-secrets init

# Set your Azure Service Bus connection string
dotnet user-secrets set "AzureServiceBus:ConnectionString" `
  "Endpoint=sb://YOUR_NAMESPACE.servicebus.windows.net/;SharedAccessKeyName=POLICY;SharedAccessKey=KEY"

# Verify secrets are set
dotnet user-secrets list
```

**Where are secrets stored?**
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<ID>\secrets.json`
- **Mac/Linux**: `~/.microsoft/usersecrets/<ID>/secrets.json`

### Step 4: Run Services

#### Option A: Run Individual Services

**Run API service:**
```powershell
dotnet run --project src/MyStartUpCompany.Api

# API will be available at: http://localhost:5000
```

**Run Worker service (in separate terminal):**
```powershell
dotnet run --project src/MyStartUpCompany.Worker
```

**Run Notifier service (in separate terminal):**
```powershell
dotnet run --project src/MyStartUpCompany.Notifier
```

#### Option B: Run in Visual Studio

1. Open `MyStartUpCompany.sln` in Visual Studio
2. Set startup project (choose one or multiple):
   - `MyStartUpCompany.Api`
   - `MyStartUpCompany.Worker`
   - `MyStartUpCompany.Notifier`
3. Press **F5** or click **Run**

#### Option C: Run with Docker Compose

```powershell
# Start all services with SQL Server
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

## Build & Test

### Build Solution

```powershell
dotnet build
```

### Run All Tests

```powershell
dotnet test
```

### Run Specific Test Project

```powershell
dotnet test tests/MyStartUpCompany.Api.Tests
dotnet test tests/MyStartUpCompany.Worker.Tests
```

## Working with Migrations

### View All Migrations

```powershell
dotnet run --project src/MigrationRunner -- --list
```

### Apply Migrations

```powershell
dotnet run --project src/MigrationRunner
```

### Create New Migration

When you modify your Entity Framework models:

```powershell
cd src/MyStartUpCompany.Persistence

# Create migration
dotnet ef migrations add YourMigrationName

# Remove last migration (if needed)
dotnet ef migrations remove
```

Then commit the migration and apply it with MigrationRunner before deployment.

See also: [Migration Strategy Guide](MIGRATION_STRATEGY.md)

## Project Structure

```
MyStartUpCompany/
├── src/
│   ├── MyStartUpCompany.Api/           # REST API endpoints
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── appsettings.json
│   │   └── Dockerfile
│   │
│   ├── MyStartUpCompany.Worker/        # Background worker
│   │   ├── Services/
│   │   ├── BackgroundTasks/
│   │   └── appsettings.json
│   │
│   ├── MyStartUpCompany.Persistence/   # Data layer
│   │   ├── DbContext/
│   │   ├── Entities/
│   │   ├── Migrations/
│   │   └── Extensions/
│   │
│   ├── MyStartUpCompany.Notifier/      # Notification service
│   │   └── Services/
│   │
│   └── MigrationRunner/                # Explicit migration tool
│       ├── Program.cs
│       └── MigrationRunner.csproj
│
├── tests/
│   ├── MyStartUpCompany.Api.Tests/
│   └── MyStartUpCompany.Worker.Tests/
│
└── docs/                                # Documentation
```

## Configuration

### Configuration Files

- `appsettings.json` - Base configuration (committed)
- `appsettings.Development.json` - Development overrides (optional)
- `appsettings.Docker.json` - Docker overrides (committed)
- User Secrets - Sensitive local values (not committed)

### Environment Variables

Key configuration can be set via environment variables:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=MyDb;..."
```

### appsettings.json Example

```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Information"
	}
  },
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyStartUpCompanyDb;Trusted_Connection=true;"
  }
}
```

## Common Development Tasks

### Add NuGet Package

```powershell
cd src/MyStartUpCompany.Api
dotnet add package PackageName
```

### Update NuGet Packages

```powershell
dotnet nuget update check
dotnet nuget update
```

### Clean Build Artifacts

```powershell
dotnet clean
rm -r -Force bin/,obj/ -ErrorAction SilentlyContinue
```

### Format Code

```powershell
dotnet format
```

### View Entity Framework Database Info

```powershell
cd src/MyStartUpCompany.Persistence

# Show pending migrations
dotnet ef migrations list

# Show current database state
dotnet ef database info
```

## Troubleshooting

### Build Fails: "Project X not found"

**Solution:** Ensure you're in the correct directory:
```powershell
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet build
```

### Migration Fails: "Connection to database failed"

**Solution:** Verify connection string:
```powershell
# Check configured connection string
$env:ConnectionStrings__DefaultConnection

# Ensure SQL Server is running
```

### Port Already in Use

**Error:** "Address already in use"

**Solution:**
```powershell
# Find process using port 5000
netstat -ano | findstr :5000

# Kill process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

### User Secrets Not Found

**Solution:**
```powershell
# List all secrets
dotnet user-secrets list

# Clear all secrets if needed
dotnet user-secrets clear

# Re-initialize
dotnet user-secrets init
```

## Debug Mode

### Visual Studio Debugging

1. Set breakpoints in your code
2. Press **F5** to start debugging
3. Application pauses at breakpoints
4. Use Debug menu for step over/into/out

### Console Application Debugging

```powershell
# Run with debug output
set ASPNETCORE_ENVIRONMENT=Development
dotnet run --project src/MigrationRunner
```

## Performance Tips

- Use **Release configuration** for performance testing
- Enable **Tiered Compilation** for faster startup
- Run unit tests in parallel: `dotnet test -- RunConfiguration.MaxCpuCount=4`

## IDE Setup

### Visual Studio Keyboard Shortcuts

- **F5** - Start/Resume debugging
- **F10** - Step over
- **F11** - Step into
- **Ctrl+K, Ctrl+C** - Comment code
- **Ctrl+K, Ctrl+U** - Uncomment code
- **Ctrl+Shift+B** - Build solution

### VS Code Extensions

Recommended extensions for .NET development:
- C# (powered by OmniSharp)
- C# Dev Kit
- .NET Runtime Install Tool
- REST Client

## Next Steps

- Learn about [Database Migrations](MIGRATION_STRATEGY.md)
- Explore [Project Architecture](ARCHITECTURE.md)
- Run [Tests](#build--test)
- Build [Docker containers](CONTAINERIZATION_GUIDE.md)
- Deploy to [Production](DEPLOYMENT_GUIDE.md)
