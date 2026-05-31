# Deployment Guide

This guide covers deploying MyStartUpCompany to production using various platforms and methods.

## Overview

Deployment involves:
1. **Apply migrations** - Update database schema
2. **Build & push image** - Create container image
3. **Deploy services** - Start application services
4. **Verify deployment** - Test endpoints and health

## Prerequisites

- Docker and Docker Compose
- Azure CLI (for Azure deployment)
- kubectl (for Kubernetes deployment)
- Git
- GitHub Actions or Azure DevOps configured

## CI/CD Pipeline

### GitHub Actions Workflow

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy

on:
  push:
	branches: [main]

jobs:
  deploy:
	runs-on: ubuntu-latest

	steps:
	  - uses: actions/checkout@v3

	  - name: Setup .NET
		uses: actions/setup-dotnet@v3
		with:
		  dotnet-version: '10.0.x'

	  - name: Build
		run: dotnet build -c Release

	  - name: Test
		run: dotnet test

	  - name: Run Migrations
		env:
		  ConnectionStrings__DefaultConnection: ${{ secrets.DATABASE_CONNECTION_STRING }}
		run: |
		  dotnet run --project src/MigrationRunner -- \
			--environment Production

	  - name: Build Docker Image
		run: |
		  docker build -f src/MyStartUpCompany.Api/Dockerfile \
			-t myregistry.azurecr.io/mystartupcompany-api:${{ github.sha }} .
		  docker tag myregistry.azurecr.io/mystartupcompany-api:${{ github.sha }} \
			myregistry.azurecr.io/mystartupcompany-api:latest

	  - name: Push to Registry
		run: |
		  docker login -u ${{ secrets.REGISTRY_USERNAME }} \
			-p ${{ secrets.REGISTRY_PASSWORD }} myregistry.azurecr.io
		  docker push myregistry.azurecr.io/mystartupcompany-api:${{ github.sha }}
		  docker push myregistry.azurecr.io/mystartupcompany-api:latest

	  - name: Deploy to Azure Container Instances
		run: |
		  az container create \
			--resource-group ${{ secrets.RESOURCE_GROUP }} \
			--name mystartupcompany-api-prod \
			--image myregistry.azurecr.io/mystartupcompany-api:latest \
			--registry-login-server myregistry.azurecr.io \
			--registry-username ${{ secrets.REGISTRY_USERNAME }} \
			--registry-password ${{ secrets.REGISTRY_PASSWORD }} \
			--ports 8080 \
			--environment-variables \
			  ASPNETCORE_ENVIRONMENT=Production \
			  ConnectionStrings__DefaultConnection=${{ secrets.DATABASE_CONNECTION_STRING }}
```

### GitHub Secrets

Configure these secrets in GitHub repository settings:

| Secret | Value |
|--------|-------|
| `REGISTRY_USERNAME` | Azure Container Registry username |
| `REGISTRY_PASSWORD` | Azure Container Registry password |
| `RESOURCE_GROUP` | Azure resource group name |
| `DATABASE_CONNECTION_STRING` | Production database connection string |

### Azure DevOps Pipeline

Create `azure-pipelines.yml`:

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  registry: 'myregistry.azurecr.io'
  imageName: 'mystartupcompany-api'

stages:
  - stage: Build
	jobs:
	  - job: BuildAndTest
		steps:
		  - task: UseDotNet@2
			inputs:
			  version: '10.0.x'

		  - task: DotNetCoreCLI@2
			displayName: 'Build'
			inputs:
			  command: 'build'
			  arguments: '-c Release'

		  - task: DotNetCoreCLI@2
			displayName: 'Test'
			inputs:
			  command: 'test'

		  - task: Docker@2
			displayName: 'Build Image'
			inputs:
			  command: 'build'
			  Dockerfile: 'src/MyStartUpCompany.Api/Dockerfile'
			  tags: |
				$(registry)/$(imageName):latest
				$(registry)/$(imageName):$(Build.BuildId)

		  - task: Docker@2
			displayName: 'Push Image'
			inputs:
			  command: 'push'
			  repository: '$(registry)/$(imageName)'

  - stage: Deploy
	dependsOn: Build
	condition: succeeded()
	jobs:
	  - job: DeployToProduction
		steps:
		  - task: AzureCLI@2
			displayName: 'Run Migrations'
			inputs:
			  azureSubscription: 'AzureSubscription'
			  scriptType: 'bash'
			  scriptLocation: 'inlineScript'
			  inlineScript: |
				dotnet run --project src/MigrationRunner -- \
				  --environment Production \
				  --connection-string "$(DATABASE_CONNECTION_STRING)"

		  - task: AzureContainerApps@0
			displayName: 'Deploy to Container Apps'
			inputs:
			  azureSubscription: 'AzureSubscription'
			  containerAppName: 'mystartupcompany-api'
			  resourceGroup: '$(RESOURCE_GROUP)'
			  imageToDeploy: '$(registry)/$(imageName):latest'
```

