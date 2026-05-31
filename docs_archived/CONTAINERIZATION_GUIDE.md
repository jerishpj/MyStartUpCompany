# Containerization Guide

This guide covers building and running MyStartUpCompany in Docker containers.

## Prerequisites

- Docker Desktop or Docker Engine (version 20.10+)
- Docker Compose (version 2.0+)
- Git

## Quick Start

Run everything locally with Docker Compose:

```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

This starts:
- SQL Server database
- API service
- Worker service
- Migration runner

## Building Docker Images

### Build All Images

```bash
docker-compose build
```

### Build Specific Service

**API Service:**
```bash
docker build -f src/MyStartUpCompany.Api/Dockerfile -t mystartupcompany-api:latest .
```

**Worker Service:**
```bash
docker build -f src/MyStartUpCompany.Worker/Dockerfile -t mystartupcompany-worker:latest .
```

**MigrationRunner:**
```bash
docker build -f src/MigrationRunner/Dockerfile -t mystartupcompany-migrations:latest .
```

## Running Containers

### Run With Docker Compose

```bash
# Start all services
docker-compose up -d

# View service status
docker-compose ps

# View logs for specific service
docker-compose logs -f api
docker-compose logs -f worker
docker-compose logs -f mssql

# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

### Run Individual Container

**Run API:**
```bash
docker run -d \
  --name api \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e "ConnectionStrings__DefaultConnection=Server=localhost;Database=MyDb;..." \
  mystartupcompany-api:latest
```

**Run Worker:**
```bash
docker run -d \
  --name worker \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e "ConnectionStrings__DefaultConnection=Server=localhost;Database=MyDb;..." \
  mystartupcompany-worker:latest
```

**Run Migrations:**
```bash
docker run --rm \
  -e "ConnectionStrings__DefaultConnection=Server=localhost;Database=MyDb;..." \
  mystartupcompany-migrations:latest
```

## Docker Compose Configuration

### File Structure

```
docker-compose.yml              Main orchestration file
docker-compose.override.yml     Development overrides (optional)
.env.example                    Environment variables template
.dockerignore                   Docker build exclusions
```

### Services

**mssql** - SQL Server database
- Image: mcr.microsoft.com/mssql/server:2022-latest
- Port: 1433
- Volume: sqlserver_data

**api** - REST API service
- Built from src/MyStartUpCompany.Api/Dockerfile
- Port: 8080
- Depends on: mssql

**worker** - Background worker service
- Built from src/MyStartUpCompany.Worker/Dockerfile
- Depends on: mssql

## Configuration via Environment Variables

Create `.env` file for local development:

```bash
# Database
DB_USER=sa
DB_PASSWORD=YourPassword123!
DB_NAME=MyStartUpCompanyDb

# API
API_PORT=8080
ASPNETCORE_ENVIRONMENT=Development

# Logging
LOG_LEVEL=Information
```

Load with docker-compose:
```bash
docker-compose --env-file .env up
```

## Dockerfile Best Practices

### Multi-Stage Build

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyStartUpCompany.Api.dll"]
```

**Benefits:**
- Smaller final image (SDK removed)
- Faster deployments
- Better security

## Port Mapping

| Service | Internal Port | External Port |
|---------|---------------|---------------|
| API | 8080 | 8080 |
| Database | 1433 | 1433 |

Access locally:
- **API**: http://localhost:8080
- **Database**: localhost:1433

## Networking

### Docker Compose Network

Services communicate via service names:

```
api ──→ mssql
worker ──→ mssql
```

**Connection string in container:**
```
Server=mssql,1433;Database=MyStartUpCompanyDb;User Id=sa;Password=...
```

## Volumes

### Data Persistence

Database volume keeps data across container restarts:

```yaml
volumes:
  sqlserver_data:
    driver: local
```

## Debugging

### View Container Logs

```bash
# Follow logs (tail -f)
docker-compose logs -f api

# Show last 100 lines
docker-compose logs --tail 100 api
```

### Execute Commands in Container

```bash
# Run command
docker-compose exec api ls -la

# Interactive shell
docker-compose exec api /bin/bash

# Run migration inside container
docker-compose exec mssql sqlcmd -S localhost -U sa -P "password" -Q "SELECT name FROM sys.databases"
```

### Inspect Container

```bash
# See environment variables
docker inspect api | grep Env

# See mounts
docker inspect api | grep Mounts

# See network settings
docker inspect api | grep Network
```

## Performance Optimization

### Multi-stage Builds

Reduces image size:
```
Before: 500 MB (includes SDK)
After: 200 MB (runtime only)
```

### Image Caching

Optimize layer caching:
```dockerfile
# Good: caches restore
COPY src/MyStartUpCompany.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish
```

### Resource Limits

```bash
docker run \
  --memory 512m \
  --cpus 1 \
  mystartupcompany-api:latest
```

## Security Considerations

### Don't Hardcode Secrets

Use environment variables instead of hardcoded secrets in Dockerfile.

### Use .dockerignore

```
.git
.gitignore
.vs
bin/
obj/
*.user
.env
secrets.json
```

### Non-Root User

```dockerfile
RUN useradd -m appuser
USER appuser
```

## Troubleshooting

### Port Already in Use

```bash
# Find process using port 8080
lsof -i :8080

# Kill process
kill -9 <PID>

# Or use different port
docker run -p 9000:8080 ...
```

### Container Won't Start

```bash
# Check logs
docker logs <container_id>

# Check exit code
docker inspect <container_id> | grep ExitCode

# Run with interactive shell for debugging
docker run -it mystartupcompany-api:latest /bin/bash
```

### Database Connection Failed

```bash
# Verify database is running
docker-compose ps mssql

# Test connection from app container
docker-compose exec api bash
# Inside container:
sqlcmd -S mssql -U sa -P "password" -Q "SELECT 1"
```

### Out of Disk Space

```bash
# See disk usage
docker system df

# Clean up unused images
docker image prune -a

# Clean up volumes
docker volume prune

# Full cleanup
docker system prune -a --volumes
```

## Production Deployment

### Push to Registry

```bash
# Tag image
docker tag mystartupcompany-api:latest myregistry.azurecr.io/mystartupcompany-api:latest

# Login to registry
docker login myregistry.azurecr.io

# Push image
docker push myregistry.azurecr.io/mystartupcompany-api:latest
```

### Deploy to Azure Container Instances

```bash
az container create \
  --resource-group mygroup \
  --name mystartupcompany-api \
  --image myregistry.azurecr.io/mystartupcompany-api:latest \
  --ports 8080 \
  --environment-variables ASPNETCORE_ENVIRONMENT=Production
```

### Deploy to Kubernetes (AKS)

```bash
# Create deployment
kubectl create deployment api --image=myregistry.azurecr.io/mystartupcompany-api:latest

# Expose service
kubectl expose deployment api --port=8080 --type=LoadBalancer

# View status
kubectl get pods
kubectl get services
```

## Related Documentation

- [Local Development Guide](LOCAL_DEVELOPMENT.md) - Development setup
- [Migration Strategy](MIGRATION_STRATEGY.md) - Database management
- [Deployment Guide](DEPLOYMENT_GUIDE.md) - Production deployment
- [Architecture & Design](ARCHITECTURE.md) - Technical decisions
