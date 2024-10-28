# SCaddins Revision Log #

# 25.0.1 #

#### FIXES ####

- Fix error in SCexport where existing files are overridden without prompting the user.
- Remove some debugging message boxes.

# 25.0.0 #

#### NEW ####

- Initial Revit Build 2025.
- Change installer backend.
- Add dark mode icons
- Add lots of examples to Run Script tool.
- Remove Revit 2018 and 2019 support.
- Add hatch from template option to Hatch Editor.
- Add option to increment viewport detail number [Increment Tool]
- New Grid Manager tool.
- New Installer
- Use Monaco(vscode) editor for Run Script.
- Use WPFUI for interface

# 24.1.1 #

#### FIXES ####

- Fix force raster print option in SCexport.
- Update WIX
- Clear rename candidates in rename tool when catergory is changed.

# 24.1.0 #

#### NEW ####

- View filters added to Rename.
- View Templates added to Rename.
- Highlight invlalid scalebars in SCexport

#### FIXES ####

- Allow empty parameters in SCexport export names(don't crash).
- Fixed view set creation and loading [SCexport] in Revit 2024
- Sort parameters in Rename.
= Sort hatches in Hatch Editor.
- Don't enable post export hook if script is not found.

# 24.0.0 #

#### NEW ####

- Initial Revit Build 2024.
- Add PDF24 support
- Filter "Appears On Schedule" column in SCexport.
- Add post export hook settings to Revit project information.

# 23.0.1 #

#### FIXES ####

- Fix error opening SCexport (due to non-valid printer settings)

# 23.0.0 #

#### NEW ####

- Initial Revit Build 2023.
- Add third (tertiary] custom sheet parameter to SheetCopier.
- Don't enable printing if printer is not found. [SCexport]
- Give warning message if user decides to disable sheet verification [SCeport]
- Change RunScript to use Lua(NLua)
- Change RunScript to use FastColoredTextBox.

#### FIXES ####

- Fix crash when associating a new level to a view. [SheetCopier].
- Fix view placement when associating a new level to a view. [SheetCopier].
- Fix incorrect(broken) view naming when assigning a new name to views with an associated level change [SheetCopier].
- Properly copy view templates between views when option is selected [ SheetCopier].

# 22.1.0 #

#### NEW ####

- Add custom sheet parameter column to SCexport GUI.
- Add option to bulk assign a value to any Yes/No instance parameter on a sheet (SCexport).
- Add customisable column to Scexport GUI.
- Add better lable to SCexport export and print buttons.
- Fix azimuth display in SolarAnalysis and add additional DMS display.
- Add option to draw solar ray with SolarAnalysis.

#### FIXES ####

- Remove overlapping text in Copy Sheets (settings dialog).

# 22.0.0 #

- Initial Revit Build 2022.
- Ghostscript PDF export removed
- Native Revit PDF Export enabled for Revit 2022.
- Add Areas to Rename Tool.
- Add Areas to Increment Tool.
- Remove Revit 2016-1017 support.
- Add align tool to SCexport.
- Add help icon to SCexport context menu.
- Add help icon to Sheet Copier.
- Add option not to delete revision clouds to Sheet Copier.
- Add Separate PDF and DWG naming rule options to config file.
- Add "element Information" tool.
- Add option to bulk assign a value to any Yes/No instance parameter on a title-block family (SCexport).

# 21.4.2 #

#### FIXES ####

- Fix "View Category" column in Sheet Copier. (replaced with a parameter that can be defined)

# 21.4.1 #

#### FIXES ####

- Option added to pad leading zeros in Increment Tool
- Fix crash if print setting cannot be assigned in SCexport.

# 21.4 #

#### NEW ####

- Add "smart" context filter to SCexport.
- Add option to assign global invalid characters to files exported with SCexport.
- Change default naming scheme in SCexport to include sheet number and name.

# 21.3.1 #

#### FIXES ####

- Allow incrementing of walls in Increment Tool

# 21.3 #

#### NEW ####

- Create user views by pre-selecting in Project Browser.
- Display and edit "View in Sheet List" parameter from SCexport.
- Add Next Sheet; Previous Sheet and Open Sheet to Revit ribbon.

# 21.2 #

#### NEW ####

- **Export Schedules tool added.**

# 21.1 #

#### NEW ####

- Additional option to create RCP's with Room Tools.
- Rename familes added to Rename Tool
- Rename groups added to Rename Tool
- Save previous(5) [SC]Exports for easier reloading.
- Option to add the Project Browser selection to Sheet Copier
- **Copy independent views using Sheet Copier.**
- Add help menu to Hatch Editor

#### FIXES ####

- Do not open Solar Analysis tools in a family environment.
- Don't check for view filters in Destructive Purge when in a family environment.
- Fix cancel button in ModelSetupWizard.

# 21.0 #

#### NEW ####

- **Revit 2021 support**
- Pin sheet contents.
- Customise user view names

#### SUMMARY ####

First release for Revit 2021
**Revit 2016-2021 Supported.**

#### FIXES #####

 - Don't create "sun-eye" views at night.
 - Reload SCexport config changes without needing to restart.
 - Fixed error where the first print would sometimes fail in SCexport.
 - Deselect all elements before exporting so they don't appear incorrectly (SCexport)

# 20.0 #

#### NEW ####

- **Revit 2020 support**

#### SUMMARY ####

First release for Revit 2020
**Revit 2016-2020 Supported.**

#### NEW ####

- New hatch editor tool.
- New model setup wizard
- New cs-script runner (BETA)
- New spell checker (BETA)

# 19.1 #

#### NEW ####

- **Completely new Interface using WPF, *many* minor changes**
- Option to force raster prints in SCexport.
- New progress dialog for SCexport that enables cancelling.
- New search field in SCexport
- Custom parameters added to SCexport (can now use any parameter for export naming)
- New solar analysis view creation tool.
- Option to bulk select sheets to copy in Sheet Copier.

#### SUMMARY ####

**Completely** revised interface using Windows Presentation Framework.
Most add-ins include minor improvements/changes.

v

# 19.0 #

#### NEW ####

- **Revit 2019 support**

#### SUMMARY ####

First release for Revit 2019
**Revit 2016-2019 Supported.**

# 18.1 #

#### NEW ####

- **New rename tool**
- New Open sheet dialog (command only)
- Upgrade reminder form updated.
- Option to select Room Department of Design Option added to Room Tools.
- Unused view filter added to destructive purge.
- Offset crop region option added to room tools when creating sheets.
- Open newly created user views.


# 18.0 #

#### SUMMARY ####

**Revit 2014-2015 Support dropped. Further releases(v18+) will support the three latest Revit versions (2016,2017 & 2018).**
Updated host page to [github](https://github.com/acnicholas/scaddins) 

#### FIXES #####

- Better tree selection in Destructive Purge.
- Better room solid creation in Room Tools.
- Check for existing view names when creating user views.
- Check for invalid view names when creating user views.
- Don't set scale bar value in SCexport if parameter is not found.

#### NEW ####

- Updated host page to [github](https://github.com/acnicholas/scaddins) 
- Remove views option added to *Copy Sheets*.
- Upgrade reminder form updated,

#### CHANGES ####

- Unjoin wall tool *removed*
- Window Manger removed.

# 17.0.1 #

#### FIXES #####

- Wall unjoiner defualt setting set to OFF.
- Fixed solar view diloag error.
- Fixed room filters in Room Tools

# 17.0 #

#### NEW ####

- **Revit 2017 support**
- **Room Tools added**
- **Unjoin Walls added**
- **Create Perspective added**
- [SCloudSched] Assign revisions to clouds.
- [SCopy] Rename sheets option added.
- [SCexport] Progress bar added when printing files.
- [SCexport] North Point column added to GUI.

#### FIXES #####

- **[SCopy] Do not copy revisions.**
- [SCopy] Fixed error when creating multiple sheet copies from the SCexport context menu.
- [SCopy] Fixed exception when clicking SCopy column headers.
- **[SCexport] Option added to remove revisions when renaming sheets.**
- [SCexport] Fixed view set drop-down error.

# 16.2 #

#### SUMMARY ####

**Configuration file format changed**. The old format will now only work with versions <= 16.1.

For details see the ; [**Wiki Page**](https://bitbucket.org/anicholas/scaddins/wiki/SCexport)


#### NEW ####

- **[SCexport] Configuration file format changed.**
- **[SCexport] Post export hooks added.**
- [SCopy] Open newly created sheet (the first sheet will be opened).
- **[SCaos] Create plan views (multi-mode).**
- **[SCam] Create plan perspective views.**

#### FIXES #####

- [SCopy] Better view placement.
- [SCexport] Portrait printing now works with Postscript(Ghostscript) option.
- [SCexport] removed unused/optional binaries from installation package.

# 16.1 #

#### NEW ####

- **[SCexport] Large format printer support**
- **[SCincrement] Custom paramter support**

#### FIXES #####

- [SCaos] Do not lock user views. 
- [SCexport] Postscript pdf printing "seems to work".

# 16.0 #

#### SUMMARY ####

**Initial release for Revit 2016**

**This release(16+) will support Revit 2014,2016 & 2016.**

####NEW####

- **Revit 2016 support**
- [SCexport] Export summary dialog added when errors occur
- [SCaos] Lock solar access views.
- [SCaos] Select export date.

#### FIXES #####

- [SCopy] Better / more accurate viewport copying. 
- [SCopy] Disabled detail line copying (CURRENTLY BROKEN)
- [SCopy] Fixed typo on main form. 
- [SCopy] Fixed error when running from SCexport context menu
- [SCwash] Properly show on-sheet vs not-on-sheet views.
- [SCaos] Don't attempt to rotate locked views.

# 15.3 #

#### NEW ####

- [SCwash] Revision node added so you can purge revisions.
- [SCopy] "Copy" Legends.
- [SCopy] Copy detail items.

#### FIXES #####

- [SCopy] Fixed crash when no Sheet Category parameter is found.

# 15.2 #

#### NEW ####

- **[SCaos] Create winter views options added, including start time, end time, and interval.**

# 15.1 #

#### NEW ####

- [SCaddins]Check for updates on start-up.
- [SCaddins]Improved update dialog box, with option to view change log.
- **[SCexport]SCopy option for multiple sheets added to the context menu.**
- [SCopy]Copy button added to enable multiple copies of sheets. 
- [SCopy]Sheet Category field added for new sheets. 

#### FIXES #####

- [SCaddins] better tooltip help added to ribbon.
- [SCexport] If destination file exists -> check if it's open/available before attempting an export.
- [SCexport] removed non-print line-type feature. (causing problems with permissions)
- [SCexport] correctly hide title blocks in when exporting from Revit 2015.
- [SCexport] check for illegal file names and replace invalid chars with a '_'
- **[SCexport] correctly assign and save AutoCAD export version.**

# 15.0 #

#### SUMMARY ####

**Initial release**

Previously released and new add-ins used to create SCaddins.

- SCexport
- SCaos
- SCulcase
- SCightlines
- Scoord
- SCwash
- SCincrement
- SCloudSched
- SCopy
- SCuv
	
#### NEW ####

 - [SCexport]Fix scale bars in selected sheets
 - [SCexport]Remove underlays from view on selected sheets
 - [SCexport]turn off dwg titleblocks by default

#### FIXES ####

 - [SCexport]Rename Sheets option
