# Local Observability Runbook

A practical guide for common observability tasks and troubleshooting in local development using OpenTelemetry.

## Quick Reference

### Start the Stack
```powershell
cd docker-compose
.\startup.ps1
```

### Stop the Stack
```powershell
cd docker-compose
.\startup.ps1 -Stop
```

### View Logs
```bash
docker-compose logs -f jaeger
docker-compose logs -f prometheus
docker-compose logs -f grafana
```

### Validate Setup
```powershell
.\validate-otel-local.ps1
```

---

## Common Tasks

### Task 1: Start Local OTEL Stack and Applications

**Prerequisites**: Docker Desktop running

**Steps**:
1. Open terminal in `docker-compose` folder
2. Run `.\startup.ps1`
3. Wait for "Local OTEL Stack Ready" message
4. In separate terminal, start API: `dotnet run --project src/MyStartUpCompany.Api`
5. In another terminal, start Worker: `dotnet run --project src/MyStartUpCompany.Worker`
6. Optionally start Notifier: `dotnet run --project src/MyStartUpCompany.Notifier`

**Verify**:
- Jaeger UI: http://localhost:16686
- Prometheus: http://localhost:9090
- Grafana: http://localhost:3000

---

### Task 2: Generate Test Traffic

**Using PowerShell**:
```powershell
# Get all companies (generates API + DB traces)
Invoke-WebRequest -Uri "http://localhost:5000/api/companies" -Method GET

# Get single company
Invoke-WebRequest -Uri "http://localhost:5000/api/companies/1" -Method GET

# Create new company
$body = @{
	name = "Test Company"
	registrationNumber = "REG123456"
	industry = "Technology"
	founded = "2024-01-01"
	headquarters = "New York"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5000/api/companies" -Method POST `
	-Headers @{"Content-Type"="application/json"} `
	-Body $body
```

**Using curl (bash)**:
```bash
# Get all companies
curl http://localhost:5000/api/companies

# Get single company
curl http://localhost:5000/api/companies/1

# Create new company
curl -X POST http://localhost:5000/api/companies \
  -H "Content-Type: application/json" \
  -d '{
	"name": "Test Company",
	"registrationNumber": "REG123456",
	"industry": "Technology",
	"founded": "2024-01-01",
	"headquarters": "New York"
  }'
```

**Check Results**:
1. Open Jaeger: http://localhost:16686
2. Select "MyStartUpCompany.Api" from service dropdown
3. Search for traces
4. Click trace ID to see the full trace with all spans

---

### Task 3: View Metrics in Prometheus

**Query Examples**:

```promql
# HTTP requests per second
rate(http_requests_total[1m])

# API request count
http_requests_total{service="api"}

# Database query duration
db_query_duration_ms{service="api"}

# Error rate
rate(http_errors_total[1m])

# Notification delivery success
notifications_delivered_total
```

**Steps**:
1. Open http://localhost:9090
2. Type query in "Expression" field
3. Click "Execute"
4. View results in "Graph" tab for visualization

---

### Task 4: Create Custom Dashboard in Grafana

**Steps**:
1. Open http://localhost:3000 (admin/admin)
2. Click "+" icon → "Dashboard" → "New dashboard"
3. Click "Add a new panel"
4. Configure panel:
   - **Data source**: Prometheus
   - **Metrics**: Select from dropdown (e.g., `http_requests_total`)
   - **Legend**: Add labels for clarity
   - **Title**: Give it a name (e.g., "HTTP Requests")
5. Click "Apply"
6. Repeat for more panels
7. Click "Save dashboard" and give it a name

**Sample Panels to Create**:

| Panel | Metric | Description |
|-------|--------|-------------|
| HTTP Request Rate | `rate(http_requests_total[1m])` | Requests per second |
| Error Rate | `rate(http_errors_total[1m])` | Errors per second |
| Database Queries | `db_queries_total` | Total DB queries |
| Query Duration | `db_query_duration_ms` | DB query latency |
| Active Spans | `traces_total` | Active traces |

---

### Task 5: Debug Trace Correlation Issues

**Problem**: Trace IDs don't match across services

**Troubleshooting**:

1. **Check Correlation ID Header**
   ```powershell
   # Make request and capture headers
   $response = Invoke-WebRequest -Uri "http://localhost:5000/api/companies" `
	   -ResponseHeadersVariable 'headers'

   # Check for trace ID header
   $headers['traceparent']
   ```

2. **Verify appsettings Configuration**
   - Check `appsettings.Development.json` in each project
   - Ensure OTLP endpoint is `http://localhost:4318`
   - Ensure Prometheus ports are different (9091, 9092, 9093)

3. **Check Console Output**
   - Look for trace ID in application console
   - Should match Jaeger trace ID
   - Format: 256-bit hex string

4. **Validate Correlation Middleware**
   ```csharp
   // Should be in Program.cs
   app.UseTraceContext();
   app.UseHttpMetrics();
   ```

---

### Task 6: Monitor Specific Service

**For API Service**:
```promql
# Requests to /api/companies endpoint
http_requests_total{service="api", path=~"/api/companies.*"}

# API response times
histogram_quantile(0.95, rate(http_request_duration_ms[5m]))

# API error count
http_errors_total{service="api"}
```

