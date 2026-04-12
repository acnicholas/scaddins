# nullCarbon Revit Export -- interactive upstream sync wizard.
#
# Defaults to a DRY RUN: previews what an upstream merge would do, lists any
# conflicts, then aborts. The user has to explicitly opt in to a real sync.
# This is the safer default because a real sync creates a branch + commits
# you'd then have to clean up if the merge isn't what you wanted.

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path "$PSScriptRoot\..").Path
Set-Location $RepoRoot

function Step($msg) { Write-Host ""; Write-Host "==> $msg" -ForegroundColor Cyan }
function Ok($msg)   { Write-Host "    $msg" -ForegroundColor Green }
function Warn($msg) { Write-Host "    $msg" -ForegroundColor Yellow }
function Fail($msg) { Write-Host ""; Write-Host "ERROR: $msg" -ForegroundColor Red; exit 1 }

# --- Header ------------------------------------------------------------------
Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host " nullCarbon Revit Export -- upstream sync wizard" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Pulls new commits from acnicholas/scaddins (the upstream)."
Write-Host ""
Write-Host "  Two modes:"
Write-Host "    [1] DRY RUN  (default) - just preview what would happen,"
Write-Host "                              show any conflicts, then abort."
Write-Host "                              Nothing is changed in your repo."
Write-Host "    [2] FULL SYNC          - create a sync/upstream-YYYY-MM-DD"
Write-Host "                              branch, merge upstream, run the"
Write-Host "                              build to verify, leave the branch"
Write-Host "                              for you to push + open a PR."
Write-Host ""

# --- Prompt: which mode? ----------------------------------------------------
$mode = Read-Host "  Choose [1=dry run, 2=full sync, Enter=dry run]"
if ([string]::IsNullOrWhiteSpace($mode)) { $mode = '1' }

if ($mode -eq '2') {
    Write-Host ""
    Warn "FULL SYNC will create a new branch with merged upstream commits."
    $confirm = Read-Host "  Type 'yes' to proceed, anything else to fall back to dry run"
    if ($confirm -ne 'yes') {
        Warn "Falling back to dry run."
        $mode = '1'
    }
}

# --- Run sync.ps1 ------------------------------------------------------------
Step "Running sync.ps1"
if ($mode -eq '1') {
    & "$PSScriptRoot\sync.ps1" -DryRun
} else {
    & "$PSScriptRoot\sync.ps1"
}
exit $LASTEXITCODE
