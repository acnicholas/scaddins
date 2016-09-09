call Build.bat || goto :error
echo ### SCaddins build complete ###

cd src/SheetCopier/
call Build.bat || goto :error
echo ### SheetCopier build complete ###
call Bundle.bat || goto :error
echo ### SheetCopier bundle created ###

cd ../SolarUtils
call Build.bat || goto :error
echo ### AngleOfSun build complete ###
call Bundle.bat || goto :error
echo ### AngleOfSun bundle created ###

echo ####################################
echo BuildAll.bat completed without errors
echo ####################################
pause

goto:eof

:error
echo Failed with error #%errorlevel%.
pause
exit /b %errorlevel%