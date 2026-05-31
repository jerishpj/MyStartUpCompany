# Complete Azure Kubernetes Service (AKS) Deployment Guide

## Overview

This guide provides a complete, production-ready deployment plan for MyStartUpCompany using:
- **Azure Kubernetes Service (AKS)** - Container orchestration
- **Azure Container Registry (ACR)** - Docker image storage
- **Azure SQL Database** - Managed relational database
- **GitHub Actions** - CI/CD automation
- **Application Insights** - Monitoring and logging

## Architecture

```
GitHub Repository
	   ↓
GitHub Actions CI/CD Pipeline
	   ├─ Build & Test
	   ├─ Build Docker Images
	   └─ Push to ACR
			  ↓
		 Azure Container Registry
			  ↓
		 AKS Cluster
	├─ API Service (Pod Replicas)
	├─ Worker Service (Pod Replicas)
	└─ MigrationRunner Job
			  ↓
	Azure SQL Database
			  ↓
	Application Insights (Monitoring)
```

---

## Part 1: Prepare Your Application

### 1.1 Application Code Review

Your application is already well-structured for cloud deployment:

✅ **API Service** (`src/MyStartUpCompany.Api/`)
- Uses environment-based configuration
- Has proper exception handling
- Supports multiple database providers
- Ready for containerization

✅ **Worker Service** (`src/MyStartUpCompany.Worker/`)
- Implements `BackgroundService` pattern
- Supports Azure Service Bus integration
- Has configuration hierarchy
- Multi-stage Dockerfile optimized for production

✅ **MigrationRunner** (`src/MigrationRunner/`)
- Separate migration management
- Can run as Kubernetes Job
- Supports multiple environments
- Explicit schema control

### 1.2 Configuration Best Practices

Your application already follows Azure best practices:

```csharp
// Configuration hierarchy (from appsettings)
1. appsettings.json              // Base configuration
2. appsettings.{Environment}.json // Environment-specific
3. User Secrets (local dev only) // Local development
4. Environment Variables          // Runtime override (HIGHEST priority in Azure)
```

In Azure, environment variables override all local settings, enabling safe secret management.

### 1.3 Database Connection String Format

For Azure SQL Database:

```
Server=tcp:yourserver.database.windows.net,1433;
Initial Catalog=YourDatabase;
User ID=sqladmin;
Password=YourPassword!;
Encrypt=true;
Connection Timeout=30;
TrustServerCertificate=false;
```

---

## Part 2: Azure Infrastructure Setup

### 2.1 Create Resource Group

```bash
# Set variables
RESOURCE_GROUP="mystartup-rg"
LOCATION="eastus"
REGISTRY_NAME="mystartupregistry"
CLUSTER_NAME="mystartup-aks"
SQL_SERVER="mystartup-sql"
SQL_DATABASE="MyStartUpCompanyDb"

# Create resource group
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

echo "Resource group created: $RESOURCE_GROUP"
```

### 2.2 Create Azure Container Registry (ACR)

```bash
# Create ACR
az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $REGISTRY_NAME \
  --sku Basic

# Get login credentials
az acr credential show \
  --resource-group $RESOURCE_GROUP \
  --name $REGISTRY_NAME

echo "Registry URL: $REGISTRY_NAME.azurecr.io"
```

**Save the credentials** - you'll need them for:
- Docker push
- GitHub Secrets
- AKS authentication

### 2.3 Create Azure SQL Database

```bash
# Create SQL Server
az sql server create \
  --resource-group $RESOURCE_GROUP \
  --name $SQL_SERVER \
  --location $LOCATION \
  --admin-user sqladmin \
  --admin-password 'YourSecure@Password123!'

# Create database
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name $SQL_DATABASE \
  --tier Basic \
  --compute-model Serverless

# Configure firewall to allow Azure services
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name "AllowAzureServices" \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Get connection string
CONNECTION_STRING="Server=tcp:$SQL_SERVER.database.windows.net,1433;Initial Catalog=$SQL_DATABASE;User ID=sqladmin;Password=YourSecure@Password123!;Encrypt=true;Connection Timeout=30;"
echo "Connection String: $CONNECTION_STRING"
```

### 2.4 Create AKS Cluster

```bash
# Create AKS cluster with ACR integration
az aks create \
  --resource-group $RESOURCE_GROUP \
  --name $CLUSTER_NAME \
  --node-count 2 \
  --vm-set-type VirtualMachineScaleSets \
  --load-balancer-sku standard \
  --enable-managed-identity \
  --attach-acr $REGISTRY_NAME \
  --generate-ssh-keys

# Get credentials
az aks get-credentials \
  --resource-group $RESOURCE_GROUP \
  --name $CLUSTER_NAME \
  --overwrite-existing

# Verify cluster
kubectl cluster-info
kubectl get nodes
```

