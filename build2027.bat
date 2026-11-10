powershell -File build.ps1 --Target "Clean"
powershell -File build.ps1 --Target "Restore-NuGet-Packages"
powershell -File build.ps1 --Target "Revit2027"
powershell -File build.ps1 --Target "Installer"
pause