**For Worker Service**:
```promql
# Messages processed
messages_processed{service="worker"}

# Message processing duration
message_processing_duration_ms{service="worker"}

# Worker errors
errors_total{service="worker"}
```

**For Notifier Service**:
```promql
# Notifications delivered
notifications_delivered_total

# Delivery duration
notification_delivery_duration_ms

# Notification errors
errors_total{service="notifier"}
```

---

### Task 7: Analyze Performance Bottleneck

**Steps**:

1. **Generate Load** (using Task 2 approach)
   - Make multiple requests to identify slowness

2. **Check Prometheus Metrics**
   - Query database query duration:
	 ```promql
	 histogram_quantile(0.95, db_query_duration_ms)
	 ```
   - If > 500ms, database might be bottleneck

3. **Check Jaeger Traces**
   - Open http://localhost:16686
   - Look for longest span
   - Identify which service/operation is slowest

4. **Optimize**
   - Add database indexes
   - Review query logic
   - Add caching if appropriate

5. **Verify**
   - Re-run test traffic
   - Compare metrics before/after

---

## Troubleshooting

### Container Won't Start

**Symptom**: `docker-compose up` fails

**Solutions**:
```powershell
# Check container logs
docker-compose logs jaeger

# Remove stopped containers
docker-compose down -v

# Rebuild from scratch
docker-compose up -d --force-recreate
```

### Port Already in Use

**Symptom**: "Port 16686 already in use"

**Solution**: Edit `docker-compose/docker-compose.yaml`
```yaml
ports:
  - "16686:16686"  # Change first number to different port
```

### Application Can't Connect to Jaeger

**Symptom**: No traces appear in Jaeger after making requests

**Solutions**:
1. Check endpoint in `appsettings.Development.json`:
   ```json
   "Endpoint": "http://localhost:4318"
   ```

2. Check Docker network from application:
   ```bash
   # From Windows/Mac
   # Use host.docker.internal instead of localhost
   # This is already configured in prometheus.yaml
   ```

3. Verify network connectivity:
   ```powershell
   Test-NetConnection -ComputerName localhost -Port 4318
   ```

### Metrics Not Appearing in Prometheus

**Symptom**: Query returns "no data"

**Solutions**:
1. Check Prometheus scrape config
   - Open http://localhost:9090/config
   - Verify targets for all three services

2. Check targets status
   - http://localhost:9090/targets
   - Should show 3 applications + prometheus itself

3. Verify application metrics endpoint:
   ```powershell
   Invoke-WebRequest -Uri "http://localhost:9091/metrics"
   # Should return Prometheus format
   ```

4. Check appsettings Prometheus config:
   ```json
   "Prometheus": {
	 "Enabled": true,
	 "Port": 9091,
	 "Path": "/metrics"
   }
   ```

### No Logs in Jaeger

**Symptom**: Traces appear but no log entries

**Solutions**:
1. Check Console exporter is enabled:
   ```json
   "Console": {
	 "Enabled": true,
	 "Targets": "All"
   }
   ```

2. Check application console output for warnings
3. Verify logging configuration in `appsettings`:
   ```json
   "Logging": {
	 "LogLevel": {
	   "Default": "Debug",
	   "Microsoft.AspNetCore": "Information"
	 }
   }
   ```

---

## Performance Tips

### Local Development
- **100% sampling** in `appsettings.Development.json` (for complete visibility)
- **Console exporter enabled** (for quick feedback)
- Keep **batch size small** (64 items) for faster export

### Production-Ready
- **Probabilistic sampling** (e.g., 10% or lower)
- **Disable console exporter** (too verbose)
- **Increase batch size** (256+ items) for efficiency
- **Use Azure Monitor exporter** instead of console

### Monitoring the Monitors
- Always monitor Jaeger/Prometheus/Grafana health
- Check http://localhost:16686/search (Jaeger API health)
- Check http://localhost:9090/-/healthy (Prometheus health)
- Check http://localhost:3000/api/health (Grafana health)

---

## Best Practices

✅ **DO**:
- Start stack before running applications
- Validate with `.\validate-otel-local.ps1` after startup
- Generate test traffic regularly to keep pipelines warm
- Create dashboards for frequently-checked metrics
- Review traces for new features before production
- Use correlation IDs for multi-service requests
- Monitor both happy path and error scenarios

❌ **DON'T**:
- Leave stack running 24/7 when not in use (wastes resources)
- Use 100% sampling in production (too much data)
- Ignore sampling configuration
- Hardcode service endpoints (use config)
- Skip testing OTEL integration for new services
- Mix local and cloud exporters in same appsettings

---

## References

- **Setup**: See [LOCAL_OTEL_TESTING_GUIDE.md](LOCAL_OTEL_TESTING_GUIDE.md)
- **Architecture**: See [OPENTELEMETRY_ARCHITECTURE.md](OPENTELEMETRY_ARCHITECTURE.md)
- **Docker Compose**: See [docker-compose/README.md](../docker-compose/README.md)
- **OTEL Documentation**: https://opentelemetry.io/docs/instrumentation/net/
- **Prometheus Queries**: https://prometheus.io/docs/prometheus/latest/querying/basics/
- **Jaeger**: https://www.jaegertracing.io/docs/

---

**Last Updated**: 2024
**Status**: Ready for Local Development
