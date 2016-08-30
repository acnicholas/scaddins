set SWITCHES=/consoleloggerparameters:Summary;NoItemAndPropertyList;Verbosity=normal /nologo
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

set R2014=Disabled
set R2015=Disabled
set R2016=Disabled
set R2017=Disabled

if exist .\bin\Release\AngleOfSun14.dll set R2014=Enabled
if exist .\bin\Release\AngleOfSun15.dll set R2015=Enabled
if exist .\bin\Release\AngleOfSun16.dll set R2016=Enabled
if exist .\bin\Release\AngleOfSun17.dll set R2017=Enabled

call %msBuildDir%\msbuild.exe %SWITCHES% installer/AngleOfSun.Installer.wixproj /property:Configuration="Release" /property:Platform="x64" /target:Clean,Build

set msBuildDir=

pause