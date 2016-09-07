;call with the following paramters
; %1%	-	View Name
; %2%	-	X position from top left
; %3%	-	Y position from top left
; %4%	-	Width
; %5%	-	Height

#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
;#Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.

ProjectBrowser := "Project Browser"
SearchDialog := "Search in Project Browser"

SetTitleMatchMode, 2
WinActivate, %ProjectBrowser%
MouseMove, x, y, 0 
ControlGet, ctrlH, HWND,, %ProjectBrowser%, Revit
ControlGetPos, X, Y,,,ahk_id %ctrlH%
X += 50
Y += 50
MouseClick,,X,Y,1
Send ^f
Send Section 4
Send !c
ExitApp


