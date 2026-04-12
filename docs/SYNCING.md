# Syncing from upstream SCaddins

This fork (`bhupas/revit`) tracks [`acnicholas/scaddins`](https://github.com/acnicholas/scaddins). The repository is structured so that `git merge upstream/master` only ever touches a tiny set of files — the export code, the rebrand, and the updater are all isolated under `src/NullCarbon/` and never collide with upstream.

There are two ways to sync: a **GitHub Actions cron** that opens a PR every Monday automatically, and a **local PowerShell script** for manual control.

---

## How it works (the architecture)

| Path | Owner | What happens on `git merge upstream/master` |
|---|---|---|
| `src/NullCarbon/**` | nullCarbon | Never conflicts. Upstream has no files in this path. |
| `src/Assets/Ribbon/nullcarbon-*.png` | nullCarbon | Never conflicts. |
| `src/Assets/nullcarbon-*` | nullCarbon | Never conflicts. |
| `setup/nullcarbon/**` | nullCarbon | Never conflicts. New folder, upstream is unaware of it. |
| `scripts/**` | nullCarbon | Never conflicts. |
| `.github/workflows/**` | nullCarbon | Never conflicts (upstream doesn't ship workflows). |
| `docs/**` | nullCarbon | Never conflicts (new folder). |
| `src/SCaddins.addin` | nullCarbon | Auto-resolved by `merge=ours` in `.gitattributes`. |
| `src/Constants.cs` | nullCarbon | Auto-resolved by `merge=ours`. |
| `src/SCaddins.cs` | shared | Manual 3-way merge. The fork only touches it inside two `#if NULLCARBON` islands. Conflicts are almost always resolved as "keep both branches of the `#if`". |
| `src/SCaddins.csproj` | shared | Manual 3-way merge. The fork's edits are inside a single `<!-- nullCarbon fork: begin/end -->` block. Watch for upstream package version bumps. |
| `src/LatestRelease.cs` | upstream | Untouched in the fork. The updater reuses its `LatestVersion` / `Asset` types via the parent namespace. |
| All excluded upstream feature folders (`DestructivePurge/`, `GridManager/`, `ExportManager/`, etc.) | upstream | Apply theirs cleanly. They are present on disk but excluded from build by `<Compile Remove>`, so any upstream change to them is harmless. |

If a sync ever produces conflicts in a file outside this table, the rebrand has leaked somewhere it shouldn't be — find the offending change and either revert it or move it into `src/NullCarbon/`.

---

## Option 1 — Automated weekly sync (recommended)

[`.github/workflows/sync-upstream.yml`](../.github/workflows/sync-upstream.yml) runs every Monday at 06:00 UTC. It:

1. Adds the upstream remote.
2. Checks if upstream/master has any commits ahead of `HEAD`.
3. Creates a `sync/upstream-YYYY-MM-DD` branch and merges upstream into it.
4. Pushes the branch and opens a PR against `main`.

If the merge has conflicts, the PR is **still opened** with conflict markers committed — that way you have somewhere to resolve them in a normal PR review flow. The PR description tells you whether the merge was clean or has conflicts.

You can also trigger it on demand: GitHub → Actions → Sync upstream → Run workflow.

## Option 2 — Local sync via PowerShell

This is the same operation, run from your machine. Useful when you want to verify the build before pushing, or when you want to control exactly when a sync happens.

### One-time setup (per fresh clone)

```bash
git remote add upstream https://github.com/acnicholas/scaddins.git
git config merge.ours.driver true
```

The `merge.ours.driver` line is **required**. Without it, `.gitattributes` rules are silently ignored and your `merge=ours` stable files will become conflict-resolution work.

### Routine sync

```powershell
# Preview only — see what would happen, don't keep anything
scripts\sync.ps1 -DryRun

# Full sync into a new branch sync/upstream-YYYY-MM-DD
scripts\sync.ps1

# Skip the post-merge build verification (faster, but riskier)
scripts\sync.ps1 -SkipBuild
```

The script:
1. Verifies the upstream remote exists and the merge driver is enabled.
2. Refuses to run if your working tree is dirty.
3. Fetches upstream and shows the incoming commits.
4. Creates a `sync/upstream-YYYY-MM-DD` branch.
5. Runs the merge.
6. If clean, runs `do\1-build.cmd` to build all three Revit configs.
7. Tells you to push the branch and open a PR.

### When the merge has conflicts

`scripts\sync.ps1` stops at the conflict and lists the affected files. Resolve them by hand using the rules in the table above:

- **`src/SCaddins.cs`**: keep both branches of the `#if NULLCARBON` islands. If the conflict is bigger than ~15 lines, that means upstream refactored code adjacent to where we hooked in — push more logic into [`src/NullCarbon/NullCarbonModule.cs`](../src/NullCarbon/NullCarbonModule.cs) so the touchpoint stays small next time.
- **`src/SCaddins.csproj`**: find the `<!-- nullCarbon fork: begin -->` / `<!-- nullCarbon fork: end -->` block and keep ours. For everything outside the block (especially package version bumps), accept upstream unless you have a reason not to.
- **Excluded feature folders**: always accept upstream.
- **`src/SCaddins.addin`**, **`src/Constants.cs`**: should auto-resolve via `merge=ours`. If you see them in the conflict list, the `git config merge.ours.driver true` step wasn't run on this clone.

After resolving:

```powershell
git add <resolved files>
git commit
do\1-build.cmd               # verify
git push -u origin sync/upstream-YYYY-MM-DD
# Open a PR against main, review, merge.
```

---

## Verification commands

If you want to convince yourself the sync hygiene is working:

```bash
# Lines edited in SCaddins.cs vs upstream — should be < 15
git diff --stat upstream/master -- src/SCaddins.cs

# LatestRelease.cs should be byte-identical to upstream
git diff upstream/master -- src/LatestRelease.cs   # expect: empty

# The total list of files differing from upstream (excluding new nullCarbon paths)
git diff --name-only upstream/master \
  | grep -v "^src/NullCarbon/" \
  | grep -v "^src/Assets/nullcarbon" \
  | grep -v "^src/Assets/Ribbon/nullcarbon" \
  | grep -v "^setup/nullcarbon/" \
  | grep -v "^scripts/" \
  | grep -v "^.github/" \
  | grep -v "^docs/"
```

The expected output is exactly:

```
.gitattributes
NOTICE
README.md
src/Constants.cs
src/SCaddins.addin
src/SCaddins.cs
src/SCaddins.csproj
```

Anything else is a leak — investigate.
