# Local OTEL Quick Reference Card

Keep this handy for common OTEL operations during development.

---

## 🚀 Quick Start (60 seconds)

```powershell
# 1. Start OTEL stack
cd docker-compose
.\startup.ps1

# 2. Start API (new terminal)
dotnet run --project src/MyStartUpCompany.Api

# 3. Generate traffic
Invoke-WebRequest -Uri "http://localhost:5000/api/companies"

# 4. View in Jaeger
# Open: http://localhost:16686
```

---

## 📊 Access Observability Tools

| Tool | URL | User | Password |
|------|-----|------|----------|
| **Jaeger** (Traces) | http://localhost:16686 | (none) | (none) |
| **Prometheus** (Metrics) | http://localhost:9090 | (none) | (none) |
| **Grafana** (Dashboards) | http://localhost:3000 | admin | admin |

---

## 🔧 Common Commands

### Docker Compose Stack
```powershell
# Start stack
cd docker-compose
.\startup.ps1

# Stop stack
.\startup.ps1 -Stop

# View logs
docker-compose logs -f jaeger

# Clean restart
.\startup.ps1 -Clean
```

### Applications
```bash
# API
dotnet run --project src/MyStartUpCompany.Api

# Worker
dotnet run --project src/MyStartUpCompany.Worker

# Notifier
dotnet run --project src/MyStartUpCompany.Notifier

# Run all OTEL tests
dotnet test tests/MyStartUpCompany.Observability.Tests
```

### Validation
```powershell
# Full stack validation
.\validate-otel-local.ps1

# Specific endpoint test
Invoke-WebRequest -Uri "http://localhost:16686" -Verbose
```

---

## 📈 Prometheus Queries (Copy-Paste)

```promql
# HTTP Requests Per Second
rate(http_requests_total[1m])

# Error Rate
rate(http_errors_total[1m])

# Database Query Count
db_queries_total

# DB Query Duration (95th percentile)
histogram_quantile(0.95, db_query_duration_ms)

# Notification Delivery Rate
rate(notifications_delivered_total[1m])

# Request Count by Service
http_requests_total{job=~"mystartupcorp.*"}

# Show All Metrics
{__name__=~".+"}
```

---

## 🐛 Troubleshooting (5 Minutes)

| Problem | Solution |
|---------|----------|
| **Container won't start** | `docker-compose down -v && docker-compose up -d` |
| **Port already in use** | Edit `docker-compose.yaml`, change first port number |
| **No traces in Jaeger** | Check app console for errors, verify `http://localhost:4318` endpoint |
| **No metrics in Prometheus** | Check targets at `http://localhost:9090/targets` |
| **Metrics endpoint 404** | Verify `appsettings.Development.json` has Prometheus config |
| **Network timeout** | On Linux, use `172.17.0.1` instead of `localhost` in app config |

See **OBSERVABILITY_RUNBOOK.md** for detailed troubleshooting.

---

## 🧪 Testing Checklist

### Pre-Start
- [ ] Docker Desktop running
- [ ] .NET 10 SDK installed
- [ ] Solution builds: `dotnet build`

### After Stack Starts
- [ ] ✅ Run: `.\validate-otel-local.ps1`
- [ ] ✅ Start API, Worker, Notifier
- [ ] ✅ Generate traffic (see below)
- [ ] ✅ See traces in Jaeger
- [ ] ✅ See metrics in Prometheus

### Generate Test Traffic
```powershell
# Single request
Invoke-WebRequest http://localhost:5000/api/companies

# Multiple requests (load)
1..10 | ForEach-Object { Invoke-WebRequest http://localhost:5000/api/companies }

# Create company
Invoke-WebRequest -Method POST http://localhost:5000/api/companies `
  -Headers @{"Content-Type"="application/json"} `
  -Body '{"name":"Test","registrationNumber":"TEST123"}'
```

---

## 📋 Three Signals Explained

| Signal | Tool | What It Shows | How to Access |
|--------|------|----------------|----------------|
| **Traces** | Jaeger | Request flow, latency, dependencies | http://localhost:16686 |
| **Metrics** | Prometheus | Counters, gauges, histograms | http://localhost:9090 |
| **Logs** | Console | Structured logs, correlation IDs | Terminal output |

---

## 🎯 Daily Workflow

```powershell
# Morning: Start the stack
cd docker-compose
.\startup.ps1

# Start services in separate terminals
dotnet run --project src/MyStartUpCompany.Api
dotnet run --project src/MyStartUpCompany.Worker

# During development: Generate traffic, monitor
Invoke-WebRequest http://localhost:5000/api/companies
# Check Jaeger at http://localhost:16686

# End of day: Stop stack
cd docker-compose
.\startup.ps1 -Stop
```

---

## 📚 Documentation Map

```
├── 🎯 START HERE
│   ├── LOCAL_OTEL_TESTING_GUIDE.md (Setup & testing)
│   └── This quick reference
├── 📖 LEARN
│   ├── OPENTELEMETRY_ARCHITECTURE.md (How it works)
│   ├── OBSERVABILITY_RUNBOOK.md (Detailed tasks)
│   └── OTEL_CONFIDENCE_CHECKLIST.md (Validation)
├── 🔧 OPERATIONS
│   ├── docker-compose/README.md (Stack management)
│   └── validate-otel-local.ps1 (Health checks)
└── 📋 REFERENCES
	├── docs/README.md (Main docs index)
	└── Archived docs for future cloud work
```

---

## ⚡ Power User Tips

### Monitor Specific Service
```promql
# API only
http_requests_total{service="api"}

# Worker only  
messages_processed{service="worker"}

# Notifier only
notifications_delivered_total
```

### See Errors
```promql
# All errors
http_errors_total
errors_total

# Errors per service
errors_total{service="api"}
```

### Performance Analysis
```promql
# Slowest requests (95th percentile)
histogram_quantile(0.95, http_request_duration_ms)

# Slowest database queries
histogram_quantile(0.95, db_query_duration_ms)
```

### Create Quick Dashboard
1. Open http://localhost:3000
2. Click "+" → "Dashboard"
3. Click "Add panel"
4. Paste query from above
5. Click "Apply"
6. Save dashboard

---

## 🚨 Emergency Procedures

### Stack Not Responding
```powershell
# Hard reset
docker-compose down -v
docker-compose up -d
.\startup.ps1
```

### Memory Issues
```bash
# Check Docker disk space
docker system df

# Clean up
docker system prune -a
```

### See All Running Containers
```bash
docker ps
docker-compose ps
```

---

## 📞 Getting Help

1. **Read the docs**: [LOCAL_OTEL_TESTING_GUIDE.md](docs/LOCAL_OTEL_TESTING_GUIDE.md)
2. **Check runbook**: [OBSERVABILITY_RUNBOOK.md](docs/OBSERVABILITY_RUNBOOK.md)
3. **View checklist**: [OTEL_CONFIDENCE_CHECKLIST.md](docs/OTEL_CONFIDENCE_CHECKLIST.md)
4. **Run validation**: `.\validate-otel-local.ps1`
5. **Check logs**: `docker-compose logs <service>`

---

## 🎓 Learning Resources

- **OpenTelemetry Docs**: https://opentelemetry.io/docs/instrumentation/net/
- **Jaeger Tutorials**: https://www.jaegertracing.io/docs/
- **Prometheus Queries**: https://prometheus.io/docs/prometheus/latest/querying/basics/
- **Grafana**: https://grafana.com/docs/grafana/latest/

---

**Keep this card visible** on your desk or monitor during OTEL development!

Last Updated: 2024 | Scope: Local Development
