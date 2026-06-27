# Pre-commit coverage check
# Add this to your git pre-commit hook to automatically check coverage

$ErrorActionPreference = "Stop"

Write-Host "🔍 Running pre-commit coverage check..." -ForegroundColor Cyan

# Run tests with coverage
Write-Host "`n📊 Running tests with coverage collection..." -ForegroundColor Yellow

$result = dotnet test --configuration Release `
	/p:CollectCoverage=true `
	/p:CoverletOutputFormat=cobertura `
	/p:CoverletOutput="./TestResults/coverage.cobertura.xml" 2>&1

if ($LASTEXITCODE -ne 0) {
	Write-Host "`n❌ Tests failed. Fix failing tests before committing." -ForegroundColor Red
	exit 1
}

# Validate coverage
Write-Host "`n✅ Tests passed. Validating coverage..." -ForegroundColor Green

$passed = & "./scripts/validate-coverage.ps1" -ReportFile "./TestResults/coverage.cobertura.xml"

if ($LASTEXITCODE -ne 0) {
	Write-Host "`n⚠️  Coverage is below threshold." -ForegroundColor Yellow
	Write-Host "   Run: ./scripts/measure-coverage.ps1 -All -GenerateReport" -ForegroundColor Yellow
	Write-Host "   Then add more tests to improve coverage." -ForegroundColor Yellow

	# Optionally allow commit anyway with warning
	$response = Read-Host "Continue with commit despite low coverage? (y/n)"
	if ($response -ne "y") {
		exit 1
	}
}

Write-Host "`n✅ Coverage check passed! Safe to commit." -ForegroundColor Green
exit 0
