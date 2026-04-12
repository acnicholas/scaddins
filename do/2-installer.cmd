@echo off
REM ======================================================================
REM  2 - PACKAGE INSTALLER
REM
REM  Wraps Inno Setup. Reads the version from the built DLL automatically.
REM  Output: setup\out\nullCarbon-LCA-Export-win64-<version>.exe
REM
REM  Click 1-build.cmd FIRST, then click this.
REM  Requires: Inno Setup 6 (winget install JRSoftware.InnoSetup).
REM ======================================================================
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0..\scripts\build-installer.ps1" %*
if errorlevel 1 (
    echo.
    echo Installer packaging failed.
    pause
    exit /b 1
)
echo.
echo Installer ready in setup\out\
pause