## Azure Deployment

### Setup Azure Resources

```bash
# Variables
RESOURCE_GROUP=mystartup-prod
LOCATION=eastus
REGISTRY_NAME=mystartupregistry
CONTAINER_APP_NAME=mystartupcompany-api

# Create resource group
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Create Container Registry
az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $REGISTRY_NAME \
  --sku Basic

# Create SQL Server
az sql server create \
  --resource-group $RESOURCE_GROUP \
  --name mystartup-sql-server \
  --admin-user sqladmin \
  --admin-password "YourSecure@Password123"

# Create database
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server mystartup-sql-server \
  --name MyStartUpCompanyDb \
  --service-objective Basic

# Create Key Vault
az keyvault create \
  --resource-group $RESOURCE_GROUP \
  --name mystartup-kv

# Store secrets
az keyvault secret set \
  --vault-name mystartup-kv \
  --name db-connection-string \
  --value "Server=tcp:mystartup-sql-server.database.windows.net,1433;Initial Catalog=MyStartUpCompanyDb;User ID=sqladmin;Password=YourSecure@Password123;Encrypt=true;Connection Timeout=30;"
```

### Deploy to Container Apps

```bash
# Build and push image
docker build -f src/MyStartUpCompany.Api/Dockerfile -t mystartupcompany-api:latest .
docker tag mystartupcompany-api:latest mystartupregistry.azurecr.io/mystartupcompany-api:latest
az acr build --registry mystartupregistry --image mystartupcompany-api:latest .

# Get database connection string
DB_CONN=$(az keyvault secret show \
  --vault-name mystartup-kv \
  --name db-connection-string \
  --query value -o tsv)

# Deploy Container App
az containerapp create \
  --name $CONTAINER_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --image mystartupregistry.azurecr.io/mystartupcompany-api:latest \
  --target-port 8080 \
  --ingress external \
  --environment-variables \
	ASPNETCORE_ENVIRONMENT=Production \
	"ConnectionStrings__DefaultConnection=$DB_CONN" \
  --registry-server mystartupregistry.azurecr.io
```

### Deploy to AKS (Kubernetes)

```bash
# Create AKS cluster
az aks create \
  --resource-group $RESOURCE_GROUP \
  --name mystartup-aks \
  --node-count 2 \
  --vm-set-type VirtualMachineScaleSets

# Get credentials
az aks get-credentials \
  --resource-group $RESOURCE_GROUP \
  --name mystartup-aks

# Create namespace
kubectl create namespace mystartup

# Create secret for database connection
kubectl create secret generic db-secret \
  --from-literal=connection-string="Server=tcp:mystartup-sql-server.database.windows.net,1433;..." \
  --namespace mystartup

# Deploy API
kubectl apply -f - <<EOF
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api
  namespace: mystartup
spec:
  replicas: 2
  selector:
	matchLabels:
	  app: api
  template:
	metadata:
	  labels:
		app: api
	spec:
	  containers:
	  - name: api
		image: mystartupregistry.azurecr.io/mystartupcompany-api:latest
		ports:
		- containerPort: 8080
		env:
		- name: ASPNETCORE_ENVIRONMENT
		  value: "Production"
		- name: ConnectionStrings__DefaultConnection
		  valueFrom:
			secretKeyRef:
			  name: db-secret
			  key: connection-string
---
apiVersion: v1
kind: Service
metadata:
  name: api-service
  namespace: mystartup
spec:
  type: LoadBalancer
  selector:
	app: api
  ports:
  - port: 80
	targetPort: 8080
EOF

# Check deployment status
kubectl get pods -n mystartup
kubectl get services -n mystartup
```

## Deployment Checklist

Before deploying to production:

- [ ] All tests pass
- [ ] Code reviewed and approved
- [ ] Database migrations reviewed
- [ ] Docker image built and tested
- [ ] Secrets configured in Key Vault/Pipeline
- [ ] Health checks configured
- [ ] Monitoring/logging configured
- [ ] Rollback plan documented
- [ ] Database backup taken
- [ ] Load balancer configured
- [ ] DNS updated (if needed)
- [ ] SSL certificate configured
- [ ] Rate limiting configured
- [ ] API keys rotated
- [ ] Deployment announcement sent

## Rolling Deployment

### Strategy: Blue-Green

