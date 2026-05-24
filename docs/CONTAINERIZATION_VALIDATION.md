# Containerization Validation & Testing Guide

This document provides step-by-step validation procedures to test the containerized MyStartUpCompany.Api and MyStartUpCompany.Worker applications.

## Pre-Validation Checklist

- [ ] Docker Desktop installed and running
- [ ] Docker Compose installed (version 2.0+)
- [ ] All files committed to git (no uncommitted changes)
- [ ] `.env` file created with database and Service Bus settings (if using Azure)
- [ ] Repository cloned to local machine

## Build Validation

### Step 1: Validate Dockerfile Syntax

```bash
# Navigate to repository root
cd /path/to/MyStartUpCompany

# Validate API Dockerfile
docker build --dry-run -f src/MyStartUpCompany.Api/Dockerfile .

# Validate Worker Dockerfile
docker build --dry-run -f src/MyStartUpCompany.Worker/Dockerfile .

# Both commands should return without errors if syntax is correct
```

### Step 2: Build Individual Images

```bash
# Build API image (this will take 2-3 minutes)
docker build -t mystartupcompany-api:latest -f src/MyStartUpCompany.Api/Dockerfile .

# Verify build succeeded
docker images | grep mystartupcompany-api

# Expected output:
# mystartupcompany-api   latest   <IMAGE_ID>   <SIZE>   <CREATED>
```

```bash
# Build Worker image
docker build -t mystartupcompany-worker:latest -f src/MyStartUpCompany.Worker/Dockerfile .

# Verify build succeeded
docker images | grep mystartupcompany-worker
```

### Step 3: Build with Docker Compose

```bash
# Build all images defined in docker-compose.yml
docker-compose build

# Monitor build progress
# Expected output shows building api, worker, eventbus services
```

### Step 4: Inspect Built Images

```bash
# Check API image layers
docker history mystartupcompany-api:latest

# Check Worker image layers
docker history mystartupcompany-worker:latest

# Verify image sizes are reasonable
# API: ~200-250MB
# Worker: ~180-220MB
```

---

## Run Validation

### Step 5: Start Services with Docker Compose

```bash
# Create .env file (if needed)
cat > .env << EOF
DB_CONNECTION_STRING=Data Source=tcp:127.0.0.1,1433;Initial Catalog=MyStartUpCompanyDb;User ID=sa;Password=P@ssw0rd123!;Encrypt=false;TrustServerCertificate=true;
EOF

# Start all services in detached mode
docker-compose up -d

# Monitor startup
docker-compose logs -f

# Wait 20-30 seconds for services to fully initialize
```

### Step 6: Verify Services Are Running

```bash
# Check container status
docker-compose ps

# Expected output:
# NAME                      STATUS                PORTS
# mystartupcompany-api      Up (healthy)          0.0.0.0:8080->8080/tcp
# mystartupcompany-worker   Up                    (no ports)
# mystartupcompany-...      Up                    (infrastructure)
```

### Step 7: Test API Connectivity

```bash
# Test API is responding
curl http://localhost:8080/openapi/v1.json

# Expected: JSON OpenAPI specification is returned

# Alternative: Use browser
# Navigate to: http://localhost:8080/openapi/v1.json
```

### Step 8: View Application Logs

```bash
# View API logs (last 50 lines)
docker-compose logs --tail=50 api

# View Worker logs (last 50 lines)
docker-compose logs --tail=50 worker

# Watch logs in real-time
docker-compose logs -f api
# (Press Ctrl+C to exit)
```

### Step 9: Test Container Networking

```bash
# Ping from Worker to API (using container DNS)
docker exec mystartupcompany-worker ping api

# Should resolve to API container IP and show responses
```

---

## Configuration Validation

### Step 10: Verify Environment Variables

```bash
# Check API environment
docker exec mystartupcompany-api env | grep -E "(ASPNETCORE|Connection|Logging)"

# Expected environment:
# ASPNETCORE_ENVIRONMENT=Development
# ASPNETCORE_URLS=http://+:8080
# ConnectionStrings__DefaultConnection=(your connection string)
```

```bash
# Check Worker environment
docker exec mystartupcompany-worker env | grep -E "(DOTNET|Connection|AzureServiceBus|Logging)"

# Expected environment:
# DOTNET_ENVIRONMENT=Development
# ConnectionStrings__DefaultConnection=(your connection string)
# AzureServiceBus__ConnectionString=(if configured)
```

### Step 11: Verify Volume Mounts

```bash
# For Worker, verify Input/Processed folders exist
docker exec mystartupcompany-worker ls -la /app/

# Expected:
# drwxr-xr-x   Input
# drwxr-xr-x   Processed

# Test volume mount by creating a file on host
mkdir -p ./worker-input
echo '{"name":"Test Company"}' > ./worker-input/test.json

# Verify file is visible in container
docker exec mystartupcompany-worker ls /app/Input/

# Expected: test.json appears
```

---

## Database Connectivity Validation

### Step 12: Test Database Connection (if using local SQL Server container)

```bash
# Check if SQL Server container is running (if uncommented in docker-compose.yml)
docker ps | grep mssql

# If using local SQL Server, test connection from API container
docker exec mystartupcompany-api dotnet new console -n DbTest

# Or test with simple SQL command
docker exec mystartupcompany-api /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -Q "SELECT @@VERSION"
```

