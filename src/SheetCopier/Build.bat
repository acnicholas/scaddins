@echo off

rd .\bin\Release* /S /Q
rd .\var\log\* /S /Q
mkdir var\log

set SWITCHES=/consoleloggerparameters:Summary;NoItemAndPropertyList;Verbosity=normal /nologo
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319
set RevDir=C:\Program Files\Autodesk

if exist "%RevDir%\Revit 2015\RevitAPI.dll" call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2015" /property:Platform="x64" /target:Clean,Build SheetCopier.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=.\var\log\SCopy_MSBuild_Release2015.log || goto :error
if exist "%RevDir%\Revit 2016\RevitAPI.dll" call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2016" /property:Platform="x64" /target:Clean,Build SheetCopier.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=.\var\log\SCopy_MSBuild_Release2016.log || goto :error
if exist "%RevDir%\Revit 2017\RevitAPI.dll" call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2017" /property:Platform="x64" /target:Clean,Build SheetCopier.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=.\var\log\SCopy_MSBuild_Release2017.log || goto :error

set msBuildDir=

echo Build complete
if "%1%" NEQ "noPauseOnCompletion" pause

goto :EOF

:error
echo Failed with error #%errorlevel%.
pause
exit /b %errorlevel%
