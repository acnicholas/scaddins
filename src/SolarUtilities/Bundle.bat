set packageName=AngleOfSun

rd .\%packageName%.bundle /S /Q

mkdir %packageName%.bundle
mkdir %packageName%.bundle\Contents
mkdir %packageName%.bundle\Contents\2015
mkdir %packageName%.bundle\Contents\2016
mkdir %packageName%.bundle\Contents\2017
mkdir %packageName%.bundle\Contents\Resources

copy %packageName%.addin %packageName%.bundle\Contents\2015\%packageName%.addin || goto :error
copy %packageName%.addin %packageName%.bundle\Contents\2016\%packageName%.addin || goto :error
copy %packageName%.addin %packageName%.bundle\Contents\2017\%packageName%.addin || goto :error

if exist bin\Release\%packageName%15.dll copy bin\Release\%packageName%15.dll %packageName%.bundle\Contents\2015\%packageName%.dll || goto :error
if exist bin\Release\%packageName%16.dll copy bin\Release\%packageName%16.dll %packageName%.bundle\Contents\2016\%packageName%.dll || goto :error
if exist bin\Release\%packageName%17.dll copy bin\Release\%packageName%17.dll %packageName%.bundle\Contents\2017\%packageName%.dll || goto :error

rem copy share\* %packageName%.bundle\Contents\Resources

echo %packageName% bundle creation complete

goto :EOF

:error
echo %packageName% bundle creation failed with error #%errorlevel%.
exit /b %errorlevel%