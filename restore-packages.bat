@echo off
echo ========================================
echo FashionStore - NuGet Package Restore
echo ========================================
echo.

REM Check if packages folder exists
if not exist "packages" (
    echo Creating packages folder...
    mkdir packages
)

REM Try to restore using nuget.exe
if exist "nuget.exe" (
    echo Using nuget.exe to restore packages...
    nuget.exe restore FashionStore.sln
    goto :end
)

REM Try to restore using MSBuild
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    echo Using MSBuild to restore packages...
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" /t:Restore FashionStore.sln
    goto :end
)

if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" (
    echo Using MSBuild to restore packages...
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" /t:Restore FashionStore.sln
    goto :end
)

echo.
echo ========================================
echo Please restore packages manually:
echo ========================================
echo 1. Open Visual Studio
echo 2. Right-click on Solution
echo 3. Select "Restore NuGet Packages"
echo.
echo Or use Package Manager Console:
echo Update-Package -reinstall
echo.

:end
pause

