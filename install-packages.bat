@echo off
echo ========================================
echo Installing NuGet Packages for FashionStore
echo ========================================
echo.
echo This script will install all required packages.
echo Please run this in Package Manager Console:
echo.
echo Copy and paste the commands below:
echo.

(
echo Install-Package Microsoft.AspNet.Mvc -Version 5.2.9 -ProjectName FashionStore
echo Install-Package Microsoft.AspNet.Razor -Version 3.2.9 -ProjectName FashionStore
echo Install-Package Microsoft.AspNet.WebPages -Version 3.2.9 -ProjectName FashionStore
echo Install-Package Microsoft.AspNet.Web.Optimization -Version 1.1.3 -ProjectName FashionStore
echo Install-Package Microsoft.Web.Infrastructure -Version 2.0.0 -ProjectName FashionStore
echo Install-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -Version 2.0.1 -ProjectName FashionStore
echo Install-Package Microsoft.AspNet.WebApi -Version 5.2.9 -ProjectName FashionStore
echo Install-Package Microsoft.AspNet.WebApi.WebHost -Version 5.2.9 -ProjectName FashionStore
echo Install-Package Microsoft.AspNet.WebApi.Core -Version 5.2.9 -ProjectName FashionStore
echo Install-Package Microsoft.AspNet.WebApi.Client -Version 5.2.9 -ProjectName FashionStore
echo Install-Package System.Net.Http -Version 4.3.4 -ProjectName FashionStore
echo Install-Package Unity.Mvc -Version 5.11.1 -ProjectName FashionStore
echo Install-Package Unity.Container -Version 5.11.11 -ProjectName FashionStore
echo Install-Package Unity.Abstractions -Version 5.11.7 -ProjectName FashionStore
echo Install-Package WebActivatorEx -Version 2.2.0 -ProjectName FashionStore
echo Install-Package EntityFramework -Version 6.4.4 -ProjectName FashionStore
echo Install-Package BCrypt.Net-Next -Version 4.0.3 -ProjectName FashionStore
echo Install-Package PagedList.Mvc -Version 4.5.0 -ProjectName FashionStore
echo Install-Package NLog -Version 4.7.15 -ProjectName FashionStore
echo Install-Package NLog.Web -Version 4.9.3 -ProjectName FashionStore
echo Install-Package Newtonsoft.Json -Version 6.0.4 -ProjectName FashionStore
echo Install-Package System.Runtime.CompilerServices.Unsafe -Version 4.5.3 -ProjectName FashionStore
echo Install-Package System.Threading.Tasks.Extensions -Version 4.5.2 -ProjectName FashionStore
) > install-commands.txt

echo Commands saved to install-commands.txt
echo.
echo Or use PowerShell script: install-packages.ps1
echo.
pause

