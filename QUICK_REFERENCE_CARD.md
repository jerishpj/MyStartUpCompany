# 📇 Azure Deployment - Quick Reference Card

**Print this page or keep it on your screen during deployment**

---

## 🎯 Essential Files

| File | Location | Purpose |
|------|----------|---------|
| **START_HERE.md** | Root | ⭐ Read first (orientation) |
| **ARCHITECTURE_DIAGRAMS.md** | Root | ⭐ Visual diagrams |
| **QUICK_START_AZURE.md** | Root | ⭐ Deployment commands |
| **DEPLOYMENT_CHECKLIST.md** | Root | ⭐ Verification checklist |
| **docs/AZURE_AKS_DEPLOYMENT_GUIDE.md** | docs/ | ⭐ Reference & troubleshooting |

---

## ⏱️ Timeline

| Phase | Time | Action |
|-------|------|--------|
| **Preparation** | 30 min | Read 4 docs, verify prerequisites |
| **Azure Infrastructure** | 30 min | Create RG, ACR, SQL, AKS |
| **Docker Build** | 30 min | Build & push 3 images |
| **Deploy to AKS** | 30 min | Run deployment script |
| **Verification** | 15 min | Test endpoints & verify pods |
| **TOTAL** | 2.5-4.5 hrs | Complete deployment |

---

## 🔑 Prerequisites to Install

```powershell
# Verify these are installed:
az --version           # Azure CLI
kubectl version --client   # Kubernetes CLI
docker --version       # Docker
```

---

## 💻 Critical Commands Reference

### **Login & Setup**
```powershell
az login
az account set --subscription "YOUR_SUBSCRIPTION_ID"
az aks get-credentials --resource-group mystartup-rg --name mystartup-aks --overwrite-existing
```

### **Check Deployment**
```powershell
# View pods
kubectl get pods -n mystartup

# View services
kubectl get svc -n mystartup

# View logs
kubectl logs -f deployment/api -n mystartup

# Get IP address
kubectl get svc api-service -n mystartup -o jsonpath='{.status.loadBalancer.ingress[0].ip}'
```

### **Troubleshooting**
```powershell
# Describe pod issues
kubectl describe pod <POD_NAME> -n mystartup

# View detailed pod logs
kubectl logs <POD_NAME> -n mystartup

# Check events
kubectl get events -n mystartup --sort-by='.lastTimestamp'
```

---

## 🛠️ Key Values to Save

**Save these during deployment - you'll need them later:**

```
Subscription ID: ________________
Resource Group: ________________
Location: ________________
Registry Name: ________________
Registry URL: ________________
SQL Server: ________________
SQL Database: ________________
AKS Cluster Name: ________________
AKS Node Count: ________________
API External IP: ________________
```

---

## 📋 Quick Deployment Steps

```
1. [ ] Read START_HERE.md (5 min)
2. [ ] Read ARCHITECTURE_DIAGRAMS.md (10 min)
3. [ ] Verify prerequisites installed (5 min)
4. [ ] Create Azure Resource Group (5 min)
5. [ ] Create Azure Container Registry (5 min)
6. [ ] Create Azure SQL Database (10 min)
7. [ ] Create AKS Cluster (15 min - takes longest)
8. [ ] Build Docker images locally (10 min)
9. [ ] Push images to ACR (5 min)
10. [ ] Create Kubernetes secrets (5 min)
11. [ ] Run migration job (5 min)
12. [ ] Deploy API service (5 min)
13. [ ] Deploy Worker service (5 min)
14. [ ] Verify all pods running (5 min)
15. [ ] Test API endpoints (5 min)
```

---

## ✅ Success Criteria

Your deployment is successful when:

- [ ] All pods running in `mystartup` namespace
- [ ] API service has external IP assigned
- [ ] Worker pods exist and running
- [ ] Database migrations completed
- [ ] API responds to health check: `curl http://<EXTERNAL_IP>/health/ready`
- [ ] No pods in "CrashLoopBackOff" state
- [ ] All replicas running (desired = current)

---

## 🆘 Quick Troubleshooting

| Problem | Check | Fix |
|---------|-------|-----|
| **Pods not starting** | `kubectl describe pod <name>` | Check events for error details |
| **Image pull failed** | Secrets created? | Run `kubectl create secret docker-registry` |
| **Connection timeout** | Network policy? | Check `kubectl get networkpolicies` |
| **Database error** | Connection string correct? | Verify secret in `kubectl get secrets` |
| **No external IP** | Service type LoadBalancer? | Check service definition |
| **Build failures** | Docker installed? | Run `docker --version` first |
| **Azure CLI errors** | Logged in? | Run `az login` first |

