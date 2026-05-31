# Local OTEL Testing Confidence Checklist

Use this checklist to validate that OpenTelemetry is working correctly in local development before committing changes or planning for cloud deployment.

## Pre-Test Preparation

- [ ] Docker Desktop is installed and running
- [ ] .NET 10 SDK is installed and working
- [ ] Solution builds successfully: `dotnet build`
- [ ] All packages are vulnerability-free: `dotnet list package --vulnerable`
- [ ] User has read [LOCAL_OTEL_TESTING_GUIDE.md](LOCAL_OTEL_TESTING_GUIDE.md)

---

## Environment Setup

- [ ] Local OTEL stack is running:
  ```powershell
  cd docker-compose
  .\startup.ps1
  ```

- [ ] All services are healthy:
  ```powershell
  .\validate-otel-local.ps1
  ```

- [ ] Three Docker containers running:
  - [ ] Jaeger (http://localhost:16686)
  - [ ] Prometheus (http://localhost:9090)
  - [ ] Grafana (http://localhost:3000)

---

## Application Configuration

- [ ] `appsettings.Development.json` updated in all three projects:
  - [ ] **MyStartUpCompany.Api**
	- [ ] ServiceName: "MyStartUpCompany.Api"
	- [ ] OTLP Endpoint: "http://localhost:4318"
	- [ ] Prometheus Port: 9091
	- [ ] Console Exporter: Enabled

  - [ ] **MyStartUpCompany.Worker**
	- [ ] ServiceName: "MyStartUpCompany.Worker"
	- [ ] OTLP Endpoint: "http://localhost:4318"
	- [ ] Prometheus Port: 9092
	- [ ] Console Exporter: Enabled

  - [ ] **MyStartUpCompany.Notifier**
	- [ ] ServiceName: "MyStartUpCompany.Notifier"
	- [ ] OTLP Endpoint: "http://localhost:4318"
	- [ ] Prometheus Port: 9093
	- [ ] Console Exporter: Enabled

---

## OTEL Integration Tests

- [ ] Run unit tests:
  ```bash
  dotnet test tests/MyStartUpCompany.Observability.Tests
  ```

- [ ] All ObservabilityServiceCollectionTests pass:
  - [ ] AddObservability_ShouldRegisterTracerProvider
  - [ ] AddObservability_ShouldRegisterMeterProvider
  - [ ] AddObservability_WithDisabledFlag_ShouldNotRegisterProviders
  - [ ] AddObservability_ShouldConfigureCorrectServiceName
  - [ ] AddObservability_ShouldEnableAllInstrumentation_WhenConfigured
  - [ ] AddObservability_ShouldApplySamplingConfiguration

- [ ] All TraceCorrelationTests pass:
  - [ ] Activity_ShouldGenerateValidTraceId
  - [ ] Activity_ShouldPropagateTraceIdToChildren
  - [ ] Activity_ShouldGenerateUniqueSpanIds
  - [ ] Activity_ShouldRecordTags
  - [ ] Activity_ShouldRecordEvents
  - [ ] Activity_ShouldRecordExceptionAsEvent
  - [ ] ActivityListener_ShouldCaptureActivityLifecycle

- [ ] All BusinessMetricsTests pass:
  - [ ] BusinessMetrics_ShouldHaveHttpMetrics
  - [ ] BusinessMetrics_ShouldHaveDatabaseMetrics
  - [ ] BusinessMetrics_ShouldHaveMessageQueueMetrics
  - [ ] BusinessMetrics_ShouldHaveNotificationMetrics
  - [ ] BusinessMetrics_ShouldHaveHealthMetrics
  - [ ] BusinessMetrics_Counter_ShouldBeNonNegative
  - [ ] BusinessMetrics_Histogram_ShouldRecordDistributions
  - [ ] BusinessMetrics_ShouldBeThreadSafe

- [ ] All OtelEndToEndValidationTests pass:
  - [ ] Traces_ShouldBeGeneratedForHttpRequests
  - [ ] Metrics_ShouldBeRecordedForDatabaseQueries
  - [ ] CorrelationIds_ShouldFlowAcrossBoundaries
  - [ ] ExceptionTracking_ShouldRecordExceptionDetails
  - [ ] SamplingStrategy_ShouldBeApplied
  - [ ] MultipleServices_ShouldShareTraceContext
  - [ ] BaggageData_ShouldPropagateAcrossBoundaries

---

## Manual Signal Validation

### Signal 1: Traces (via Jaeger)

- [ ] Start API:
  ```bash
  dotnet run --project src/MyStartUpCompany.Api
  ```

- [ ] Generate traffic:
  ```powershell
  Invoke-WebRequest -Uri "http://localhost:5000/api/companies" -Method GET
  ```

- [ ] Check Jaeger:
  - [ ] Open http://localhost:16686
  - [ ] Service dropdown shows "MyStartUpCompany.Api"
  - [ ] Can search for recent traces
  - [ ] Click trace to see spans
  - [ ] Spans include:
	- [ ] HTTP request span
	- [ ] Database query spans
	- [ ] Proper span duration and nesting

- [ ] Verify trace data:
  - [ ] TraceID is consistent
  - [ ] SpanID is unique per span
  - [ ] Parent-child relationships are correct
  - [ ] Timestamps are accurate

### Signal 2: Metrics (via Prometheus)

- [ ] Check Prometheus:
  - [ ] Open http://localhost:9090
  - [ ] Click "Status" → "Targets"
  - [ ] Verify three services are listed:
	- [ ] mystartupcorp-api (http://host.docker.internal:9091/metrics)
	- [ ] mystartupcorp-worker (http://host.docker.internal:9092/metrics)
	- [ ] mystartupcorp-notifier (http://host.docker.internal:9093/metrics)
  - [ ] All targets show "UP" status

- [ ] Query basic metrics:
  - [ ] In "Expression" field, type: `http_requests_total`
  - [ ] Execute query
  - [ ] Results show HTTP requests with service label
  - [ ] Try query: `http_errors_total` (check error tracking)
  - [ ] Try query: `db_queries_total` (check database queries)

- [ ] Verify metric types:
  - [ ] Counters (increasing only): `http_requests_total`, `db_queries_total`
  - [ ] Histograms (distributions): `http_request_duration_ms`, `db_query_duration_ms`
  - [ ] Gauges (point-in-time): `process_runtime_* metrics`

### Signal 3: Logs (via Console Output)

- [ ] Check application console output:
  - [ ] Look for OTEL initialization message:
	```
	OpenTelemetry configured for [ServiceName]
	```
  - [ ] Look for trace ID in logs:
	```
	TraceId: [256-bit hex string]
	```
  - [ ] Look for structured log entries with:
	- [ ] Timestamp
	- [ ] LogLevel (Information, Warning, Error)
	- [ ] Message
	- [ ] CorrelationId

- [ ] Verify correlation:
  - [ ] Trace ID from Jaeger matches console log
  - [ ] Same trace ID appears across API, Worker, Notifier logs

---

## Correlation & Cross-Service Validation

- [ ] Start multiple services:
  - [ ] Terminal 1: `dotnet run --project src/MyStartUpCompany.Api`
  - [ ] Terminal 2: `dotnet run --project src/MyStartUpCompany.Worker`

- [ ] Generate cross-service request:
  ```powershell
  # This should trigger API → Database → (possibly Worker)
  Invoke-WebRequest -Uri "http://localhost:5000/api/companies" -Method POST `
	-Headers @{"Content-Type"="application/json"} `
	-Body '{"name":"Test","registrationNumber":"TEST123"}'
  ```

- [ ] Verify correlation in Jaeger:
  - [ ] Open http://localhost:16686
  - [ ] Find the trace for the POST request
  - [ ] Trace ID should be consistent across all services involved
  - [ ] Duration should represent end-to-end time

- [ ] Verify in Prometheus:
  - [ ] Query `http_requests_total` with service labels
  - [ ] Should show traffic to multiple services
  - [ ] Durations should match Jaeger measurements

---

## Error Handling Validation

- [ ] Generate an error:
  ```powershell
  # Request non-existent company
  Invoke-WebRequest -Uri "http://localhost:5000/api/companies/99999" -Method GET
  ```

- [ ] Verify error is captured:
  - [ ] Check Jaeger for error span
  - [ ] Error span should have exception event with:
	- [ ] Exception type
	- [ ] Exception message
	- [ ] Stack trace
  - [ ] Check Prometheus: `http_errors_total` should increase
  - [ ] Check console output for error log

- [ ] Check error metrics:
  - [ ] In Prometheus, query: `http_errors_total`
  - [ ] Should show increase after error was generated
  - [ ] Labels should indicate error type/endpoint

---

## Grafana Dashboard Validation

- [ ] Access Grafana:
  - [ ] Open http://localhost:3000
  - [ ] Login: admin / admin

- [ ] Create at least one dashboard:
  - [ ] Create HTTP request rate panel:
	- [ ] Query: `rate(http_requests_total[1m])`
	- [ ] Should show requests per second
  - [ ] Create error rate panel:
	- [ ] Query: `rate(http_errors_total[1m])`
	- [ ] Should be zero under normal load
  - [ ] Create database queries panel:
	- [ ] Query: `db_queries_total`
	- [ ] Should show cumulative query count

- [ ] Verify dashboard updates:
  - [ ] Generate traffic
  - [ ] Dashboard graphs should update in real-time
  - [ ] Panels should be interactive

---

## Documentation Verification

- [ ] Review documentation:
  - [ ] [OPENTELEMETRY_ARCHITECTURE.md](OPENTELEMETRY_ARCHITECTURE.md) - Read for understanding
  - [ ] [LOCAL_OTEL_TESTING_GUIDE.md](LOCAL_OTEL_TESTING_GUIDE.md) - Verify covers all test scenarios
  - [ ] [OBSERVABILITY_RUNBOOK.md](OBSERVABILITY_RUNBOOK.md) - Verify troubleshooting steps work
  - [ ] [docker-compose/README.md](../docker-compose/README.md) - Verify startup instructions work

- [ ] Documentation is complete:
  - [ ] No broken links
  - [ ] All commands are tested and working
  - [ ] Examples match current configuration
  - [ ] Troubleshooting section covers observed issues

---

## Team Readiness

- [ ] Team has been informed:
  - [ ] Shared OTEL architecture overview
  - [ ] Demonstrated local OTEL stack startup
  - [ ] Shown how to access Jaeger, Prometheus, Grafana
  - [ ] Explained signal types (traces, metrics, logs)

- [ ] Knowledge transfer items:
  - [ ] How to start local stack
  - [ ] How to interpret traces in Jaeger
  - [ ] How to query metrics in Prometheus
  - [ ] How to create dashboards in Grafana
  - [ ] How to troubleshoot common issues

- [ ] Runbooks are accessible:
  - [ ] Team can find [OBSERVABILITY_RUNBOOK.md](OBSERVABILITY_RUNBOOK.md)
  - [ ] Team can run validation script
  - [ ] Team knows where to find troubleshooting docs

---

## Performance & Load Testing

- [ ] Baseline metrics captured:
  - [ ] Normal request latency (p50, p95, p99)
  - [ ] Error rate under normal load
  - [ ] Memory usage during operation
  - [ ] Trace volume per second

- [ ] Load test completed:
  - [ ] Generated sustained traffic (e.g., 10 req/sec for 5 minutes)
  - [ ] System remained stable
  - [ ] Traces continued to be generated and captured
  - [ ] No OTEL exporter errors observed
  - [ ] Memory didn't leak during load

- [ ] Metrics under load:
  - [ ] Prometheus continued collecting metrics
  - [ ] No gaps in metric data
  - [ ] Latency metrics reasonable
  - [ ] Error metrics tracked correctly

---

## Cloud Readiness (Future)

- [ ] Scope verified:
  - [ ] [ ] Azure exporters NOT enabled in appsettings.Development.json
  - [ ] Confirmed local-only for now per requirement
  - [ ] Cloud deployment deferred to separate phase

- [ ] Azure artifacts archived:
  - [ ] [ ] Azure AKS deployment guide is in docs_archived/
  - [ ] [ ] K8s manifests are in k8s_archived/
  - [ ] [ ] Ready to be revisited when Azure work begins

---

## Final Sign-Off

- [ ] All checklist items completed
- [ ] All tests passing
- [ ] All signals validated (traces, metrics, logs)
- [ ] Team trained and ready
- [ ] Documentation complete and verified
- [ ] Date completed: ___________
- [ ] Completed by: ___________

---

## Confidence Statement

Once all items are checked:

> ✅ **Local OTEL implementation is validated and ready for use. All three signal types are flowing correctly, tests are passing, and the team understands how to use the local observability stack.**

---

## Next Steps

When ready for cloud deployment:
1. Review archived Azure/K8s documentation
2. Configure Azure Monitor exporters in appsettings
3. Set up Application Insights in Azure
4. Deploy to Azure Container Instances or AKS
5. Verify signals flow in Application Insights
6. Update monitoring and alerting rules

See [OPENTELEMETRY_ARCHITECTURE.md](OPENTELEMETRY_ARCHITECTURE.md#future-cloud-deployment) for more details.

---

## References

- **Guides**: [LOCAL_OTEL_TESTING_GUIDE.md](LOCAL_OTEL_TESTING_GUIDE.md)
- **Runbook**: [OBSERVABILITY_RUNBOOK.md](OBSERVABILITY_RUNBOOK.md)
- **Docker**: [docker-compose/README.md](../docker-compose/README.md)
- **Architecture**: [OPENTELEMETRY_ARCHITECTURE.md](OPENTELEMETRY_ARCHITECTURE.md)
- **Main Docs**: [docs/README.md](README.md)

---

**Document Status**: Ready for Use  
**Last Updated**: 2024  
**Scope**: Local Development and Testing
