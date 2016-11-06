@echo off

rd .\bin\Release* /S /Q
rd .\var\log\* /S /Q
mkdir var\log

set SWITCHES=/consoleloggerparameters:Summary;NoItemAndPropertyList;Verbosity=normal /nologo
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319
set RevDir=C:\Program Files\Autodesk

if exist "%RevDir%\Revit 2014\RevitAPI.dll" call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2014" /property:Platform="x64" /target:Clean,Build SCaddins.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\MSBuild_Release2014.log || goto :error
if exist "%RevDir%\Revit 2015\RevitAPI.dll" call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2015" /property:Platform="x64" /target:Clean,Build SCaddins.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\MSBuild_Release2015.log || goto :error
if exist "%RevDir%\Revit 2016\RevitAPI.dll" call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2016" /property:Platform="x64" /target:Clean,Build SCaddins.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\MSBuild_Release2016.log || goto :error
if exist "%RevDir%\Revit 2017\RevitAPI.dll" call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2017" /property:Platform="x64" /target:Clean,Build SCaddins.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\MSBuild_Release2017.log || goto :error

set R2014=Disabled
set R2015=Disabled
set R2016=Disabled
set R2017=Disabled

if exist .\bin\Release\SCaddins14.dll set R2014=Enabled
if exist .\bin\Release\SCaddins15.dll set R2015=Enabled
if exist .\bin\Release\SCaddins16.dll set R2016=Enabled
if exist .\bin\Release\SCaddins17.dll set R2017=Enabled

powershell -Command "(gc SCaddins.addin) -replace '_REVIT_VERSION_', '2014' | Out-File bin\Release\SCaddins2014.addin" || goto :error
powershell -Command "(gc SCaddins.addin) -replace '_REVIT_VERSION_', '2015' | Out-File bin\Release\SCaddins2015.addin" || goto :error
powershell -Command "(gc SCaddins.addin) -replace '_REVIT_VERSION_', '2016' | Out-File bin\Release\SCaddins2016.addin" || goto :error
powershell -Command "(gc SCaddins.addin) -replace '_REVIT_VERSION_', '2017' | Out-File bin\Release\SCaddins2017.addin" || goto :error

call %msBuildDir%\msbuild.exe %SWITCHES% installer/SCaddins.Installer.wixproj /property:Configuration="Release" /property:Platform="x64" /target:Clean,Build /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\MSBuild_Installer.log || goto :error

set msBuildDir=

echo Build complete.
echo Revit versions suppoerted:
if %R2014% EQU Enabled echo --- R2014 build OK ---
if %R2015% EQU Enabled echo --- R2015 build OK ---
if %R2016% EQU Enabled echo --- R2016 build OK ---
if %R2017% EQU Enabled echo --- R2017 build OK ---

goto :EOF

:error
echo Failed with error #%errorlevel%.
pause
exit /b %errorlevel%
