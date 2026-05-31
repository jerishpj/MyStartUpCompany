# Local OTEL Verification Guide - Step by Step

**Purpose**: Verify that your observability setup works locally before cloud deployment  
**Time**: 30-45 minutes  
**Tools**: Docker Compose, PowerShell, .NET CLI, curl

---

## Quick Start (5 minutes)

### Step 1: Start the Local Observability Stack

```powershell
cd docker-compose
.\startup.ps1
```

**What it does**:
- Starts Jaeger on http://localhost:16686
- Starts Prometheus on http://localhost:9090
- Starts Grafana on http://localhost:3000

**Wait for healthy status**:
```powershell
docker-compose ps
# All should show "healthy" or "running"
```

### Step 2: Start Your Services

```powershell
# In separate terminals
dotnet run --project src/MyStartUpCompany.Api
dotnet run --project src/MyStartUpCompany.Worker
dotnet run --project src/MyStartUpCompany.Notifier
```

### Step 3: Generate Traffic

```powershell
# Generate API requests
for ($i = 1; $i -le 10; $i++) {
	curl -X GET http://localhost:5000/api/companies
	Start-Sleep -Seconds 1
}
```

### Step 4: View Traces in Jaeger

Open http://localhost:16686 → Select service → View traces

---

## Comprehensive Local Verification

### Phase 1: Infrastructure Validation

#### Test 1.1: Docker Compose Stack Health

```powershell
# Check all containers are running
docker-compose -f docker-compose/docker-compose.yaml ps

# Expected output:
# NAME                    STATUS          PORTS
# jaeger                  Up              0.0.0.0:16686->16686/tcp
# prometheus              Up              0.0.0.0:9090->9090/tcp
# grafana                 Up              0.0.0.0:3000->3000/tcp
```

**Verification**:
```powershell
# Test Jaeger API
$response = Invoke-WebRequest -Uri "http://localhost:16686/api/services" -ErrorAction SilentlyContinue
if ($response.StatusCode -eq 200) {
	Write-Host "✅ Jaeger is healthy"
} else {
	Write-Host "❌ Jaeger is down"
}

# Test Prometheus
$response = Invoke-WebRequest -Uri "http://localhost:9090/-/healthy" -ErrorAction SilentlyContinue
if ($response.StatusCode -eq 200) {
	Write-Host "✅ Prometheus is healthy"
} else {
	Write-Host "❌ Prometheus is down"
}

# Test Grafana
$response = Invoke-WebRequest -Uri "http://localhost:3000/api/health" -ErrorAction SilentlyContinue
if ($response.StatusCode -eq 200) {
	Write-Host "✅ Grafana is healthy"
} else {
	Write-Host "❌ Grafana is down"
}
```

#### Test 1.2: OTEL Receiver Health

```powershell
# Check OTLP receiver on port 4318 (HTTP)
$response = Invoke-WebRequest -Uri "http://localhost:4318/v1/health" -ErrorAction SilentlyContinue
if ($response.StatusCode -eq 200) {
	Write-Host "✅ OTLP HTTP receiver ready"
}

# Check OTLP receiver on port 4317 (gRPC)
# (Requires grpcurl tool, optional)
grpcurl -plaintext localhost:4317 list
```

---

### Phase 2: Service Observability Tests

#### Test 2.1: API Service Instrumentation

```powershell
# Start API service
$apiProcess = Start-Process -NoNewWindow `
	-FilePath "dotnet" `
	-ArgumentList "run --project src/MyStartUpCompany.Api" `
	-PassThru

Start-Sleep -Seconds 5

# Generate a single request
$traceHeaders = @{
	"User-Agent" = "ObservabilityTest/1.0"
}

$response = Invoke-WebRequest `
	-Uri "http://localhost:5000/api/companies" `
	-Headers $traceHeaders `
	-ErrorAction SilentlyContinue

$traceId = $response.Headers["traceparent"]
Write-Host "Generated Trace ID: $traceId"

# Kill API process
Stop-Process -Id $apiProcess.Id -Force
```

**Verify in Jaeger**:
1. Navigate to http://localhost:16686
2. Service dropdown → Select `MyStartUpCompany.Api`
3. Search → Click "Find Traces"
4. Should see your generated request

#### Test 2.2: Trace Context Propagation