### 2.5 Create Key Vault for Secrets

```bash
# Create Key Vault
VAULT_NAME="mystartup-kv"
az keyvault create \
  --name $VAULT_NAME \
  --resource-group $RESOURCE_GROUP

# Store secrets
az keyvault secret set \
  --vault-name $VAULT_NAME \
  --name "DatabaseConnectionString" \
  --value "$CONNECTION_STRING"

az keyvault secret set \
  --vault-name $VAULT_NAME \
  --name "AzureServiceBusConnectionString" \
  --value "Endpoint=sb://your-namespace.servicebus.windows.net/;..."
```

---

## Part 3: Docker Images

### 3.1 Verify Dockerfiles

Your Dockerfiles are production-ready. Key features:

**API Dockerfile** (src/MyStartUpCompany.Api/Dockerfile)
- ✅ Multi-stage build (SDK → Runtime)
- ✅ Alpine base image (minimal size)
- ✅ Non-root user for security
- ✅ Proper port exposure (8080)
- ✅ Health check ready

**Worker Dockerfile** (src/MyStartUpCompany.Worker/Dockerfile)
- ✅ Multi-stage build
- ✅ Alpine runtime
- ✅ Folder structure for I/O
- ✅ Non-root user
- ✅ Optimized for long-running processes

### 3.2 Build and Push Images

```bash
# Build API image
docker build \
  -f src/MyStartUpCompany.Api/Dockerfile \
  -t mystartupregistry.azurecr.io/api:v1.0.0 \
  .

# Build Worker image
docker build \
  -f src/MyStartUpCompany.Worker/Dockerfile \
  -t mystartupregistry.azurecr.io/worker:v1.0.0 \
  .

# Build MigrationRunner image
docker build \
  -f src/MigrationRunner/Dockerfile \
  -t mystartupregistry.azurecr.io/migrations:v1.0.0 \
  .

# Login to ACR
az acr login --name mystartupregistry

# Push images
docker push mystartupregistry.azurecr.io/api:v1.0.0
docker push mystartupregistry.azurecr.io/worker:v1.0.0
docker push mystartupregistry.azurecr.io/migrations:v1.0.0

# Verify images in ACR
az acr repository list --name mystartupregistry
```

---

## Part 4: Kubernetes Configuration

### 4.1 Create Namespace

File: `k8s/namespace.yml`

```yaml
apiVersion: v1
kind: Namespace
metadata:
  name: mystartup
  labels:
	name: mystartup
```

Apply:
```bash
kubectl apply -f k8s/namespace.yml
```

### 4.2 Create Secrets

File: `k8s/secrets.yml`

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: app-secrets
  namespace: mystartup
type: Opaque
stringData:
  database-connection-string: "Server=tcp:mystartup-sql.database.windows.net,1433;Initial Catalog=MyStartUpCompanyDb;User ID=sqladmin;Password=YourSecure@Password123!;Encrypt=true;Connection Timeout=30;"
  azure-servicebus-connectionstring: "Endpoint=sb://your-namespace.servicebus.windows.net/..."
```

Apply:
```bash
kubectl apply -f k8s/secrets.yml
```

### 4.3 Create ConfigMap

File: `k8s/configmap.yml`

```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: app-config
  namespace: mystartup
data:
  ASPNETCORE_ENVIRONMENT: "Production"
  LOG_LEVEL: "Information"
  API_PORT: "8080"
```

### 4.4 Create API Deployment

File: `k8s/api-deployment.yml`

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api
  namespace: mystartup
  labels:
	app: api
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
		image: mystartupregistry.azurecr.io/api:v1.0.0
		imagePullPolicy: Always
		ports:
		- containerPort: 8080
		  name: http

		# Environment variables from ConfigMap and Secrets
		envFrom:
		- configMapRef:
			name: app-config
		env:
		- name: ConnectionStrings__DefaultConnection
		  valueFrom:
			secretKeyRef:
			  name: app-secrets
			  key: database-connection-string

		# Resource requests and limits
		resources:
		  requests:
			memory: "256Mi"
			cpu: "100m"
		  limits:
			memory: "512Mi"
			cpu: "500m"

		# Readiness probe - is container ready to accept traffic?
		readinessProbe:
		  httpGet:
			path: /health/ready
			port: 8080
		  initialDelaySeconds: 10
		  periodSeconds: 10
		  timeoutSeconds: 3
		  failureThreshold: 3

		# Liveness probe - is container still alive?
		livenessProbe:
		  httpGet:
			path: /health/live
			port: 8080
		  initialDelaySeconds: 30
		  periodSeconds: 10
		  timeoutSeconds: 3
		  failureThreshold: 3
```

