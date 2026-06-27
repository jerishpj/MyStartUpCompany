# Measure coverage for a single test project or all test projects
# Usage: ./measure-coverage.ps1 -Project "MyStartUpCompany.Api.Tests" -GenerateReport
# Usage: ./measure-coverage.ps1 -All -GenerateReport

param(
	[string]$Project,
	[switch]$All,
	[switch]$GenerateReport,
	[string]$OutputDir = "TestResults",
	[string]$ReportDir = "CoverageReport"
)

$ErrorActionPreference = "Stop"

# Determine which projects to test
$testProjects = @()
if ($All) {
	$testProjects = @(
		"tests/MyStartUpCompany.Api.Tests/MyStartUpCompany.Api.Tests.csproj",
		"tests/MyStartUpCompany.Worker.Tests/MyStartUpCompany.Worker.Tests.csproj",
		"tests/MyStartUpCompany.Observability.Tests/MyStartUpCompany.Observability.Tests.csproj"
	)
	Write-Host "📊 Measuring coverage for ALL test projects..." -ForegroundColor Cyan
} elseif ($Project) {
	$testProjects = @("tests/$Project/$Project.csproj")
	Write-Host "📊 Measuring coverage for: $Project" -ForegroundColor Cyan
} else {
	Write-Host "❌ Please specify -Project or -All" -ForegroundColor Red
	exit 1
}

# Create output directory
if (-not (Test-Path $OutputDir)) {
	New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

# Run tests with coverage
$successCount = 0
$failureCount = 0

foreach ($projectPath in $testProjects) {
	if (-not (Test-Path $projectPath)) {
		Write-Host "⚠️  Project not found: $projectPath" -ForegroundColor Yellow
		$failureCount++
		continue
	}

	$projectName = Split-Path -Leaf (Split-Path -Parent $projectPath)
	Write-Host "`n▶️  Testing: $projectName" -ForegroundColor White

	try {
		dotnet test $projectPath `
			--configuration Release `
			/p:CollectCoverage=true `
			/p:CoverletOutputFormat=cobertura `
			/p:CoverletOutput="./$OutputDir/$projectName.cobertura.xml" `
			/p:CoverletExclude="[*]*.Tests*" `
			2>&1 | Out-Host

		if ($LASTEXITCODE -eq 0) {
			Write-Host "✅ $projectName - Tests passed with coverage collected" -ForegroundColor Green
			$successCount++
		} else {
			Write-Host "❌ $projectName - Tests failed" -ForegroundColor Red
			$failureCount++
		}
	} catch {
		Write-Host "❌ Error testing $projectName : $_" -ForegroundColor Red
		$failureCount++
	}
}

Write-Host "`n" 
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "Test Summary: $successCount passed, $failureCount failed" -ForegroundColor Cyan
Write-Host "Coverage files saved to: $OutputDir" -ForegroundColor Cyan

# Generate report if requested
if ($GenerateReport -and $successCount -gt 0) {
	Write-Host "`n📈 Generating coverage report..." -ForegroundColor Yellow

	# Check if ReportGenerator is installed
	$reportGenExists = dotnet tool list --global | Select-String "ReportGenerator"
	if (-not $reportGenExists) {
		Write-Host "Installing ReportGenerator..." -ForegroundColor Yellow
		dotnet tool install --global ReportGenerator
	}

	# Get all coverage XML files
	$coverageFiles = Get-ChildItem -Path $OutputDir -Filter "*.cobertura.xml" -ErrorAction SilentlyContinue

	if ($coverageFiles) {
		$reportPath = $coverageFiles[0].FullName
		ReportGenerator `
			-reports:"$reportPath" `
			-targetdir:"$ReportDir" `
			-reporttypes:"Html;HtmlSummary;MarkdownSummary"

		Write-Host "✅ Coverage report generated: $ReportDir/index.html" -ForegroundColor Green

		# Open report in browser (Windows only)
		if ($PSVersionTable.Platform -eq $null -or $PSVersionTable.Platform -eq "Win32NT") {
			Start-Process "$ReportDir/index.html"
		}
	}
}

if ($failureCount -gt 0) {
	exit 1
}
