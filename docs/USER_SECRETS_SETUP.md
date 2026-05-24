# User Secrets Setup Guide

This guide explains how to use User Secrets for local development in the **MyStartUpCompany.Worker** project.

## Overview

User Secrets is a .NET feature that stores sensitive configuration values locally on your development machine, keeping them out of your source code repository. This is the recommended approach for local development.

**Key Benefits:**
- Sensitive data is never committed to Git
- Each developer can have their own local values
- Simple to set up and use
- Automatically loaded in Development environment

## Prerequisites

- .NET 10 SDK installed
- Project ID in `MyStartUpCompany.Worker.csproj` (already configured)

## Setting Up User Secrets

### Step 1: Initialize User Secrets (One-time)

Open PowerShell and navigate to the worker project:

```powershell
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany\src\MyStartUpCompany.Worker
```

Initialize User Secrets for the project:

```powershell
dotnet user-secrets init
```

This creates a `secrets.json` file in:
```
%APPDATA%\Microsoft\UserSecrets\{ProjectUserSecretsId}\secrets.json
```

The `{ProjectUserSecretsId}` is stored in the `.csproj` file and is unique per project.

### Step 2: Configure Database Connection String

Set your local SQL Server connection string:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=MyStartUpCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

**Examples:**

For SQL Server (local):
```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\mssqllocaldb;Database=MyStartUpCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

For SQL Server (named instance):
```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=COMPUTER\SQLEXPRESS;Database=MyStartUpCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### Step 3: Configure Azure Service Bus (Optional)

If you want to test Azure Service Bus integration locally, set the connection string:

```powershell
dotnet user-secrets set "AzureServiceBus:ConnectionString" "Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YOUR_KEY"
```

Or set individual properties:

```powershell
dotnet user-secrets set "AzureServiceBus:TopicName" "companycreatedevent"
dotnet user-secrets set "AzureServiceBus:SubscriptionName" "MyStartupLocalDevSubscription"
```

## Viewing Your Secrets

To view all configured secrets:

```powershell
dotnet user-secrets list
```

To view a specific secret:

```powershell
dotnet user-secrets list --json
```

## Clearing Secrets

To remove all secrets for this project:

```powershell
dotnet user-secrets clear
```

To remove a specific secret:

```powershell
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

## How It Works

### Configuration Hierarchy (in order of precedence)

When the application starts in **Development** environment, configuration is loaded in this order:

1. **appsettings.json** (checked in, no secrets)
2. **appsettings.Development.json** (checked in, dev-specific logging)
3. **User Secrets** (not checked in, local only)
4. **Environment Variables** (runtime override)

Later sources override earlier ones, so your User Secrets will override values in appsettings.json.

### Automatic Loading

In `Program.cs`, User Secrets are automatically loaded when the environment is **Development**:

```csharp
if (builder.Environment.IsDevelopment())
{
	builder.Configuration.AddUserSecrets<Program>(optional: true);
}
```

No additional code is needed in your application.

## Running the Worker

Once User Secrets are configured, run the worker:

```powershell
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
dotnet run --project src\MyStartUpCompany.Worker
```

The worker will automatically load your User Secrets and connect to the database/Azure Service Bus using those values.

## For Non-Development Environments

- **Docker**: Use environment variables or mounted secret files
- **Azure Cloud**: Use Azure Key Vault via Managed Identity
- **On-Premises**: Use environment variables or config management tools

*(Docker and cloud setup will be added in future phases)*

## Troubleshooting

### Secrets Not Loading

**Check ASPNETCORE_ENVIRONMENT:**
```powershell
echo $env:ASPNETCORE_ENVIRONMENT
```

Should be `Development` for User Secrets to load. Set it if needed:
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

### Connection String Not Found

1. Verify User Secrets are initialized:
```powershell
dotnet user-secrets list
```

2. Check that the secret key matches exactly (case-sensitive after the first level):
```
ConnectionStrings:DefaultConnection
```

3. Verify you're in the correct project directory when setting secrets.

### Rebuild Required

If you added/modified User Secrets while the app is running, you may need to restart the application:
```
Ctrl+C (stop the running app)
dotnet run --project src\MyStartUpCompany.Worker
```

## Security Notes

- User Secrets are stored locally on your machine and are **not** synced to Git or shared
- Each developer maintains their own local `secrets.json` file
- Never commit `secrets.json` or `.gitignore` the User Secrets ID
- When sharing your `MyStartUpCompany.Worker.csproj`, the `UserSecretsId` is committed, but the actual secret values are not

## Next Steps

Once local development is working smoothly with User Secrets, we can add:
- Docker configuration with environment variables
- Azure Key Vault integration for production
- GitHub Actions CI/CD pipeline
