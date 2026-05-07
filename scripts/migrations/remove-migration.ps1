Write-Host "Removing last migration..." -ForegroundColor Yellow
Set-Location "$PSScriptRoot\..\..\src\MyStartUpCompany.Persistence"
dotnet ef migrations remove
Set-Location "$PSScriptRoot\..\.."