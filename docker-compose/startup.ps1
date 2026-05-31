# Quick Start Script for Local OTEL Testing Stack (PowerShell)
# Run: .\startup.ps1 from docker-compose directory

param(
	[switch]$Clean,
	[switch]$Logs,
	[switch]$Stop
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "MyStartUpCompany OTEL Local Stack Startup" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Handle stop flag
if ($Stop) {
	Write-Host "Stopping Docker Compose stack..." -ForegroundColor Yellow
	docker-compose down
	Write-Host "✅ Stack stopped" -ForegroundColor Green
	exit 0
}

# Check if docker is installed
Write-Host "Checking Docker installation..." -ForegroundColor Gray
try {
	$dockerVersion = docker --version
	Write-Host "✅ Docker found: $dockerVersion" -ForegroundColor Green
} catch {
	Write-Host "❌ Docker not found. Please install Docker Desktop." -ForegroundColor Red
	exit 1
}

# Check if docker daemon is running
Write-Host "Checking Docker daemon..." -ForegroundColor Gray
try {
	docker ps | Out-Null
	Write-Host "✅ Docker daemon is running" -ForegroundColor Green
} catch {
	Write-Host "❌ Docker daemon not running. Please start Docker Desktop." -ForegroundColor Red
	exit 1
}

Write-Host ""

# Handle clean flag
if ($Clean) {
	Write-Host "Cleaning up existing containers and volumes..." -ForegroundColor Yellow
	docker-compose down -v
	Write-Host "✅ Cleanup complete" -ForegroundColor Green
	Write-Host ""
}

# Start containers
Write-Host "Starting Docker Compose stack..." -ForegroundColor Yellow
docker-compose up -d

Write-Host ""
Write-Host "Waiting for services to be healthy..." -ForegroundColor Gray
Start-Sleep -Seconds 10

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Service Status:" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

$services = @("jaeger", "prometheus", "grafana")
foreach ($service in $services) {
	$status = docker-compose ps | Select-String $service
	if ($status) {
		Write-Host "✅ $service is running" -ForegroundColor Green
	} else {
		Write-Host "❌ $service failed to start" -ForegroundColor Red
	}
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Local OTEL Stack Ready!" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Service URLs:" -ForegroundColor Cyan
Write-Host "  * Jaeger UI (Traces):   http://localhost:16686"
Write-Host "  * Prometheus (Metrics): http://localhost:9090"
Write-Host "  * Grafana (Dashboards): http://localhost:3000 (admin/admin)"
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Run your .NET applications"
Write-Host "  2. Make HTTP requests to generate traffic"
Write-Host "  3. Check Jaeger for distributed traces"
Write-Host "  4. Check Prometheus for metrics"
Write-Host "  5. Create Grafana dashboards"
Write-Host ""
Write-Host "Commands:" -ForegroundColor Cyan
Write-Host "  docker-compose logs -f jaeger      # Follow Jaeger logs"
Write-Host "  docker-compose logs -f prometheus  # Follow Prometheus logs"
Write-Host "  docker-compose logs -f grafana     # Follow Grafana logs"
Write-Host "  .\startup.ps1 -Stop                # Stop the stack"
Write-Host ""

if ($Logs) {
	Write-Host "Following logs (Ctrl+C to stop)..." -ForegroundColor Yellow
	docker-compose logs -f
}
