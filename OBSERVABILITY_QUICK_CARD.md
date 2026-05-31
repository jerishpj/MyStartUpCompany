# OBSERVABILITY QUICK REFERENCE CARD

**Print this and pin it to your desk!**

---

## 🎯 The Bottom Line

| Question | Answer |
|----------|--------|
| **Keep separate Observability library?** | ✅ YES - Industry standard |
| **Is this the right architecture?** | ✅ YES - Used by Microsoft, AWS, etc. |
| **Should we use Aspire now?** | ❌ NO - Do it later for cloud phase |
| **How to verify it works?** | ✅ Run tests + Docker Compose stack |
| **What's the action plan?** | ✅ Validate this week, enhance next week |

---

## 🚀 Quick Start (5 minutes)

```powershell
# Terminal 1: Start observability stack
cd docker-compose
.\startup.ps1

# Terminal 2: Start API service
dotnet run --project src/MyStartUpCompany.Api

# Terminal 3: Generate traffic
for ($i = 1; $i -le 10; $i++) {
	curl -X GET http://localhost:5000/api/companies
	Start-Sleep -Seconds 1
}

# Open dashboards
Start-Process "http://localhost:16686"  # Jaeger - View Traces
Start-Process "http://localhost:9090"   # Prometheus - View Metrics
Start-Process "http://localhost:3000"   # Grafana - View Dashboards
```

---

## ✅ Verification Checklist

Run these commands to verify everything works:

```powershell
# 1. Run OTEL tests (2 min)
dotnet test tests/MyStartUpCompany.Observability.Tests

# 2. Run comprehensive validation (15 min)
.\validate-otel-local.ps1

# 3. Manual verification
# - Open Jaeger at http://localhost:16686
# - Open Prometheus at http://localhost:9090
# - Make API calls and see traces/metrics appear
```

**Expected Results**:
- ✅ All 29 OTEL tests pass
- ✅ Docker Compose stack healthy
- ✅ Traces visible in Jaeger
- ✅ Metrics visible in Prometheus

---

## 📚 Read These Documents (In Order)

### 1. Get the Executive Summary (10 min)
📄 **`OBSERVABILITY_SUMMARY.md`** - Start here!

### 2. Understand the Architecture (15 min)
📄 **`OBSERVABILITY_ARCHITECTURE_REVIEW.md`** - Deep dive on why your approach is right

### 3. See the Decision Matrix (10 min)
📄 **`OBSERVABILITY_DECISION_MATRIX.md`** - Scoring of all options

### 4. Run the Verification (30 min)
📄 **`LOCAL_OTEL_COMPREHENSIVE_VERIFICATION.md`** - Step-by-step validation

### 5. Quick Reference During Development
📄 **`docs/QUICK_REFERENCE.md`** - Common commands and questions

---

## 🏗️ Architecture at a Glance

```
┌─────────────────────────────────────────────────────────────┐
│                     YOUR APPLICATION                        │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Services:                                                   │
│  ├─ Api                    (OpenTelemetry enabled)          │
│  ├─ Worker                 (OpenTelemetry enabled)          │
│  └─ Notifier               (OpenTelemetry enabled)          │
│        ↓                                                      │
│  └─→ MyStartUpCompany.Observability (Shared Library)        │
│        └─ AddObservability() extension method               │
│                                                              │
├─────────────────────────────────────────────────────────────┤
│                   LOCAL OBSERVABILITY STACK                 │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Trace Collection:                                           │
│  ├─ Service → OTLP Exporter → Jaeger (port 4318)          │
│  └─ Jaeger UI on http://localhost:16686                    │
│                                                              │
│  Metrics Collection:                                         │
│  ├─ Service → Prometheus Exporter (port 9091)              │
│  └─ Prometheus UI on http://localhost:9090                 │
│                                                              │
│  Visualization:                                              │
│  └─ Grafana Dashboards on http://localhost:3000            │
│                                                              │
│  Logs:                                                       │
│  ├─ Console Exporter → Terminal Output                      │
│  └─ Structured logs with Correlation IDs                   │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔧 Common Tasks

### Task 1: Start Everything

```powershell
# Start observability stack
cd docker-compose && .\startup.ps1

# In new terminal windows:
dotnet run --project src/MyStartUpCompany.Api
dotnet run --project src/MyStartUpCompany.Worker
dotnet run --project src/MyStartUpCompany.Notifier
```

### Task 2: View Traces

1. Open http://localhost:16686
2. Service dropdown → Select `MyStartUpCompany.Api`
3. Click "Find Traces"
4. Click on a trace to see details

### Task 3: View Metrics

1. Open http://localhost:9090
2. Type metric name in search box (e.g., `up`, `http_requests_total`)
3. Click "Execute"
4. Graph appears below

### Task 4: View Custom Dashboards

1. Open http://localhost:3000
2. Login: admin / admin (default)
3. Home → Select dashboard
4. View real-time metrics

### Task 5: Run Tests

```powershell
# Full test suite
dotnet test tests/MyStartUpCompany.Observability.Tests

# Specific test
dotnet test tests/MyStartUpCompany.Observability.Tests `
	--filter "MethodName=TraceCorrelationId_Should"

# With coverage
dotnet test tests/MyStartUpCompany.Observability.Tests `
	--collect:"XPlat Code Coverage"
```

### Task 6: Generate Test Traffic

```powershell
# Generate 10 API calls
for ($i = 1; $i -le 10; $i++) {
	curl -X GET http://localhost:5000/api/companies
	Start-Sleep -Seconds 1
}

