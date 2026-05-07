# MyStartUpCompany Solution

A .NET 10 solution demonstrating Entity Framework Core migrations with SQL Server, featuring a clean architecture approach for managing company data.

## 📋 Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Database Migrations](#database-migrations)
- [Configuration](#configuration)
- [Troubleshooting](#troubleshooting)

## 🎯 Overview

This solution showcases best practices for managing Entity Framework Core migrations in a .NET 10 application. It includes:

- **Entity Framework Core** for data access
- **SQL Server** as the database provider
- **Design-time DbContext factory** for migration support
- **PowerShell scripts** for streamlined migration commands
- **Company entity** with validation and configuration

## ✅ Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express, or full version)
- [Visual Studio 2026](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- Git (for version control)

## 🚀 Getting Started

### 1. Clone the Repository

git clone <your-repo-url> cd MyStartUpCompany

### 2. Restore .NET Tools

The project uses .NET local tools to ensure consistent EF Core tooling versions across the team.

```bash
dotnet tool restore
```

This will install `dotnet-ef` as specified in `.config/dotnet-tools.json`.

### 3. Configure Database Connection

Update the connection string in `src/MyStartUpCompany.Persistence/appsettings.json`:

**Connection String Examples:**

- **SQL Server LocalDB** (default):
  ```json
  "Server=(localdb)\\mssqllocaldb;Database=MyStartUpCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  ```

### 4. Create and Apply Initial Migration

.\scripts\ef.ps1 add InitialCreate

.\scripts\ef.ps1 update

## 🗄️ Database Migrations

### Using PowerShell Helper Scripts (Recommended)

The solution includes a convenient PowerShell script (`scripts/ef.ps1`) to simplify common migration tasks.

#### Create a New Migration

**Example:**

````````

````````

#### Apply Migrations to Database

````````
.\scripts\ef.ps1 update
````````

This command applies all pending migrations to the database.

#### List All Migrations

````````
.\scripts\ef.ps1 list
````````

Shows all migrations and their status (pending/applied).

#### Remove Last Migration

````````
.\scripts\ef.ps1 remove
````````

This command removes the last applied migration.

⚠️ **Warning:** Only removes migrations that haven't been applied to the database.

#### Drop Database

````````
.\scripts\ef.ps1 drop
````````

Drops the entire database. Use with caution!

⚠️ **Warning:** Only removes migrations that haven't been applied to the database.

#### Drop Database

````````
.\scripts\ef.ps1 drop
````````

Drops the entire database. Use with caution!

### Using .NET CLI

Alternatively, you can use the .NET CLI commands directly:

#### Create a New Migration

```bash
dotnet ef migrations add <MigrationName> --project src/MyStartUpCompany.Persistence --startup-project src/MyStartUpCompany.Api
```

#### Apply Migrations to Database

```bash
dotnet ef database update --project src/MyStartUpCompany.Persistence --startup-project src/MyStartUpCompany.Api
```

#### Remove Last Migration

```bash
dotnet ef migrations remove --project src/MyStartUpCompany.Persistence --startup-project src/MyStartUpCompany.Api
```

## ⚙️ Configuration

### AppSettings Configuration

The main application settings are located in `src/MyStartUpCompany.Api/appsettings.json`. Key settings include:

- **ConnectionStrings**: Database connection strings
- **Logging**: Logging configuration
- **AllowedHosts**: Host filtering

### Environmental Variables

For sensitive data or environment-specific settings, use environmental variables:

```bash
# Example: Setting the ASPNETCORE_ENVIRONMENT variable
$env:ASPNETCORE_ENVIRONMENT="Development"
```

### User Secrets (Development Only)

For development purposes, you can use ASP.NET Core's Secret Manager to store secrets locally:

```bash
dotnet user-secrets set "DbPassword" "YourSecurePassword"
```

## 🚧 Troubleshooting

- **Common Issues**:
  - If you encounter issues, ensure your SQL Server instance is running and the connection string is correct.
  - For migration problems, check the output logs for specific error messages.

- **FAQs**:
  - **Q: How do I change the database provider?**
    A: Update the `DbContext` options in `src/MyStartUpCompany.Persistence/MyStartUpCompanyDbContext.cs` and modify the connection string accordingly.

  - **Q: Can I use a different version of .NET?**
    A: This solution is built on .NET 10. While it may work on other versions, it is not guaranteed.

