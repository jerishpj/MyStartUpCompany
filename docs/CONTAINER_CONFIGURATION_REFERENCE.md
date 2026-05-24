# Container Configuration Reference

This document provides a comprehensive reference for all configuration options available when running MyStartUpCompany containers.

## Configuration Overview

Both API and Worker containers support configuration through:
1. **appsettings.json** files (base configuration, checked in)
2. **appsettings.{ENVIRONMENT}.json** files (environment-specific, checked in)
3. **Environment Variables** (injected at runtime via Docker Compose or deployment platform)

The .NET configuration system loads them in order, with later sources overriding earlier ones:
```
appsettings.json 
  → appsettings.{ENVIRONMENT}.json 
	→ Environment Variables (highest priority)
```

---

## Environment Variable Naming Convention

.NET uses `:` (colon) in JSON configuration but converts to `__` (double underscore) for environment variables:

```json
// JSON structure
{
  "ConnectionStrings": {
	"DefaultConnection": "..."
  },
  "Logging": {
	"LogLevel": {
	  "Default": "Information"
	}
  }
}

// Environment variables
ConnectionStrings__DefaultConnection=...
Logging__LogLevel__Default=Information
```

---

## API Container Configuration

### Environment Variables

#### Core Application Settings

| Variable | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `ASPNETCORE_ENVIRONMENT` | string | No | `Production` | Environment: Development, Staging, Production |
| `ASPNETCORE_URLS` | string | No | `http://+:8080` | Listening URLs (use `+` for all interfaces) |
| `ASPNETCORE_HTTPS_PORT` | int | No | `8081` | HTTPS port |

#### Connection String

| Variable | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `ConnectionStrings__DefaultConnection` | string | Yes | (empty) | SQL Server connection string |

**Connection String Format Examples:**

Local SQL Server (in container):
```
Data Source=tcp:mssql,1433;Initial Catalog=MyStartUpCompanyDb;User ID=sa;Password=P@ssw0rd123!;Encrypt=false;TrustServerCertificate=true;
```

Azure SQL Database:
```
Server=tcp:myserver.database.windows.net,1433;Initial Catalog=MyStartUpCompanyDb;Persist Security Info=False;User ID=adminuser;Password=YourPassword;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;
```

Local development (LocalDB):
```
Server=(localdb)\mssqllocaldb;Database=MyStartUpCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true
```

#### Logging Configuration

| Variable | Type | Default | Description |
|----------|------|---------|-------------|
| `Logging__LogLevel__Default` | string | `Information` | Default log level: Critical, Error, Warning, Information, Debug, Trace, None |
| `Logging__LogLevel__Microsoft` | string | `Warning` | Logging level for Microsoft.* namespaces |
| `Logging__LogLevel__Microsoft.AspNetCore` | string | `Warning` | Logging level for ASP.NET Core |

#### API Security

| Variable | Type | Default | Description |
|----------|------|---------|-------------|
| `AllowedHosts` | string | `*` | Comma-separated allowed hosts (Host header validation) |

---

## Worker Container Configuration

### Environment Variables

#### Core Application Settings

| Variable | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `DOTNET_ENVIRONMENT` | string | No | `Production` | Environment: Development, Staging, Production |

#### Connection String

| Variable | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `ConnectionStrings__DefaultConnection` | string | Yes | (empty) | SQL Server connection string (same format as API) |

#### Azure Service Bus Configuration

| Variable | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `AzureServiceBus__ConnectionString` | string | No | (empty) | Service Bus connection string |
| `AzureServiceBus__TopicName` | string | No | `companycreatedevent` | Topic name to listen to |
| `AzureServiceBus__SubscriptionName` | string | No | `worker-subscription` | Subscription name |
| `AzureServiceBus__MaxConcurrentCalls` | int | No | `5` | Max concurrent message handlers |
| `AzureServiceBus__MaxAutoLockRenewalDuration` | int | No | `300` | Lock renewal duration in seconds |
| `AzureServiceBus__AutoCompleteMessages` | bool | No | `false` | Auto-complete messages after processing |

**Service Bus Connection String Format:**
```
Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YourAccessKey==
```

#### Logging Configuration

| Variable | Type | Default | Description |
|----------|------|---------|-------------|
| `Logging__LogLevel__Default` | string | `Information` | Default log level |
| `Logging__LogLevel__Microsoft` | string | `Warning` | Logging level for Microsoft.* namespaces |
| `Logging__LogLevel__Microsoft.Hosting.Lifetime` | string | `Information` | Worker lifetime events |

---

## Docker Compose Environment Configuration

### Using .env File

Create a `.env` file in the repository root:

```bash
# Database Configuration
DB_CONNECTION_STRING=Data Source=tcp:127.0.0.1,1433;Initial Catalog=MyStartUpCompanyDb;User ID=sa;Password=P@ssw0rd123!;Encrypt=false;TrustServerCertificate=true;

# Azure Service Bus Configuration
SERVICE_BUS_CONNECTION_STRING=Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YourKey==
SERVICE_BUS_TOPIC_NAME=companycreatedevent
SERVICE_BUS_SUBSCRIPTION_NAME=worker-subscription
```

Docker Compose automatically loads variables from `.env` and can reference them in `docker-compose.yml`:

```yaml
environment:
  ConnectionStrings__DefaultConnection: ${DB_CONNECTION_STRING}
  AzureServiceBus__ConnectionString: ${SERVICE_BUS_CONNECTION_STRING}
```

### Command-Line Environment Variables

