# Azure Deployment Checklist

## Pre-Deployment

### Account & Credentials
- [ ] Azure subscription with active billing
- [ ] Azure CLI installed (`az --version`)
- [ ] kubectl installed (`kubectl version --client`)
- [ ] Docker installed (`docker --version`)
- [ ] Logged in to Azure: `az login`
- [ ] Correct subscription selected: `az account show`
- [ ] Git repository cloned locally

### Project Review
- [ ] Reviewed `docs/AZURE_AKS_DEPLOYMENT_GUIDE.md`
- [ ] Reviewed `AZURE_DEPLOYMENT_SUMMARY.md`
- [ ] Reviewed `QUICK_START_AZURE.md`
- [ ] Dockerfiles reviewed (API, Worker, MigrationRunner)
- [ ] Connection string format understood
- [ ] Budget approved for Azure costs (~$235/month estimated)

---

## Phase 1: Azure Infrastructure Setup

### Resource Group
- [ ] Created resource group: `az group create --name mystartup-rg --location eastus`
- [ ] Verified: `az group list --query "[?name=='mystartup-rg']"`

### Container Registry (ACR)
- [ ] Created ACR: `az acr create --resource-group mystartup-rg --name mystartupregistry --sku Basic`
- [ ] Retrieved credentials: `az acr credential show --name mystartupregistry`
- [ ] Saved username and password
- [ ] Tested login: `az acr login --name mystartupregistry`

### Azure SQL Database
- [ ] Created SQL Server: `az sql server create --resource-group mystartup-rg --name mystartup-sql --admin-user sqladmin --admin-password 'YourSecure@Password123!'`
- [ ] Created database: `az sql db create --resource-group mystartup-rg --server mystartup-sql --name MyStartUpCompanyDb --tier Basic`
- [ ] Configured firewall: `az sql server firewall-rule create --resource-group mystartup-rg --server mystartup-sql --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0`
- [ ] Generated connection string
- [ ] Tested connection (optional): Used SQL Management Studio or sqlcmd

### AKS Cluster
- [ ] Created AKS cluster: `az aks create --resource-group mystartup-rg --name mystartup-aks --node-count 2 --attach-acr mystartupregistry --generate-ssh-keys`
- [ ] Waited for cluster creation (10-15 minutes)
- [ ] Retrieved credentials: `az aks get-credentials --resource-group mystartup-rg --name mystartup-aks --overwrite-existing`
- [ ] Verified connectivity: `kubectl cluster-info`
- [ ] Checked nodes: `kubectl get nodes`

### Key Vault (Optional but Recommended)
- [ ] Created Key Vault: `az keyvault create --name mystartup-kv --resource-group mystartup-rg`
- [ ] Stored database connection string
- [ ] Stored Service Bus connection string
- [ ] Configured access policies for managed identity

---

## Phase 2: Docker Image Build & Push

### Build Images
- [ ] Built API image: `docker build -f src/MyStartUpCompany.Api/Dockerfile -t mystartupregistry.azurecr.io/api:latest .`
- [ ] Built Worker image: `docker build -f src/MyStartUpCompany.Worker/Dockerfile -t mystartupregistry.azurecr.io/worker:latest .`
- [ ] Built Migration image: `docker build -f src/MigrationRunner/Dockerfile -t mystartupregistry.azurecr.io/migrations:latest .`
- [ ] Verified images locally: `docker images | grep mystartupregistry`

### Push to ACR
- [ ] Logged in to ACR: `az acr login --name mystartupregistry`
- [ ] Pushed API image: `docker push mystartupregistry.azurecr.io/api:latest`
- [ ] Pushed Worker image: `docker push mystartupregistry.azurecr.io/worker:latest`
- [ ] Pushed Migration image: `docker push mystartupregistry.azurecr.io/migrations:latest`
- [ ] Verified in ACR: `az acr repository list --name mystartupregistry`

---

## Phase 3: Kubernetes Configuration

### Namespace & ConfigMap
- [ ] Created namespace: `kubectl apply -f k8s/01-namespace.yml`
- [ ] Applied ConfigMap: `kubectl apply -f k8s/02-configmap.yml`
- [ ] Verified: `kubectl get configmap -n mystartup`

### Secrets
- [ ] Created secrets in Kubernetes:
  ```
  kubectl create secret generic app-secrets \
	--from-literal=database-connection-string='<YOUR_CONNECTION_STRING>' \
	--from-literal=azure-servicebus-connectionstring='<YOUR_SERVICEBUS_STRING>' \
	-n mystartup
  ```
- [ ] Verified: `kubectl get secrets -n mystartup`

### Network Policies (Optional)
- [ ] Applied network policies: `kubectl apply -f k8s/07-network-policies.yml`
- [ ] Verified: `kubectl get networkpolicies -n mystartup`

---

## Phase 4: Database Migrations

