rd .\bin\x64\Release* /S /Q
rd .\var\log\* /S /Q
mkdir var\log

set SWITCHES=/consoleloggerparameters:Summary;NoItemAndPropertyList;Verbosity=normal /nologo
set msBuildDir=%WINDIR%\Microsoft.NET\Framework\v4.0.30319

if  not "%1"=="-noBuildBump" (
    call .\bin\UpdateVersion.exe -r Increment -i .\src\Properties\AssemblyInfo.cs -o .\src\Properties\AssemblyInfo.cs
)

call %msBuildDir%\msbuild.exe %SWITCHES% /property:Configuration="Release" /property:Platform="x64" /target:Clean,Build SCaddins.csproj /l:FileLogger,Microsoft.Build.Engine;logfile=var\log\MSBuild_Release2015.log

set R2014=Disabled
set R2015=Disabled

if exist .\bin\x64\Release\SCaddins.dll set R2014=Enabled
if exist .\bin\x64\Release\SCaddins.dll set R2015=Enabled

rem call %msBuildDir%\msbuild.exe %SWITCHES% installer/SCaddins.Installer.wixproj /property:Configuration="Release" /property:Platform="x64"

set msBuildDir=

pause