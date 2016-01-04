#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
; #Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.

WinGetActiveTitle, Title
WinRestore, %Title%
SysGet, MonitorPrimary, MonitorPrimary
SysGet, MainWindow, MonitorWorkArea, MonitorPrimary
SysGet, X1, 76
SysGet, Y1, 77
SysGet, Width, 78
;SysGet, Height, 1
WinMove, %Title%,, X1, Y1, Width, MainWindowBottom
Exit