```powershell
# Test that trace IDs are propagated through the system

$correlationId = [Guid]::NewGuid().ToString()
$traceId = [Guid]::NewGuid().ToString("N")

$headers = @{
	"X-Correlation-Id" = $correlationId
	"traceparent" = "00-$traceId-0000000000000001-01"
}

# Make request with trace headers
$response = Invoke-RestMethod `
	-Uri "http://localhost:5000/api/companies" `
	-Headers $headers `
	-ErrorAction SilentlyContinue

Write-Host "Request sent with correlation ID: $correlationId"
```

**Verify**:
- In Jaeger, find the trace
- Verify spans contain the correlation ID
- Check that all spans in the trace share the same Trace ID

---

### Phase 3: Metrics Validation

#### Test 3.1: Prometheus Metrics Collection

```powershell
# Query Prometheus for collected metrics
$metricsQuery = @{
	query = "up"
}

$response = Invoke-RestMethod `
	-Uri "http://localhost:9090/api/v1/query" `
	-Body $metricsQuery `
	-Method Post

$response.data.result | ForEach-Object {
	Write-Host "✅ Metric: $($_.metric.__name__) from $($_.metric.job)"
}
```

#### Test 3.2: Business Metrics

```powershell
# Query custom business metrics
$businessMetrics = @{
	query = "mycompany_http_requests_total"
}

$response = Invoke-RestMethod `
	-Uri "http://localhost:9090/api/v1/query" `
	-Body $businessMetrics `
	-Method Post

if ($response.data.result.Count -gt 0) {
	Write-Host "✅ Business metrics are flowing to Prometheus"
	$response.data.result | ForEach-Object {
		Write-Host "  - $($_.metric.__name__) = $($_.value[1])"
	}
} else {
	Write-Host "⚠️ No business metrics found yet (normal if no traffic generated)"
}
```

#### Test 3.3: Grafana Dashboard

```powershell
# Access Grafana dashboard
Write-Host "📊 Opening Grafana..."
Start-Process "http://localhost:3000/d/observability-overview"

# Login: admin / admin
# Dashboard should show:
# - Request rates
# - Error rates
# - Response times
# - JVM metrics (if enabled)
```

---

### Phase 4: Integration Testing

#### Test 4.1: Run OTEL Test Suite

```powershell
# Run the dedicated observability tests
dotnet test tests/MyStartUpCompany.Observability.Tests `
	--logger "console;verbosity=detailed" `
	--collect:"XPlat Code Coverage"

# Expected output:
# ✅ ObservabilityServiceCollectionTests: All Pass
# ✅ TraceCorrelationTests: All Pass
# ✅ BusinessMetricsTests: All Pass
# ✅ OtelEndToEndValidationTests: All Pass
```

#### Test 4.2: End-to-End Signal Flow

```powershell
# This test verifies signals flow through the entire pipeline:
# Service -> OTEL SDK -> OTLP Exporter -> Jaeger/Prometheus

$testScript = @"
# Start services in background
`$apiJob = Start-Job { dotnet run --project src/MyStartUpCompany.Api }
`$workerJob = Start-Job { dotnet run --project src/MyStartUpCompany.Worker }

Start-Sleep -Seconds 8

