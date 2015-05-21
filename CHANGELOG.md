#15.1(pre-release)#

####NEW####

- [SCaddins]Check for updates on start-up.
- [SCaddins]Improved update dialog box, with option to view change log.
- **[SCexport]SCopy option for multiple sheets added to the context menu.**
- [SCopy]Copy button added to enable multiple copies of sheets. 

####FIXES#####

- [SCaddins] better tooltip help added to ribbon.
- [SCexport] If destination file exists -> check if it's open/available before attempting an export.
- [SCexport] removed non-print line-type feature. (causing problems with permissions)
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
