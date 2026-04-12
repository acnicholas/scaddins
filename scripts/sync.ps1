# nullCarbon Revit Export -- upstream sync helper.
#
# Fetches acnicholas/scaddins, attempts a merge into a sync branch, and
# (if .NET SDK is available) builds Release2023/2024/2025/2026 to confirm nothing
# broke. See docs\SYNCING.md for the full workflow this script implements.
#
# Usage:
#     scripts\sync.ps1                  # full sync into a new branch
#     scripts\sync.ps1 -DryRun          # preview the merge, don't keep it
#     scripts\sync.ps1 -SkipBuild       # merge only, don't build
#     scripts\sync.ps1 -UpstreamBranch master

[CmdletBinding()]
param(
    [string]$UpstreamBranch = 'master',
    [switch]$DryRun,
    [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path "$PSScriptRoot\..").Path
Set-Location $RepoRoot

function Step($msg) { Write-Host ""; Write-Host "==> $msg" -ForegroundColor Cyan }
function Ok($msg)   { Write-Host "    $msg" -ForegroundColor Green }
function Warn($msg) { Write-Host "    $msg" -ForegroundColor Yellow }
function Fail($msg) { Write-Host "ERROR: $msg" -ForegroundColor Red; exit 1 }

# --- Preflight ---------------------------------------------------------------
if (-not (Test-Path "$RepoRoot\.git")) { Fail "Not a git repo." }

$remotes = git remote
if ($remotes -notcontains 'upstream') {
    Fail "No 'upstream' remote. Add it once with:`n    git remote add upstream https://github.com/acnicholas/scaddins.git"
}

$driver = git config --get merge.ours.driver
if (-not $driver) {
    Warn "merge.ours driver not enabled -- .gitattributes rules will be ignored."
    Warn "Enable it once per clone with:  git config merge.ours.driver true"
}

$dirty = git status --porcelain
if ($dirty) { Fail "Working tree is dirty. Commit or stash first." }

# --- Fetch upstream ----------------------------------------------------------
Step "Fetching upstream"
git fetch upstream --tags --quiet
if ($LASTEXITCODE -ne 0) { Fail "git fetch upstream failed." }

$ahead  = (git rev-list --count "HEAD..upstream/$UpstreamBranch").Trim()
$behind = (git rev-list --count "upstream/$UpstreamBranch..HEAD").Trim()
Ok "upstream/$UpstreamBranch is $ahead ahead, $behind behind of HEAD"

if ([int]$ahead -eq 0) {
    Step "Nothing to sync"
    Ok "Already up to date with upstream/$UpstreamBranch."
    exit 0
}

# --- Show what's coming ------------------------------------------------------
Step "Incoming commits"
git log --oneline --no-decorate "HEAD..upstream/$UpstreamBranch" | Select-Object -First 30
$total = [int]$ahead
if ($total -gt 30) { Warn "(... and $($total - 30) more)" }

# --- Dry run -----------------------------------------------------------------
if ($DryRun) {
    Step "DRY RUN -- previewing merge without keeping it"
    $tmp = "sync/dryrun-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    git checkout -b $tmp | Out-Null
    git merge --no-commit --no-ff "upstream/$UpstreamBranch" 2>&1 | Out-Null
    $conflicts = git diff --name-only --diff-filter=U
    if ($conflicts) {
        Step "Conflict surface"
        $conflicts | ForEach-Object { Write-Host "    $_" -ForegroundColor Yellow }
    } else {
        Ok "Clean merge -- no conflicts."
    }
    git merge --abort 2>&1 | Out-Null
    git checkout - 2>&1 | Out-Null
    git branch -D $tmp 2>&1 | Out-Null
    exit 0
}

# --- Real sync ---------------------------------------------------------------
$branch = "sync/upstream-$(Get-Date -Format 'yyyy-MM-dd')"
$existing = git branch --list $branch
if ($existing) {
    Warn "Branch $branch already exists; appending timestamp."
    $branch = "sync/upstream-$(Get-Date -Format 'yyyy-MM-dd-HHmmss')"
}

Step "Creating $branch"
git checkout -b $branch | Out-Null

Step "Merging upstream/$UpstreamBranch"
git merge --no-edit "upstream/$UpstreamBranch"
$mergeExit = $LASTEXITCODE

if ($mergeExit -ne 0) {
    Step "Conflicts detected"
    $conflicts = git diff --name-only --diff-filter=U
    $conflicts | ForEach-Object { Write-Host "    $_" -ForegroundColor Yellow }
    Write-Host ""
    Warn "Resolve the conflicts, then:"
    Warn "  1. git add <resolved files>"
    Warn "  2. git commit"
    Warn "  3. scripts\build.ps1     (verify the build still works)"
    Warn "  4. git push -u origin $branch"
    Warn "  5. Open a PR against main and merge."
    Warn ""
    Warn "Conflict resolution tips: docs\SYNCING.md"
    exit $mergeExit
}

Ok "Merged cleanly."

# --- Build verification ------------------------------------------------------
if (-not $SkipBuild) {
    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($dotnet) {
        Step "Verifying build"
        & "$PSScriptRoot\build.ps1"
        if ($LASTEXITCODE -ne 0) {
            Fail "Build failed after merge. Inspect the failure, fix on $branch, then push."
        }
    } else {
        Warn "dotnet CLI not found -- skipping build verification."
        Warn "Verify manually in Visual Studio (Release2023/2024/2025/2026) before pushing."
    }
}

Step "Done"
Ok "Sync complete on branch $branch."
Ok "Next: git push -u origin $branch  (then open a PR against main)"
