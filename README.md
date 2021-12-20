![SCexport logo](https://bitbucket.org/anicholas/scaddins/raw/master/share/icons/scaddins-wix.png)

# SCaddins #

SCaddins is a collection of Revit add-ins. Currently Revit versions 2016-2022 are supported.  

Have a look at the [**wiki**](https://github.com/acnicholas/scaddins/wiki/Home) for more information on each utility.

### Latest Version ###

Download the latest version from [**here**](https://github.com/acnicholas/scaddins/releases/latest)

### Build from Source ###

Run the powershell script `build.ps1` to build the project.

SCaddins will attempt to build for availble Revit versions(2018-2022), by looking for RevitAPI[UI].dll in the following standard locations:

 - C:\Program Files\AutoDesk\Revit 2018
 - C:\Program Files\AutoDesk\Revit 2019
 - C:\Program Files\AutoDesk\Revit 2020
 - C:\Program Files\AutoDesk\Revit 2021
 - C:\Program Files\AutoDesk\Revit 2022

 
To build the msi installer run:

`build.bat`

or

`build.ps1 -Target "Dist"`
