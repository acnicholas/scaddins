![SCexport logo](https://bitbucket.org/anicholas/scaddins/raw/master/share/icons/scaddins-wix.png)

# SCaddins #

SCaddins is a collection of Revit add-ins. Currently Revit versions 2016-2019 are supported.  

Have a look at the [**wiki**](https://github.com/acnicholas/scaddins/wiki/Home) for more information on each utility.

### Latest Version ###

Download the latest version from [**here**](https://github.com/acnicholas/scaddins/releases/latest)

### Build from Source ###

Run the powershell script `build.ps1` to build the project.

SCaddins will attempt to build for availble Revit versions (2016-2018) by looking for RevitAPI[UI].dll in the following standard locations:

 - C:\Program Files\AutoDesk\Revit 2016
 - C:\Program Files\AutoDesk\Revit 2017
 - C:\Program Files\AutoDesk\Revit 2018
 - C:\Program Files\AutoDesk\Revit 2019
 
To build the msi installer run:

`build.ps1 -Target "Dist"`
