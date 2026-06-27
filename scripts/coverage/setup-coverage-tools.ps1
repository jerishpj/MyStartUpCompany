# Setup code coverage tools and configuration
# Run this once to prepare your environment

$ErrorActionPreference = "Stop"

Write-Host "🔧 Setting up Code Coverage Tools..." -ForegroundColor Cyan

# Check if global tools are installed
$toolsToInstall = @()

$reportGenExists = dotnet tool list --global 2>/dev/null | Select-String "ReportGenerator"
if (-not $reportGenExists) {
	$toolsToInstall += "ReportGenerator"
}

$openCoverExists = dotnet tool list --global 2>/dev/null | Select-String "OpenCover"
if ($openCoverExists) {
	Write-Host "✅ OpenCover already installed" -ForegroundColor Green
}

# Install missing tools
if ($toolsToInstall.Count -gt 0) {
	Write-Host "`n📦 Installing global tools..." -ForegroundColor Yellow
	foreach ($tool in $toolsToInstall) {
		Write-Host "  Installing $tool..." -ForegroundColor Gray
		dotnet tool install --global $tool
		if ($LASTEXITCODE -eq 0) {
			Write-Host "  ✅ $tool installed" -ForegroundColor Green
		}
	}
}

# Create coverlet configuration if it doesn't exist
$runsettingsPath = "./coverlet.runsettings"
if (-not (Test-Path $runsettingsPath)) {
	Write-Host "`n📝 Creating coverlet.runsettings..." -ForegroundColor Yellow

	$runsettingsContent = @'
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
	<TargetPlatform>x64</TargetPlatform>
	<FrameworkVersion>.NET 10</FrameworkVersion>
  </RunConfiguration>
  <DataCollectionRunSettings>
	<DataCollectors>
	  <DataCollector friendlyName="XPlat code coverage">
		<Configuration>
		  <Format>cobertura</Format>
		  <UseSourceLink>true</UseSourceLink>
		  <Exclude>[*]*.Tests*,[*]*.Test.*</Exclude>
		</Configuration>
	  </DataCollector>
	</DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
'@
	$runsettingsContent | Out-File -Encoding UTF8 $runsettingsPath
	Write-Host "✅ Created: $runsettingsPath" -ForegroundColor Green
}

# Create coverage targets if it doesn't exist
$targetsPath = "./coverage-targets.json"
if (-not (Test-Path $targetsPath)) {
	Write-Host "`n📝 Creating coverage-targets.json..." -ForegroundColor Yellow

	$targetsContent = @{
		coverageTargets = @{
			overall = 80
			projectSpecific = @{
				"MyStartUpCompany.Api" = 85
				"MyStartUpCompany.Worker" = 80
				"MyStartUpCompany.Observability" = 75
				"MyStartUpCompany.Persistence" = 70
			}
			classMinimum = 70
		}
	} | ConvertTo-Json

	$targetsContent | Out-File $targetsPath
	Write-Host "✅ Created: $targetsPath" -ForegroundColor Green
}

# Create directories if needed
@("TestResults", "CoverageReport", "scripts") | ForEach-Object {
	if (-not (Test-Path $_)) {
		New-Item -ItemType Directory -Path $_ -Force | Out-Null
		Write-Host "✅ Created directory: $_" -ForegroundColor Green
	}
}

Write-Host "`n✅ Setup complete!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "  1. Measure coverage: ./scripts/measure-coverage.ps1 -All -GenerateReport"
Write-Host "  2. Validate coverage: ./scripts/validate-coverage.ps1"
Write-Host "  3. Read guide:       Get-Content docs/CODE_COVERAGE_VERIFICATION_GUIDE.md"
Write-Host "`n"
