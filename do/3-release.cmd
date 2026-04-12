@echo off
REM ======================================================================
REM  3 - SHIP A NEW RELEASE  (interactive wizard)
REM
REM  Just double-click. The wizard:
REM    - Shows the current version
REM    - Prompts for the new version (validates X.Y.Z format)
REM    - Asks whether to push the tag (default: no)
REM    - Confirms before doing anything
REM    - Then runs the full release: build all 4 configs, package the
REM      installer, create a git tag, optionally push.
REM
REM  Pushing the tag triggers .github\workflows\release.yml which builds
REM  on a clean Windows runner and publishes a GitHub Release with the
REM  installer attached.
REM ======================================================================
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0..\scripts\release-interactive.ps1"
if errorlevel 1 (
    echo.
    echo Release failed or was cancelled.
    pause
    exit /b 1
)
echo.
pause
