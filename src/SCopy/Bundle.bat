set packageName=SheetCopier

rd .\%packageName%.bundle /S /Q

pause

mkdir %packageName%.bundle
mkdir %packageName%.bundle\Contents
mkdir %packageName%.bundle\Contents\2014
mkdir %packageName%.bundle\Contents\2015
mkdir %packageName%.bundle\Contents\2016
mkdir %packageName%.bundle\Contents\2017
mkdir %packageName%.bundle\Contents\Resources

copy %packageName%.addin %packageName%.bundle\Contents\2014\%packageName%.addin
copy %packageName%.addin %packageName%.bundle\Contents\2015\%packageName%.addin
copy %packageName%.addin %packageName%.bundle\Contents\2016\%packageName%.addin
copy %packageName%.addin %packageName%.bundle\Contents\2017\%packageName%.addin

copy bin\Release\%packageName%14.dll %packageName%.bundle\Contents\2014\%packageName%.dll
copy bin\Release\%packageName%15.dll %packageName%.bundle\Contents\2015\%packageName%.dll
copy bin\Release\%packageName%16.dll %packageName%.bundle\Contents\2016\%packageName%.dll
copy bin\Release\%packageName%17.dll %packageName%.bundle\Contents\2017\%packageName%.dll

copy share\* %packageName%.bundle\Contents\Resources

pause