# Containerization Guide for MyStartUpCompany

This guide explains how to containerize and run the MyStartUpCompany.Api and MyStartUpCompany.Worker services using Docker and Docker Compose.

## Prerequisites

- Docker Desktop or Docker Engine installed (version 20.10+)
- Docker Compose installed (version 2.0+)
- .NET 10 SDK (for building locally without containers)

## Project Structure

```
src/
├── MyStartUpCompany.Api/          # REST API service
│   ├── Dockerfile                 # Multi-stage build for API
│   ├── appsettings.json           # Base configuration
│   ├── appsettings.Development.json
│   └── appsettings.Production.json
├── MyStartUpCompany.Worker/       # Background worker service
│   ├── Dockerfile                 # Multi-stage build for Worker
│   ├── appsettings.json           # Base configuration
│   ├── appsettings.Development.json
│   └── appsettings.Production.json
└── MyStartUpCompany.Persistence/  # Shared EF Core data layer
	└── Migrations/                # Database migrations
```

---

## Quick Start: Local Development with Docker Compose

### 1. Build and Start All Services

```bash
# Build all containers and start services in detached mode
docker-compose up -d

# Or with verbose output (for debugging)
docker-compose up
```

This will:
- Build the API container from `src/MyStartUpCompany.Api/Dockerfile`
- Build the Worker container from `src/MyStartUpCompany.Worker/Dockerfile`
- Start all services in the `mystartupcompany-network`
- Create volumes for Worker file processing (`worker-input`, `worker-processed`)

### 2. Verify Services are Running

```bash
# List running containers
docker-compose ps

# Check API is responsive
curl http://localhost:8080/openapi/v1.json

# View API logs
docker-compose logs -f api

# View Worker logs
docker-compose logs -f worker
```

### 3. Stop Services

```bash
# Stop all services (containers remain)
docker-compose stop

# Stop and remove all containers, networks, volumes
docker-compose down

# Remove volumes as well (caution: deletes data)
docker-compose down -v
```

---

## Building Individual Containers

### Build API Container Only

```bash
# Build the API image
docker build -t mystartupcompany-api:latest -f src/MyStartUpCompany.Api/Dockerfile .

# Run the API container
docker run -d \
  --name mystartupcompany-api \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e "ConnectionStrings__DefaultConnection=Data Source=tcp:127.0.0.1,1433;Initial Catalog=MyStartUpCompanyDb;User ID=sa;Password=P@ssw0rd123!;Encrypt=false;TrustServerCertificate=true;" \
  mystartupcompany-api:latest
```

### Build Worker Container Only

```bash
# Build the Worker image
docker build -t mystartupcompany-worker:latest -f src/MyStartUpCompany.Worker/Dockerfile .

# Run the Worker container
docker run -d \
  --name mystartupcompany-worker \
  -e DOTNET_ENVIRONMENT=Development \
  -e "ConnectionStrings__DefaultConnection=Data Source=tcp:127.0.0.1,1433;Initial Catalog=MyStartUpCompanyDb;User ID=sa;Password=P@ssw0rd123!;Encrypt=false;TrustServerCertificate=true;" \
  -v /path/to/worker-input:/app/Input \
  -v /path/to/worker-processed:/app/Processed \
  mystartupcompany-worker:latest
```

---

## Configuration via Environment Variables

Both containers support configuration through environment variables. The .NET configuration system automatically loads these with the format `ConnectionStrings__DefaultConnection`, `AzureServiceBus__ConnectionString`, etc.

### API Container Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name (Development/Staging/Production) | `Production` |
| `ASPNETCORE_URLS` | URLs to listen on | `http://+:8080` |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | (empty) |
| `Logging__LogLevel__Default` | Default log level | `Warning` |

### Worker Container Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `DOTNET_ENVIRONMENT` | Environment name (Development/Staging/Production) | `Production` |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | (empty) |
| `AzureServiceBus__ConnectionString` | Service Bus connection string | (empty) |
| `AzureServiceBus__TopicName` | Topic name | `companycreatedevent` |
| `AzureServiceBus__SubscriptionName` | Subscription name | `worker-subscription` |
| `AzureServiceBus__MaxConcurrentCalls` | Max concurrent message handlers | `5` |
| `AzureServiceBus__AutoCompleteMessages` | Auto-complete processed messages | `false` |
| `Logging__LogLevel__Default` | Default log level | `Warning` |

---

## Database Configuration

### Using Local SQL Server (Docker Compose)

Uncomment the `mssql` service in `docker-compose.yml`:

```yaml
mssql:
  image: mcr.microsoft.com/mssql/server:2022-latest
  environment:
	SA_PASSWORD: P@ssw0rd123!
	ACCEPT_EULA: Y
  ports:
	- "1433:1433"
```

Then apply migrations:

```bash
# Inside the container or with dotnet CLI
dotnet ef database update --project src/MyStartUpCompany.Persistence
```

### Using Azure SQL Database

Set the connection string via environment variable:

```bash
export DB_CONNECTION_STRING="Server=tcp:your-server.database.windows.net,1433;Initial Catalog=MyStartUpCompanyDb;Persist Security Info=False;User ID=youradmin;Password=YourPassword;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;"

docker-compose up -d
```

