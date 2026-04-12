# nullCarbon Revit Export -- release helper.
#
# What this does:
#   1. Bumps the version in src\SCaddins.csproj (AssemblyVersion + friends)
#   2. Builds Release2024, Release2025, Release2026
#   3. Packages the Inno Setup installer
#   4. Creates a git tag (vX.Y.Z) and an annotated commit
#   5. (Optional) Pushes the tag -- triggers .github\workflows\release.yml
#
# Usage:
#     scripts\release.ps1 -Version 26.3.2
#     scripts\release.ps1 -Version 26.3.2 -Push
#     scripts\release.ps1 -Version 26.3.2 -SkipBuild   # if you've already built
#     scripts\release.ps1 -Version 26.3.2 -DryRun      # show what would change
#
# Notes:
#   - Won't run if the working tree is dirty (commit/stash first).
#   - The tag triggers GitHub Actions to publish a Release with assets if
#     .github\workflows\release.yml is enabled and the runner has Inno Setup.

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    [switch]$Push,
    [switch]$SkipBuild,
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path "$PSScriptRoot\..").Path
Set-Location $RepoRoot

function Step($msg) { Write-Host ""; Write-Host "==> $msg" -ForegroundColor Cyan }
function Ok($msg)   { Write-Host "    $msg" -ForegroundColor Green }
function Warn($msg) { Write-Host "    $msg" -ForegroundColor Yellow }
function Fail($msg) { Write-Host "ERROR: $msg" -ForegroundColor Red; exit 1 }

if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Fail "Version must be SemVer-ish (e.g. 26.3.2)."
}

# --- Preflight ---------------------------------------------------------------
if (-not (Test-Path "$RepoRoot\.git")) { Fail "Not a git repo." }

$dirty = git status --porcelain
if ($dirty -and -not $DryRun) {
    Fail "Working tree is dirty. Commit or stash first."
}

$tag = "v$Version"
$existingTag = git tag --list $tag
if ($existingTag) { Fail "Tag $tag already exists." }

# --- Bump version in csproj ---------------------------------------------------
$csproj = "$RepoRoot\src\SCaddins.csproj"
$content = Get-Content -Raw $csproj
$bumped = $content `
    -replace '<AssemblyVersion>[^<]*</AssemblyVersion>', "<AssemblyVersion>$Version.0</AssemblyVersion>" `
    -replace '<FileVersion>[^<]*</FileVersion>', "<FileVersion>$Version</FileVersion>" `
    -replace '<VersionPrefix>[^<]*</VersionPrefix>', "<VersionPrefix>$Version</VersionPrefix>" `
    -replace '<AssemblyInformationalVersion>[^<]*</AssemblyInformationalVersion>', "<AssemblyInformationalVersion>$Version</AssemblyInformationalVersion>" `
    -replace '<AssemblyInformationalVersionAttribute>[^<]*</AssemblyInformationalVersionAttribute>', "<AssemblyInformationalVersionAttribute>$Version</AssemblyInformationalVersionAttribute>" `
    -replace '<InformationalVersionAttribute>[^<]*</InformationalVersionAttribute>', "<InformationalVersionAttribute>$Version.0</InformationalVersionAttribute>"

if ($DryRun) {
    Step "DRY RUN -- would bump csproj to $Version"
    Compare-Object ($content -split "`n") ($bumped -split "`n") | Select-Object -First 20
    return
}

Step "Bumping version to $Version"
Set-Content -Path $csproj -Value $bumped -Encoding UTF8 -NoNewline
Ok "csproj updated"

# --- Build & package ----------------------------------------------------------
if (-not $SkipBuild) {
    Step "Building"
    & "$PSScriptRoot\build.ps1" -Clean
    if ($LASTEXITCODE -ne 0) { Fail "Build failed; aborting release." }

    Step "Packaging installer"
    & "$PSScriptRoot\build-installer.ps1" -Version $Version
    if ($LASTEXITCODE -ne 0) { Fail "Installer packaging failed; aborting release." }
}

# --- Commit + tag -------------------------------------------------------------
Step "Committing version bump"
git add $csproj
git commit -m "release: v$Version"

Step "Creating tag $tag"
git tag -a $tag -m "Release $tag"
Ok "tag $tag created"

if ($Push) {
    Step "Pushing tag and commit"
    git push origin HEAD
    git push origin $tag
    Ok "pushed"
} else {
    Warn "Tag created locally only. Push with: git push origin $tag"
    Warn "Pushing the tag triggers .github\workflows\release.yml (if enabled)."
}

Step "Done"
Ok "Installer: setup\out\nullCarbon-LCA-Export-win64-$Version.exe"