### Migration Job
- [ ] Applied migration job: `kubectl apply -f k8s/06-migration-job.yml`
- [ ] Monitored job: `kubectl get job migration-runner -n mystartup -w`
- [ ] Checked logs: `kubectl logs -l job-name=migration-runner -n mystartup`
- [ ] Verified job completed: Job shows "1/1" in COMPLETIONS
- [ ] Verified database schema in Azure SQL (optional)

---

## Phase 5: Service Deployments

### API Deployment
- [ ] Applied API deployment: `kubectl apply -f k8s/04-api-deployment.yml`
- [ ] Checked pods: `kubectl get pods -n mystartup -l app=api -o wide`
- [ ] Monitored rollout: `kubectl rollout status deployment/api -n mystartup --timeout=300s`
- [ ] Verified pods running: At least 2 replicas in "Running" state
- [ ] Checked logs: `kubectl logs -f deployment/api -n mystartup`

### Worker Deployment
- [ ] Applied Worker deployment: `kubectl apply -f k8s/05-worker-deployment.yml`
- [ ] Checked pods: `kubectl get pods -n mystartup -l app=worker -o wide`
- [ ] Monitored rollout: `kubectl rollout status deployment/worker -n mystartup --timeout=300s`
- [ ] Verified pods running: At least 1 replica in "Running" state
- [ ] Checked logs: `kubectl logs -f deployment/worker -n mystartup`

### Pod Disruption Budgets
- [ ] Applied PDBs: `kubectl apply -f k8s/08-pod-disruption-budget.yml`
- [ ] Verified: `kubectl get pdb -n mystartup`

### Service Verification
- [ ] Listed all services: `kubectl get svc -n mystartup -o wide`
- [ ] Noted LoadBalancer external IP
- [ ] Waited for external IP (may take 1-2 minutes)

---

## Phase 6: Verify Deployment

### Health Checks
- [ ] API service has LoadBalancer IP assigned
- [ ] All pods are "Running": `kubectl get pods -n mystartup`
- [ ] All deployments are "Ready": `kubectl get deployments -n mystartup`
- [ ] API health endpoint responds:
  ```
  $IP = kubectl get svc api-service -n mystartup -o jsonpath='{.status.loadBalancer.ingress[0].ip}'
  curl http://$IP/health/ready
  ```

### Log Verification
- [ ] API logs show no errors: `kubectl logs deployment/api -n mystartup --tail=50`
- [ ] Worker logs show no errors: `kubectl logs deployment/worker -n mystartup --tail=50`
- [ ] Check for database connection messages in logs

### Scaling Verification
- [ ] HPA status: `kubectl get hpa -n mystartup`
- [ ] API HPA metrics showing CPU/memory: `kubectl get hpa api-hpa -n mystartup -w`

---

## Phase 7: Application Testing

### Basic Functionality
- [ ] API responds to requests: `curl http://$IP/api/endpoint`
- [ ] Health endpoints working: `/health/ready` and `/health/live`
- [ ] OpenAPI docs accessible (if enabled): `http://$IP/openapi` or `http://$IP/scalar`
- [ ] Database queries working (test via API)
- [ ] Worker service logs show processing activity

### Load Testing (Optional)
- [ ] Deployed load testing tool
- [ ] Ran load tests to verify scaling
- [ ] Verified HPA scales up replicas under load
- [ ] Verified HPA scales down replicas when load reduces
- [ ] Checked cost implications of scaling

---

## Phase 8: Monitoring Setup

### Application Insights
- [ ] Created Application Insights resource: `az monitor app-insights component create --app mystartup-insights --location eastus --resource-group mystartup-rg`
- [ ] Retrieved instrumentation key
- [ ] Configured in application (if needed)
- [ ] Verified telemetry appearing in Portal

### Alerts & Notifications
- [ ] Created action group for notifications
- [ ] Set up alerts for:
  - [ ] CPU > 80%
  - [ ] Memory > 80%
  - [ ] API response time > 1000ms
  - [ ] Failed requests > 5%
- [ ] Configured notification recipients

### Dashboard
- [ ] Created Azure Dashboard with key metrics
- [ ] Added cards for:
  - [ ] Pod status
  - [ ] Service availability
  - [ ] Response time
  - [ ] Error rate
  - [ ] Resource utilization

---

## Phase 9: GitHub Actions CI/CD

### GitHub Secrets Setup
- [ ] Created service principal: `az ad sp create-for-rbac --name github-deployer --role Contributor --scopes /subscriptions/<SUBSCRIPTION_ID>`
- [ ] Added to GitHub Secrets:
  - [ ] `AZURE_CLIENT_ID`
  - [ ] `AZURE_TENANT_ID`
  - [ ] `AZURE_SUBSCRIPTION_ID`
  - [ ] `REGISTRY_USERNAME`
  - [ ] `REGISTRY_PASSWORD`

