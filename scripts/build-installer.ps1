# nullCarbon Revit Export -- installer packager.
#
# Wraps Inno Setup. Reads the assembly version from one of the built DLLs,
# auto-detects which Release<year> folders exist, and passes /DR<year>=Enabled
# defines so the .iss script bundles only those Revit versions.
#
# Usage:
#     scripts\build-installer.ps1                         # auto-detect version + configs
#     scripts\build-installer.ps1 -Version 26.3.1
#     scripts\build-installer.ps1 -InnoSetupExe "C:\Program Files (x86)\Inno Setup 6\iscc.exe"
#
# If iscc.exe is not on PATH and -InnoSetupExe is not given, the script tries
# the standard install location: C:\Program Files (x86)\Inno Setup 6\iscc.exe.
#
# Output: ONE file per release at setup\out\nullCarbon-LCA-Export-win64-<version>.exe
# (matches the upstream SCaddins installer naming pattern.)

[CmdletBinding()]
param(
    [string]$Version,
    [string]$InnoSetupExe
)

$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path "$PSScriptRoot\..").Path
Set-Location $RepoRoot

function Step($msg) { Write-Host ""; Write-Host "==> $msg" -ForegroundColor Cyan }
function Ok($msg)   { Write-Host "    $msg" -ForegroundColor Green }
function Fail($msg) { Write-Host "ERROR: $msg" -ForegroundColor Red; exit 1 }

# --- Locate iscc.exe ----------------------------------------------------------
if (-not $InnoSetupExe) {
    $candidate = Get-Command iscc -ErrorAction SilentlyContinue
    if ($candidate) {
        $InnoSetupExe = $candidate.Path
    } else {
        $defaults = @(
            'C:\Program Files (x86)\Inno Setup 6\iscc.exe',
            'C:\Program Files\Inno Setup 6\iscc.exe'
        )
        foreach ($p in $defaults) {
            if (Test-Path $p) { $InnoSetupExe = $p; break }
        }
    }
}
if (-not $InnoSetupExe -or -not (Test-Path $InnoSetupExe)) {
    Fail "Inno Setup not found. Install from https://jrsoftware.org/isinfo.php and re-run, or pass -InnoSetupExe."
}
Step "Inno Setup"
Ok $InnoSetupExe

# --- Detect built configurations ---------------------------------------------
$years = @('2023','2024','2025','2026')
$builtYears = @()
foreach ($y in $years) {
    if (Test-Path "$RepoRoot\src\bin\Release$y\SCaddins.dll") { $builtYears += $y }
}
if ($builtYears.Count -eq 0) {
    Fail "No built configurations found under src\bin\Release<year>. Run do\1-build.cmd first."
}
Step "Detected built configurations"
$builtYears | ForEach-Object { Ok "Release$_" }

# --- Resolve version ----------------------------------------------------------
if (-not $Version) {
    foreach ($y in @('2025','2026','2024','2023')) {
        $dll = "$RepoRoot\src\bin\Release$y\SCaddins.dll"
        if (Test-Path $dll) {
            $info = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dll)
            $Version = "$($info.FileMajorPart).$($info.FileMinorPart).$($info.FileBuildPart)"
            Step "Detected version $Version from $dll"
            break
        }
    }
}
if (-not $Version) {
    Fail "Could not auto-detect version. Pass -Version explicitly."
}

# --- Run Inno Setup -----------------------------------------------------------
$iss = "$RepoRoot\setup\nullcarbon\nullcarbon-installer.iss"
if (-not (Test-Path $iss)) { Fail "Installer script not found: $iss" }

New-Item -ItemType Directory -Force -Path "$RepoRoot\setup\out" | Out-Null

# Build the iscc argument list. /D defines must be passed individually so the
# preprocessor can evaluate the #if R20XX == "Enabled" blocks at compile time.
$isccArgs = @("/DMyAppVersion=$Version")
foreach ($y in $builtYears) {
    $isccArgs += "/DR$y=Enabled"
}
$isccArgs += $iss

Step "Compiling installer"
Ok ("iscc " + ($isccArgs -join ' '))
& $InnoSetupExe @isccArgs
if ($LASTEXITCODE -ne 0) { Fail "Inno Setup compile failed (exit $LASTEXITCODE)" }

Step "Done"
$out = "$RepoRoot\setup\out\nullCarbon-LCA-Export-win64-$Version.exe"
if (Test-Path $out) {
    $size = [math]::Round((Get-Item $out).Length / 1MB, 1)
    Ok "Output: $out ($size MB)"
    Ok "Bundles: $($builtYears -join ', ')"
} else {
    Fail "Expected output not found: $out"
}
