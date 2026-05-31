# OpenTelemetry Observability Architecture for MyStartUpCompany

**Version:** 2.0 (Local Development & Testing Phase)  
**Date:** May 2026  
**Target Environment:** Local Development (Docker Compose, Console Output)  
**Framework:** .NET 10  
**Status:** ✅ OTEL Implementation Complete - Local Testing Phase Active

> **Note:** This document covers local OTEL implementation and testing. Azure AKS deployment documentation has been archived for future use. See `docs_archived/AZURE_AKS_DEPLOYMENT_GUIDE.md` when ready for cloud deployment.

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Architecture Overview (Local)](#architecture-overview-local)
3. [Components](#components)
4. [Instrumentation Strategy](#instrumentation-strategy)
5. [Local Setup & Tools](#local-setup--tools)
6. [Testing & Validation](#testing--validation)
7. [Configuration](#configuration)
8. [Next Steps: Cloud Deployment](#next-steps-cloud-deployment)

---

## Executive Summary

This document outlines the OpenTelemetry observability implementation for MyStartUpCompany microservices, **focused on local development and validation**. The solution provides:

- **Distributed Tracing:** End-to-end request tracking across all services (Jaeger visualization)
- **Metrics Collection:** Application and runtime metrics (Prometheus + Grafana)
- **Structured Logging:** Console output + correlation IDs (extensible)
- **Custom Instrumentation:** Business-critical event tracking
- **Local Testing Tools:** Console, Jaeger, Prometheus, Grafana (Docker Compose)
- **Production Preparation:** Validated patterns ready for Azure AKS migration

---

## Architecture Overview (Local)

### Local Development Environment

```
┌─────────────────────────────────────────────────────────────────┐
│              Local Development Machine                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Services (Running locally or in containers):                   │
│  ┌──────────────────┐  ┌──────────────────┐  ┌────────────────┐│
│  │  MyStartUp.Api   │  │ MyStartUp.Worker │  │ MyStartUp.     ││
│  │  (localhost:5000)│  │ (Background Svc) │  │ Notifier       ││
│  │                  │  │                  │  │ (localhost:5001)││
│  └────────┬─────────┘  └────────┬─────────┘  └────────┬────────┘│
│           │                     │                     │         │
│           └─────────────────────┴─────────────────────┘         │
│                    │                                             │
│                    ▼                                             │
│          ┌──────────────────────┐                               │
│          │  OTEL Exporters      │                               │
│          ├──────────────────────┤                               │
│          │ • Console (stdout)   │────► Local console output     │
│          │ • OTLP HTTP          │                               │
│          │ • Prometheus Metrics │                               │
│          └──────────────────────┘                               │
│                    │                                             │
│    ┌───────────────┼───────────────┐                           │
│    │               │               │                           │
│    ▼               ▼               ▼                           │
├────────────────────────────────────────────────────────────────┤
│  Docker Compose Services (Optional, for visualization):         │
│                                                                  │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐  │
│  │ Jaeger UI      │  │  Prometheus    │  │   Grafana      │  │
│  │ :16686         │  │   :9090        │  │   :3000        │  │
│  │ (Traces)       │  │ (Metrics DB)   │  │ (Dashboards)   │  │
│  └────────────────┘  └────────────────┘  └────────────────┘  │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### Data Flow (Local Testing)

1. **Trace Exporting:**
   - Applications generate spans using OTEL SDK
   - Exported via OTLP/HTTP protocol to Jaeger
   - Jaeger stores and visualizes trace data
   - Access at `http://localhost:16686`

2. **Metrics Exporting:**
   - Applications expose metrics in Prometheus format
   - Prometheus scrapes metrics at configurable intervals
   - Grafana queries Prometheus for dashboard visualization
   - Access at `http://localhost:3000` (Grafana) or `http://localhost:9090` (Prometheus)

3. **Logs:**
   - Structured logging via console output (immediately visible)
   - Logs include correlation IDs for trace linkage
   - Can be extended to ELK/Loki stack later

---

## Components

### 1. OpenTelemetry SDK (.NET)

#### Packages Required
```
OpenTelemetry
OpenTelemetry.Exporter.Console (Development)
OpenTelemetry.Exporter.OTLP
OpenTelemetry.Exporter.AzureMonitor
OpenTelemetry.Instrumentation.AspNetCore
OpenTelemetry.Instrumentation.Http
OpenTelemetry.Instrumentation.SqlClient
OpenTelemetry.Instrumentation.Runtime
OpenTelemetry.Instrumentation.Process
OpenTelemetry.Instrumentation.StackExchangeRedis (if using Redis)
OpenTelemetry.Extensions.Hosting
```

#### Key Features
- **Traces:** Automatic ASP.NET Core instrumentation
- **Metrics:** HTTP, database, runtime metrics
- **Logs:** Structured logging integration
- **Context Propagation:** W3C Trace Context by default
- **Sampling:** Configurable sampling strategies

### 2. Observability Library

Create a shared NuGet package pattern:
- Centralized configuration
- Reusable middleware
- Common instrumentation
- Consistent across all services

### 3. OpenTelemetry Collector

Kubernetes native deployment:
- Local sidecar pattern or DaemonSet
- Receives metrics, traces, logs from applications
- Performs local sampling, filtering, enrichment
- Exports to backends (Azure Monitor, Prometheus, etc.)

### 4. Backend & Visualization

**Azure Monitor:**
- Application Insights
- Log Analytics Workspace
- Native Azure integration

**Grafana + Prometheus:**
- Self-hosted option
- Full control over data
- Cost savings for high-volume scenarios

### 5. Correlation Infrastructure

- **Trace ID:** Unique identifier for entire request
- **Span ID:** Segment of work within trace
- **W3C Trace Context:** Standard header propagation
- **Correlation ID:** Business-level tracking

---

## Local Setup & Tools

### Prerequisites
- Docker Desktop (for Jaeger, Prometheus, Grafana)
- .NET 10 SDK
- Visual Studio Community 2026 or VS Code
- PowerShell or Bash terminal

### Quick Start Checklist

**Step 1: Launch Local Observability Stack**
```bash
cd docker-compose  # Directory with local compose file
docker-compose up -d
```

**Step 2: Verify Services Running**
- **Jaeger UI:** http://localhost:16686
- **Prometheus:** http://localhost:9090
- **Grafana:** http://localhost:3000 (admin/admin)

**Step 3: Run Applications Locally**
```powershell
# Terminal 1: API
dotnet run --project src/MyStartUpCompany.Api/MyStartUpCompany.Api.csproj

# Terminal 2: Worker
dotnet run --project src/MyStartUpCompany.Worker/MyStartUpCompany.Worker.csproj

# Terminal 3: Notifier
dotnet run --project src/MyStartUpCompany.Notifier/MyStartUpCompany.Notifier.csproj
```

**Step 4: Generate Traffic & Observe**
- Make HTTP requests to API: `curl http://localhost:5000/api/companies`
- Check console output for logs with trace IDs
- View traces in Jaeger: http://localhost:16686 → Search "MyStartUpCompany"
- View metrics in Grafana: http://localhost:3000 → Dashboards

### Tools Overview

| Tool | Purpose | URL | Port |
|------|---------|-----|------|
| **Jaeger** | Distributed trace visualization | http://localhost:16686 | 6831 (UDP), 4318 (HTTP OTEL) |
| **Prometheus** | Metrics collection & storage | http://localhost:9090 | 9090 |
| **Grafana** | Metrics dashboards & alerts | http://localhost:3000 | 3000 |
| **Console Output** | Real-time logs & trace data | Terminal stdout | N/A |

### Configuration Files

**appsettings.Development.json** (per project):
```json
{
  "Observability": {
    "Enabled": true,
    "ServiceName": "MyStartUpCompany.Api",
    "ServiceVersion": "1.0.0",
    "Environment": "development",
    "SamplingRate": 1.0,  // 100% in development
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
        "Port": 9090
      }
    }
  }
}
```

---

## Testing & Validation

### Test Scenarios

#### 1. Trace Collection Test
- **Objective:** Verify spans are created and exported correctly
- **Action:** Make GET request to `/api/companies`
- **Validation:** 
  - Check console output for trace IDs
  - Search Jaeger for traces with service "MyStartUpCompany.Api"
  - Verify spans include: HTTP handler, database query, response

#### 2. Metrics Collection Test
- **Objective:** Verify metrics are recorded and exported
- **Action:** Make 10 requests in sequence
- **Validation:**
  - Prometheus shows `http.requests.total` counter incrementing
  - Grafana dashboard shows request rate graph

#### 3. Log Correlation Test
- **Objective:** Verify logs include correlation IDs
- **Action:** Make request, review console output
- **Validation:**
  - Each log line includes `TraceId` or `CorrelationId`
  - Can filter logs by trace ID in aggregated logs

#### 4. Error Handling Test
- **Objective:** Verify exceptions are tracked
- **Action:** Trigger error (invalid request, DB error, etc.)
- **Validation:**
  - Exception appears in Jaeger trace
  - Error event recorded in metrics
  - Console shows error log with trace context

#### 5. End-to-End Workflow Test
- **Objective:** Trace complete business flow (create company → notify)
- **Action:** Create company via API → observe Worker processing
- **Validation:**
  - Request trace flows across Api → Worker → Notifier
  - All services share same correlation ID
  - Metrics show complete flow timing

---



### By Application Type

#### MyStartUpCompany.Api (ASP.NET Web API)

**Automatic Instrumentation:**
- HTTP request/response
- Database queries (EF Core → SQL)
- Dependency calls
- Exception tracking

**Custom Instrumentation:**
- Query handler execution time
- Business logic events
- Custom metrics (e.g., "companies_queried", "projects_created")
- Feature flags and A/B tests

**Key Metrics:**
```
http.request.duration             // Request latency
http.requests.total               // Request count
database.query.duration           // DB query time
database.connections.active       // Connection pool state
handler.execution.duration        // Custom business logic
```

#### MyStartUpCompany.Worker (Background Service)

**Automatic Instrumentation:**
- Azure Service Bus message reception
- Message processing time
- Retry attempts
- Exception tracking

**Custom Instrumentation:**
- Message processing stages
- External API calls
- File processing metrics
- Queue depth and latency

**Key Metrics:**
```
servicebus.messages.received      // Message count
servicebus.messages.failed        // Failure count
processing.duration               // Processing time
external.api.calls.duration       // Dependency time
queue.depth                       // Current backlog
```

#### MyStartUpCompany.Notifier (ASP.NET Web API)

**Automatic Instrumentation:**
- HTTP endpoints
- Notification delivery
- Email/SMS/Push sending

**Custom Instrumentation:**
- Notification queue depth
- Delivery success rate
- Retry metrics
- Channel-specific metrics

**Key Metrics:**
```
notifications.sent.total          // Total sent
notifications.failed              // Failures
delivery.latency                  // Send time
retry.attempts                    // Retry count
```

### Sampling Strategy

**Development Environment:**
```
Trace Sampling:    100% (capture all for debugging)
Metrics:           All high-frequency metrics
Logs:              All severity levels
Batch Size:        Small (64 spans)
Export Interval:   10 seconds
```

**Production Environment:**
```
Trace Sampling:    5-10% (configurable based on volume)
  - Errors:        100% (always sample errors)
  - Slow requests: 100% (> 1000ms)
  - Normal:        5% (random sampling)
Metrics:           Aggregated (1-minute intervals)
Logs:              ERROR, WARNING, INFO (no DEBUG)
Batch Size:        Large (1024 spans)
Export Interval:   30 seconds
```

### Metrics Categories

#### System Metrics
```
cpu.usage                         // CPU percentage
memory.usage                      // Memory consumption
disk.io.read_bytes               // Disk reads
disk.io.write_bytes              // Disk writes
network.io.bytes_sent            // Network traffic
network.io.bytes_received
```

#### Application Metrics
```
http.request.duration            // Latency percentiles (p50, p95, p99)
http.requests.total              // Request count by method/path
database.queries.duration        // Query performance
database.connections.active      // Connection pool metrics
exceptions.total                 // Error tracking by type
```

#### Business Metrics
```
companies.created                // Business events
projects.assigned_type           // Feature usage
user.actions.processed           // User activity
api.endpoint.latency             // SLA tracking
message.processing.failed        // Processing errors
```

---

## Data Flow

### Request Lifecycle with Tracing

```
1. Incoming Request (API)
   ├─ TraceID: auto-generated (W3C)
   ├─ SpanID: generated for HTTP span
   ├─ HTTP Request Span (automatic)
   │  └─ Duration, method, path, status code
   │
2. Query Handler Execution
   ├─ SpanID: new span for handler
   ├─ Database Query Spans (automatic)
   │  ├─ SQL execution time
   │  ├─ Connection time
   │  └─ Connection pool state
   ├─ Custom Instrumentation (explicit)
   │  └─ Business logic duration
   │
3. Response Generation
   ├─ Response span metrics
   ├─ Serialization time
   └─ Exception (if any)
   │
4. Distributed Export
   ├─ Batch collection (1024 spans)
   ├─ OTLP export to collector
   └─ Collector processes & routes
	   ├─ To Azure Monitor
	   ├─ To Prometheus
	   └─ To Log Analytics
```

### Message Processing Lifecycle

```
1. Message Received (Worker)
   ├─ Extract trace context from message headers
   ├─ Create processor span with inherited TraceID
   │
2. Message Processing
   ├─ Parse & validate
   ├─ External API calls (instrumented)
   ├─ Database operations (instrumented)
   │
3. State Management
   ├─ Success: mark span as successful
   ├─ Failure: record exception
   ├─ Retry: create new span with same TraceID
   │
4. Telemetry Export
   └─ Send complete trace to backend
```

---

## Configuration

### Environment Variables (AKS)

```bash
# OpenTelemetry Settings
OTEL_EXPORTER_OTLP_ENDPOINT=http://otel-collector:4317
OTEL_TRACES_EXPORTER=otlp
OTEL_METRICS_EXPORTER=otlp
OTEL_LOGS_EXPORTER=otlp
OTEL_PROPAGATORS=tracecontext,baggage

# Service Identity
OTEL_SERVICE_NAME=mystartup-api
OTEL_SERVICE_VERSION=1.0.0
OTEL_SERVICE_NAMESPACE=mystartupcorp

# Sampling
OTEL_TRACES_SAMPLER=traceidratio
OTEL_TRACES_SAMPLER_ARG=0.10  # 10% sampling

# Resource Detection
OTEL_RESOURCE_DETECTORS=gcp,env,process
OTEL_RESOURCE_DETECTION_ENABLED=true
```

### appsettings.json

```json
{
  "Observability": {
	"Enabled": true,
	"ServiceName": "MyStartUpCompany.Api",
	"ServiceVersion": "1.0.0",
	"Environment": "production",
	"SamplingRate": 0.10,
	"BatchSize": 1024,
	"ExportInterval": 30000,
	"Exporters": {
	  "Otlp": {
		"Enabled": true,
		"Endpoint": "http://otel-collector:4317",
		"Protocol": "grpc"
	  },
	  "AzureMonitor": {
		"Enabled": false,
		"ConnectionString": ""
	  },
	  "Console": {
		"Enabled": false
	  }
	},
	"Tracing": {
	  "Enabled": true,
	  "AspNetCore": true,
	  "Http": true,
	  "SqlClient": true
	},
	"Metrics": {
	  "Enabled": true,
	  "AspNetCore": true,
	  "Runtime": true,
	  "Process": true
	}
  }
}
```

---

## Deployment

### Azure AKS Considerations

#### Resource Requirements

```yaml
# OpenTelemetry Collector Pod
resources:
  requests:
	cpu: 100m
	memory: 128Mi
  limits:
	cpu: 500m
	memory: 512Mi

# Application Pods (with observability)
resources:
  requests:
	cpu: 200m
	memory: 256Mi
  limits:
	cpu: 1000m
	memory: 512Mi  # Extra memory for instrumentation
```

#### Network Policies

```
- Allow internal cluster communication on port 4317 (OTLP/gRPC)
- Allow outbound to backend (Azure Monitor or Prometheus)
- Secure TLS for external connections
```

#### Node Affinity

```yaml
affinity:
  podAntiAffinity:
	preferredDuringSchedulingIgnoredDuringExecution:
	- weight: 100
	  podAffinityTerm:
		labelSelector:
		  matchExpressions:
		  - key: app
			operator: In
			values:
			- otel-collector
		topologyKey: kubernetes.io/hostname
```

### Health Checks

```csharp
// Health check for observability
app.MapHealthChecks("/health/observability", new HealthCheckOptions
{
	Predicate = check => check.Tags.Contains("observability"),
	ResponseWriter = WriteHealthResponse
});
```

---

## Monitoring & Dashboards

### Key Dashboards

#### 1. System Health Dashboard
- Pod CPU/Memory utilization
- Network I/O
- Disk usage
- Node status

#### 2. Application Performance Dashboard
- Request latency (p50, p95, p99)
- Error rate
- Throughput (RPS)
- Active connections

#### 3. Database Performance Dashboard
- Query count and duration
- Connection pool state
- Slow queries (top 10)
- Index usage

#### 4. Business Metrics Dashboard
- Companies created/updated
- Projects by type
- Processing queue depth
- Message failures

#### 5. Service Dependencies Dashboard
- Service-to-service latency
- Dependency error rates
- Circuit breaker state
- Cache hit rates

### Alert Rules

```
System Alerts:
├─ Pod CrashLoopBackOff
├─ High CPU (> 80%)
├─ High Memory (> 85%)
├─ Disk Space Low (< 10%)

Application Alerts:
├─ Error Rate > 1%
├─ Latency p95 > 1000ms
├─ Request Timeout > 5%
└─ Health Check Failed

Business Alerts:
├─ Processing Queue > 1000 messages
├─ Message Processing Failure > 5%
├─ Notification Delivery Failure > 2%
└─ External API Timeout > 3%
```

---

## Best Practices

### 1. Trace Context Propagation
- Always use W3C Trace Context headers
- Propagate across service boundaries
- Include TraceID in logs for correlation
- Use Baggage for custom context

### 2. Instrumentation
- Instrument at service boundaries
- Capture meaningful business context
- Use consistent naming conventions
- Document custom metrics

### 3. Sampling
- Never sample errors
- Always sample slow requests
- Use adaptive sampling in production
- Monitor sampling impact on insights

### 4. Performance
- Batch spans before export
- Use async export operations
- Monitor SDK overhead (< 1% CPU)
- Profile in production-like environment

### 5. Data Management
- Implement retention policies
- Anonymize sensitive data
- Compress traces before export
- Archive for compliance

### 6. Cost Optimization
- Right-size sampling rates
- Aggregate metrics at collection
- Use metric cardinality limits
- Archive old data

---

## Cost Optimization

### Azure Monitor Pricing Model
```
Data Ingestion:
├─ Logs: $0.50-2.50/GB (depending on tier)
├─ Metrics: $0.50-1.50/million datapoints
└─ Traces: $0.50-5.00/million samples

Typical Monthly Cost (100K requests/day):
├─ Traces: $50-150 (5% sampling)
├─ Metrics: $100-300 (high cardinality)
├─ Logs: $200-500
└─ Total: ~$500-1000
```

### Cost Reduction Strategies

```
1. Sampling Strategy
   ├─ Reduce trace sampling to 1-5%
   ├─ Error-based sampling (100%)
   └─ Latency-based sampling (slow requests)

2. Metric Aggregation
   ├─ Pre-aggregate at SDK level
   ├─ Reduce metric cardinality
   └─ Use coarse-grained metrics

3. Log Management
   ├─ Exclude debug logs in production
   ├─ Sample verbose endpoints
   └─ Archive older data

4. Infrastructure
   ├─ Use self-hosted Prometheus (free)
   ├─ Grafana Cloud (cheaper for metrics)
   └─ Dual-export strategy
```

---

## Security Considerations

### Data Protection
- Encrypt traces in transit (TLS/gRPC)
- Encrypt at rest (encryption at storage level)
- Redact sensitive data (PII, credentials)
- Implement field-level filtering

### Access Control
- RBAC for dashboard access
- Service principal for exports
- Network policies for collector access
- Audit logging for configuration changes

### Compliance
- GDPR compliance (PII handling)
- SOC 2 requirements (data retention)
- Audit trails for all configuration
- Data residency requirements

---

## Future Enhancements

1. **Machine Learning Integration**
   - Anomaly detection for metrics
   - Intelligent alerting
   - Capacity planning

2. **Advanced Correlation**
   - User journey tracking
   - Feature usage correlation
   - Business impact analysis

3. **Synthetic Monitoring**
   - Synthetic transactions
   - SLA monitoring
   - Dependency checks

4. **Custom Dashboards**
   - Business KPI tracking
   - SLA dashboards
   - Cost tracking

5. **Advanced Analytics**
   - Trace analysis automation
   - Root cause analysis
   - Performance trend analysis

---

## Conclusion

## Next Steps: Cloud Deployment

Once local testing confirms OTEL is working correctly, the following steps prepare for Azure/AKS deployment:

### Pre-Cloud Checklist
- [ ] All local tests passing (trace, metrics, logs, E2E)
- [ ] Sampling strategy validated for expected load
- [ ] Custom instrumentation meets requirements
- [ ] Documentation complete and reviewed
- [ ] Team trained on OTEL concepts

### Azure/AKS Migration Plan
When ready for cloud deployment, refer to archived documentation:
- `docs_archived/AZURE_AKS_DEPLOYMENT_GUIDE.md` - Full AKS deployment guide
- `k8s_archived/` - Kubernetes manifests for services
- `k8s_archived/02-otel-collector.yaml` - OTEL Collector for K8s
- `k8s_archived/03-jaeger.yaml` - Jaeger backend setup
- `k8s_archived/04-prometheus.yaml` - Prometheus installation

### Cloud Configuration Changes
```
LOCAL (Development)          │  AZURE (Production)
─────────────────────────────┼──────────────────────
Console Exporter: Enabled    │  Disabled
OTLP Exporter: Local         │  Azure Monitor Exporter
Sampling: 100%               │  5-10% (errors: 100%)
Batch Size: 64               │  1024
Export Interval: 10s          │  30s
```

---

## Summary

This observability architecture provides:

✅ **Local Testing First** - Validates OTEL before cloud deployment  
✅ **Complete Visibility** - Traces, metrics, logs across all services  
✅ **Production-Ready** - Enterprise-grade reliability and scalability  
✅ **Cost-Optimized** - Configurable sampling and aggregation  
✅ **Cloud-Native Ready** - Patterns optimized for Azure AKS deployment  
✅ **Flexible** - Support for multiple backends and exporters  
✅ **Standards-Based** - Using OpenTelemetry and W3C standards  

The implementation follows industry best practices and provides a solid foundation for local development, testing, troubleshooting, and performance optimization of the MyStartUpCompany microservices. Cloud deployment patterns are documented and ready for future use.

---

**Document Version:** 2.0 (Local Testing Phase)  
**Last Updated:** May 2026  
**Status:** ✅ Local Implementation Complete - Testing Phase Active  
**Next Update:** After Azure/AKS deployment preparation
