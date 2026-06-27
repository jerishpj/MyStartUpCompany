# Validate code coverage against defined thresholds
# Usage: ./validate-coverage.ps1 -ReportFile "./TestResults/coverage.cobertura.xml"

param(
	[string]$ReportFile = "./TestResults/coverage.cobertura.xml",
	[double]$MinimumThreshold = 0.80,
	[switch]$Strict
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $ReportFile)) {
	Write-Host "❌ Coverage report not found: $ReportFile" -ForegroundColor Red
	Write-Host "Run measurement first: ./measure-coverage.ps1 -All" -ForegroundColor Yellow
	exit 1
}

Write-Host "📊 Validating code coverage..." -ForegroundColor Cyan
Write-Host "Report: $ReportFile" -ForegroundColor Gray

# Parse coverage XML
try {
	[xml]$coverage = Get-Content $ReportFile
} catch {
	Write-Host "❌ Error parsing coverage report: $_" -ForegroundColor Red
	exit 1
}

# Extract coverage metrics
$linesCovered = [int]$coverage.coverage.'lines-valid'
$linesTotal = [int]$coverage.coverage.'lines-total'
$lineCoverageRate = [double]$coverage.coverage.'line-rate'
$lineCoveragePercent = [math]::Round($lineCoverageRate * 100, 2)

# Extract by branch if available
$branchesValid = $coverage.coverage.'branches-valid'
$branchesTotal = $coverage.coverage.'branches-total'

Write-Host "`n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
Write-Host "Coverage Metrics:" -ForegroundColor White
Write-Host "  Lines: $linesCovered / $linesTotal ($lineCoveragePercent%)" -ForegroundColor White
Write-Host "  Target: $([math]::Round($MinimumThreshold * 100, 2))%" -ForegroundColor White
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

# Validate
$passed = $true
if ($lineCoverageRate -lt $MinimumThreshold) {
	Write-Host "`n❌ FAILED: Coverage $lineCoveragePercent% is below threshold $([math]::Round($MinimumThreshold * 100, 2))%" -ForegroundColor Red
	Write-Host "   Add $([math]::Ceiling(($MinimumThreshold - $lineCoverageRate) * $linesTotal)) more lines of coverage" -ForegroundColor Yellow
	$passed = $false
} else {
	$difference = [math]::Round(($lineCoverageRate - $MinimumThreshold) * 100, 2)
	Write-Host "`n✅ PASSED: Coverage $lineCoveragePercent% exceeds threshold (${difference}% above)" -ForegroundColor Green
}

# Analyze by namespace if detailed
if ($coverage.coverage.package) {
	Write-Host "`n📦 Coverage by Package:" -ForegroundColor Cyan
	$coverage.coverage.package | ForEach-Object {
		$name = $_.name
		$rate = [math]::Round([double]$_.rate * 100, 2)
		if ($rate -lt 70) {
			$icon = "🔴"
		} elseif ($rate -lt 85) {
			$icon = "🟡"
		} else {
			$icon = "🟢"
		}
		Write-Host "  $icon $name : $rate%"
	}
}

# Generate recommendations
if (-not $passed) {
	Write-Host "`n💡 Recommendations:" -ForegroundColor Yellow
	Write-Host "  1. Run: dotnet test /p:CollectCoverage=true" -ForegroundColor Gray
	Write-Host "  2. Open: ./CoverageReport/index.html (after ReportGenerator)" -ForegroundColor Gray
	Write-Host "  3. Focus on: Lines marked as red (uncovered)" -ForegroundColor Gray
	Write-Host "  4. Add tests for: Critical business logic, edge cases" -ForegroundColor Gray
}

Write-Host "`n"

if ($passed) {
	exit 0
} else {
	exit 1
}