Deploy new version alongside old, then switch traffic:

```bash
# Deploy new version (green)
kubectl set image deployment/api api=mystartupregistry.azurecr.io/mystartupcompany-api:v2 \
  --namespace mystartup

# Wait for new pods to be ready
kubectl rollout status deployment/api -n mystartup

# Switch traffic to new version (automatic via deployment)
# If needed, rollback:
kubectl rollout undo deployment/api -n mystartup
```

### Strategy: Canary

Route percentage of traffic to new version:

```bash
# Deploy with traffic split (requires Istio or similar)
kubectl apply -f canary-deployment.yaml

# Monitor metrics
kubectl logs -f deployment/api -n mystartup

# Complete rollout when stable
kubectl apply -f production-deployment.yaml
```

## Health Checks

### Readiness Probe

Service is ready to accept traffic:

```yaml
readinessProbe:
  httpGet:
	path: /health/ready
	port: 8080
  initialDelaySeconds: 10
  periodSeconds: 10
```

### Liveness Probe

Service is still running:

```yaml
livenessProbe:
  httpGet:
	path: /health/live
	port: 8080
  initialDelaySeconds: 30
  periodSeconds: 10
```

## Monitoring & Logging

### Application Insights

```csharp
// In Program.cs
builder.Services.AddApplicationInsights();
```

### Azure Monitor

```bash
# View logs
az monitor log-analytics query \
  --workspace-id $WORKSPACE_ID \
  --analytics-query "ContainerAppConsoleLogs | take 50"
```

### Alerts

```bash
# Create alert for high CPU
az monitor metrics alert create \
  --name high-cpu-alert \
  --resource-group $RESOURCE_GROUP \
  --scopes /subscriptions/$SUBSCRIPTION/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.App/containerApps/api \
  --condition "avg Percentage CPU > 80" \
  --window-size 5m
```

## Rollback

### Rollback Kubernetes Deployment

```bash
# View history
kubectl rollout history deployment/api -n mystartup

# Rollback to previous version
kubectl rollout undo deployment/api -n mystartup

# Rollback to specific revision
kubectl rollout undo deployment/api --to-revision=2 -n mystartup
```

### Rollback Database

```bash
# If migration failed, restore from backup
# 1. Stop services
# 2. Restore database from backup
# 3. Restart services
```

## Performance Tuning

### Scale Deployment

```bash
# Scale to 5 replicas
kubectl scale deployment/api --replicas=5 -n mystartup

# Autoscaling based on CPU
kubectl autoscale deployment/api --min=2 --max=10 --cpu-percent=70 -n mystartup
```

### Resource Limits

```yaml
resources:
  requests:
	memory: "256Mi"
	cpu: "250m"
  limits:
	memory: "512Mi"
	cpu: "500m"
```

## Troubleshooting Deployment

### Deployment Won't Start

```bash
# Check events
kubectl describe pod <pod-name> -n mystartup

# View logs
kubectl logs <pod-name> -n mystartup

# Check resource availability
kubectl top nodes
kubectl top pods -n mystartup
```

### Connection String Issues

```bash
# Verify secret is set
kubectl get secrets -n mystartup
kubectl describe secret db-secret -n mystartup

# View environment variables in pod
kubectl exec <pod-name> -n mystartup -- env | grep Connection
```

### Service Unreachable

```bash
# Check service
kubectl get svc -n mystartup

# Test connectivity
kubectl port-forward svc/api-service 8080:80 -n mystartup
curl http://localhost:8080/health/ready
```

## Disaster Recovery

### Backup Strategy

- Daily database backups (Azure SQL automatic)
- Container images pushed to registry
- Infrastructure as Code (Terraform/Bicep)

### Recovery Procedure

1. **Database failed:**
   - Restore from backup using `az sql db restore`
   - Reapply migrations if needed

2. **Application crashed:**
   - Kubernetes automatically restarts pods
   - Manual rollback if needed

3. **Entire region down:**
   - Use geo-redundant backup to recover in another region

## Security in Production

- ✅ Secrets in Key Vault, not in code or images
- ✅ HTTPS/TLS for all communications
- ✅ Network policies to restrict traffic
- ✅ Managed identity for Azure authentication
- ✅ Regular security scanning of images
- ✅ Database encryption at rest and in transit
- ✅ Audit logging enabled

## Next Steps

- [Local Development Guide](LOCAL_DEVELOPMENT.md) - Development setup
- [Containerization Guide](CONTAINERIZATION_GUIDE.md) - Docker commands
- [Migration Strategy](MIGRATION_STRATEGY.md) - Database management
- [Architecture & Design](ARCHITECTURE.md) - Technical decisions
