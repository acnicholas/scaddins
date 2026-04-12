# Releasing nullCarbon Revit Export

## TL;DR

```powershell
do\3-release.cmd -Version 26.3.2 -Push
```

That bumps the version in `src\SCaddins.csproj`, builds Release2023/2024/2025/2026, packages an Inno Setup installer, creates a git tag `v26.3.2`, and pushes the tag. The push triggers [`.github/workflows/release.yml`](../.github/workflows/release.yml) which builds again on a clean Windows runner and publishes a GitHub Release with the installer attached. Users get the update via the in-app one-click updater on their next Revit start.

---

## What gets published

**Exactly one artifact per release**, regardless of how many Revit versions it supports:

```
nullCarbon-LCA-Export-win64-<version>.exe
```

This single Inno Setup installer (built from [`setup/nullcarbon/nullcarbon-installer.iss`](../setup/nullcarbon/nullcarbon-installer.iss)) bundles every Revit version that was built before packaging. The structure mirrors upstream's `setup/SCaddins.Installer.iss` exactly:

- Per-Revit-version content is gated by `#if R20XX == "Enabled"` preprocessor defines.
- [`scripts/build-installer.ps1`](../scripts/build-installer.ps1) auto-detects which `src\bin\Release<year>\` folders exist and passes the matching `/DR<year>=Enabled` flags to `iscc.exe`.
- The installer wizard shows an Inno Setup **components page** where the user ticks which Revit versions to install (defaults to all bundled).

It:

- Installs per-user under `%LocalAppData%\Studio.SC\nullCarbon-LCA-Export\<year>\` — **no admin needed**.
- Drops a `nullCarbon-LCA-Export.addin` manifest into `%AppData%\Autodesk\Revit\Addins\<year>\` for each ticked version.
- Detects Revit running and asks the user to close it before replacing files (`CloseApplications=yes`).
- Runs uninstall cleanly via Add/Remove Programs.

If you build only some Revit versions (e.g. `do\1-build.cmd -Configurations Release2025`), only those components show up in the installer wizard. If you build none, `do\2-installer.cmd` errors out before invoking Inno Setup.

The installer asset filename suffix (`.exe`) is what the in-app updater (`NullCarbonUpdater`) looks for via [`Branding.PreferredAssetSuffix`](../src/NullCarbon/Branding.cs). Don't change the suffix without also updating Branding.cs.

---

## Release tag format

The updater parses the GitHub tag name as a `System.Version` after stripping a leading `v`. Use SemVer-ish numeric tags only:

| Good | Bad |
|---|---|
| `v26.3.1` | `release-2026-q1` |
| `26.3.1` | `v26.3.1-beta` |
| `26.3.1.0` | `v26.3` |

The tag must match `^v\d+\.\d+\.\d+$` for `release.yml` to accept it.

---

## Three release paths

Pick one based on what you want to control.

### A. Fully automated — push a tag, GitHub Actions does the rest

```powershell
do\3-release.cmd -Version 26.3.2 -Push
```

This bumps the csproj, builds locally as a sanity check, tags, and pushes. The tag push triggers the workflow which builds + packages on a clean runner and publishes the Release.

Use this when you trust the local build matches what CI will produce. Recommended for normal releases.

### B. Build locally, push tag manually

```powershell
do\3-release.cmd -Version 26.3.2     :: no -Push
# inspect setup\out\nullCarbon-LCA-Export-win64-26.3.2.exe
git push origin v26.3.2                  # then push to trigger CI
```

Use this when you want to test the installer locally before triggering the public release.

### C. CI-only via workflow_dispatch

GitHub → Actions → Release → Run workflow → enter version `26.3.2`. Skips the local steps entirely. Useful when you don't have a build environment handy and just want to ship a hotfix.

---

## Local build environment

You only need this for paths A and B.

| Tool | Why | Get it |
|---|---|---|
| .NET SDK 8.x (Windows Desktop) | Builds Release2025 / Release2026 | https://dot.net |
| .NET Framework 4.8 dev pack | Builds Release2023 / Release2024 (both target `net48`) | https://dotnet.microsoft.com/download/dotnet-framework/net48 (scroll to "Developer Pack") |
| Inno Setup 6+ | Packages the installer | https://jrsoftware.org/isinfo.php or `choco install innosetup` |

After installing Inno Setup, `do\2-installer.cmd` (which wraps `scripts\build-installer.ps1`) finds it via:
1. `iscc` on PATH
2. `C:\Program Files (x86)\Inno Setup 6\iscc.exe` (default install location)
3. The `-InnoSetupExe <path>` parameter

---

## How the in-app updater finds your release

1. On Revit startup, [`NullCarbonModule`](../src/NullCarbon/NullCarbonModule.cs) fires a background task that calls `NullCarbonUpdater.CheckForUpdates(quietIfNotNewer: true)`.
2. The updater hits `https://api.github.com/repos/bhupas/revit/releases/latest`.
3. It parses `tag_name` as a version and compares against the running assembly version.
4. If newer, it picks the first asset whose name ends with `.exe` (configurable via `Branding.PreferredAssetSuffix`).
5. It shows a Revit `TaskDialog` with **Update now** / **Later**.
6. **Update now** downloads the .exe to `%TEMP%\nullCarbon-RevitExport-Update\` and launches it with `/SILENT /CLOSEAPPLICATIONS /NORESTART`.
7. Inno Setup detects Revit running, closes it cleanly, replaces the files, and finishes.
8. User re-opens Revit on the new version.

If GitHub is unreachable, the silent path swallows the error so Revit startup is never blocked.

---

## Smoke test the updater (without cutting a real release)

1. Pick a throwaway test repo or branch and publish a fake release tagged higher than the currently-installed version, with a dummy `.exe` asset.
2. Edit [`src/NullCarbon/Branding.cs`](../src/NullCarbon/Branding.cs) `GitHubOwner`/`GitHubRepo` to point at the throwaway.
3. Build Release2025, sideload the DLL, restart Revit.
4. The startup background check should pop the upgrade dialog within a few seconds.
5. Click **Update now** — verify the download and the `/SILENT` install behavior end-to-end.
6. **Revert the Branding.cs change before committing.**

---

## Versioning policy

The version lives in [`src/SCaddins.csproj`](../src/SCaddins.csproj). `do\3-release.cmd` rewrites:

- `<AssemblyVersion>X.Y.Z.0</AssemblyVersion>`
- `<FileVersion>X.Y.Z</FileVersion>`
- `<VersionPrefix>X.Y.Z</VersionPrefix>`
- `<AssemblyInformationalVersion>X.Y.Z</AssemblyInformationalVersion>`
- `<AssemblyInformationalVersionAttribute>X.Y.Z</AssemblyInformationalVersionAttribute>`
- `<InformationalVersionAttribute>X.Y.Z.0</InformationalVersionAttribute>`

Use SemVer roughly:

- **Patch** (`26.3.1` → `26.3.2`): bug fixes, internal cleanups, no schema or API changes.
- **Minor** (`26.3.x` → `26.4.0`): new features, new exported fields, backward-compatible.
- **Major** (`26.x.x` → `27.0.0`): incompatible Revit version drop, breaking nullCarbon API change.

When upstream SCaddins bumps its version, you don't need to follow it.

---

## Pre-release checklist

- [ ] All changes merged to `main`, working tree clean.
- [ ] `git log v<previous>..HEAD` reviewed for the changelog notes.
- [ ] [`CHANGELOG.md`](../CHANGELOG.md) updated (optional but nice).
- [ ] No leaked rebrand (run the verification command in [docs/SYNCING.md](SYNCING.md#verification-commands)).
- [ ] Local smoke test on at least Revit 2025.
- [ ] `do\3-release.cmd -Version <X.Y.Z>` runs without errors.
- [ ] Inspect `setup\out\nullCarbon-LCA-Export-win64-<X.Y.Z>.exe`.
- [ ] Push the tag (or let the workflow do it).
- [ ] After CI, download the published artifact and install on a clean machine.
