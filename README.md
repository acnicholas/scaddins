![SCexport logo](https://bitbucket.org/anicholas/scaddins/raw/master/share/icons/scaddins-wix.png)

# SCaddins #

SCaddins is a collection of Revit add-ins. Currently Revit versions 2015-2017 are supported.  

Have a look at the [**wiki**](https://github.com/anderob/scaddins.wiki/Home) for more information on each utility.


## Building and Installing ##

Run the powershell script `build.ps1` to build the project.

SCaddins attempt to build for Revit versions 2015-2017 by looking for RevitAPI.dll and RevitAPIUI.dll in the following standard locations:

 - C:\Program Files\AutoDesk\Revit 2015
 - C:\Program Files\AutoDesk\Revit 2016
 - C:\Program Files\AutoDesk\Revit 2017
 
To build the msi installer run:

`build.ps1 --Target=dist`
