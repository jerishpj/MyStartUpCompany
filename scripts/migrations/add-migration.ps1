param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

Write-Host "Creating migration: $MigrationName" -ForegroundColor Green
Set-Location "$PSScriptRoot\..\..\src\MyStartUpCompany.Persistence"
dotnet ef migrations add $MigrationName
Set-Location "$PSScriptRoot\..\.."