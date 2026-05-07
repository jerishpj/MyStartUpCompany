Write-Host "Listing all migrations..." -ForegroundColor Cyan
Set-Location "$PSScriptRoot\..\..\src\MyStartUpCompany.Persistence"
dotnet ef migrations list
Set-Location "$PSScriptRoot\..\.."