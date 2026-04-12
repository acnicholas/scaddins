# What to click

Five files in this folder. **Numbered in the order you'll typically use them.** Just double-click in Explorer.

| # | File | What it does | When to click |
|---|---|---|---|
| **0** | **`0-install-prerequisites.cmd`** | Auto-installs .NET SDK 8 + .NET 4.8 Dev Pack + Inno Setup via winget. Idempotent — skips anything already installed. | **First time on a fresh machine**, then never again. |
| **1** | **`1-build.cmd`** | Build the add-in for Revit 2023 + 2024 + 2025 + 2026 | After making code changes |
| **2** | **`2-installer.cmd`** | Package the `.exe` installer | After `1-build.cmd`, when you want to test the installer locally |
| **3** | **`3-release.cmd`** | **Interactive release wizard.** Prompts for version + push y/n + confirms, then does build + installer + tag + (push). | When you want to **ship a new version**. Replaces 1+2 — does it all. |
| **4** | **`4-sync.cmd`** | **Interactive sync wizard.** Defaults to a safe dry-run preview; asks for explicit confirmation before doing a real merge. | Periodically. Or just let the weekly GitHub Action do it for you. |

## First time on this machine

Just double-click **`0-install-prerequisites.cmd`**. It checks for and installs:

- .NET SDK 8 (`winget install --id Microsoft.DotNet.SDK.8`)
- .NET Framework 4.8 Developer Pack (winget if available, otherwise opens the manual download page)
- Inno Setup 6 (`winget install --id JRSoftware.InnoSetup`)

After it finishes, **close this terminal/Explorer window and re-open one** so PATH picks up the new tools. Then double-click `1-build.cmd`.

## Most common workflows

```cmd
:: First time on the machine:
0-install-prerequisites.cmd
::   ... close and re-open Explorer ...

:: I just want to compile after a code change:
1-build.cmd

:: I want to test the installer locally:
1-build.cmd
2-installer.cmd

:: I want to ship a new release publicly (interactive):
3-release.cmd
::   wizard prompts for version + push y/n + confirms

:: I want to see what upstream has changed (safe preview):
4-sync.cmd
::   wizard defaults to dry run; press Enter for preview only
```

## What `3-release.cmd` does step by step

1. Reads the current version from `src\SCaddins.csproj` and shows it.
2. Prompts you for the new version (validates `MAJOR.MINOR.PATCH` format).
3. **Checks `RELEASE_NOTES.md` for a `## v<new-version>` section.** If missing, opens Notepad on the file so you can add one. Save + close Notepad to continue. The wizard refuses to release without notes.
4. **Shows a preview of the notes** that will end up on GitHub Releases AND in the in-app updater dialog.
5. Asks whether to push the tag to GitHub (default: no).
6. Shows a summary and waits for `yes` to proceed.
7. Bumps the version in `src\SCaddins.csproj`.
8. Builds Release2023 + Release2024 + Release2025 + Release2026.
9. Packages the Inno Setup installer.
10. Creates a `vX.Y.Z` git tag.
11. If you said yes to push: pushes the tag, which triggers `.github\workflows\release.yml` to publish a GitHub Release with the installer attached AND the release notes from RELEASE_NOTES.md.

You can Ctrl-C any time before step 7 and nothing will have changed.

## Where do I write the changelog / what's new?

**One file**: [`RELEASE_NOTES.md`](../RELEASE_NOTES.md) at the repo root. Add a `## v26.3.2 -- 2026-04-15` heading per version with a few bullet points underneath. The release wizard refuses to ship a version without a matching section, so you can't forget.

That same text shows up in **two places**:

1. The **GitHub Release description** at https://github.com/bhupas/revit/releases — extracted by `.github\workflows\release.yml` via `scripts\extract-release-notes.ps1` and used as the release `body`.
2. The **in-app "Update now" dialog** that pops up when a user has an older version — `NullCarbonUpdater` reads `latest.body` from the GitHub API, which is exactly what step 1 set.

Single source of truth. Write once, both surfaces show it.

## What `4-sync.cmd` does step by step

1. Asks: dry run (default) or full sync?
2. **Dry run**: previews the upstream merge, lists any conflicts, aborts cleanly. Nothing in your repo changes.
3. **Full sync** (only if you typed `yes` to confirm): creates `sync/upstream-YYYY-MM-DD`, merges upstream into it, runs the build to verify, then tells you to push the branch and open a PR.

If you just want to know "is there anything new upstream?", press Enter at the prompt and let the dry run answer.

## Ignore the other files

The repo root has `build.bat`, `build.ps1`, `build_installer.bat`, `build_tests.bat`, `run_tests.bat`. **Those are upstream's old Cake-based pipeline. Ignore them.** They're left untouched so upstream syncs (`4-sync.cmd`) stay clean. Don't double-click them — they require Cake setup that we don't have.

**Always use the five files in this folder.**
