; F7 - Set Custom Colours in Windows "Edit Colours" Dialog
; F8 - Set Custom Colours in Revit "Colors" Dialog
; F9 - Append colour under cursor to file 
; F12- Set RGB values in revit/windows colour chooser to the colour under the cursor

; Set window's "Edit Colours" dilaog's custom colours to match the colours in a file
; Max 16 colours
F7::
  FileSelectFile, colorFile,, X:\AndrewN\Revit\AutoHotkey Scripts, Select colour scheme, *.col
  SetControlDelay -1
  ControlClick, x27 y232, A
  Loop, read, %colorFile%
  {
      SetColourByHex(A_LoopReadLine)
      ControlClick, Button6, A
  }
  return

; Set Revit custom colours to colours in a file
; Max 16 colours
F8::
  FileSelectFile, colorFile,, C:\Code\cs\scaddins\src\ModelSetupWizard\AutoHotkey, Select colour scheme, *.col
  SetControlDelay -1
  ControlClick, x32 y228, A
  Loop, read, %colorFile%
  {
      SetColourByHex(A_LoopReadLine)
      ControlClick, Button2, A
  }
  return

; Append colour under cursor to file
F9::
    ;partially from https://autohotkey.com/board/topic/64390-getpixelcolor-how-to-separate-rgb-values/ 
    MouseGetPos, MouseX, MouseY 
    ; PixelGetColor, color, %MouseX%, %MouseY%;color under the mouse
    PixelGetColor, color, %MouseX%, %MouseY%, RGB ;color under the mouse
    AppendColourToFile(color)
    return
        
SetColourByHex(color)
{
    Blue = HexToDec(0x + SubStr(color,3,2))
    Green = HexToDec(0x + SubStr(color,5,2))
    Red = HexToDec(0x + SubStr(color,7,2))
    ControlSetText ,Edit4, %Red%, A
    ControlSetText ,Edit5, %Green%, A
    ControlSetText ,Edit6, %Blue%, A
}

;https://www.autohotkey.com/boards/viewtopic.php?t=6434
HexToDec(hex)
{
    VarSetCapacity(dec, 66, 0)
    , val := DllCall("msvcrt.dll\_wcstoui64", "Str", hex, "UInt", 0, "UInt", 16, "CDECL Int64")
    , DllCall("msvcrt.dll\_i64tow", "Int64", val, "Str", dec, "UInt", 10, "CDECL")
    return dec
}

AppendColourToFile(color)
{
    FileAppend, %color%`n, ColourDump.txt
}