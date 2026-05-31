# Local OTEL Stack - Docker Compose

This directory contains Docker Compose setup for local OpenTelemetry testing.

## Quick Start

### Windows / PowerShell
```powershell
# Start the stack
.\startup.ps1

# View logs
docker-compose logs -f

# Stop the stack
.\startup.ps1 -Stop

# Clean and restart
.\startup.ps1 -Clean
```

### macOS / Linux / Bash
```bash
# Start the stack
./startup.sh

# View logs
docker-compose logs -f

# Stop the stack
docker-compose down
```

## Services

| Service | Port | URL | Purpose |
|---------|------|-----|---------|
| **Jaeger** | 16686 | http://localhost:16686 | Distributed Trace Visualization |
| **Prometheus** | 9090 | http://localhost:9090 | Metrics Collection & Querying |
| **Grafana** | 3000 | http://localhost:3000 | Dashboards (admin/admin) |

## Configuration Files

- **docker-compose.yaml** - Service definitions and networking
- **prometheus.yaml** - Prometheus scrape configuration
- **grafana-datasource.yaml** - Grafana data source setup
- **.env** - Environment variables (optional overrides)

## Metrics Endpoints (from applications)

Applications should expose metrics on:
- API: http://localhost:9091/metrics
- Worker: http://localhost:9092/metrics
- Notifier: http://localhost:9093/metrics

Configure these in `appsettings.Development.json`:
```json
"Exporters": {
  "Prometheus": {
	"Enabled": true,
	"Port": 9091,
	"Path": "/metrics"
  }
}
```

## Troubleshooting

### Containers won't start
```bash
# Check container logs
docker-compose logs jaeger
docker-compose logs prometheus
docker-compose logs grafana

# Restart services
docker-compose restart

# Full reset
docker-compose down -v
docker-compose up -d
```

### Port conflicts
Edit `docker-compose.yaml` to change ports:
```yaml
ports:
  - "16686:16686"  # Change first number to different port
```

### Applications can't reach Jaeger

**Windows/Mac:** Use `host.docker.internal` (configured in prometheus.yaml)
**Linux:** Use host IP address (usually 172.17.0.1)

In appsettings:
```json
"Otlp": {
  "Endpoint": "http://localhost:4318"
}
```

## Next Steps

1. Start the stack: `.\startup.ps1`
2. Run .NET applications locally
3. Make HTTP requests to generate traffic
4. View traces in Jaeger: http://localhost:16686
5. View metrics in Prometheus: http://localhost:9090
6. Create dashboards in Grafana: http://localhost:3000

See `docs/LOCAL_OTEL_TESTING_GUIDE.md` for complete testing guide.

## Documentation

- **LOCAL_OTEL_TESTING_GUIDE.md** - Complete testing guide
- **OPENTELEMETRY_ARCHITECTURE.md** - Architecture overview
- **docs/README.md** - Documentation index
