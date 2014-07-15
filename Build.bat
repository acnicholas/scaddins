rd .\BuildResults /S /Q
rd .\bin\x64\Release* /S /Q
md .\BuildResults

set SWITCHES=/consoleloggerparameters:Summary;NoItemAndPropertyList;Verbosity=normal /nologo
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

if  not "%1"=="-noBuildBump" (
    call .\bin\UpdateVersion.exe -r Increment -i .\src\Properties\AssemblyInfo.cs -o .\src\Properties\AssemblyInfo.cs
)

call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release 2012" /property:Platform="x64"  /target:Clean,Build SCexport.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=MSBuild_Release2012.log
call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release 2013" /property:Platform="x64" /target:Clean,Build SCexport.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=MSBuild_Release2013.log
call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release 2014" /property:Platform="x64" /target:Clean,Build SCexport.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=MSBuild_Release2014.log
call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release 2015" /property:Platform="x64" /target:Clean,Build SCexport.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=MSBuild_Release2015.log

XCOPY ".\bin\x64\Release 2012" .\BuildResults\ 
XCOPY ".\bin\x64\Release 2013" .\BuildResults\ 
XCOPY ".\bin\x64\Release 2014" .\BuildResults\ 
XCOPY ".\bin\x64\Release 2015" .\BuildResults\ 

set R2012=Disabled
set R2013=Disabled
set R2014=Disabled
set R2015=Disabled

if exist .\BuildResults\SCexport12.dll set R2012=Enabled
if exist .\BuildResults\SCexport13.dll set R2013=Enabled
if exist .\BuildResults\SCexport14.dll set R2014=Enabled
if exist .\BuildResults\SCexport15.dll set R2015=Enabled

call %msBuildDir%\msbuild.exe %SWITCHES% installer/SCexport.Setup.wixproj /property:Configuration="Release" /property:Platform="x64"

set msBuildDir=
