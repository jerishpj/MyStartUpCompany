# Getting Started - Complete Setup Guide

Welcome to MyStartUpCompany project! This guide will help you set up your development environment securely.

## Table of Contents

1. [Quick Start (5 minutes)](#quick-start)
2. [Local Development Setup](#local-development-setup)
3. [Docker Development](#docker-development)
4. [Azure Deployment](#azure-deployment)
5. [Troubleshooting](#troubleshooting)

---

## Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Visual Studio 2026 Community](https://visualstudio.microsoft.com/) or VS Code
- [SQL Server 2022 (localdb or Docker)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Docker & Docker Compose](https://www.docker.com/products/docker-desktop) (optional, for containerized dev)
- Git & PowerShell

### Setup (Minutes 0-5)

```powershell
# 1. Clone repository
git clone https://github.com/jerishpj/MyStartUpCompany.git
cd MyStartUpCompany

# 2. Initialize User Secrets for local development
cd src\MyStartUpCompany.Worker
dotnet user-secrets init

# 3. Set your Azure Service Bus connection string
# Replace YOUR_NAMESPACE, POLICY, and KEY with actual values
dotnet user-secrets set "AzureServiceBus:ConnectionString" `
  "Endpoint=sb://YOUR_NAMESPACE.servicebus.windows.net/;SharedAccessKeyName=POLICY;SharedAccessKey=KEY"

# 4. Run application
dotnet run
```

**✓ Done!** Your application is running with secure configuration.

---

## Local Development Setup

### Step 1: Clone Repository

```powershell
git clone https://github.com/jerishpj/MyStartUpCompany.git
cd MyStartUpCompany
```

### Step 2: Initialize User Secrets

User Secrets store sensitive configuration locally (outside your project folder) and are **never** committed to version control.

```powershell
cd src\MyStartUpCompany.Worker

# Initialize User Secrets
dotnet user-secrets init

# Output shows your User Secrets ID:
# User secrets initialized. Secrets ID: a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

**Where are secrets stored?**
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<ID>\secrets.json`
- **Mac**: `~/.microsoft/usersecrets/<ID>/secrets.json`
- **Linux**: `~/.microsoft/usersecrets/<ID>/secrets.json`

### Step 3: Set Local Secrets

Get your Azure credentials and set them:

```powershell
# Get from Azure Portal:
# 1. Service Bus Namespace → Shared access policies → RootManageSharedAccessKey
# 2. Copy the Primary Connection String

dotnet user-secrets set "AzureServiceBus:ConnectionString" "Endpoint=sb://YOUR_NAMESPACE.servicebus.windows.net/;..."

# Optional: If using SQL Server authentication (instead of Windows Auth)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" `
  "Server=YOUR_SERVER;Database=MyStartUpCompanyDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"

# Verify secrets are set
dotnet user-secrets list
```

### Step 4: Run in Visual Studio

1. Open `MyStartUpCompany.sln`
2. Set `MyStartUpCompany.Worker` as startup project
3. Press **F5** or click **Run**
4. Check output window for "Azure Service Bus Consumer Service started successfully"

### Step 5: Run from Command Line

```powershell
cd src\MyStartUpCompany.Worker
dotnet run

# Output should show:
# Company File Processor Worker started at: [timestamp]
# Azure Service Bus Consumer Service started successfully. Listening to Topic: companycreatedevent...
```

---

## Docker Development

Docker allows you to run the entire stack locally (SQL Server + Worker service).

### Step 1: Create Environment File

```powershell
# Copy example file
Copy-Item .env.example -Destination .env

# Edit .env with your values
notepad .env
```

**Edit .env:**
```bash
ASPNETCORE_ENVIRONMENT=Docker
AZURE_SERVICEBUS_CONNECTIONSTRING=Endpoint=sb://YOUR_NAMESPACE...
AZURE_SERVICEBUS_TOPICNAME=companycreatedevent
AZURE_SERVICEBUS_SUBSCRIPTIONNAME=MyStartupLocalDevSubscription
SQL_CONNECTION_STRING=Server=mssql,1433;Database=MyStartUpCompanyDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True
MSSQL_SA_PASSWORD=YourStrong@Passw0rd
LOGGING_LOGLEVEL_DEFAULT=Information
```

### Step 2: Copy Docker Compose File

```powershell
# Copy example to actual file
Copy-Item docker-compose.example.yml -Destination docker-compose.yml
```

### Step 3: Run Docker Compose

```powershell
# Start containers
docker-compose up -d

# View logs
docker-compose logs -f worker

# View database logs
docker-compose logs -f mssql

# Stop containers
docker-compose down

# Clean up volumes
docker-compose down -v
```

### Step 4: Test

```powershell
# Check if services are running
docker-compose ps

# Access SQL Server from local machine
# Server: localhost,1433
# User: sa
# Password: [from .env MSSQL_SA_PASSWORD]
```

---

## Azure Deployment

### Prerequisites

- Azure subscription with billing enabled
- Azure CLI installed: `az --version`
- Contributor access to your resource group

### Step 1: Create Azure Resources

```bash
# Set variables
RESOURCE_GROUP=myStartupResourceGroup
LOCATION=eastus
KEYVAULT_NAME=mystartupkv
APP_SERVICE_NAME=mystartupcompany-worker
SERVICEBUS_NAMESPACE=mystartupcompany-sb

# Create resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create Key Vault
az keyvault create --name $KEYVAULT_NAME --resource-group $RESOURCE_GROUP --location $LOCATION

# Create App Service Plan
az appservice plan create --name mystartupplan --resource-group $RESOURCE_GROUP --sku B1 --is-linux

# Create App Service (Worker Service/Container Apps)
az webapp create --resource-group $RESOURCE_GROUP --plan mystartupplan \
  --name $APP_SERVICE_NAME --runtime "DOTNET:10.0"

# Create Service Bus (if not already created)
az servicebus namespace create --name $SERVICEBUS_NAMESPACE --resource-group $RESOURCE_GROUP --sku Standard
```

### Step 2: Add Secrets to Key Vault

```bash
# Service Bus connection string
az keyvault secret set --vault-name $KEYVAULT_NAME --name AzureServiceBusConnectionString \
  --value "Endpoint=sb://YOUR_NAMESPACE.servicebus.windows.net/;..."

# Database connection string
az keyvault secret set --vault-name $KEYVAULT_NAME --name ConnectionStringsDefaultConnection \
  --value "Server=tcp:YOUR_SERVER.database.windows.net;..."
```

### Step 3: Configure GitHub Actions

1. Go to GitHub repository → **Settings** → **Secrets and variables** → **Actions**
2. Add these secrets:

```
AZURE_CREDENTIALS = (JSON output from: az ad sp create-for-rbac --role Owner --scopes /subscriptions/YOUR_SUBSCRIPTION)
AZURE_PUBLISH_PROFILE = (Download from App Service → Deployment center)
AZURE_APP_NAME = mystartupcompany-worker
AZURE_RESOURCE_GROUP = myStartupResourceGroup
KEYVAULT_URI = https://mystartupkv.vault.azure.net/
SERVICEBUS_TOPIC_NAME = companycreatedevent
SERVICEBUS_SUBSCRIPTION_NAME = MyStartupLocalDevSubscription
```

### Step 4: Deploy

```bash
# Push to main branch
git push origin main

# GitHub Actions will:
# 1. Build the application
# 2. Run tests
# 3. Publish to App Service
# 4. Configure app settings from secrets
```

Check GitHub Actions → Workflows for deployment status.

---

## File Structure & Security

### Important Files

```
MyStartUpCompany/
├── .gitignore                          # Excludes secrets files
├── .env.example                        # Template for docker secrets
├── secrets.example.json                # Template for local secrets
├── docker-compose.example.yml          # Template for docker compose
│
├── src/
│   ├── MyStartUpCompany.Worker/
│   │   ├── appsettings.json           # ✓ Checked in (NO secrets)
│   │   ├── appsettings.Development.json # ✓ Checked in (structure only)
│   │   ├── appsettings.Docker.json     # ✓ Checked in (env var placeholders)
│   │   └── Program.cs                  # Configuration hierarchy
│   │
│   └── MyStartUpCompany.Persistence/
│
├── docs/
│   ├── DEVELOPER_SETUP_GUIDE.md       # User Secrets setup
│   └── SECRETS_MANAGEMENT_GUIDE.md    # Comprehensive security guide
│
├── SECRETS_MANAGEMENT_GUIDE.md        # In root
│
└── .github/
	└── workflows/
		└── deploy.yml                  # CI/CD pipeline
```

### What Gets Committed

✓ **Committed to Git (Safe)**
- `appsettings.json` - No secrets, defaults only
- `appsettings.Development.json` - Structure only
- `appsettings.Docker.json` - Structure with placeholders
- `.env.example` - Template showing what to configure
- `secrets.example.json` - Template structure
- All application code
- Documentation

✗ **NOT Committed (In .gitignore)**
- `.env` - Contains actual secrets
- `secrets.json` - Contains actual secrets
- User secrets (stored in user profile)
- IDE temporary files
- Build artifacts

---

## Common Workflows

### I want to work locally

```powershell
# Initial setup (one-time)
cd src\MyStartUpCompany.Worker
dotnet user-secrets init
dotnet user-secrets set "AzureServiceBus:ConnectionString" "..."

# Run application
dotnet run

# Or in Visual Studio: F5
```

### I want to use Docker

```powershell
# Copy files
Copy-Item .env.example .env
Copy-Item docker-compose.example.yml docker-compose.yml

# Edit .env with your credentials
notepad .env

# Run
docker-compose up -d
```

### I want to deploy to Azure

```bash
# Ensure GitHub Actions secrets are configured
# Then just push to main
git push origin main

# Monitor: GitHub → Actions → deploy.yml
```

### I need to rotate Azure credentials

```bash
# 1. Revoke old key in Azure Portal
# 2. Generate new key
# 3. Update in Key Vault:
az keyvault secret set --vault-name mystartupkv --name AzureServiceBusConnectionString --value "NEW_CONNECTION_STRING"
# 4. Application automatically reloads (1 hour interval)
```

---

## Troubleshooting

### "User Secrets not being loaded"

```powershell
# Check if User Secrets are initialized
cd src\MyStartUpCompany.Worker
dotnet user-secrets id

# If empty, initialize
dotnet user-secrets init

# Verify secrets are set
dotnet user-secrets list
```

### "Azure Service Bus Consumer not starting"

Check logs:
```
Look for: "Azure Service Bus connection string not configured"

Solution: Set AzureServiceBus:ConnectionString in:
- Local: dotnet user-secrets set "AzureServiceBus:ConnectionString" "..."
- Docker: Set in .env file
- Azure: Set in App Service Configuration
```

### "Cannot connect to SQL Server from Docker"

```
Error: "Cannot connect to server"

Solution in .env:
- Use: Server=mssql,1433  (service name from docker-compose)
- NOT: Server=localhost,1433
```

### "GitHub Actions deployment failed"

1. Check workflow logs: GitHub → Actions → [Latest run]
2. Verify GitHub secrets are configured: Settings → Secrets and variables
3. Verify Azure service principal has permissions
4. Check App Service logs: Azure Portal → App Service → Log stream

---

## Security Checklist

Before committing code:

- [ ] No Azure credentials in appsettings files
- [ ] `.env` file is in `.gitignore`
- [ ] User Secrets initialized locally
- [ ] GitHub Actions secrets configured
- [ ] Key Vault created in Azure (for production)
- [ ] Rotation plan documented
- [ ] Team members have setup guide

---

## Need Help?

1. **Local Development**: See `docs/DEVELOPER_SETUP_GUIDE.md`
2. **Secrets Management**: See `SECRETS_MANAGEMENT_GUIDE.md` (in root)
3. **Azure Deployment**: See SECRETS_MANAGEMENT_GUIDE.md → Azure Cloud Deployment section
4. **GitHub Actions**: Check workflow file `.github/workflows/deploy.yml`

---

## Additional Resources

- [Microsoft: Configuration in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [Microsoft: Secrets Management](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault Documentation](https://learn.microsoft.com/en-us/azure/key-vault/)
- [Docker Documentation](https://docs.docker.com/)
