<#
.SYNOPSIS
	Run database migrations for MyStartUpCompany

.DESCRIPTION
	This script runs database migrations using the MigrationRunner console application.
	It's useful for local development and CI/CD pipelines.

.PARAMETER Environment
	Target environment: Development, Staging, or Production (default: Development)

.PARAMETER ConnectionString
	Override database connection string. If not provided, uses appsettings values.

.PARAMETER ListOnly
	If true, lists pending migrations without applying them

.PARAMETER DotNetProjectPath
	Path to the MigrationRunner project (default: ./src/MigrationRunner)

.EXAMPLE
	# Run migrations in Development
	.\scripts\migrate.ps1

.EXAMPLE
	# Run migrations in Production
	.\scripts\migrate.ps1 -Environment Production

.EXAMPLE
	# Override connection string
	.\scripts\migrate.ps1 -ConnectionString "Server=myserver;Database=mydb;User Id=sa;Password=pass;"

.EXAMPLE
	# List pending migrations only
	.\scripts\migrate.ps1 -ListOnly

.NOTES
	Author: MyStartUpCompany Team
	Prerequisites: .NET 10 SDK, PowerShell 5.0+
#>

param(
	[ValidateSet('Development', 'Staging', 'Production')]
	[string]$Environment = 'Development',

	[string]$ConnectionString = '',

	[switch]$ListOnly = $false,

	[string]$DotNetProjectPath = './src/MigrationRunner'
)

# Get script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir

# Resolve paths
$projectPath = Join-Path $repoRoot $DotNetProjectPath
$appsettingsPath = Join-Path $projectPath '../MyStartUpCompany.Persistence/appsettings.json'

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "MyStartUpCompany Database Migration Tool" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Validate paths
if (-not (Test-Path $projectPath)) {
	Write-Host "ERROR: MigrationRunner project not found at: $projectPath" -ForegroundColor Red
	exit 1
}

Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host "MigrationRunner: $projectPath" -ForegroundColor Yellow

# Build arguments
$arguments = @(
	'run',
	'--project', $projectPath,
	'--'
)

# Add environment argument
$arguments += '--environment', $Environment

# Add connection string if provided
if ($ConnectionString) {
	Write-Host "Connection String: (provided via parameter)" -ForegroundColor Yellow
	$arguments += '--connection-string', $ConnectionString
}

# Add list-only flag if requested
if ($ListOnly) {
	Write-Host "Mode: List pending migrations only" -ForegroundColor Yellow
	$arguments += '--list-migrations'
}

Write-Host ""

# Run migration
Write-Host "Running migrations..." -ForegroundColor Cyan
Write-Host ""

$env:ASPNETCORE_ENVIRONMENT = $Environment
& dotnet $arguments

$exitCode = $LASTEXITCODE

Write-Host ""
if ($exitCode -eq 0) {
	Write-Host "========================================" -ForegroundColor Green
	Write-Host "Migration completed successfully!" -ForegroundColor Green
	Write-Host "========================================" -ForegroundColor Green
	exit 0
}
else {
	Write-Host "========================================" -ForegroundColor Red
	Write-Host "Migration failed with exit code: $exitCode" -ForegroundColor Red
	Write-Host "========================================" -ForegroundColor Red
	exit $exitCode
}
