rd .\bin\Release* /S /Q
rd .\var\log\* /S /Q
mkdir var\log

set SWITCHES=/consoleloggerparameters:Summary;NoItemAndPropertyList;Verbosity=normal /nologo
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2014" /property:Platform="x64" /target:Clean,Build SCaddins.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\MSBuild_Release2014.log
call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2015" /property:Platform="x64" /target:Clean,Build SCaddins.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\MSBuild_Release2015.log
call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release2016" /property:Platform="x64" /target:Clean,Build SCaddins.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\MSBuild_Release2016.log

set R2014=Disabled
set R2015=Disabled
set R2016=Disabled

if exist .\bin\Release\SCaddins14.dll set R2014=Enabled
if exist .\bin\Release\SCaddins15.dll set R2015=Enabled
if exist .\bin\Release\SCaddins16.dll set R2016=Enabled

call %msBuildDir%\msbuild.exe %SWITCHES% installer/SCaddins.Installer.wixproj /property:Configuration="Release" /property:Platform="x64" /target:Clean,Build

set msBuildDir=

pause