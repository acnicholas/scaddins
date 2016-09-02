call ./Build.bat

cd src/SCopy/
call ./Build.bat
call ./Bundle.bat

cd ../SCaos
call ./Build.bat
call ./Bundle.bat

set msBuildDir=

pause