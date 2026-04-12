# nullCarbon Revit Export -- interactive release wizard.
#
# Walks the user through a release: shows the current version, prompts for
# the new version, asks whether to push, confirms, then calls release.ps1
# which does the actual work (bump csproj, build, package installer, tag,
# optional push). Idempotent prompts -- you can Ctrl-C any time before the
# final confirmation.

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path "$PSScriptRoot\..").Path
Set-Location $RepoRoot

function Step($msg) { Write-Host ""; Write-Host "==> $msg" -ForegroundColor Cyan }
function Ok($msg)   { Write-Host "    $msg" -ForegroundColor Green }
function Warn($msg) { Write-Host "    $msg" -ForegroundColor Yellow }
function Fail($msg) { Write-Host ""; Write-Host "ERROR: $msg" -ForegroundColor Red; exit 1 }

# --- Read current version from csproj ----------------------------------------
$csproj = "$RepoRoot\src\SCaddins.csproj"
if (-not (Test-Path $csproj)) { Fail "src\SCaddins.csproj not found." }
$content = Get-Content -Raw $csproj
$currentVersion = $null
if ($content -match '<FileVersion>([^<]+)</FileVersion>') {
    $currentVersion = $matches[1]
}
if (-not $currentVersion) { $currentVersion = "0.0.0" }

# --- Header ------------------------------------------------------------------
Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host " nullCarbon Revit Export -- release wizard" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Current version (from src\SCaddins.csproj): $currentVersion"
Write-Host ""
Write-Host "  This wizard will:"
Write-Host "    1. Bump the version in src\SCaddins.csproj"
Write-Host "    2. Build Release2023 + Release2024 + Release2025 + Release2026"
Write-Host "    3. Package the Inno Setup installer (.exe)"
Write-Host "    4. Create a git tag (vX.Y.Z) and a release commit"
Write-Host "    5. (Optional) Push the tag to GitHub which triggers"
Write-Host "       the Actions workflow that publishes a GitHub Release"
Write-Host ""

# --- Prompt: new version -----------------------------------------------------
$newVersion = $null
while (-not $newVersion) {
    $input = Read-Host "  New version (e.g. 26.3.2)  [or Enter to cancel]"
    if ([string]::IsNullOrWhiteSpace($input)) {
        Write-Host ""
        Warn "Cancelled. Nothing changed."
        exit 0
    }
    if ($input -notmatch '^\d+\.\d+\.\d+$') {
        Warn "Invalid version. Use the form MAJOR.MINOR.PATCH (e.g. 26.3.2)."
        continue
    }
    if ($input -eq $currentVersion) {
        Warn "That's the same as the current version. Pick a higher one."
        continue
    }
    $newVersion = $input
}

# --- Check release notes -----------------------------------------------------
Step "Checking RELEASE_NOTES.md for v$newVersion"
$notesFile = Join-Path $RepoRoot 'RELEASE_NOTES.md'
if (-not (Test-Path $notesFile)) {
    Fail "RELEASE_NOTES.md not found at $notesFile -- create it before releasing."
}
& "$PSScriptRoot\extract-release-notes.ps1" -Version $newVersion -CheckOnly 2>$null
if ($LASTEXITCODE -ne 0) {
    Warn "No '## v$newVersion' section in RELEASE_NOTES.md."
    Warn "I'm opening RELEASE_NOTES.md in Notepad now. Add a section like:"
    Warn ""
    Warn "    ## v$newVersion -- $(Get-Date -Format 'yyyy-MM-dd')"
    Warn ""
    Warn "    ### Added"
    Warn "    - Whatever you added"
    Warn ""
    Warn "    ### Fixed"
    Warn "    - Whatever you fixed"
    Warn ""
    Warn "Save the file and CLOSE Notepad to continue. (Or close without"
    Warn "saving to cancel the release.)"
    Write-Host ""
    Read-Host "  Press Enter to open Notepad"
    $notepad = Start-Process notepad.exe -ArgumentList $notesFile -PassThru
    $notepad.WaitForExit()
    & "$PSScriptRoot\extract-release-notes.ps1" -Version $newVersion -CheckOnly 2>$null
    if ($LASTEXITCODE -ne 0) {
        Fail "Still no '## v$newVersion' section in RELEASE_NOTES.md. Aborting."
    }
}
Ok "Release notes section found."

# Show what will be published so the user can sanity-check before tagging.
Write-Host ""
Write-Host "  --- Notes that will be shown on GitHub Releases AND in the in-app updater ---" -ForegroundColor DarkGray
$preview = & "$PSScriptRoot\extract-release-notes.ps1" -Version $newVersion 2>$null
$preview -split "`n" | ForEach-Object { Write-Host "  $_" }
Write-Host "  --- end of notes ---" -ForegroundColor DarkGray

# --- Prompt: push? -----------------------------------------------------------
Write-Host ""
Write-Host "  Push the tag to GitHub after the local build?"
Write-Host "    Yes = pushing the tag triggers the GitHub Actions release workflow,"
Write-Host "          which publishes a public release with the installer attached."
Write-Host "    No  = local-only build + tag. You can push later with: git push origin v$newVersion"
Write-Host ""
$pushChoice = Read-Host "  Push tag to GitHub? [y/N]"
$push = ($pushChoice -match '^[Yy]')

# --- Confirm -----------------------------------------------------------------
Write-Host ""
Write-Host "----------------------------------------------------------------" -ForegroundColor Cyan
Write-Host "  Ready to release:"
Write-Host "    From version : $currentVersion"
Write-Host "    To version   : $newVersion"
if ($push) {
    Write-Host "    Push tag     : YES (will trigger GitHub Release publish)" -ForegroundColor Yellow
} else {
    Write-Host "    Push tag     : no  (local only)"
}
Write-Host "----------------------------------------------------------------" -ForegroundColor Cyan
Write-Host ""
$confirm = Read-Host "  Type 'yes' to proceed, anything else to cancel"
if ($confirm -ne 'yes') {
    Warn "Cancelled. Nothing changed."
    exit 0
}

# --- Run release.ps1 ---------------------------------------------------------
# Note: do NOT name this variable $args -- that's a PowerShell automatic
# variable. Splatting @args would use the script's positional params, not
# our local assignment, and release.ps1 would receive nothing.
Step "Running release.ps1"
$releaseArgs = @('-Version', $newVersion)
if ($push) { $releaseArgs += '-Push' }
& "$PSScriptRoot\release.ps1" @releaseArgs
exit $LASTEXITCODE
