# nullCarbon Revit Export -- one-shot first-time install of build prerequisites.
#
# Installs the three things you need to build the add-in:
#   - .NET SDK 8.x
#   - .NET Framework 4.8 Developer Pack
#   - Inno Setup 6
#
# Idempotent: each tool is checked first and skipped if already installed.
# Run this once on a fresh machine, then close + re-open the terminal so PATH
# picks up the new tools, then double-click do\1-build.cmd.

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path "$PSScriptRoot\..").Path
Set-Location $RepoRoot

function Step($msg) { Write-Host ""; Write-Host "==> $msg" -ForegroundColor Cyan }
function Ok($msg)   { Write-Host "    $msg" -ForegroundColor Green }
function Warn($msg) { Write-Host "    $msg" -ForegroundColor Yellow }
function Skip($msg) { Write-Host "    $msg" -ForegroundColor DarkGray }
function Fail($msg) { Write-Host "ERROR: $msg" -ForegroundColor Red; exit 1 }

# --- winget availability -----------------------------------------------------
Step "Checking winget"
$winget = Get-Command winget -ErrorAction SilentlyContinue
if (-not $winget) {
    Fail @"
winget is not installed on this machine.

winget ships with Windows 10 (1809+) / 11 via the App Installer package.
If it's missing, install 'App Installer' from the Microsoft Store, then
re-run this script.

Or, install everything manually from the URLs in do\README.md.
"@
}
Ok "winget is available."

function Install-IfMissing {
    param(
        [string]$DisplayName,
        [string]$WingetId,
        [scriptblock]$AlreadyInstalledTest,
        [string]$ManualUrl
    )
    Step $DisplayName
    if (& $AlreadyInstalledTest) {
        Skip "Already installed."
        return
    }
    Write-Host "    Installing via: winget install --id $WingetId"
    & winget install --id $WingetId --silent --accept-package-agreements --accept-source-agreements
    if ($LASTEXITCODE -ne 0) {
        Warn "winget install failed (exit $LASTEXITCODE)."
        if ($ManualUrl) {
            Warn "Install manually from:"
            Warn "    $ManualUrl"
        }
        return
    }
    Ok "Installed."
}

# --- 1) .NET SDK 8 -----------------------------------------------------------
Install-IfMissing `
    -DisplayName ".NET SDK 8" `
    -WingetId "Microsoft.DotNet.SDK.8" `
    -ManualUrl "https://dotnet.microsoft.com/download/dotnet/8.0" `
    -AlreadyInstalledTest {
        $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
        if (-not $dotnet) { return $false }
        $sdks = & dotnet --list-sdks 2>$null
        if (-not $sdks) { return $false }
        return ($sdks | Where-Object { $_ -match '^8\.' }) -ne $null
    }

# --- 2) .NET Framework 4.8 Developer Pack ------------------------------------
# winget IDs for the 4.8 dev pack are inconsistent across winget versions.
# We try the most-common ID, and if winget doesn't recognize it, fall back to
# opening the manual download page in the browser.
Step ".NET Framework 4.8 Developer Pack"
$net48Path = "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\mscorlib.dll"
if (Test-Path $net48Path) {
    Skip "Already installed (v4.8 reference assemblies present)."
} else {
    $tryIds = @(
        'Microsoft.DotNet.Framework.DeveloperPack_4.8',
        'Microsoft.DotNet.Framework.DeveloperPack_4',
        'Microsoft.NETFramework.DevelopersPack.4.8'
    )
    $installed = $false
    foreach ($id in $tryIds) {
        Write-Host "    Trying winget id: $id"
        & winget install --id $id --silent --accept-package-agreements --accept-source-agreements 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0 -and (Test-Path $net48Path)) {
            Ok "Installed via $id."
            $installed = $true
            break
        }
    }
    if (-not $installed) {
        Warn "winget could not install the 4.8 dev pack."
        Warn "Opening the manual download page in your browser..."
        Warn "    https://dotnet.microsoft.com/download/dotnet-framework/net48"
        Warn "    (scroll to 'Developer Pack' and click the download link)"
        Start-Process "https://dotnet.microsoft.com/download/dotnet-framework/net48"
    }
}

# --- 3) Inno Setup 6 ---------------------------------------------------------
Install-IfMissing `
    -DisplayName "Inno Setup 6" `
    -WingetId "JRSoftware.InnoSetup" `
    -ManualUrl "https://jrsoftware.org/isinfo.php" `
    -AlreadyInstalledTest {
        if (Get-Command iscc -ErrorAction SilentlyContinue) { return $true }
        return (Test-Path 'C:\Program Files (x86)\Inno Setup 6\iscc.exe') -or
               (Test-Path 'C:\Program Files\Inno Setup 6\iscc.exe')
    }

# --- Done --------------------------------------------------------------------
Step "Done"
Ok "All prerequisites checked / installed."
Write-Host ""
Write-Host "    NEXT: close this window and any open terminal/Explorer," -ForegroundColor Yellow
Write-Host "    open a NEW Explorer window at the repo, then double-click" -ForegroundColor Yellow
Write-Host "    do\1-build.cmd. This is required so the new tools land on" -ForegroundColor Yellow
Write-Host "    your PATH." -ForegroundColor Yellow
Write-Host ""