---

## Azure Service Bus Configuration

### Setting Service Bus Connection in Docker Compose

Add to your `.env` file or pass as environment variables:

```bash
export SERVICE_BUS_CONNECTION_STRING="Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YourKey=="
export SERVICE_BUS_TOPIC_NAME="companycreatedevent"
export SERVICE_BUS_SUBSCRIPTION_NAME="worker-subscription"

docker-compose up -d
```

Or create a `.env` file in the repository root:

```
DB_CONNECTION_STRING=Data Source=tcp:127.0.0.1,1433;Initial Catalog=MyStartUpCompanyDb;User ID=sa;Password=P@ssw0rd123!;Encrypt=false;TrustServerCertificate=true;
SERVICE_BUS_CONNECTION_STRING=Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YourKey==
SERVICE_BUS_TOPIC_NAME=companycreatedevent
SERVICE_BUS_SUBSCRIPTION_NAME=worker-subscription
```

Docker Compose will automatically load variables from `.env`.

---

## Container Image Details

### API Container (`mystartupcompany-api`)

**Base Image:** `mcr.microsoft.com/dotnet/aspnet:10.0-alpine`

**Key Features:**
- Multi-stage build for minimal image size (~200MB)
- Non-root user (`dotnetuser:1000`) for enhanced security
- Health check configured (checks OpenAPI endpoint)
- Exposes port 8080 (HTTP) and 8081 (HTTPS)

**Build Process:**
1. **Build Stage:** Compiles .NET code using SDK
2. **Publish Stage:** Creates optimized publish output
3. **Runtime Stage:** Copies only required files to minimal Alpine image

**Image Size:** ~200-250MB (with curl for health checks)

### Worker Container (`mystartupcompany-worker`)

**Base Image:** `mcr.microsoft.com/dotnet/runtime:10.0-alpine`

**Key Features:**
- Multi-stage build for minimal image size (~180MB)
- Uses runtime image (no ASP.NET Core)
- Non-root user for security
- Pre-creates Input/Processed folders for file processing
- No exposed ports (background service)

**Build Process:**
1. **Build Stage:** Compiles .NET code
2. **Publish Stage:** Creates optimized output
3. **Runtime Stage:** Copies only runtime files

**Image Size:** ~180-220MB

---

## Troubleshooting

### Container Won't Start

```bash
# Check container logs
docker logs <container-id>

# Get more details
docker inspect <container-id>

# Run interactively to debug
docker run -it --entrypoint /bin/sh mystartupcompany-api:latest
```

### Database Connection Issues

```bash
# Test connection from inside container
docker exec <container-id> curl -v http://localhost:8080/openapi/v1.json

# Verify environment variables
docker exec <container-id> env | grep Connection
```

### Service Bus Connection Failed

1. Verify connection string format: `Endpoint=sb://namespace.servicebus.windows.net/;...`
2. Check that topic and subscription exist
3. Verify shared access key has `Listen` and `Send` permissions
4. Check firewall rules allow connection from container

### Volume Mount Issues (Worker)

```bash
# Check volume mount is working
docker exec mystartupcompany-worker ls -la /app/Input

# Verify files are synced from host
ls -la ./worker-input/
```

---

## Docker Compose Networking

All services communicate through the `mystartupcompany-network` bridge network:

- **API Container:** Accessible as `api:8080` within the network
- **Worker Container:** Accessible as `worker` within the network
- **Service Bus:** Uses environment variables for connection

To test connectivity between containers:

```bash
# Ping from Worker to API
docker exec mystartupcompany-worker curl http://api:8080/openapi/v1.json
```

---

## Security Best Practices

1. **Non-Root User:** Both containers run as `dotnetuser:1000` (not root)
2. **Minimal Base Images:** Alpine-based images reduce attack surface
3. **Multi-Stage Builds:** Only runtime dependencies in final image
4. **Environment Variables:** Secrets injected at runtime, not in images
5. **Health Checks:** API container includes health checks for orchestration
6. **.dockerignore:** Excludes unnecessary files from build context

---

## Performance Optimization

### Image Size Reduction

- Alpine base images: ~50MB (vs ~700MB for full Framework)
- Multi-stage builds: Only production dependencies in final image
- .dockerignore: Eliminates unnecessary files from build context

### Build Speed

- Separate restore step: Docker caches dependency layer
- Project file copying before source: Maximizes layer caching

### Runtime Performance

- Alpine Linux: Minimal overhead
- Non-root user: Required for Kubernetes and security scanning

---

## Next Steps

1. **Local Testing:** Run `docker-compose up` and verify both services work
2. **Azure Deployment:** Use these images with Azure Container Registry (ACR) and Azure Container Instances (ACI) or App Service
3. **CI/CD Integration:** Add Docker build steps to your GitHub Actions or Azure Pipelines
4. **Kubernetes:** Deploy containers to AKS with proper health checks and resource limits

See `docs/AZURE_DEPLOYMENT_GUIDE.md` for Azure-specific deployment instructions (coming in next phase).

---

## References

- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [.NET in Docker](https://github.com/dotnet/dotnet-docker)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Alpine Linux in Containers](https://alpinelinux.org/)
