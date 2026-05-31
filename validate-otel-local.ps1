# Local OTEL Validation Script
# Validates that the local OTEL observability stack is working correctly
# Run this after docker-compose is up and applications are running

param(
	[switch]$Verbose,
	[int]$TimeoutSeconds = 30
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Local OTEL Stack Validation" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$allPassed = $true

# Function to test HTTP endpoint
function Test-Endpoint {
	param(
		[string]$Url,
		[string]$ServiceName,
		[string]$SuccessIndicator
	)

	Write-Host "Testing $ServiceName at $Url..." -ForegroundColor Gray

	try {
		$response = Invoke-WebRequest -Uri $Url -TimeoutSec 5 -ErrorAction Stop

		if ($response.StatusCode -eq 200) {
			if ($SuccessIndicator -and $response.Content -notlike "*$SuccessIndicator*") {
				Write-Host "  ❌ Connected but unexpected content" -ForegroundColor Red
				return $false
			}
			Write-Host "  ✅ Accessible and responding" -ForegroundColor Green
			return $true
		}
		else {
			Write-Host "  ⚠️  Status: $($response.StatusCode)" -ForegroundColor Yellow
			return $false
		}
	}
	catch {
		if ($Verbose) {
			Write-Host "  ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
		}
		else {
			Write-Host "  ❌ Not accessible" -ForegroundColor Red
		}
		return $false
	}
}

# Function to test Docker container status
function Test-DockerContainer {
	param(
		[string]$ContainerName
	)

	Write-Host "Checking Docker container: $ContainerName..." -ForegroundColor Gray

	try {
		$status = docker-compose ps $ContainerName 2>&1 | Select-String -Pattern "(Up|Exited)" | Out-String

		if ($status -match "Up") {
			Write-Host "  ✅ Container is running" -ForegroundColor Green
			return $true
		}
		elseif ($status -match "Exited") {
			Write-Host "  ❌ Container has exited" -ForegroundColor Red
			return $false
		}
		else {
			Write-Host "  ⚠️  Status unknown" -ForegroundColor Yellow
			return $false
		}
	}
	catch {
		Write-Host "  ❌ Cannot determine status: $($_.Exception.Message)" -ForegroundColor Red
		return $false
	}
}

Write-Host "📊 STEP 1: Validate Docker Compose Services" -ForegroundColor Cyan
Write-Host ""

$dockerChecks = @(
	@{ Name = "Jaeger"; Container = "jaeger" },
	@{ Name = "Prometheus"; Container = "prometheus" },
	@{ Name = "Grafana"; Container = "grafana" }
)

foreach ($check in $dockerChecks) {
	if (-not (Test-DockerContainer -ContainerName $check.Container)) {
		$allPassed = $false
	}
}

Write-Host ""
Write-Host "🌐 STEP 2: Validate Service Endpoints" -ForegroundColor Cyan
Write-Host ""

$endpointChecks = @(
	@{ Url = "http://localhost:16686"; Name = "Jaeger UI"; Indicator = "traces" },
	@{ Url = "http://localhost:9090"; Name = "Prometheus"; Indicator = "prometheus" },
	@{ Url = "http://localhost:3000"; Name = "Grafana"; Indicator = "grafana" }
)

foreach ($check in $endpointChecks) {
	if (-not (Test-Endpoint -Url $check.Url -ServiceName $check.Name -SuccessIndicator $check.Indicator)) {
		$allPassed = $false
	}
}

Write-Host ""
Write-Host "📈 STEP 3: Validate Application Metrics Endpoints" -ForegroundColor Cyan
Write-Host ""

$appMetricsChecks = @(
	@{ Url = "http://localhost:9091/metrics"; Name = "API Metrics"; Port = 9091 },
	@{ Url = "http://localhost:9092/metrics"; Name = "Worker Metrics"; Port = 9092 },
	@{ Url = "http://localhost:9093/metrics"; Name = "Notifier Metrics"; Port = 9093 }
)

foreach ($check in $appMetricsChecks) {
	Write-Host "Checking $($check.Name) at http://localhost:$($check.Port)/metrics..." -ForegroundColor Gray

	try {
		$response = Invoke-WebRequest -Uri $check.Url -TimeoutSec 5 -ErrorAction Stop

		if ($response.StatusCode -eq 200 -and $response.Content -match "^# HELP") {
			Write-Host "  ✅ Metrics endpoint is active (found Prometheus format)" -ForegroundColor Green
		}
		elseif ($response.StatusCode -eq 200) {
			Write-Host "  ✅ Metrics endpoint responds" -ForegroundColor Green
		}
		else {
			Write-Host "  ⚠️  Status: $($response.StatusCode)" -ForegroundColor Yellow
			$allPassed = $false
		}
	}
	catch {
		Write-Host "  ℹ️  Endpoint not yet available (app may not be running)" -ForegroundColor Yellow
		# Not marking as failure since apps might not be running yet
	}
}

Write-Host ""
Write-Host "🔗 STEP 4: Validate Prometheus Scrape Targets" -ForegroundColor Cyan
Write-Host ""

Write-Host "Checking Prometheus targets..." -ForegroundColor Gray

try {
	$targetsResponse = Invoke-WebRequest -Uri "http://localhost:9090/api/v1/targets" -TimeoutSec 5
	$targetsContent = $targetsResponse.Content | ConvertFrom-Json

	if ($targetsContent.data.activeTargets.Count -gt 0) {
		Write-Host "  ✅ Found $($targetsContent.data.activeTargets.Count) active targets" -ForegroundColor Green

		foreach ($target in $targetsContent.data.activeTargets) {
			Write-Host "    • $($target.labels.job) ($($target.scrapeUrl))" -ForegroundColor Gray
		}
	}
	else {
		Write-Host "  ⚠️  No active scrape targets found yet" -ForegroundColor Yellow
	}
}
catch {
	Write-Host "  ℹ️  Cannot query Prometheus targets yet" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "📊 STEP 5: Network Connectivity Test" -ForegroundColor Cyan
Write-Host ""

Write-Host "Testing Docker network connectivity..." -ForegroundColor Gray

try {
	# Test if docker network exists
	$networks = docker network ls | Select-String "observability"

	if ($networks) {
		Write-Host "  ✅ Observability network exists" -ForegroundColor Green
	}
	else {
		Write-Host "  ⚠️  Observability network not found" -ForegroundColor Yellow
	}
}
catch {
	Write-Host "  ⚠️  Cannot verify network" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

if ($allPassed) {
	Write-Host "✅ All validation checks passed!" -ForegroundColor Green
	Write-Host ""
	Write-Host "Next Steps:" -ForegroundColor Cyan
	Write-Host "  1. Start your .NET applications (API, Worker, Notifier)"
	Write-Host "  2. Make HTTP requests to generate traffic"
	Write-Host "  3. Check Jaeger at http://localhost:16686 for traces"
	Write-Host "  4. Check Prometheus at http://localhost:9090 for metrics"
	Write-Host "  5. Create dashboards in Grafana at http://localhost:3000"
	Write-Host ""
}
else {
	Write-Host "⚠️  Some validation checks did not pass" -ForegroundColor Yellow
	Write-Host ""
	Write-Host "Troubleshooting:" -ForegroundColor Cyan
	Write-Host "  • Ensure Docker Desktop is running"
	Write-Host "  • Ensure docker-compose is up: cd docker-compose && .\startup.ps1"
	Write-Host "  • Check logs: docker-compose logs <service_name>"
	Write-Host "  • See LOCAL_OTEL_TESTING_GUIDE.md for detailed troubleshooting"
	Write-Host ""
}

Write-Host "📚 Documentation:" -ForegroundColor Cyan
Write-Host "  • docs/LOCAL_OTEL_TESTING_GUIDE.md - Complete testing guide"
Write-Host "  • docs/OPENTELEMETRY_ARCHITECTURE.md - Architecture overview"
Write-Host "  • docker-compose/README.md - Docker Compose documentation"
Write-Host ""
