; nullCarbon Revit Export — Inno Setup script
;
; Mirrors the structure of upstream's setup/SCaddins.Installer.iss exactly:
;
;   - ONE installer .exe per release.
;   - Per-Revit-version content is gated by #if R20XX == "Enabled" preprocessor
;     defines passed in by the build script (scripts\build-installer.ps1).
;   - Inno Setup [Components] page lets the user pick which Revit versions
;     to install (Types: full custom).
;
; Build via:
;     do\2-installer.cmd                      (recommended — double-click)
;     scripts\build-installer.ps1             (PowerShell wrapper)
;     iscc /DMyAppVersion=26.3.1 /DR2024=Enabled /DR2025=Enabled /DR2026=Enabled \
;          setup\nullcarbon\nullcarbon-installer.iss
;
; Requires Inno Setup 6+ (https://jrsoftware.org/isinfo.php).

#define MyAppName "nullCarbon-LCA-Export"
#define MyAppDisplayName "nullCarbon Revit Export"
#define MyAppPublisher "nullCarbon"
#define MyAppURL "https://github.com/bhupas/revit"
#define MyAppContact "contact@nullcarbon.dk"

#ifndef MyAppVersion
  #define MyAppVersion "0.0.0"
#endif

[Setup]
; Stable AppId — keep across versions so upgrades replace cleanly.
AppId={{B4F7A2E1-3C8D-4A5F-9E2B-7F1C8D3E5A92}
AppName={#MyAppDisplayName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}/issues
AppUpdatesURL={#MyAppURL}/releases/latest
AppContact={#MyAppContact}

DefaultDirName={localappdata}\Studio.SC\{#MyAppName}
DisableDirPage=yes
DefaultGroupName={#MyAppDisplayName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest

OutputDir=..\..\setup\out
OutputBaseFilename={#MyAppName}-win64-{#MyAppVersion}
Compression=lzma2/ultra64
LZMANumBlockThreads=4
CompressionThreads=4
LZMADictionarySize=262144
LZMAUseSeparateProcess=yes
SolidCompression=yes
WizardStyle=modern
WizardSmallImageFile=nullcarbon-wizard.bmp,nullcarbon-wizard.bmp
SetupIconFile=nullcarbon.ico
UninstallDisplayIcon={uninstallexe}
UninstallDisplayName={#MyAppDisplayName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

; Cleanly close Revit before replacing files.
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Components]
Name: main; Description: "Install shared files required by {#MyAppDisplayName}"; Types: full compact custom; Flags: fixed
#if R2023 == "Enabled"
Name: revit2023; Description: "Revit 2023 add-in"; Types: full custom
#endif
#if R2024 == "Enabled"
Name: revit2024; Description: "Revit 2024 add-in"; Types: full custom
#endif
#if R2025 == "Enabled"
Name: revit2025; Description: "Revit 2025 add-in"; Types: full custom
#endif
#if R2026 == "Enabled"
Name: revit2026; Description: "Revit 2026 add-in"; Types: full custom
#endif

[Files]
; Always-installed shared files (just the uninstaller icon for now).
Source: "nullcarbon.ico"; DestDir: "{app}"; Flags: ignoreversion; Components: main

#if R2023 == "Enabled"
    Source: "..\..\src\bin\Release2023\nullCarbon-LCA-Export-2023.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2023\"; DestName: "nullCarbon-LCA-Export.addin"; Flags: ignoreversion; Components: revit2023
    Source: "..\..\src\bin\Release2023\*.dll"; Excludes: "Revit*.dll,AdWindows.dll,UIFramework.dll,RevitAPI*.dll"; DestDir: "{app}\2023\"; Flags: ignoreversion; Components: revit2023
    Source: "..\..\src\bin\Release2023\Monaco\*"; DestDir: "{app}\2023\Monaco\"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist; Components: revit2023
#endif

#if R2024 == "Enabled"
    Source: "..\..\src\bin\Release2024\nullCarbon-LCA-Export-2024.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2024\"; DestName: "nullCarbon-LCA-Export.addin"; Flags: ignoreversion; Components: revit2024
    Source: "..\..\src\bin\Release2024\*.dll"; Excludes: "Revit*.dll,AdWindows.dll,UIFramework.dll,RevitAPI*.dll"; DestDir: "{app}\2024\"; Flags: ignoreversion; Components: revit2024
    Source: "..\..\src\bin\Release2024\Monaco\*"; DestDir: "{app}\2024\Monaco\"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist; Components: revit2024
#endif

#if R2025 == "Enabled"
    Source: "..\..\src\bin\Release2025\nullCarbon-LCA-Export-2025.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2025\"; DestName: "nullCarbon-LCA-Export.addin"; Flags: ignoreversion; Components: revit2025
    Source: "..\..\src\bin\Release2025\*.dll"; Excludes: "Revit*.dll,AdWindows.dll,UIFramework.dll,RevitAPI*.dll"; DestDir: "{app}\2025\"; Flags: ignoreversion; Components: revit2025
    Source: "..\..\src\bin\Release2025\Monaco\*"; DestDir: "{app}\2025\Monaco\"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist; Components: revit2025
#endif

#if R2026 == "Enabled"
    Source: "..\..\src\bin\Release2026\nullCarbon-LCA-Export-2026.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\2026\"; DestName: "nullCarbon-LCA-Export.addin"; Flags: ignoreversion; Components: revit2026
    Source: "..\..\src\bin\Release2026\*.dll"; Excludes: "Revit*.dll,AdWindows.dll,UIFramework.dll,RevitAPI*.dll"; DestDir: "{app}\2026\"; Flags: ignoreversion; Components: revit2026
    Source: "..\..\src\bin\Release2026\Monaco\*"; DestDir: "{app}\2026\Monaco\"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist; Components: revit2026
#endif
; NOTE: Don't use "Flags: ignoreversion" on any shared system files.

[Icons]
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppDisplayName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppDisplayName}}"; Filename: "{uninstallexe}"
