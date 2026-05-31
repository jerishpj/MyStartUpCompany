# Documentation Scope Review - Local OTEL Testing Phase

**Status:** Current Phase - Local Development & Testing Only  
**Future Phase:** Azure AKS & Kubernetes Deployment (deferred)

---

## Documentation Files - Categorization

### ✅ KEEP (Local Development Scope)

| File | Purpose | Status |
|------|---------|--------|
| docs/LOCAL_DEVELOPMENT.md | Local dev environment setup | Keep - Update with OTEL local setup |
| docs/OPENTELEMETRY_ARCHITECTURE.md | OTEL design & components | Keep - Update to remove Azure/AKS references |
| docs/TESTING_GUIDE.md | Testing strategy & approach | Keep - Extend with OTEL validation tests |
| docs/IMPLEMENTATION_CHECKLIST.md | Implementation progress tracking | Keep - Local features only |
| docs/ARCHITECTURE.md | System architecture overview | Keep - Core design (no Azure specifics) |
| docs/README.md | Documentation index | Keep - Update navigation |
| docs/SOLUTION_SUMMARY.md | Project overview | Keep - Current state |

### 📦 ARCHIVE (Future Azure/K8s Phase)

| File | Purpose | Archive Reason |
|------|---------|-----------------|
| docs/AZURE_AKS_DEPLOYMENT_GUIDE.md | AKS deployment procedures | Out of current scope |
| docs/DEPLOYMENT_GUIDE.md | Cloud deployment instructions | Out of current scope |
| docs/CONTAINERIZATION_GUIDE.md | Docker image building | Out of current scope |
| docs/COMPLETE_IMPLEMENTATION_GUIDE.md | Full implementation across cloud | Out of current scope |

### 📚 REFERENCE (Keep for Context)

| File | Purpose | Use |
|------|---------|-----|
| docs/FLUENT_VALIDATION*.md | Validation framework docs | Reference only |
| docs/MESSAGE_MAPPING_ARCHITECTURE.md | Message processing design | Reference only |
| docs/MIGRATION_STRATEGY.md | Database migration approach | Reference only |
| docs/QUICK_REFERENCE_ADD_SOURCE.md | Quick refs | Reference only |
| docs/VISUAL_ARCHITECTURE_GUIDE.md | Visual architecture | Reference only |
| docs/ProjectTypeReferenceData.md | Reference data | Reference only |
| docs/DOCUMENTATION_INDEX.md | Doc index | Keep updated |

---

## Kubernetes Files - Categorization

### 📦 ARCHIVE (All K8s Manifests)

**Location:** Move all to `k8s_archived/`

```
k8s/00-namespaces.yaml
k8s/01-namespace.yml
k8s/01-observability-config.yaml
k8s/02-configmap.yml
k8s/02-otel-collector.yaml
k8s/03-jaeger.yaml
k8s/03-secrets.yml
k8s/04-api-deployment.yml
k8s/04-prometheus.yaml
k8s/05-grafana.yaml
k8s/05-worker-deployment.yml
k8s/06-api-deployment.yaml
k8s/06-migration-job.yml
k8s/07-network-policies.yml
k8s/07-worker-deployment.yaml
k8s/08-notifier-deployment.yaml
k8s/08-pod-disruption-budget.yml
k8s/09-ingress-and-network-policies.yaml
k8s/10-secrets-template.yaml
k8s/deploy-aks.sh
k8s/deploy.ps1
k8s/deploy.sh
```

---

## New Files to Create (Local OTEL Testing Phase)

### Local Testing & Configuration
1. **docker-compose.yaml** - Local Jaeger, Prometheus, Grafana stack
2. **LOCAL_OTEL_TESTING_GUIDE.md** - Step-by-step local OTEL validation
3. **OTEL_LOCAL_SETUP.md** - Docker Compose + configuration details
4. **OTEL_TESTING_CHECKLIST.md** - Validation checklist before confidence

### Configuration Files (per project)
- **appsettings.Development.json** - Configure for console + local Jaeger exporters
- **.dockerignore** - Local development files (when containerizing later)

### Testing & Validation
1. **OTEL Integration Tests** - Verify OTEL services registration
2. **OTEL End-to-End Tests** - Generate traffic, validate signals
3. **Test Data & Scenarios** - Sample loads for validation

---

## Action Summary

### To Do
- [ ] Create `docs_archived/` folder, move 4 archived docs
- [ ] Create `k8s_archived/` folder, move all k8s yaml/shell files
- [ ] Update OPENTELEMETRY_ARCHITECTURE.md (remove Azure/AKS, focus on local)
- [ ] Create LOCAL_OTEL_TESTING_GUIDE.md
- [ ] Create docker-compose.yaml for Jaeger + Prometheus + Grafana
- [ ] Update appsettings.Development.json files
- [ ] Create OTEL integration & E2E tests
- [ ] Update docs/README.md with new structure

---

## Phase Timeline

### Phase 1: Local Development & Testing (NOW)
- ✅ OTEL implementation complete
- ✅ Console exporter working
- 📋 TODO: Local Jaeger/Prometheus/Grafana stack
- 📋 TODO: Validation tests & confidence checks

### Phase 2: Azure/K8s Deployment (Future)
- Containerization (Docker)
- AKS deployment (YAML manifests)
- Azure Monitor integration
- Production observability patterns

