rd .\bin\Release* /S /Q
rd .\var\log\* /S /Q
mkdir var\log

set SWITCHES=/consoleloggerparameters:Summary;NoItemAndPropertyList;Verbosity=normal /nologo
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319
set RevDir=C:\Program Files\Autodesk

if exist "%RevDir%\Revit 2015\RevitAPI.dll" call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2015" /property:Platform="x64" /target:Clean,Build AngleOfSun.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\AngleOfSun_MSBuild_Release2015.log || goto :error
if exist "%RevDir%\Revit 2016\RevitAPI.dll" call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2016" /property:Platform="x64" /target:Clean,Build AngleOfSun.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\AngleOfSun_MSBuild_Release2016.log || goto :error
if exist "%RevDir%\Revit 2017\RevitAPI.dll" call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2017" /property:Platform="x64" /target:Clean,Build AngleOfSun.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\AngleOfSun_MSBuild_Release2017.log || goto :error

set R2015=Disabled
set R2016=Disabled
set R2017=Disabled

if exist .\bin\Release\AngleOfSun15.dll set R2015=Enabled
if exist .\bin\Release\AngleOfSun16.dll set R2016=Enabled
if exist .\bin\Release\AngleOfSun17.dll set R2017=Enabled

call %msBuildDir%\msbuild.exe %SWITCHES% installer/AngleOfSun.Installer.wixproj /property:Configuration="Release" /property:Platform="x64" /target:Clean,Build /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\AngleOfSun_SBuild_Installer.log || goto :error

set msBuildDir=

echo Build complete

goto :EOF

:error
echo Failed with error #%errorlevel%.
pause
exit /b %errorlevel%