### Step 13: Verify Database Migrations

```bash
# Apply migrations (if using local SQL Server)
docker exec mystartupcompany-api dotnet ef database update --project src/MyStartUpCompany.Persistence

# Verify tables were created
docker exec mystartupcompany-api /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"
```

---

## API Functional Validation

### Step 14: Test API Endpoints

```bash
# Get all companies (if database is populated)
curl http://localhost:8080/api/companies

# Expected: JSON array of companies (or empty array if no data)

# Get company by ID (replace {id} with actual company ID)
curl http://localhost:8080/api/companies/{id}

# Expected: Single company JSON object (or 404 if not found)
```

### Step 15: Test API Health Check

```bash
# Check API health status
curl -i http://localhost:8080/health

# Expected:
# HTTP/1.1 200 OK
# Content-Type: application/json
# {"status":"Healthy"}
```

---

## Worker Functional Validation

### Step 16: Test Worker File Processing

```bash
# Create a test company JSON file
cat > ./worker-input/company1.json << 'EOF'
[
  {
	"name": "Test Company Inc",
	"address": "123 Business Street",
	"city": "Springfield",
	"postalCode": "12345",
	"country": "USA",
	"phone": "555-1234"
  }
]
EOF

# Monitor Worker logs to see file processing
docker-compose logs --tail=20 -f worker

# Check if file was moved to Processed folder
ls ./worker-processed/

# Expected: File moved with status suffix (company1_success_*.json, etc.)
```

### Step 17: Test Worker Error Handling

```bash
# Create invalid JSON file
echo "{ invalid json }" > ./worker-input/invalid.json

# Monitor logs
docker-compose logs --tail=20 -f worker

# Check Processed folder
ls ./worker-processed/

# Expected: File moved with error suffix (invalid_error_*.json)
```

---

## Performance Validation

### Step 18: Check Image Layer Efficiency

```bash
# Analyze image layers and sizes
docker inspect --format='{{json .RootFS.Layers}}' mystartupcompany-api:latest | jq length

# Get detailed layer information
docker history --human --no-trunc mystartupcompany-api:latest

# Look for:
# - Large COPY operations that could be optimized
# - Unnecessary layers that could be combined
```

### Step 19: Monitor Runtime Performance

```bash
# Check resource usage of containers
docker stats mystartupcompany-api mystartupcompany-worker

# Monitor CPU, memory, network, I/O
# Press Ctrl+C to exit

# Look for:
# - Memory usage (should be < 500MB per container)
# - CPU usage patterns (should be low at rest)
```

---

## Security Validation

### Step 20: Verify Non-Root User

```bash
# Check API is running as non-root
docker exec mystartupcompany-api whoami

# Expected: dotnetuser (UID 1000)

# Check Worker is running as non-root
docker exec mystartupcompany-worker whoami

# Expected: dotnetuser (UID 1000)
```

### Step 21: Verify Minimal Base Image

```bash
# Check API base image
docker inspect mystartupcompany-api:latest | grep -A2 '"RootFS"'

# Expected: Should reference Alpine Linux layers (much smaller than full OS)

# Verify no unnecessary tools are present
docker exec mystartupcompany-api which apt-get

# Expected: (no output - apt-get not installed)
```

---

## Cleanup

### Step 22: Stop and Remove Containers

```bash
# Stop all running containers
docker-compose down

# Remove containers and networks
docker-compose down -v

# Remove unused images
docker image prune -a

# Expected: All containers stopped, networks removed
```

---

## Validation Checklist Summary

- [ ] Dockerfiles build successfully without syntax errors
- [ ] Both API and Worker images build in < 5 minutes
- [ ] Docker Compose starts all services within 30 seconds
- [ ] API container shows healthy status
- [ ] API endpoint responds to HTTP requests
- [ ] Worker container processes files from Input folder
- [ ] Environment variables are correctly set in containers
- [ ] Volume mounts work for Worker file processing
- [ ] Database connection works (if using local SQL Server)
- [ ] Both containers run as non-root user (dotnetuser)
- [ ] Container images use minimal Alpine base
- [ ] Health checks pass for API
- [ ] Resource usage is reasonable (< 1GB combined memory)

---

## Troubleshooting Reference

| Issue | Solution |
|-------|----------|
| Docker daemon not running | Start Docker Desktop or docker service |
| Port 8080 already in use | Change port in docker-compose.yml or stop conflicting container |
| Container exits immediately | Check logs with `docker logs <container-id>` |
| Database connection refused | Verify connection string, ensure SQL Server is accessible |
| Worker doesn't process files | Check Input folder permissions, verify worker is running |
| High memory usage | Check application logs for memory leaks, review heap size settings |

---

## Next Steps After Validation

1. ✅ **Containerization Complete** - All services containerized and tested locally
2. 🔄 **Azure Deployment** - Push images to Azure Container Registry (ACR)
3. 🔄 **Orchestration** - Deploy to Azure Container Instances (ACI) or App Service
4. 🔄 **CI/CD Pipeline** - Add automated builds and deployments

See `docs/AZURE_DEPLOYMENT_GUIDE.md` for the next phase (coming soon).
