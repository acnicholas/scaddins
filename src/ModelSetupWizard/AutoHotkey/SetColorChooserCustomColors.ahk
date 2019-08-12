; F8 - Set Custom Colours in Revit "Colors" Dialog
; F9 - Append colour under cursor to file 

; Set Revit custom colours to colours in a file
; Max 16 colours
F8::
  FileSelectFile, colorFile,, C:\Code\cs\scaddins\src\ModelSetupWizard\AutoHotkey, Select colour scheme, *.col
  SetControlDelay -1
  ControlClick,x32 y228 , WinTitle,,,2, NA 
  ;ControlClick, x32 y228, Color
  ;ControlClick, x32 y228, A
  Loop, read, %colorFile%
  {
      ; Red = % SubStr(A_LoopReadLine,3,2)
      ; Green = % SubStr(A_LoopReadLine,5,2)
      ; Blue = % SubStr(A_LoopReadLine,7,2)
      ; MsgBox % Red
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
    Red = % SubStr(A_LoopReadLine,3,2)
    Green = % SubStr(A_LoopReadLine,5,2)
    Blue = % SubStr(A_LoopReadLine,7,2)
    Blue = % HexToDec("0x" Blue)
    Green = % HexToDec("0x" Green)
    Red = % HexToDec("0x" Red)
    ; MsgBox % Red
    ControlSetText ,Edit4, %Blue%, A
    ControlSetText ,Edit5, %Green%, A
    ControlSetText ,Edit6, %Red%, A
    ;Sleep (500)
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