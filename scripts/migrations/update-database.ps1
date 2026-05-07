Write-Host "Updating database..." -ForegroundColor Green
Set-Location "$PSScriptRoot\..\..\src\MyStartUpCompany.Persistence"
dotnet ef database update
Set-Location "$PSScriptRoot\..\.."
Write-Host "Database updated successfully!" -ForegroundColor Green