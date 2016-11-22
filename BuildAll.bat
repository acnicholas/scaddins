set SCB=SCaddins build status unknown
set SCI=SCaddins Installer build status unknown
set SCOB=SheetCopier build status unknown
set SCOI=SheetCopier Installer build status unknown
set SUB=SheetCopier build status unknown
set SUI=SheetCopier Installer build status unknown

call Build.bat noPauseOnCompletion || goto :error
set SCB=SCaddins build succesfull
cd installer
call Build.bat noPauseOnCompletion || goto :error
set SCI=SCaddins installer build succesfull

cd ../src/SheetCopier/
call Build.bat noPauseOnCompletion|| goto :error
set SCOB=SheetCopier build OK
cd installer
call Build.bat noPauseOnCompletion|| goto :error
set SCOI=SheetCopier Installer build OK
cd ..
call Bundle.bat || goto :error

cd ../SolarUtilities
call Build.bat noPauseOnCompletion || goto :error
set SUB=SolarUtilities build OK
cd installer
call Build.bat noPauseOnCompletion|| goto :error
set SUI=SolarUtilities Installer build OK
cd ..
call Bundle.bat || goto :error
echo :
echo :
echo :
echo + BuildAll.bat completed without errors
echo :
echo +---+    
echo     : Summary:
echo     : %SCB%
echo     : %SCI%
echo     : %SCOB%
echo     : %SCOI%
echo     : %SUB%
echo     : %SUI%
echo +---+
echo :
echo : 
echo : 
pause

goto:eof

:error
echo Failed with error #%errorlevel%.
pause
exit /b %errorlevel%