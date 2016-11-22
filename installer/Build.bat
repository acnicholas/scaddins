@echo off

rd var\log\* /S /Q
mkdir var\log

set SWITCHES=/consoleloggerparameters:Summary;NoItemAndPropertyList;Verbosity=normal /nologo
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

set R2015=Disabled
set R2016=Disabled
set R2017=Disabled

if exist ..\bin\Release\SCaddins15.dll set R2015=Enabled
if exist ..\bin\Release\SCaddins16.dll set R2016=Enabled
if exist ..\bin\Release\SCaddins17.dll set R2017=Enabled

call %msBuildDir%\msbuild.exe %SWITCHES% SCaddins.Installer.wixproj /property:Configuration="Release" /property:Platform="x64" /target:Clean,Build /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\MSBuild_Installer.log || goto :error

set msBuildDir=

echo Build complete.
echo --- Installer build OK ---
if "%1%" NEQ "noPauseOnCompletion" pause

goto :EOF

:error
echo Failed with error #%errorlevel%.
pause
exit /b %errorlevel%
