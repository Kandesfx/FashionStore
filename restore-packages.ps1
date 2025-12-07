# Script to restore NuGet packages for FashionStore project
Write-Host "Starting NuGet package restore..." -ForegroundColor Green

# Check if nuget.exe exists
$nugetPath = ".\nuget.exe"
if (-not (Test-Path $nugetPath)) {
    Write-Host "Downloading NuGet.exe..." -ForegroundColor Yellow
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nugetPath
}

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Green
& $nugetPath restore FashionStore.sln

if ($LASTEXITCODE -eq 0) {
    Write-Host "Package restore completed successfully!" -ForegroundColor Green
} else {
    Write-Host "Package restore failed. Please check the errors above." -ForegroundColor Red
    Write-Host "Trying alternative method..." -ForegroundColor Yellow
    
    # Try using dotnet restore if available
    if (Get-Command dotnet -ErrorAction SilentlyContinue) {
        Write-Host "Using dotnet restore..." -ForegroundColor Yellow
        dotnet restore FashionStore.sln
    } else {
        Write-Host "Please restore packages manually in Visual Studio:" -ForegroundColor Yellow
        Write-Host "1. Right-click on Solution -> Restore NuGet Packages" -ForegroundColor Yellow
        Write-Host "2. Or use Package Manager Console: Update-Package -reinstall" -ForegroundColor Yellow
    }
}

Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

