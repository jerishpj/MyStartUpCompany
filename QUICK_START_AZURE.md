# Quick Start: Azure AKS Deployment

## TL;DR - 5 Minute Quick Start

### 1. Prerequisites
```powershell
# Install Azure CLI, kubectl, Docker
# Login to Azure
az login
az account set --subscription "<YOUR_SUBSCRIPTION_ID>"
```

### 2. Create Azure Infrastructure
```powershell
# Set variables
$ResourceGroup = "mystartup-rg"
$Location = "eastus"
$ClusterName = "mystartup-aks"
$RegistryName = "mystartupregistry"

# Create resource group
az group create --name $ResourceGroup --location $Location

# Create Container Registry
az acr create --resource-group $ResourceGroup --name $RegistryName --sku Basic

# Create SQL Server and Database
az sql server create --resource-group $ResourceGroup --name mystartup-sql `
  --location $Location --admin-user sqladmin --admin-password "YourSecure@Password123!"

az sql db create --resource-group $ResourceGroup --server mystartup-sql `
  --name MyStartUpCompanyDb --tier Basic

# Create AKS Cluster
az aks create --resource-group $ResourceGroup --name $ClusterName --node-count 2 `
  --attach-acr $RegistryName --generate-ssh-keys
```

### 3. Build and Push Docker Images
```powershell
# Get ACR URL
$RegistryUrl = "$RegistryName.azurecr.io"

# Login to ACR
az acr login --name $RegistryName

# Build images
docker build -f src/MyStartUpCompany.Api/Dockerfile -t $RegistryUrl/api:latest .
docker build -f src/MyStartUpCompany.Worker/Dockerfile -t $RegistryUrl/worker:latest .
docker build -f src/MigrationRunner/Dockerfile -t $RegistryUrl/migrations:latest .

# Push images
docker push $RegistryUrl/api:latest
docker push $RegistryUrl/worker:latest
docker push $RegistryUrl/migrations:latest
```

### 4. Get AKS Credentials
```powershell
az aks get-credentials --resource-group $ResourceGroup --name $ClusterName --overwrite-existing
```

### 5. Create Secrets in AKS
```powershell
# Get your Azure SQL connection string
$ConnectionString = "Server=tcp:mystartup-sql.database.windows.net,1433;Initial Catalog=MyStartUpCompanyDb;User ID=sqladmin;Password=YourSecure@Password123!;Encrypt=true;Connection Timeout=30;"

# Get your Service Bus connection string (if used)
$ServiceBusConnectionString = "Endpoint=sb://namespace.servicebus.windows.net/;..."

# Create namespace and secrets
kubectl create namespace mystartup
kubectl create secret generic app-secrets `
  --from-literal=database-connection-string=$ConnectionString `
  --from-literal=azure-servicebus-connectionstring=$ServiceBusConnectionString `
  -n mystartup
```

### 6. Deploy to AKS
```powershell
# Run the deployment script
./k8s/deploy.ps1 -ResourceGroup $ResourceGroup -ClusterName $ClusterName `
  -RegistryName $RegistryName -Namespace "mystartup"

# Or manually apply manifests
kubectl apply -f k8s/02-configmap.yml
kubectl apply -f k8s/06-migration-job.yml
kubectl apply -f k8s/04-api-deployment.yml
kubectl apply -f k8s/05-worker-deployment.yml
```

### 7. Verify Deployment
```powershell
# Check pods
kubectl get pods -n mystartup

# Get service endpoint
kubectl get svc api-service -n mystartup

# Test API
$IP = kubectl get svc api-service -n mystartup -o jsonpath='{.status.loadBalancer.ingress[0].ip}'
curl http://$IP/health/ready
```

---

## File Reference

| File | Purpose |
|------|---------|
| `docs/AZURE_AKS_DEPLOYMENT_GUIDE.md` | Complete deployment guide (10 parts) |
| `AZURE_DEPLOYMENT_SUMMARY.md` | High-level summary of deliverables |
| `k8s/deploy.ps1` | PowerShell deployment automation (Windows) |
| `k8s/deploy.sh` | Bash deployment automation (Linux/Mac) |
| `.github/workflows/deploy-to-aks.yml` | GitHub Actions CI/CD pipeline |
| `k8s/01-namespace.yml` | Kubernetes namespace |
| `k8s/02-configmap.yml` | Configuration |
| `k8s/03-secrets.yml` | Secrets template |
| `k8s/04-api-deployment.yml` | API deployment/service/HPA |
| `k8s/05-worker-deployment.yml` | Worker deployment/HPA |
| `k8s/06-migration-job.yml` | Database migration job |
| `k8s/07-network-policies.yml` | Network security policies |
| `k8s/08-pod-disruption-budget.yml` | Pod disruption budgets |

---

## GitHub Actions Setup

### 1. Create Service Principal
```powershell
az ad sp create-for-rbac --name "github-deployer" --role "Contributor" `
  --scopes "/subscriptions/<SUBSCRIPTION_ID>"
```

### 2. Add GitHub Secrets
Go to **Settings → Secrets → Actions** and add:
- `AZURE_CLIENT_ID` - from service principal
- `AZURE_TENANT_ID` - from service principal
- `AZURE_SUBSCRIPTION_ID` - your subscription ID
- `REGISTRY_USERNAME` - from ACR
- `REGISTRY_PASSWORD` - from ACR

