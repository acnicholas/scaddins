#SCaddins Revision Log#

#19.0#

####NEW####

- **Revit 2019 support**

####SUMMARY####

First release for Revit 2019
**Revit 2016-2019 Supported.

#18.1#

####NEW####

- **New rename tool**
- New Open sheet dialog (command only)
- Upgrade reminder form updated.
- Option to select Room Department of Design Option added to Room Tools.
- Unused view filter added to destructive purge.
- Offset crop region option added to room tools when creating sheets.
- Open newly created user views.


#18.0#

####SUMMARY####

**Revit 2014-2015 Support dropped. Further releases(v18*) will support the three latest Revit versions (2016,2017 & 2018).**
Updated host page to [github](https://github.com/acnicholas/scaddins) 

####FIXES#####

- Better tree selection in Destructive Purge.
- Better room solid creation in Room Tools.
- Check for existing view names when creating user views.
- Check for invalid view names when creating user views.
- Don't set scale bar value in SCexport if parameter is not found.

####NEW####

- Updated host page to [github](https://github.com/acnicholas/scaddins) 
- Remove views option added to *Copy Sheets*.
- Upgrade reminder form updated,

####CHANGES####

- Unjoin wall tool *removed*
- Window Manger removed.

#17.0.1#

####FIXES#####

- Wall unjoiner defualt setting set to OFF.
- Fixed solar view diloag error.
- Fixed room filters in Room Tools

#17.0#

####NEW####

- **Revit 2017 support**
- **Room Tools added**
- **Unjoin Walls added**
- **Create Perspective added**
- [SCloudSched] Assign revisions to clouds.
- [SCopy] Rename sheets option added.
- [SCexport] Progress bar added when printing files.
- [SCexport] North Point column added to GUI.

####FIXES#####

- **[SCopy] Do not copy revisions.**
- [SCopy] Fixed error when creating multiple sheet copies from the SCexport context menu.
- [SCopy] Fixed exception when clicking SCopy column headers.
- **[SCexport] Option added to remove revisions when renaming sheets.**
- [SCexport] Fixed view set drop-down error.

#16.2#

####SUMMARY####

**Configuration file format changed**. The old format will now only work with versions <= 16.1.

For details see the ; [**Wiki Page**](https://bitbucket.org/anicholas/scaddins/wiki/SCexport)


####NEW####

- **[SCexport] Configuration file format changed.**
- **[SCexport] Post export hooks added.**
- [SCopy] Open newly created sheet (the first sheet will be opened).
- **[SCaos] Create plan views (multi-mode).**
- **[SCam] Create plan perspective views.**

####FIXES#####

- [SCopy] Better view placement.
- [SCexport] Portrait printing now works with Postscript(Ghostscript) option.
- [SCexport] removed unused/optional binaries from installation package.

#16.1#

####NEW####

- **[SCexport] Large format printer support**
- **[SCincrement] Custom paramter support**

####FIXES#####

- [SCaos] Do not lock user views. 
- [SCexport] Postscript pdf printing "seems to work".

#16.0#

####SUMMARY####

**Initial release for Revit 2016**

**This release(16*) will support Revit 2014,2016 & 2016.**

####NEW####

- **Revit 2016 support**
- [SCexport] Export summary dialog added when errors occur
- [SCaos] Lock solar access views.
- [SCaos] Select export date.

####FIXES#####

- [SCopy] Better / more accurate viewport copying. 
- [SCopy] Disabled detail line copying (CURRENTLY BROKEN)
- [SCopy] Fixed typo on main form. 
- [SCopy] Fixed error when running from SCexport context menu
- [SCwash] Properly show on-sheet vs not-on-sheet views.
- [SCaos] Don't attempt to rotate locked views.

#15.3#

####NEW####

- [SCwash] Revision node added so you can purge revisions.
- [SCopy] "Copy" Legends.
- [SCopy] Copy detail items.

####FIXES#####

- [SCopy] Fixed crash when no Sheet Category parameter is found.

#15.2#

####NEW####

- **[SCaos] Create winter views options added, including start time, end time, and interval.**

#15.1#

####NEW####

- [SCaddins]Check for updates on start-up.
- [SCaddins]Improved update dialog box, with option to view change log.
- **[SCexport]SCopy option for multiple sheets added to the context menu.**
- [SCopy]Copy button added to enable multiple copies of sheets. 
- [SCopy]Sheet Category field added for new sheets. 

####FIXES#####

- [SCaddins] better tooltip help added to ribbon.
- [SCexport] If destination file exists -> check if it's open/available before attempting an export.
- [SCexport] removed non-print line-type feature. (causing problems with permissions)
- [SCexport] correctly hide title blocks in when exporting from Revit 2015.
- [SCexport] check for illegal file names and replace invalid chars with a '_'
- **[SCexport] correctly assign and save AutoCAD export version.**

#15.0#

####SUMMARY####

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
	
####NEW####

 - [SCexport]Fix scale bars in selected sheets
 - [SCexport]Remove underlays from view on selected sheets
 - [SCexport]turn off dwg titleblocks by default

####FIXES####

 - [SCexport]Rename Sheets option