# Generate with custom headers
$headers = @{ "X-Correlation-Id" = [Guid]::NewGuid().ToString() }
Invoke-WebRequest -Uri "http://localhost:5000/api/companies" -Headers $headers
```

### Task 7: Troubleshoot (Nothing Appearing in Jaeger)

```powershell
# 1. Verify Docker is running
docker-compose -f docker-compose/docker-compose.yaml ps

# 2. Check service is sending OTEL
# Look in console for "OpenTelemetry" or "OTLP"

# 3. Verify config is enabled
Get-Content src/MyStartUpCompany.Api/appsettings.Development.json | ConvertFrom-Json | Select-Object -ExpandProperty Observability

# 4. Check Jaeger OTLP receiver health
curl -X GET http://localhost:16686/api/services
```

---

## 📊 Performance Expectations

| Scenario | Expected Behavior | Verification |
|----------|-------------------|--------------|
| **Single request** | Trace appears in 1-2 seconds | View in Jaeger |
| **10 requests/sec** | All traces collected | All visible in Jaeger |
| **100 requests/sec** | Some sampling may occur | Check sampling config |
| **Metric emission** | Appears in Prometheus in 5 sec | Query Prometheus |
| **High volume (1000+)** | OTEL overhead ~5-10% | Monitor response times |

---

## 🎓 Key Concepts

### Trace ID
Unique identifier for entire request across all services
```
Example: 4bf92f3577b34da6a3ce929d0e0e4736
Visible in: Jaeger, Logs, Response headers (traceparent)
```

### Span
Single operation within a trace
```
Example: HTTP GET /api/companies
Has: Start time, duration, status, attributes, events
```

### Correlation ID
Custom identifier your app assigns to requests
```
Example: order-12345
Used to: Group related operations
Visible in: Logs and custom metrics
```

### Sampling
Percentage of traces kept (100% = all, 10% = every 10th)
```
For local dev: 100% (see all traces)
For production: 1-10% (reduce costs)
```

### Exporter
System that sends telemetry somewhere
```
Local: Console, OTLP, Prometheus
Cloud: Azure App Insights, Datadog, Dynatrace
```

---

## ❌ Common Mistakes (Don't Do These!)

| ❌ DON'T | ✅ DO INSTEAD |
|---------|--------------|
| Copy OTEL config to every Program.cs | Use `AddObservability()` extension |
| Ignore correlation IDs | Use `CorrelationIdAccessor` everywhere |
| Set sampling to 100% in production | Use 1-5% for production |
| Forget to await traces on shutdown | Use proper graceful shutdown |
| Hardcode OTLP endpoint | Use appsettings.json configuration |
| Mix OTEL libraries from different versions | Use same version for all |
| Disable OTEL in test environments | Keep enabled, use Console exporter |
| Forget to check Jaeger is running | Always start Docker stack first |

---

## 🕐 Development Timeline

```
Week 1:
├─ Mon: Validate tests pass ✅
├─ Tue: Verify Docker stack ✅
├─ Wed: Run comprehensive validation ✅
├─ Thu: Team alignment ✅
└─ Fri: Create runbooks ✅

Week 2:
├─ Mon-Tue: Enhance tests (40+) ✅
├─ Wed-Thu: Create dashboards ✅
└─ Fri: Team training ✅

Later (6-12 months):
├─ Evaluate Aspire ⏸️
├─ Add Azure App Insights ⏸️
└─ Cloud migration ⏸️
```

---

## 🌐 Useful Links

### Local Dashboards
- 🔗 **Jaeger UI**: http://localhost:16686
- 🔗 **Prometheus UI**: http://localhost:9090
- 🔗 **Grafana UI**: http://localhost:3000 (admin/admin)
- 🔗 **OTLP Receiver**: http://localhost:4318

### Documentation
- 📄 Architecture Review: `OBSERVABILITY_ARCHITECTURE_REVIEW.md`
- 📄 Local Testing: `LOCAL_OTEL_COMPREHENSIVE_VERIFICATION.md`
- 📄 Decision Matrix: `OBSERVABILITY_DECISION_MATRIX.md`
- 📄 Runbook: `docs/OBSERVABILITY_RUNBOOK.md`
- 📄 Quick Reference: `docs/QUICK_REFERENCE.md`

### Code
- 📁 Library: `src/MyStartUpCompany.Observability/`
- 📁 Tests: `tests/MyStartUpCompany.Observability.Tests/`
- 📁 Config: `docker-compose/`
- 📁 Docs: `docs/`

### Tools
- 📦 **Docker**: `docker-compose up`
- 📦 **.NET CLI**: `dotnet test`
- 📦 **PowerShell**: `.\validate-otel-local.ps1`

---

## ✅ Decision Summary

| Decision | Choice | Confidence |
|----------|--------|-----------|
| Keep shared library? | ✅ YES | 9.65/10 |
| Industry standard? | ✅ YES | Industry proven |
| Local testing strategy? | ✅ Docker Compose | Battle-tested |
| Use Aspire now? | ❌ NO | Later (before cloud) |
| Code quality? | ✅ GOOD | Follows best practices |
| Ready to proceed? | ✅ YES | Validate this week |

---

## 🎯 Your Next 3 Steps

### Step 1 (Today - 30 min)
```
[ ] Read: OBSERVABILITY_SUMMARY.md
[ ] Read: This quick reference card
[ ] Understand: Why your architecture is correct
```

### Step 2 (This Week - 1 hour)
```
[ ] Run: dotnet test tests/MyStartUpCompany.Observability.Tests
[ ] Run: .\validate-otel-local.ps1
[ ] Verify: All checks pass
```

### Step 3 (Next Week - 2-3 hours)
```
[ ] Enhance: Add more test scenarios
[ ] Create: Grafana dashboards
[ ] Document: Team runbooks
```

---

**You're ready. Go verify your OTEL setup this week!** ✅