### 4.5 Create API Service

File: `k8s/api-service.yml`

```yaml
apiVersion: v1
kind: Service
metadata:
  name: api-service
  namespace: mystartup
  labels:
	app: api
spec:
  type: LoadBalancer
  selector:
	app: api
  ports:
  - port: 80
	targetPort: 8080
	protocol: TCP
	name: http
```

### 4.6 Create Worker Deployment

File: `k8s/worker-deployment.yml`

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: worker
  namespace: mystartup
  labels:
	app: worker
spec:
  replicas: 1
  selector:
	matchLabels:
	  app: worker
  template:
	metadata:
	  labels:
		app: worker
	spec:
	  containers:
	  - name: worker
		image: mystartupregistry.azurecr.io/worker:v1.0.0
		imagePullPolicy: Always

		# Environment variables
		envFrom:
		- configMapRef:
			name: app-config
		env:
		- name: ConnectionStrings__DefaultConnection
		  valueFrom:
			secretKeyRef:
			  name: app-secrets
			  key: database-connection-string
		- name: AzureServiceBusSettings__ConnectionString
		  valueFrom:
			secretKeyRef:
			  name: app-secrets
			  key: azure-servicebus-connectionstring

		# Resource requests and limits
		resources:
		  requests:
			memory: "512Mi"
			cpu: "200m"
		  limits:
			memory: "1Gi"
			cpu: "500m"
```

### 4.7 Create MigrationRunner Job

File: `k8s/migration-job.yml`

```yaml
apiVersion: batch/v1
kind: Job
metadata:
  name: migration-runner
  namespace: mystartup
spec:
  backoffLimit: 3
  template:
	spec:
	  serviceAccountName: default
	  restartPolicy: Never
	  containers:
	  - name: migrations
		image: mystartupregistry.azurecr.io/migrations:v1.0.0
		imagePullPolicy: Always

		env:
		- name: ConnectionStrings__DefaultConnection
		  valueFrom:
			secretKeyRef:
			  name: app-secrets
			  key: database-connection-string
		- name: ASPNETCORE_ENVIRONMENT
		  value: "Production"

		resources:
		  requests:
			memory: "256Mi"
			cpu: "100m"
		  limits:
			memory: "512Mi"
			cpu: "500m"
```

---

## Part 5: Deploy to AKS

### 5.1 Manual Deployment Steps

```bash
# 1. Apply namespace
kubectl apply -f k8s/namespace.yml

# 2. Apply secrets and config
kubectl apply -f k8s/secrets.yml
kubectl apply -f k8s/configmap.yml

# 3. Run migrations
kubectl apply -f k8s/migration-job.yml

# Wait for migration to complete
kubectl wait --for=condition=complete job/migration-runner -n mystartup --timeout=300s

# 4. Deploy API
kubectl apply -f k8s/api-deployment.yml
kubectl apply -f k8s/api-service.yml

# 5. Deploy Worker
kubectl apply -f k8s/worker-deployment.yml

# Verify deployments
kubectl get deployments -n mystartup
kubectl get pods -n mystartup
kubectl get services -n mystartup
```

### 5.2 Check Deployment Status

```bash
# View all resources
kubectl get all -n mystartup

# View pod details
kubectl describe pod <pod-name> -n mystartup

# View logs
kubectl logs -f deployment/api -n mystartup
kubectl logs -f deployment/worker -n mystartup

# Get service external IP
kubectl get service api-service -n mystartup -o wide

# Test API
curl http://<EXTERNAL-IP>/health/ready
```

### 5.3 Rollout Updates

```bash
# Update image
kubectl set image deployment/api \
  api=mystartupregistry.azurecr.io/api:v1.0.1 \
  -n mystartup

# Check rollout status
kubectl rollout status deployment/api -n mystartup

# Rollback if needed
kubectl rollout undo deployment/api -n mystartup

# View rollout history
kubectl rollout history deployment/api -n mystartup
```

---

## Part 6: GitHub Actions CI/CD Pipeline

File: `.github/workflows/deploy-to-aks.yml`

```yaml
name: Build and Deploy to AKS

on:
  push:
	branches: [main]
  workflow_dispatch:

env:
  REGISTRY_URL: mystartupregistry.azurecr.io
  RESOURCE_GROUP: mystartup-rg
  CLUSTER_NAME: mystartup-aks

