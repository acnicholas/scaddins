![SCexport logo](https://bitbucket.org/anicholas/scaddins/raw/master/share/icons/scaddins-wix.png)

# SCaddins #

SCaddins is a collection of Revit add-ins. Currently Revit versions 2020-2024 are supported.  

Have a look at the [**wiki**](https://github.com/acnicholas/scaddins/wiki/Home) for more information on each utility.

### Latest Version ###

Download the latest version from [**here**](https://github.com/acnicholas/scaddins/releases/latest)

### Build from Source ###

Run the `build.bat` to build the project and installer.

SCaddins will attempt to build for availble Revit versions(2020-2024), by looking for RevitAPI[UI].dll in the following standard locations:

 - C:\Program Files\AutoDesk\Revit 2020
 - C:\Program Files\AutoDesk\Revit 2021
 - C:\Program Files\AutoDesk\Revit 2022
 - C:\Program Files\AutoDesk\Revit 2023
 - C:\Program Files\AutoDesk\Revit 2024

 
To build the installer run:

`build_installer.bat`