```bash
# Export environment variables before running docker-compose
export DB_CONNECTION_STRING="Data Source=..."
export SERVICE_BUS_CONNECTION_STRING="Endpoint=..."

docker-compose up -d
```

### Override in docker-compose.yml

```yaml
services:
  api:
	environment:
	  ASPNETCORE_ENVIRONMENT: Production
	  ConnectionStrings__DefaultConnection: "your-connection-string"
```

---

## Development vs Production Configuration

### Development Environment (`ASPNETCORE_ENVIRONMENT=Development` / `DOTNET_ENVIRONMENT=Development`)

**Enabled by default:**
- OpenAPI/Swagger documentation
- Detailed error pages
- SQL query logging
- More verbose logging

**Configuration loading:**
```
appsettings.json → appsettings.Development.json → Environment Variables
```

**Example setup for local dev with Docker Compose:**
```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Development
  Logging__LogLevel__Default: Debug
```

### Production Environment (default)

**Enabled by default:**
- Minimal error details
- Performance optimizations
- Standard logging

**Configuration loading:**
```
appsettings.json → appsettings.Production.json → Environment Variables
```

**Example setup for production:**
```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Production
  Logging__LogLevel__Default: Warning
```

---

## Common Configuration Scenarios

### Scenario 1: Local Development with Local SQL Server Container

```bash
# docker-compose.yml or .env
ASPNETCORE_ENVIRONMENT=Development
DOTNET_ENVIRONMENT=Development
DB_CONNECTION_STRING=Data Source=tcp:mssql,1433;Initial Catalog=MyStartUpCompanyDb;User ID=sa;Password=P@ssw0rd123!;Encrypt=false;TrustServerCertificate=true;
SERVICE_BUS_CONNECTION_STRING=  # Leave empty, will skip Service Bus
```

### Scenario 2: Azure SQL Database with Azure Service Bus

```bash
DB_CONNECTION_STRING=Server=tcp:myserver.database.windows.net,1433;Initial Catalog=MyStartUpCompanyDb;Persist Security Info=False;User ID=adminuser;Password=YourPassword;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;
SERVICE_BUS_CONNECTION_STRING=Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YourKey==
SERVICE_BUS_TOPIC_NAME=companycreatedevent
SERVICE_BUS_SUBSCRIPTION_NAME=worker-subscription
```

### Scenario 3: Production with Minimal Logging

```bash
ASPNETCORE_ENVIRONMENT=Production
DOTNET_ENVIRONMENT=Production
Logging__LogLevel__Default=Warning
Logging__LogLevel__Microsoft=Error
```

---

## Configuration Validation

### Verify Configuration is Loaded

```bash
# Check API configuration at runtime
docker exec mystartupcompany-api env | grep -E "ASPNETCORE|Connection|Logging"

# Check Worker configuration
docker exec mystartupcompany-worker env | grep -E "DOTNET|Connection|AzureServiceBus"
```

### Test Database Connection

```bash
# Attempt connection from API container
docker exec mystartupcompany-api curl -v http://localhost:8080/api/companies

# Check logs for connection errors
docker-compose logs api | grep -i connection
```

### Test Service Bus Connection

```bash
# Monitor Worker logs for Service Bus connection
docker-compose logs worker | grep -i "service bus"

# Check for message processing activity
docker-compose logs worker | grep -i "message"
```

---

## Troubleshooting Configuration Issues

### Issue: "Connection string not found" or empty

**Solution:**
1. Verify environment variable is set: `docker exec <container> env | grep Connection`
2. Check connection string format is correct for your database type
3. Verify database is accessible from container network
4. Check firewall rules allow container to connect

### Issue: API/Worker won't start with configuration error

**Solution:**
1. Check logs: `docker-compose logs api`
2. Verify JSON format of connection string (no unescaped quotes)
3. Validate environment variable names use `__` not `:` or `.`
4. Check appsettings.{ENVIRONMENT}.json files for syntax errors

### Issue: Service Bus messages not being processed

**Solution:**
1. Verify connection string format: `Endpoint=sb://...`
2. Check Worker logs: `docker-compose logs worker | grep -i "service bus"`
3. Verify topic and subscription exist in Service Bus namespace
4. Confirm subscription has "Listen" permissions in shared access policy

### Issue: High memory or CPU usage

**Solution:**
1. Check logging level - reduce verbosity in Production
2. Monitor with: `docker stats mystartupcompany-api`
3. Check application logs for loops or memory leaks
4. Review `MaxConcurrentCalls` setting in Worker (reduce if too high)

---

## Security Considerations

### Secrets Management Best Practices

1. **Never commit secrets to source control:**
   - `.gitignore` should include `.env`
   - Connection strings should never be in checked-in files

2. **Use environment variables at runtime:**
   - Docker Compose with `.env` (local dev only)
   - Azure Key Vault (production)
   - Kubernetes Secrets (orchestrated environments)

3. **Principle of least privilege:**
   - Database user should have minimal required permissions
   - Service Bus shared access key should be scoped to needed operations

4. **Rotate credentials regularly:**
   - Change database passwords periodically
   - Rotate Service Bus access keys
   - Update connection strings in deployment systems

---

## References

- [ASP.NET Core Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [.NET Generic Host Configuration](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host)
- [Docker Environment Variables](https://docs.docker.com/compose/environment-variables/)
- [Azure SQL Connection Strings](https://docs.microsoft.com/en-us/azure/azure-sql/database/connection-strings-odbc)
- [Azure Service Bus Connection Strings](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-connection-string-to-resource)