jobs:
  build-and-deploy:
	runs-on: ubuntu-latest

	steps:
	  - uses: actions/checkout@v3

	  - name: Setup .NET
		uses: actions/setup-dotnet@v3
		with:
		  dotnet-version: '10.0.x'

	  - name: Restore dependencies
		run: dotnet restore

	  - name: Build solution
		run: dotnet build -c Release --no-restore

	  - name: Run tests
		run: dotnet test -c Release --no-build

	  - name: Setup Docker Buildx
		uses: docker/setup-buildx-action@v2

	  - name: Login to ACR
		uses: azure/docker-login@v1
		with:
		  login-server: ${{ env.REGISTRY_URL }}
		  username: ${{ secrets.REGISTRY_USERNAME }}
		  password: ${{ secrets.REGISTRY_PASSWORD }}

	  - name: Build and push API image
		uses: docker/build-push-action@v4
		with:
		  context: .
		  file: ./src/MyStartUpCompany.Api/Dockerfile
		  push: true
		  tags: |
			${{ env.REGISTRY_URL }}/api:${{ github.sha }}
			${{ env.REGISTRY_URL }}/api:latest

	  - name: Build and push Worker image
		uses: docker/build-push-action@v4
		with:
		  context: .
		  file: ./src/MyStartUpCompany.Worker/Dockerfile
		  push: true
		  tags: |
			${{ env.REGISTRY_URL }}/worker:${{ github.sha }}
			${{ env.REGISTRY_URL }}/worker:latest

	  - name: Build and push Migration image
		uses: docker/build-push-action@v4
		with:
		  context: .
		  file: ./src/MigrationRunner/Dockerfile
		  push: true
		  tags: |
			${{ env.REGISTRY_URL }}/migrations:${{ github.sha }}
			${{ env.REGISTRY_URL }}/migrations:latest

	  - name: Azure Login
		uses: azure/login@v1
		with:
		  creds: ${{ secrets.AZURE_CREDENTIALS }}

	  - name: Get AKS credentials
		run: |
		  az aks get-credentials \
			--resource-group ${{ env.RESOURCE_GROUP }} \
			--name ${{ env.CLUSTER_NAME }}

	  - name: Update Kubernetes manifests with image
		run: |
		  sed -i 's|IMAGE_TAG|${{ github.sha }}|g' k8s/*.yml

	  - name: Deploy to AKS
		run: |
		  kubectl apply -f k8s/
		  kubectl rollout status deployment/api -n mystartup
		  kubectl rollout status deployment/worker -n mystartup
```

### Required GitHub Secrets

Add these to your GitHub repository (Settings → Secrets):

```
REGISTRY_USERNAME          = <ACR username>
REGISTRY_PASSWORD          = <ACR password>
AZURE_CREDENTIALS          = <Azure service principal JSON>
```

Get Azure credentials:
```bash
az ad sp create-for-rbac \
  --name "github-deployer" \
  --role "Contributor" \
  --scopes "/subscriptions/<SUBSCRIPTION_ID>"
```

---

## Part 7: Monitoring and Logging

### 7.1 Enable Application Insights

```bash
# Create Application Insights
az monitor app-insights component create \
  --app mystartup-insights \
  --location $LOCATION \
  --resource-group $RESOURCE_GROUP \
  --application-type web

# Get instrumentation key
az monitor app-insights component show \
  --app mystartup-insights \
  --resource-group $RESOURCE_GROUP
```

### 7.2 Configure in Application

Add to `Program.cs`:

```csharp
builder.Services.AddApplicationInsightsTelemetry(
	builder.Configuration["ApplicationInsights:InstrumentationKey"]
);

builder.Logging.AddApplicationInsights();
```

### 7.3 View Logs in Portal

- Azure Portal → Application Insights
- Live Metrics Stream → Real-time monitoring
- Performance → Identify bottlenecks
- Failures → Track errors
- Alerts → Set up notifications

---

## Part 8: Security Best Practices

### 8.1 Network Security

```yaml
# Network Policy - restrict traffic
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: api-network-policy
  namespace: mystartup
spec:
  podSelector:
	matchLabels:
	  app: api
  policyTypes:
  - Ingress
  ingress:
  - from:
	- podSelector: {}
	ports:
	- protocol: TCP
	  port: 8080
```

### 8.2 Managed Identity

```bash
# Enable managed identity for AKS
az aks update \
  --resource-group $RESOURCE_GROUP \
  --name $CLUSTER_NAME \
  --enable-managed-identity

# Assign role to access Key Vault
az role assignment create \
  --assignee-object-id <MANAGED_IDENTITY_ID> \
  --assignee-principal-type ServicePrincipal \
  --role "Key Vault Secrets User" \
  --scope /subscriptions/<SUBSCRIPTION_ID>/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.KeyVault/vaults/$VAULT_NAME
```

### 8.3 RBAC Configuration

```bash
# Create service account for deployments
kubectl create serviceaccount deployer -n mystartup

# Create role with limited permissions
kubectl create role deployer \
  --verb=get,list,watch,create,update,patch \
  --resource=deployments,pods \
  -n mystartup

# Bind role to service account
kubectl create rolebinding deployer \
  --role=deployer \
  --serviceaccount=mystartup:deployer \
  -n mystartup
```

---

## Part 9: Cost Optimization

### 9.1 Cluster Autoscaling

```bash
# Enable autoscaler
az aks update \
  --resource-group $RESOURCE_GROUP \
  --name $CLUSTER_NAME \
  --enable-cluster-autoscaler \
  --min-count 2 \
  --max-count 5
```

### 9.2 Pod Autoscaling

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: api-autoscaler
  namespace: mystartup
spec:
  scaleTargetRef:
	apiVersion: apps/v1
	kind: Deployment
	name: api
  minReplicas: 2
  maxReplicas: 5
  metrics:
  - type: Resource
	resource:
	  name: cpu
	  target:
		type: Utilization
		averageUtilization: 70
  - type: Resource
	resource:
	  name: memory
	  target:
		type: Utilization
		averageUtilization: 80
```

### 9.3 Right-sizing Resources

```bash
# Monitor node usage
kubectl top nodes

# Monitor pod usage
kubectl top pods -n mystartup

# Review and adjust resource requests/limits based on metrics
```

---

## Part 10: Troubleshooting

### 10.1 Pod Won't Start

```bash
# Check pod status
kubectl describe pod <pod-name> -n mystartup

# Check logs
kubectl logs <pod-name> -n mystartup

# Check events
kubectl get events -n mystartup --sort-by='.lastTimestamp'
```

### 10.2 Service Connection Issues

```bash
# Test connectivity between pods
kubectl exec -it <pod-name> -n mystartup -- /bin/sh
# Inside pod: ping api-service, curl http://api-service:80

# Check DNS
kubectl run -it --image=busybox:1.28 debug -- /bin/sh
# Inside pod: nslookup api-service.mystartup.svc.cluster.local
```

### 10.3 Database Connection Failed

```bash
# Verify connection string
kubectl get secret app-secrets -n mystartup -o yaml

# Check firewall rules
az sql server firewall-rule list \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER

# Test from pod
kubectl exec -it <pod-name> -n mystartup -- /bin/bash
# Inside pod: sqlcmd -S server.database.windows.net -U user -P password -Q "SELECT 1"
```

### 10.4 Image Pull Errors

```bash
# Verify image exists in ACR
az acr repository list --name mystartupregistry

# Check image pull secret
kubectl get secrets -n mystartup

# Verify credentials
az acr credential show --name mystartupregistry
```

---

## Summary Checklist

- [ ] Azure subscription and CLI installed
- [ ] Resource Group created
- [ ] Azure Container Registry (ACR) created and credentials saved
- [ ] Azure SQL Database created with proper firewall rules
- [ ] AKS Cluster created and running
- [ ] Docker images built and pushed to ACR
- [ ] Kubernetes manifests created (namespace, secrets, configmap, deployments, services, job)
- [ ] Migrations run successfully on AKS
- [ ] API and Worker services deployed and running
- [ ] Services accessible via LoadBalancer
- [ ] GitHub Actions workflow created and working
- [ ] Application Insights configured
- [ ] Monitoring and logging working
- [ ] Security policies applied
- [ ] Autoscaling configured
- [ ] Disaster recovery plan in place

---

## Next Steps

1. **Day 1**: Setup Azure infrastructure (Part 2)
2. **Day 2**: Create Kubernetes manifests (Part 4) and deploy (Part 5)
3. **Day 3**: Configure CI/CD pipeline (Part 6)
4. **Day 4**: Setup monitoring (Part 7) and security (Part 8)
5. **Day 5**: Test, optimize, document

---

**Related Documentation:**
- [LOCAL_DEVELOPMENT.md](LOCAL_DEVELOPMENT.md) - Local development setup
- [CONTAINERIZATION_GUIDE.md](CONTAINERIZATION_GUIDE.md) - Docker fundamentals
- [MIGRATION_STRATEGY.md](MIGRATION_STRATEGY.md) - Database migrations
- [ARCHITECTURE.md](ARCHITECTURE.md) - Application architecture
