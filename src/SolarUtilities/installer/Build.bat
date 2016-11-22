@echo off

rd .\bin\Release* /S /Q
rd .\var\log\* /S /Q
mkdir var\log

set SWITCHES=/consoleloggerparameters:Summary;NoItemAndPropertyList;Verbosity=normal /nologo
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

set R2015=Disabled
set R2016=Disabled
set R2017=Disabled

if exist ..\bin\Release\AngleOfSun15.dll set R2015=Enabled
if exist ..\bin\Release\AngleOfSun16.dll set R2016=Enabled
if exist ..\bin\Release\AngleOfSun17.dll set R2017=Enabled

call %msBuildDir%\msbuild.exe %SWITCHES% AngleOfSun.Installer.wixproj /property:Configuration="Release" /property:Platform="x64" /target:Clean,Build /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\AngleOfSun_SBuild_Installer.log || goto :error

set msBuildDir=

echo Build complete
if "%1%" NEQ "noPauseOnCompletion" pause

goto :EOF

:error
echo Failed with error #%errorlevel%.
pause
exit /b %errorlevel%