### Workflow Deployment
- [ ] Workflow file created: `.github/workflows/deploy-to-aks.yml`
- [ ] Verified workflow file syntax: `git push origin <test-branch>`
- [ ] Triggered workflow manually: Actions → "Build and Deploy to AKS" → Run workflow
- [ ] Verified workflow completed successfully
- [ ] Checked deployment: `kubectl get deployments -n mystartup`

### Continuous Deployment
- [ ] Pushed changes to `main` branch
- [ ] Verified workflow automatically triggered
- [ ] Confirmed automatic deployment to AKS
- [ ] Verified smoke tests passed in workflow

---

## Phase 10: Security Hardening

### RBAC Configuration
- [ ] Created service account for deployments (if needed)
- [ ] Applied RBAC roles: `kubectl create role...`
- [ ] Verified least privilege access

### Network Security
- [ ] Applied network policies: `k8s/07-network-policies.yml` (done in Phase 3)
- [ ] Tested pod-to-pod connectivity restrictions (optional)
- [ ] Configured Azure Firewall (optional)

### Secrets Management
- [ ] Verified secrets not in code
- [ ] Verified secrets stored in Kubernetes Secret or Key Vault
- [ ] Set up secret rotation policy
- [ ] Verified no secrets in logs: `kubectl logs deployment/api -n mystartup | grep -i password`

### Container Security
- [ ] Containers running as non-root user
- [ ] No privileged containers
- [ ] Read-only root filesystem enabled (where applicable)
- [ ] Image scanning enabled in ACR

---

## Phase 11: Backup & Disaster Recovery

### Database Backups
- [ ] Verified Azure SQL backups enabled
- [ ] Configured backup retention (7-35 days)
- [ ] Tested backup restore (optional)

### Application Backups
- [ ] AKS cluster backups configured
- [ ] Container images tagged and versioned
- [ ] Deployment manifests in Git (✅ Already done)

### Disaster Recovery Plan
- [ ] Documented recovery procedures
- [ ] Tested failover process
- [ ] Documented RTO and RPO targets
- [ ] Team trained on runbooks

---

## Phase 12: Documentation & Handoff

### Documentation
- [ ] `docs/AZURE_AKS_DEPLOYMENT_GUIDE.md` - ✅ Completed
- [ ] `AZURE_DEPLOYMENT_SUMMARY.md` - ✅ Completed
- [ ] `QUICK_START_AZURE.md` - ✅ Completed
- [ ] Runbooks created for operations team
- [ ] Troubleshooting guide shared with team
- [ ] On-call documentation updated

### Team Training
- [ ] Team trained on deployment procedures
- [ ] Team trained on monitoring and alerts
- [ ] Team trained on troubleshooting steps
- [ ] Team trained on escalation procedures

### Final Checks
- [ ] All resources tagged with environment, owner, cost center
- [ ] Resource naming follows company standards
- [ ] Budget alerts configured
- [ ] Cost optimization recommendations applied
- [ ] Performance baseline established

---

## Post-Deployment

### Week 1
- [ ] Monitor performance and alerts
- [ ] Check database growth
- [ ] Review logs for errors
- [ ] Validate backup procedures
- [ ] Get user feedback

### Month 1
- [ ] Optimize resource allocation based on metrics
- [ ] Review security policies
- [ ] Plan disaster recovery drill
- [ ] Update documentation with lessons learned
- [ ] Review costs and optimization opportunities

### Quarterly
- [ ] Review infrastructure scaling
- [ ] Update security patches
- [ ] Conduct security audit
- [ ] Plan capacity for next quarter
- [ ] Review disaster recovery procedures

---

## Troubleshooting Quick Reference

| Issue | Quick Fix |
|-------|-----------|
| Pods not starting | `kubectl describe pod <name> -n mystartup` |
| Images not found | `az acr repository list --name mystartupregistry` |
| DB connection error | `kubectl get secret app-secrets -n mystartup -o yaml` |
| Service not accessible | `kubectl get svc -n mystartup` → Check EXTERNAL-IP |
| Migration failed | `kubectl logs -l job-name=migration-runner -n mystartup` |
| High CPU usage | `kubectl top pods -n mystartup` |

---

## Success Criteria

✅ **All steps in this checklist are completed**

✅ **All pods are running**: `kubectl get pods -n mystartup` shows all pods in "Running"

✅ **API is accessible**: `curl http://<EXTERNAL-IP>/health/ready` returns 200

✅ **Monitoring is active**: Metrics visible in Azure Portal

✅ **CI/CD is working**: GitHub Actions successfully deploys on push to main

✅ **Team is trained**: Operations team can handle day-to-day operations

✅ **Backups are enabled**: Database and cluster backups configured

✅ **Alerts are active**: Team receives notifications for critical issues

---

**Deployment is complete and ready for production use!** 🎉