---

## 🔗 Important URLs

| Resource | Where to Find |
|----------|---------------|
| Azure Portal | https://portal.azure.com |
| AKS Cluster | Azure Portal → Resource Group → AKS Cluster |
| Container Registry | Azure Portal → Resource Group → Container Registry |
| SQL Database | Azure Portal → Resource Group → SQL Database |
| Application Insights | Azure Portal → Resource Group → App Insights |

---

## 🔐 Secrets You Need

During deployment, create these Kubernetes secrets:

```yaml
database-connection-string: "Server=tcp:YOUR_SQL_SERVER;Initial Catalog=YOUR_DB;..."
azure-servicebus-connectionstring: "Endpoint=sb://YOUR_SERVICE_BUS.servicebus.windows.net/..."
```

---

## 🐳 Docker Images to Build

```
1. MyStartUpCompany.Api
   Location: src/MyStartUpCompany.Api/Dockerfile
   Tag: mystartupregistry.azurecr.io/api:latest

2. MyStartUpCompany.Worker
   Location: src/MyStartUpCompany.Worker/Dockerfile
   Tag: mystartupregistry.azurecr.io/worker:latest

3. MigrationRunner
   Location: src/MigrationRunner/Dockerfile
   Tag: mystartupregistry.azurecr.io/migration:latest
```

---

## 📊 Default Configuration

| Setting | Value | Notes |
|---------|-------|-------|
| **Namespace** | `mystartup` | Kubernetes namespace |
| **API Replicas** | 2-5 | Auto-scales based on CPU |
| **Worker Replicas** | 1-3 | Auto-scales based on CPU |
| **API Port (Internal)** | 8080 | Container port |
| **API Port (External)** | 80 | LoadBalancer port |
| **CPU Request** | 100m | Minimum CPU per pod |
| **Memory Request** | 128Mi | Minimum memory per pod |
| **CPU Limit** | 500m | Maximum CPU per pod |
| **Memory Limit** | 512Mi | Maximum memory per pod |

---

## 🔄 CI/CD (Optional)

If setting up GitHub Actions:

```
1. Commit & push code to GitHub
2. GitHub Actions workflow triggers
3. Builds Docker images
4. Pushes to ACR
5. Deploys to AKS automatically
```

File: `.github/workflows/deploy-to-aks.yml`

---

## 📞 Document Quick Links

**If you need to...**

| Task | Document | Section |
|------|----------|---------|
| Understand the system | START_HERE.md | Entire file |
| See architecture | ARCHITECTURE_DIAGRAMS.md | High-Level Arch |
| Run commands | QUICK_START_AZURE.md | Each phase |
| Verify completion | DEPLOYMENT_CHECKLIST.md | All phases |
| Understand details | docs/AZURE_AKS_DEPLOYMENT_GUIDE.md | Parts 1-9 |
| Fix errors | docs/AZURE_AKS_DEPLOYMENT_GUIDE.md | Part 10 |

---

## 💰 Cost Tracking

| Resource | Est. Cost | Notes |
|----------|-----------|-------|
| AKS (2 nodes, 1 month) | $150 | Standard_D2s_v5 |
| SQL Database | $50 | Basic tier |
| Container Registry | $10 | Basic tier |
| LoadBalancer | $20 | Standard tier |
| App Insights | $5 | 1GB ingestion |
| **Total Monthly** | **~$235** | Varies by region |

💡 Use Azure Pricing Calculator for exact costs in your region

---

## 🎯 Your Next Step

1. You've read this reference card
2. **→ Next: Open START_HERE.md**
3. Follow the sequence it provides
4. Come back to this card during deployment for quick lookups

---

## ✨ Key Reminders

✅ **Save important values** (IP addresses, connection strings)  
✅ **Wait for commands to complete** before running next one  
✅ **Check the checklist** as you go through each phase  
✅ **Read error messages fully** if something fails  
✅ **Reference the troubleshooting guide** if stuck  
✅ **Test API endpoints** after deployment  
✅ **Keep this card handy** during deployment  

---

**You've got this! 🚀 Start with START_HERE.md**
