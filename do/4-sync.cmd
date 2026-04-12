@echo off
REM ======================================================================
REM  4 - SYNC FROM UPSTREAM  (interactive, dry-run by default)
REM
REM  Pulls new commits from acnicholas/scaddins. Two modes:
REM
REM    [1] DRY RUN (default) - preview what would happen, list any
REM                            conflicts, abort. Nothing changes in
REM                            your repo. Safe.
REM
REM    [2] FULL SYNC         - create a sync branch, merge upstream,
REM                            run the build to verify. Asks for an
REM                            extra confirmation before doing it.
REM
REM  Just double-click and answer the prompt. Default is dry run.
REM ======================================================================
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0..\scripts\sync-interactive.ps1"
if errorlevel 1 (
    echo.
    echo Sync did not complete cleanly. See messages above.
    pause
    exit /b 1
)
echo.
pause
