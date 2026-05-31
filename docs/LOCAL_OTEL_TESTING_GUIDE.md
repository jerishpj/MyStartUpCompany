# Local OpenTelemetry Testing Guide

**Version:** 1.0  
**Date:** May 2026  
**Purpose:** Complete guide for testing OTEL implementation locally before Azure deployment  

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Local Stack Setup](#local-stack-setup)
3. [Application Configuration](#application-configuration)
4. [Running Applications Locally](#running-applications-locally)
5. [Testing OTEL Signals](#testing-otel-signals)
6. [Validation Checklist](#validation-checklist)
7. [Troubleshooting](#troubleshooting)
8. [Next Steps](#next-steps)

---

## Prerequisites

### Required Software
- **Docker Desktop** (includes Docker Engine and Docker Compose)
  - Download: https://www.docker.com/products/docker-desktop
  - Test: `docker --version`

- **.NET 10 SDK**
  - Download: https://dotnet.microsoft.com/en-us/download/dotnet/10.0
  - Test: `dotnet --version`

- **PowerShell 7+ or Bash**
  - PowerShell: https://github.com/PowerShell/PowerShell
  - Test: `$PSVersionTable.PSVersion`

- **Visual Studio Community 2026 or VS Code**
  - For development and debugging

### System Requirements
- **RAM:** Minimum 8 GB (recommended 16 GB)
  - Docker containers: ~2-3 GB
  - Running 3 .NET services: ~1-2 GB
- **Disk Space:** 10 GB free (for containers, packages, build artifacts)
- **Network:** Localhost ports available: 5000, 5001, 5002, 3000, 6831, 9090, 16686

---

## Local Stack Setup

### Step 1: Create docker-compose.yaml

Create a `docker-compose.yaml` file in the project root or dedicated `docker-compose` directory:

```yaml
version: '3.8'

services:
  # Jaeger for Distributed Tracing
  jaeger:
	image: jaegertracing/all-in-one:latest
	container_name: otel-jaeger
	ports:
	  - "16686:16686"    # Jaeger UI
	  - "4318:4318"      # OTEL HTTP receiver (traces)
	  - "4317:4317"      # OTEL gRPC receiver
	  - "6831:6831/udp"  # Jaeger UDP (thrift)
	environment:
	  - COLLECTOR_OTLP_ENABLED=true
	networks:
	  - observability

  # Prometheus for Metrics
  prometheus:
	image: prom/prometheus:latest
	container_name: otel-prometheus
	ports:
	  - "9090:9090"
	volumes:
	  - ./prometheus.yaml:/etc/prometheus/prometheus.yaml
	  - prometheus_data:/prometheus
	command:
	  - '--config.file=/etc/prometheus/prometheus.yaml'
	  - '--storage.tsdb.path=/prometheus'
	networks:
	  - observability
	depends_on:
	  - jaeger

  # Grafana for Dashboards
  grafana:
	image: grafana/grafana:latest
	container_name: otel-grafana
	ports:
	  - "3000:3000"
	environment:
	  GF_SECURITY_ADMIN_PASSWORD: admin
	  GF_SECURITY_ADMIN_USER: admin
	  GF_INSTALL_PLUGINS: grafana-piechart-panel
	volumes:
	  - grafana_data:/var/lib/grafana
	  - ./grafana-datasource.yaml:/etc/grafana/provisioning/datasources/datasource.yaml
	  - ./grafana-dashboards.yaml:/etc/grafana/provisioning/dashboards/dashboard.yaml
	networks:
	  - observability
	depends_on:
	  - prometheus

volumes:
  prometheus_data:
  grafana_data:

networks:
  observability:
	driver: bridge
```

### Step 2: Create Prometheus Configuration

Create `prometheus.yaml` in the same directory:

```yaml
global:
  scrape_interval: 15s
  evaluation_interval: 15s

scrape_configs:
  - job_name: 'otel-collector'
	static_configs:
	  - targets: ['localhost:8888']

  - job_name: 'mystartupcorp-api'
	static_configs:
	  - targets: ['localhost:9091']
	metrics_path: '/metrics'

  - job_name: 'mystartupcorp-worker'
	static_configs:
	  - targets: ['localhost:9092']
	metrics_path: '/metrics'

  - job_name: 'mystartupcorp-notifier'
	static_configs:
	  - targets: ['localhost:9093']
	metrics_path: '/metrics'
```

### Step 3: Create Grafana Datasource Configuration

Create `grafana-datasource.yaml`:

```yaml
apiVersion: 1

datasources:
  - name: Prometheus
	type: prometheus
	access: proxy
	url: http://prometheus:9090
	isDefault: true
	editable: true
```

### Step 4: Create Grafana Dashboard Configuration

Create `grafana-dashboards.yaml`:

```yaml
apiVersion: 1

providers:
  - name: 'Dashboards'
	orgId: 1
	folder: ''
	type: file
	disableDeletion: false
	updateIntervalSeconds: 10
	allowUiUpdates: true
	options:
	  path: /var/lib/grafana/dashboards
```

### Step 5: Start Docker Compose Stack

```bash
# Navigate to docker-compose directory
cd docker-compose

# Start containers in background
docker-compose up -d

# Verify containers are running
docker-compose ps

# View logs (optional)
docker-compose logs -f
```

### Step 6: Verify Services Running

| Service | URL | Status Check |
|---------|-----|--------------|
| Jaeger | http://localhost:16686 | Should load search interface |
| Prometheus | http://localhost:9090 | Should show Targets page |
| Grafana | http://localhost:3000 | Should show login (admin/admin) |

---

## Application Configuration

### Update appsettings.Development.json

Each project needs OTEL configuration. Example for `MyStartUpCompany.Api`:

**File:** `src/MyStartUpCompany.Api/appsettings.Development.json`

```json
{
  "Observability": {
	"Enabled": true,
	"ServiceName": "MyStartUpCompany.Api",
	"ServiceVersion": "1.0.0",
	"Environment": "development",
	"SamplingRate": 1.0,
	"BatchSize": 64,
	"ExportInterval": 10000,
	"Exporters": {
	  "Console": {
		"Enabled": true,
		"Targets": "Traces|Metrics|Logs"
	  },
	  "Otlp": {
		"Enabled": true,
		"Endpoint": "http://localhost:4318",
		"Protocol": "http/protobuf"
	  },
	  "Prometheus": {
		"Enabled": true,
		"Port": 9091,
		"Path": "/metrics"
	  }
	}
  }
}
```

**Repeat for other projects:**
- `MyStartUpCompany.Worker` → Port 9092
- `MyStartUpCompany.Notifier` → Port 9093

### Environment Variables (Optional)

Override settings without changing files:

```powershell
# PowerShell
$env:Observability__Enabled = "true"
$env:Observability__ServiceName = "MyStartUpCompany.Api"
$env:Observability__Exporters__Otlp__Endpoint = "http://localhost:4318"
```

---

## Running Applications Locally

### Option A: Run in Visual Studio

1. Open `MyStartUpCompany.sln` in Visual Studio
2. Set startup projects to all three services (Ctrl+Alt+N)
3. Press F5 to start with debugging

### Option B: Run from Terminal

**Terminal 1: API Service**
```powershell
cd src/MyStartUpCompany.Api
dotnet run --configuration Development
# Should see: "Now listening on: https://localhost:7000"
```

**Terminal 2: Worker Service**
```powershell
cd src/MyStartUpCompany.Worker
dotnet run --configuration Development
# Should start background worker
```

**Terminal 3: Notifier Service**
```powershell
cd src/MyStartUpCompany.Notifier
dotnet run --configuration Development
# Should see: "Now listening on: https://localhost:7001"
```

### Expected Console Output

Each service should display OTEL exporters starting:

```
info: OpenTelemetry.Trace.TracerProvider[0]
	  TracerProvider is initialized
info: MyStartUpCompany.Observability[0]
	  OpenTelemetry configured: Console exporter enabled
info: MyStartUpCompany.Observability[0]
	  OpenTelemetry configured: OTLP exporter enabled
info: MyStartUpCompany.Observability[0]
	  OpenTelemetry configured: Prometheus metrics enabled on port 9091
```

---

## Testing OTEL Signals

### Test 1: Console Trace Output

**Action:** Make HTTP request to API

```powershell
curl -X GET "http://localhost:5000/api/companies"
```

**Expected Output in Console:**
- Log entries with TraceId in brackets
- HTTP request spans logged
- Response details

**Success Criteria:** ✅ TraceId visible in every log line

---

### Test 2: Jaeger Trace Visualization

**Action:** 
1. Make 5-10 HTTP requests: `curl "http://localhost:5000/api/companies"`
2. Open Jaeger UI: http://localhost:16686
3. Select service: "MyStartUpCompany.Api"
4. Click "Find Traces"

**Expected:**
- Traces appear in list with service name
- Click trace to see span details
- Spans show: HTTP handler, database query, response time

**Success Criteria:** ✅ Traces visible in Jaeger UI with correct service and spans

---

### Test 3: Prometheus Metrics

**Action:**
1. Make 5-10 HTTP requests
2. Open Prometheus: http://localhost:9090
3. Go to "Graph" tab
4. Search for metric: `http_requests_total`

**Expected:**
- Metric appears with labels
- Counter value > 0
- Can select different services/endpoints

**Success Criteria:** ✅ Metrics scraped and queryable in Prometheus

---

### Test 4: Grafana Dashboard

**Action:**
1. Open Grafana: http://localhost:3000
2. Login: admin/admin
3. Create new dashboard
4. Add Prometheus data source if not present
5. Add panel: Select metric `http_requests_total`

**Expected:**
- Dashboard loads without errors
- Prometheus datasource available
- Metrics graph shows request rate

**Success Criteria:** ✅ Grafana can query Prometheus and display metrics

---

### Test 5: Correlation Across Services

**Action:**
1. Create company via API: `curl -X POST "http://localhost:5000/api/companies" -d "{...}"`
2. Observe message published to Service Bus
3. Worker picks up message and processes
4. Check logs across all services

**Expected:**
- Same TraceId appears in API, Worker, and Notifier logs
- Can correlate request across all services
- Timing shows end-to-end latency

**Success Criteria:** ✅ TraceId flows across service boundaries

---

### Test 6: Error Handling

**Action:**
1. Trigger error: `curl "http://localhost:5000/api/companies/invalid-id"`
2. Check console for error logs
3. Check Jaeger for error span
4. Check Prometheus for error metrics

**Expected:**
- Error appears in console with full stack trace
- Jaeger shows error span with exception details
- Error counter increments in Prometheus

**Success Criteria:** ✅ Errors captured in all three signal types

---

## Validation Checklist

### Phase 1: Setup Verification
- [ ] Docker containers running (`docker-compose ps`)
- [ ] Jaeger UI accessible (http://localhost:16686)
- [ ] Prometheus UI accessible (http://localhost:9090)
- [ ] Grafana UI accessible (http://localhost:3000)
- [ ] All three services running without errors
- [ ] Console showing OTEL initialization logs

### Phase 2: Signal Verification
- [ ] ✅ **Console Traces:** TraceIds visible in console output
- [ ] ✅ **Jaeger Traces:** Traces appear after making requests
- [ ] ✅ **Prometheus Metrics:** http_requests_total visible
- [ ] ✅ **Grafana Dashboard:** Can display metrics
- [ ] ✅ **Correlation:** Same TraceId across services
- [ ] ✅ **Error Handling:** Exceptions captured and visible

### Phase 3: Production Readiness
- [ ] All tests passing locally
- [ ] Sampling strategy validated (100% in dev = expected)
- [ ] Custom business metrics working
- [ ] Startup time < 5 seconds per service
- [ ] No memory leaks (monitor for 10+ minutes)
- [ ] Documentation complete

---

## Troubleshooting

### Container Issues

**Problem:** Containers won't start
```bash
# Check logs
docker-compose logs jaeger

# Restart containers
docker-compose restart

# Reset everything
docker-compose down -v
docker-compose up -d
```

**Problem:** Port already in use
```bash
# Find what's using port (e.g., 16686)
netstat -ano | findstr :16686

# Kill process or change port in docker-compose.yaml
```

### OTEL Export Issues

**Problem:** "Failed to export traces"
- Check endpoint URL matches docker-compose (localhost:4318)
- Verify containers are running: `docker-compose ps`
- Check firewall isn't blocking localhost

**Problem:** "No traces in Jaeger"
- Verify OTLP exporter enabled in appsettings
- Check service logs for export errors
- Ensure batch export interval allows time for export (10s)

### Metrics Issues

**Problem:** "No metrics in Prometheus"
- Verify metrics endpoint reachable: `curl http://localhost:9091/metrics`
- Check prometheus.yaml job configuration
- Verify scrape interval (15s default)

**Problem:** "Can't connect Grafana to Prometheus"
- Ensure both containers on same network: `docker network inspect docker-compose_observability`
- Use service name not localhost: `http://prometheus:9090`

### Application Issues

**Problem:** "OTEL initialization failing"
- Check appsettings.json valid JSON
- Verify export endpoints reachable
- Check service has permission to create metrics

**Problem:** "High memory usage"
- Reduce batch size in appsettings
- Increase export interval
- Check for memory leaks in custom instrumentation

---

## Next Steps

### Confidence Milestones

Once all validation checks pass:

1. ✅ **Local Testing Complete** - OTEL works in development
2. ⏭️ **Integration Tests** - Add OTEL verification tests
3. ⏭️ **Load Testing** - Validate performance under load
4. ⏭️ **Documentation** - Team trained on OTEL patterns
5. ⏭️ **Azure Preparation** - Plan AKS deployment

### Team Training

- Share this guide with team members
- Run through validation checklist together
- Discuss trace/metric interpretation
- Establish observability standards

### Monitoring Improvement

With local OTEL working:
- Add custom business metrics
- Create Grafana dashboards for your SLOs
- Set up alerts for critical services
- Document runbook for common issues

### Cloud Deployment

When ready for Azure/AKS:
1. Review `docs_archived/AZURE_AKS_DEPLOYMENT_GUIDE.md`
2. Plan OTEL Collector deployment
3. Set up Azure Monitor integration
4. Configure production sampling

---

## References

- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)
- [Jaeger Getting Started](https://www.jaegertracing.io/docs/getting-started/)
- [Prometheus First Steps](https://prometheus.io/docs/prometheus/latest/getting_started/)
- [Grafana Dashboards](https://grafana.com/grafana/dashboards/)

---

**Document Version:** 1.0  
**Last Updated:** May 2026  
**Status:** ✅ Ready for Local Testing
