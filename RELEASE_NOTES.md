# Release notes

This file is the single source of truth for what shows up in:

- The **GitHub Release description** at https://github.com/bhupas/revit/releases
- The **in-app "Update now" dialog** that pops up when a new version is detected (the dialog reads the GitHub release body)

## How to write a new entry

Before running `do\3-release.cmd`, add a section at the top of this file with the version you're about to ship. The wizard refuses to release a version that doesn't have an entry here. The release workflow extracts the section between `## vX.Y.Z` and the next `## v...` heading and ships it as the release body.

Format:

```markdown
## v26.3.2 — 2026-04-15

### Added
- New "Export to PDF" option in the export dialog.

### Fixed
- Schedule export no longer crashes when a column has a null formula.

### Changed
- Login window now remembers the last successful username.
```

Section headings (`### Added`, `### Fixed`, `### Changed`, `### Removed`, `### Security`) follow the [Keep a Changelog](https://keepachangelog.com/) convention. They're optional — you can also write free-form prose under the version heading.

Keep entries **short and user-facing**. The in-app updater dialog truncates to 600 characters, so put the most important things first.

---

## v26.3.2 -- 2026-04-12

First public nullCarbon-branded release.

### Added
- Single ribbon button **nullCarbon Export** with the nullCarbon logo (light + dark variants).
- Login window with token caching against the nullCarbon API.
- One-click in-app updater that downloads + runs the installer when a new release is published.
- Automatic startup update check (silent if already on the latest version, never blocks Revit launch).
- Single `.exe` installer covering Revit 2023, 2024, 2025 and 2026 (per-user, no admin needed).
- `RELEASE_NOTES.md` as the single source of truth for this changelog and the in-app updater dialog.
- Interactive `do\3-release.cmd` and `do\4-sync.cmd` wizards.
- One-shot `do\0-install-prerequisites.cmd` that installs the .NET SDK 8, .NET Framework 4.8 Dev Pack, and Inno Setup 6 via winget.

### Changed
- Based on SCaddins by Andrew Nicholas (LGPL-3.0). Every other SCaddins feature is hidden from the user; only the rebranded ExportSchedules window is exposed.

---

## v26.3.1 -- placeholder

Initial nullCarbon-branded test build. Replace this entry the next time you actually ship a 26.3.1.