### 3. Push to Main Branch
```powershell
git push origin main
```

GitHub Actions will automatically:
1. ✅ Build .NET solution
2. ✅ Run tests
3. ✅ Build Docker images
4. ✅ Push to ACR
5. ✅ Deploy to AKS
6. ✅ Run smoke tests

---

## Useful Commands

### View Resources
```powershell
# Pods
kubectl get pods -n mystartup -o wide

# Services
kubectl get svc -n mystartup -o wide

# Deployments
kubectl get deployments -n mystartup

# Jobs
kubectl get jobs -n mystartup
```

### View Logs
```powershell
# API logs (last 50 lines, follow)
kubectl logs -f deployment/api -n mystartup --tail=50

# Worker logs
kubectl logs -f deployment/worker -n mystartup

# Migration job logs
kubectl logs -l job-name=migration-runner -n mystartup

# Specific pod
kubectl logs <pod-name> -n mystartup
```

### Scale Deployments
```powershell
# Manual scale
kubectl scale deployment api --replicas=3 -n mystartup

# View HPA status
kubectl get hpa -n mystartup
```

### Port Forward
```powershell
# Forward API port to local machine
kubectl port-forward svc/api-service 8080:80 -n mystartup

# Access locally
curl http://localhost:8080/health/ready
```

### Execute Commands in Pod
```powershell
# Get shell access
kubectl exec -it <pod-name> -n mystartup -- /bin/bash

# Run command
kubectl exec <pod-name> -n mystartup -- dotnet --version
```

### Troubleshoot
```powershell
# Pod details
kubectl describe pod <pod-name> -n mystartup

# Pod events
kubectl get events -n mystartup --sort-by='.lastTimestamp'

# Deployment status
kubectl rollout status deployment/api -n mystartup

# View deployment history
kubectl rollout history deployment/api -n mystartup

# Rollback
kubectl rollout undo deployment/api -n mystartup
```

---

## Monitoring

### View in Azure Portal
1. Go to your AKS Cluster resource
2. Click "Insights" → "Metrics"
3. View CPU, memory, network usage
4. Go to Application Insights resource
5. View application telemetry

### Azure CLI Monitoring
```powershell
# Get node metrics
kubectl top nodes

# Get pod metrics
kubectl top pods -n mystartup

# View cluster health
az aks show --resource-group $ResourceGroup --name $ClusterName
```

---

## Troubleshooting Quick Fixes

### Pod won't start
```powershell
# Check pod status
kubectl describe pod <pod-name> -n mystartup

# Check logs
kubectl logs <pod-name> -n mystartup

# Common: Image pull error
# → Verify ACR login: az acr login --name mystartupregistry
# → Verify image exists: az acr repository list --name mystartupregistry
```

### Service not accessible
```powershell
# Check service status
kubectl get svc api-service -n mystartup

# If EXTERNAL-IP is <pending>
# → Wait a few moments, Azure needs to provision LoadBalancer IP
# → Check if Azure has resources to allocate
```

### Migrations failed
```powershell
# Check migration job
kubectl get job migration-runner -n mystartup

# View logs
kubectl logs -l job-name=migration-runner -n mystartup

# Describe job
kubectl describe job migration-runner -n mystartup

# Re-run migration
kubectl delete job migration-runner -n mystartup
kubectl apply -f k8s/06-migration-job.yml
```

### Database connection error
```powershell
# Check secrets exist
kubectl get secret app-secrets -n mystartup -o yaml

# Verify connection string
kubectl get secret app-secrets -n mystartup -o jsonpath='{.data.database-connection-string}' | base64 -d

# Test from pod
kubectl exec -it <pod-name> -n mystartup -- /bin/bash
# Inside: sqlcmd -S server.database.windows.net -U user -P password
```

---

## Cost Estimation

### Typical Monthly Costs (Estimated)

| Resource | Configuration | Monthly Cost |
|----------|---------------|--------------|
| AKS Cluster | 2 nodes (Standard_D2s_v5) | ~$150 |
| Azure SQL | Basic tier + serverless | ~$50 |
| Container Registry | Basic tier | ~$10 |
| Load Balancer | Standard | ~$20 |
| Application Insights | 1GB ingestion | ~$5 |
| **Total** | | **~$235** |

*Costs vary by region. Use [Azure Pricing Calculator](https://azure.microsoft.com/en-us/pricing/calculator/) for exact estimates.*

---

## Next Steps

1. ✅ Read: `docs/AZURE_AKS_DEPLOYMENT_GUIDE.md` (complete guide)
2. ⏳ Setup: Follow "Create Azure Infrastructure" section above
3. ⏳ Build: Build and push Docker images
4. ⏳ Deploy: Run deployment script
5. ⏳ Verify: Check services and test endpoints
6. ⏳ Monitor: View logs in Azure Portal
7. ⏳ CI/CD: Setup GitHub Actions secrets and enable automatic deployments

---

**For detailed documentation, see:**
- `docs/AZURE_AKS_DEPLOYMENT_GUIDE.md` - Complete deployment guide
- `AZURE_DEPLOYMENT_SUMMARY.md` - Summary of all deliverables
- `docs/ARCHITECTURE.md` - Architecture and design decisions
