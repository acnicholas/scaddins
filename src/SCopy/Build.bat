set SWITCHES=/consoleloggerparameters:Summary;NoItemAndPropertyList;Verbosity=normal /nologo
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

set R2014=Disabled
set R2015=Disabled
set R2016=Disabled
set R2017=Disabled

if exist .\bin\Release\SheetCopier14.dll set R2014=Enabled
if exist .\bin\Release\SheetCopier15.dll set R2015=Enabled
if exist .\bin\Release\SheetCopier16.dll set R2016=Enabled
if exist .\bin\Release\SheetCopier17.dll set R2017=Enabled

call %msBuildDir%\msbuild.exe %SWITCHES% installer/SheetCopier.Installer.wixproj /property:Configuration="Release" /property:Platform="x64" /target:Clean,Build

set msBuildDir=

pause