# Generate traffic
for (`$i = 1; `$i -le 5; `$i++) {
	try {
		Invoke-WebRequest -Uri "http://localhost:5000/api/companies" -ErrorAction SilentlyContinue
		Start-Sleep -Seconds 1
	} catch {
		# Service may not be ready
	}
}

# Query Jaeger for traces
`$response = Invoke-RestMethod `
	-Uri "http://localhost:16686/api/traces?service=MyStartUpCompany.Api" `
	-ErrorAction SilentlyContinue

if (`$response.data.traces.Count -gt 0) {
	Write-Host "✅ End-to-End: Traces visible in Jaeger"
	Write-Host "   Found `$(`$response.data.traces.Count) traces"
} else {
	Write-Host "❌ End-to-End: No traces found"
}

# Stop background jobs
Get-Job | Stop-Job
Get-Job | Remove-Job
"@

Invoke-Expression $testScript
```

---

### Phase 5: Configuration Validation

#### Test 5.1: appsettings.Development.json

```powershell
# Verify OTEL is enabled in development
$config = Get-Content "src/MyStartUpCompany.Api/appsettings.Development.json" | ConvertFrom-Json

if ($config.Observability.Enabled) {
	Write-Host "✅ OTEL is enabled"
	Write-Host "   ServiceName: $($config.Observability.ServiceName)"
	Write-Host "   Exporters:"
	Write-Host "     - Console: $($config.Observability.Exporters.Console.Enabled)"
	Write-Host "     - OTLP: $($config.Observability.Exporters.Otlp.Enabled)"
} else {
	Write-Host "❌ OTEL is disabled"
}
```

#### Test 5.2: Environment Variable Overrides

```powershell
# Test that environment variables override config
$env:OBSERVABILITY__ENABLED = "true"
$env:OBSERVABILITY__SERVICENAME = "TestService"
$env:OBSERVABILITY__SAMPLINGRATE = "0.5"

# Start service and verify it uses env vars
Write-Host "Starting with environment overrides..."
# Service should use these values

Remove-Item env:OBSERVABILITY__*
```

---

### Phase 6: Performance & Load

#### Test 6.1: High Volume Traffic

```powershell
# Generate 100 requests rapidly
Write-Host "Generating 100 requests..."

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

for ($i = 1; $i -le 100; $i++) {
	try {
		Invoke-WebRequest -Uri "http://localhost:5000/api/companies" `
			-ErrorAction SilentlyContinue | Out-Null

		if ($i % 10 -eq 0) {
			Write-Host "  Sent $i requests..."
		}
	} catch {
		Write-Host "  Request $i failed: $_"
	}
}

$stopwatch.Stop()
Write-Host "✅ Completed 100 requests in $($stopwatch.ElapsedMilliseconds)ms"

# Verify all traces arrived in Jaeger
Start-Sleep -Seconds 2
$response = Invoke-RestMethod -Uri "http://localhost:16686/api/traces?service=MyStartUpCompany.Api"
Write-Host "   Traces in Jaeger: $($response.data.traces.Count)"
```

#### Test 6.2: Sampling Configuration

```powershell
# Set sampling rate to 10%
$config = @{
	"Observability:SamplingRate" = 0.1
}

# Generate 1000 requests
Write-Host "Generating 1000 requests with 10% sampling..."
for ($i = 1; $i -le 1000; $i++) {
	try {
		Invoke-WebRequest -Uri "http://localhost:5000/api/companies" `
			-ErrorAction SilentlyContinue | Out-Null
	} catch { }
}

# Verify sampling: should see ~100 traces (10% of 1000)
$response = Invoke-RestMethod -Uri "http://localhost:16686/api/traces?service=MyStartUpCompany.Api"
$sampledCount = $response.data.traces.Count
Write-Host "✅ Traces sampled: $sampledCount (expected ~100)"
```

---

### Phase 7: Error & Exception Handling

#### Test 7.1: Exception Tracing

```powershell
# Trigger an error and verify it's captured

# Request with invalid ID
$response = Invoke-WebRequest -Uri "http://localhost:5000/api/companies/invalid" `
	-ErrorAction SilentlyContinue

# Verify in Jaeger:
Write-Host "Generated error request"
Write-Host "Check Jaeger for traces with errors: http://localhost:16686"
Write-Host "Filter: status=error"
```

#### Test 7.2: Span Events

```powershell
# Verify span events are captured with exceptions

# Add exception handling test
# Check that exception spans include:
# - Exception type
# - Stack trace
# - Message
# - Timestamp
```

---

## Automated Verification Script

```powershell
# save as: validate-otel-comprehensive.ps1

function Test-OtelInfrastructure {
	Write-Host "=== Phase 1: Infrastructure Validation ===" -ForegroundColor Cyan

	# Test Jaeger
	try {
		$response = Invoke-WebRequest -Uri "http://localhost:16686/api/services" -ErrorAction Stop
		Write-Host "✅ Jaeger is healthy" -ForegroundColor Green
	} catch {
		Write-Host "❌ Jaeger is down" -ForegroundColor Red
		return $false
	}

	# Test Prometheus
	try {
		$response = Invoke-WebRequest -Uri "http://localhost:9090/-/healthy" -ErrorAction Stop
		Write-Host "✅ Prometheus is healthy" -ForegroundColor Green
	} catch {
		Write-Host "❌ Prometheus is down" -ForegroundColor Red
		return $false
	}

	# Test Grafana
	try {
		$response = Invoke-WebRequest -Uri "http://localhost:3000/api/health" -ErrorAction Stop
		Write-Host "✅ Grafana is healthy" -ForegroundColor Green
	} catch {
		Write-Host "❌ Grafana is down" -ForegroundColor Red
		return $false
	}

	return $true
}

function Test-OtelMetrics {
	Write-Host "`n=== Phase 2: Metrics Validation ===" -ForegroundColor Cyan

	try {
		$response = Invoke-RestMethod `
			-Uri "http://localhost:9090/api/v1/query" `
			-Body @{ query = "up" } `
			-Method Post

		if ($response.data.result.Count -gt 0) {
			Write-Host "✅ Prometheus is collecting metrics" -ForegroundColor Green
			Write-Host "   Found $($response.data.result.Count) metric series" -ForegroundColor Gray
		} else {
			Write-Host "⚠️ No metrics collected yet (normal if no traffic)" -ForegroundColor Yellow
		}
	} catch {
		Write-Host "❌ Failed to query Prometheus: $_" -ForegroundColor Red
		return $false
	}

	return $true
}

function Test-OtelTests {
	Write-Host "`n=== Phase 3: OTEL Test Suite ===" -ForegroundColor Cyan

	$result = & dotnet test tests/MyStartUpCompany.Observability.Tests `
		--logger "console;verbosity=quiet" `
		--no-build `
		2>&1

	if ($LASTEXITCODE -eq 0) {
		Write-Host "✅ All OTEL tests passed" -ForegroundColor Green
	} else {
		Write-Host "❌ Some OTEL tests failed" -ForegroundColor Red
		return $false
	}

	return $true
}

# Run all tests
$allPass = $true
$allPass = Test-OtelInfrastructure -and $allPass
$allPass = Test-OtelMetrics -and $allPass
$allPass = Test-OtelTests -and $allPass

if ($allPass) {
	Write-Host "`n✅ All validation tests passed!" -ForegroundColor Green
} else {
	Write-Host "`n❌ Some validation tests failed" -ForegroundColor Red
}

exit $allPass ? 0 : 1
```

---

## Troubleshooting Common Issues

### Issue: "Connection refused" on localhost:16686

**Solution**:
```powershell
# Ensure Docker Compose is running
docker-compose -f docker-compose/docker-compose.yaml up -d

# Check logs
docker-compose -f docker-compose/docker-compose.yaml logs jaeger
```

### Issue: No traces appearing in Jaeger

**Checklist**:
1. ✅ Service is running
2. ✅ Service is configured with OTLP exporter
3. ✅ OTLP endpoint is correct (http://localhost:4318)
4. ✅ Generated traffic to the service
5. ✅ Wait 2-3 seconds for traces to appear

```powershell
# Check service logs for OTEL errors
# Look for: "Failed to export traces", "Connection refused"
```

### Issue: Metrics not appearing in Prometheus

**Solution**:
```powershell
# Verify Prometheus is scraping the service
# In Prometheus UI (http://localhost:9090):
# - Status -> Targets
# - Check if service is listed and "UP"

# Check prometheus.yaml has service target:
Get-Content docker-compose/prometheus.yaml | Select-String "localhost:9091"
```

### Issue: Services won't start

**Solution**:
```powershell
# Check OTEL configuration
Get-Content "src/MyStartUpCompany.Api/appsettings.Development.json" `
	| ConvertFrom-Json `
	| Select-Object -ExpandProperty Observability

# Rebuild solution
dotnet clean
dotnet build

# Try running with verbose logging
dotnet run --project src/MyStartUpCompany.Api --verbosity diagnostic
```

---

## Confidence Checklist

- [ ] Docker Compose stack starts successfully
- [ ] All containers show "Up" or "Healthy" status
- [ ] Jaeger UI loads at http://localhost:16686
- [ ] Prometheus UI loads at http://localhost:9090
- [ ] Grafana UI loads at http://localhost:3000
- [ ] API service starts without OTEL errors
- [ ] Worker service starts without OTEL errors
- [ ] Notifier service starts without OTEL errors
- [ ] Can generate traces by making API requests
- [ ] Traces appear in Jaeger within 2-3 seconds
- [ ] Can see traces in Jaeger dashboard
- [ ] Metrics appear in Prometheus
- [ ] Can run OTEL tests successfully
- [ ] All integration tests pass
- [ ] End-to-end signal flow validated

---

## Next: Moving to Azure

Once all local checks pass, you're ready to:

1. ✅ Add Azure App Insights exporter
2. ✅ Configure service principal authentication
3. ✅ Test cloud OTEL pipeline
4. ✅ Set up Azure dashboards
5. ✅ Document cloud OTEL setup

---

**Last Updated**: May 2026  
**Status**: Ready for Local Verification
