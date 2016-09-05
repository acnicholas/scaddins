call ./Build.bat || goto :error

cd src/SCopy/
call ./Build.bat || goto :error
call ./Bundle.bat || goto :error

cd ../SCaos
call ./Build.bat || goto :error
call ./Bundle.bat || goto :error

echo ####################################
echo BuildAll.bat completed without errors
echo ####################################
pause

goto:eof

:error
echo Failed with error #%errorlevel%.
pause
exit /b %errorlevel%