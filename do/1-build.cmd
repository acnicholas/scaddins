@echo off
REM ======================================================================
REM  1 - BUILD
REM
REM  Builds the add-in for Revit 2023 + 2024 + 2025 + 2026.
REM  Output: src\bin\Release<year>\
REM
REM  Click this first when you've changed code.
REM  Requires: .NET SDK 8 (the script tells you how to install it).
REM ======================================================================
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0..\scripts\build.ps1" %*
if errorlevel 1 (
    echo.
    echo Build failed.
    pause
    exit /b 1
)
echo.
echo Build OK.
pause
