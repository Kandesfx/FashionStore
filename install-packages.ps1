# Script to install all required NuGet packages for FashionStore project
# Compatible with ASP.NET MVC 5 + Web API + Unity template

Write-Host "========================================" -ForegroundColor Green
Write-Host "Installing NuGet Packages for FashionStore" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Core ASP.NET MVC packages
Write-Host "Installing ASP.NET MVC packages..." -ForegroundColor Yellow
Install-Package Microsoft.AspNet.Mvc -Version 5.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.Razor -Version 3.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.WebPages -Version 3.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.Web.Optimization -Version 1.1.3 -ProjectName FashionStore
Install-Package Microsoft.Web.Infrastructure -Version 2.0.0 -ProjectName FashionStore
Install-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -Version 2.0.1 -ProjectName FashionStore

# Web API packages
Write-Host "Installing Web API packages..." -ForegroundColor Yellow
Install-Package Microsoft.AspNet.WebApi -Version 5.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.WebApi.WebHost -Version 5.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.WebApi.Core -Version 5.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.WebApi.Client -Version 5.2.9 -ProjectName FashionStore
Install-Package System.Net.Http -Version 4.3.4 -ProjectName FashionStore

# Unity packages (Dependency Injection)
Write-Host "Installing Unity packages..." -ForegroundColor Yellow
Install-Package Unity.Mvc -Version 5.11.1 -ProjectName FashionStore
Install-Package Unity.Container -Version 5.11.11 -ProjectName FashionStore
Install-Package Unity.Abstractions -Version 5.11.7 -ProjectName FashionStore
Install-Package WebActivatorEx -Version 2.2.0 -ProjectName FashionStore

# Entity Framework
Write-Host "Installing Entity Framework..." -ForegroundColor Yellow
Install-Package EntityFramework -Version 6.4.4 -ProjectName FashionStore

# Additional packages
Write-Host "Installing additional packages..." -ForegroundColor Yellow
Install-Package BCrypt.Net-Next -Version 4.0.3 -ProjectName FashionStore
Install-Package PagedList.Mvc -Version 4.5.0 -ProjectName FashionStore
Install-Package NLog -Version 4.7.15 -ProjectName FashionStore
Install-Package NLog.Web -Version 4.9.3 -ProjectName FashionStore
Install-Package Newtonsoft.Json -Version 6.0.4 -ProjectName FashionStore

# Dependencies
Install-Package System.Runtime.CompilerServices.Unsafe -Version 4.5.3 -ProjectName FashionStore
Install-Package System.Threading.Tasks.Extensions -Version 4.5.2 -ProjectName FashionStore

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Package installation completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Unload and Reload project in Visual Studio" -ForegroundColor Yellow
Write-Host "2. Clean Solution" -ForegroundColor Yellow
Write-Host "3. Rebuild Solution" -ForegroundColor Yellow

