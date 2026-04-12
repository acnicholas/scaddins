# nullCarbon Revit Export -- one-command build script.
#
# Builds the SCaddins.dll for one or more Revit versions and writes the
# per-version .addin manifests next to each output. After this completes,
# scripts\build-installer.ps1 can package an Inno Setup .exe.
#
# Usage:
#     scripts\build.ps1                            # Release2023 + 2024 + 2025 + 2026
#     scripts\build.ps1 -Configurations Release2025
#     scripts\build.ps1 -Clean                     # full clean rebuild
#     scripts\build.ps1 -SkipRestore               # faster repeat builds
#
# Requirements (script will tell you the install commands if missing):
#   - .NET SDK 8.x with Windows Desktop workload  (for Release2025 / Release2026)
#   - .NET Framework 4.8 developer pack            (for Release2023 / Release2024)

[CmdletBinding()]
param(
    [string[]]$Configurations = @('Release2023','Release2024','Release2025','Release2026'),
    [switch]$Clean,
    [switch]$SkipRestore
)

$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path "$PSScriptRoot\..").Path
Set-Location $RepoRoot

function Step($msg) { Write-Host ""; Write-Host "==> $msg" -ForegroundColor Cyan }
function Ok($msg)   { Write-Host "    $msg" -ForegroundColor Green }
function Warn($msg) { Write-Host "    $msg" -ForegroundColor Yellow }
function Fail($msg) {
    Write-Host ""
    Write-Host "ERROR: $msg" -ForegroundColor Red
    Write-Host ""
    exit 1
}

# --- Preflight: .NET SDK -----------------------------------------------------
Step ".NET SDK"

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Fail @"
The 'dotnet' command was not found on PATH.

You need the .NET SDK 8 to build. Install it with:

    winget install --id Microsoft.DotNet.SDK.8 --silent --accept-package-agreements --accept-source-agreements

After it finishes, CLOSE this terminal and re-open a NEW one,
then double-click do\1-build.cmd again.
(A new terminal is required so PATH picks up the new SDK.)
"@
}

# Distinguish "SDK installed" from "runtime only".
# `dotnet --list-sdks` exits 0 with output if any SDK is installed,
# and exits non-zero (or empty) if only the runtime is present.
$sdks = $null
try { $sdks = & dotnet --list-sdks 2>$null } catch { }
if ($LASTEXITCODE -ne 0 -or -not $sdks) {
    Fail @"
You have the .NET RUNTIME installed, but NOT the .NET SDK.
The SDK is required to build (the runtime can only RUN .NET apps,
not COMPILE them).

Install the SDK with:

    winget install --id Microsoft.DotNet.SDK.8 --silent --accept-package-agreements --accept-source-agreements

After it finishes, CLOSE this terminal and re-open a NEW one,
then double-click do\1-build.cmd again.
"@
}
$sdks | ForEach-Object { Ok $_ }

$has8 = $sdks | Where-Object { $_ -match '^8\.' }
if (-not $has8) {
    Warn ".NET SDK 8.x was not found in the list above."
    Warn "Release2025 and Release2026 may fail to build."
    Warn "Install SDK 8 with:"
    Warn "    winget install --id Microsoft.DotNet.SDK.8"
}

# --- Preflight: .NET Framework 4.8 dev pack (for Release2023 / Release2024) --
# Release2023 and Release2024 target net48 and need the actual 4.8 reference
# assemblies on disk. The 4.8.1 dev pack does NOT include them -- they're a
# separate download.
$wantsNet48 = ($Configurations -contains 'Release2023') -or ($Configurations -contains 'Release2024')
if ($wantsNet48) {
    Step ".NET Framework 4.8 Developer Pack"
    $net48Probe   = 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\mscorlib.dll'
    $net481Probe  = 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\mscorlib.dll'
    $has48  = Test-Path $net48Probe
    $has481 = Test-Path $net481Probe

    if ($has48) {
        Ok "v4.8 reference assemblies found."
    } elseif ($has481) {
        Warn "Found v4.8.1 but not v4.8."
        Warn "Building net48 against v4.8.1 ref assemblies will most likely FAIL with"
        Warn "  'The reference assemblies for .NETFramework,Version=v4.8 were not found.'"
        Warn ""
        Warn "Install the actual 4.8 dev pack -- winget IDs vary, the safest is the manual download:"
        Warn "    https://dotnet.microsoft.com/download/dotnet-framework/net48"
        Warn "(scroll to 'Developer Pack')."
        Warn ""
        Warn "Skipping Release2023 / Release2024 for now. Re-run after installing if you want them."
        $Configurations = @($Configurations | Where-Object { $_ -ne 'Release2023' -and $_ -ne 'Release2024' })
    } else {
        Warn "Neither v4.8 nor v4.8.1 reference assemblies found."
        Warn "Install the .NET Framework 4.8 Developer Pack from:"
        Warn "    https://dotnet.microsoft.com/download/dotnet-framework/net48"
        Warn "(scroll to 'Developer Pack')."
        Warn ""
        Warn "Skipping Release2023 / Release2024 for now."
        $Configurations = @($Configurations | Where-Object { $_ -ne 'Release2023' -and $_ -ne 'Release2024' })
    }

    if ($Configurations.Count -eq 0) {
        Fail "No buildable configurations remain. Install the 4.8 Dev Pack and re-run."
    }
}

# --- Locate csproj -----------------------------------------------------------
$csproj = Join-Path $RepoRoot 'src\SCaddins.csproj'
if (-not (Test-Path $csproj)) { Fail "src\SCaddins.csproj not found." }

# --- Clean --------------------------------------------------------------------
if ($Clean) {
    Step "Cleaning bin/ and obj/"
    Remove-Item -Recurse -Force "$RepoRoot\src\bin","$RepoRoot\src\obj" -ErrorAction SilentlyContinue
    Ok "removed src\bin and src\obj"
}

# --- Build each requested config ---------------------------------------------
foreach ($cfg in $Configurations) {
    Step "Building $cfg"
    $buildArgs = @('build', $csproj, '-c', $cfg, '-v:minimal', '-nologo')
    if ($SkipRestore) { $buildArgs += '--no-restore' }
    & dotnet @buildArgs
    if ($LASTEXITCODE -ne 0) { Fail "Build failed: $cfg" }
    Ok "$cfg built"

    # Stage the per-version .addin manifest with _REVIT_VERSION_ resolved.
    $year = ($cfg -replace 'Release','')
    $template = Get-Content -Raw "$RepoRoot\src\SCaddins.addin"
    $resolved = $template -replace '_REVIT_VERSION_', $year
    $outDir = "$RepoRoot\src\bin\$cfg"
    if (-not (Test-Path $outDir)) { Fail "Expected output dir $outDir not found." }
    $addinPath = Join-Path $outDir "nullCarbon-LCA-Export-$year.addin"
    Set-Content -Path $addinPath -Value $resolved -Encoding UTF8
    Ok "wrote $addinPath"
}

Step "Done"
Ok "Build artifacts: $RepoRoot\src\bin\<Release...>"
Ok "Next: double-click do\2-installer.cmd to package the installer"
