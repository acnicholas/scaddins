using Cake.Common.Diagnostics;

// ARGUMENTS

var target = Argument("target", "Default");
var solutionFile = GetFiles("./*.sln").First();
var solutionFileWix = GetFiles("installer/SCaddins.Installer.wixproj").First();

// PREPARATION

// Define directories.
var buildDir = Directory("./bin/Release");

// TASKS

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore(solutionFile);
});

powershell -Command "(gc SCaddins.addin) -replace '_REVIT_VERSION_', '2015' | Out-File bin\Release\SCaddins2015.addin" || goto :error
powershell -Command "(gc SCaddins.addin) -replace '_REVIT_VERSION_', '2016' | Out-File bin\Release\SCaddins2016.addin" || goto :error
powershell -Command "(gc SCaddins.addin) -replace '_REVIT_VERSION_', '2017' | Out-File bin\Release\SCaddins2017.addin" || goto :error

Task("CreateAddinManifests")
    .Does(() =>
{
    string text = File.ReadAllText("SCaddins.addin");
    File.WriteAllText("bin\Release\SCaddins2015.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2015"));
    File.WriteAllText("bin\Release\SCaddins2016.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2016"));
    File.WriteAllText("bin\Release\SCaddins2017.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2017"));
});


Task("Revit2015")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(FileExists(@"C:\Program Files\Autodesk\Revit 2015\RevitAPI.dll"))
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      MSBuild(solutionFile, settings => settings
			.SetConfiguration("Release2015")
			.WithTarget("Clean,Build")
            .WithProperty("Platform","x64")
            .SetVerbosity(Verbosity.Normal));
    } 
});

Task("Revit2016")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(FileExists(@"C:\Program Files\Autodesk\Revit 2016\RevitAPI.dll"))
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      MSBuild(solutionFile, settings => settings
			.SetConfiguration("Release2016")
			.WithTarget("Clean,Build")
            .WithProperty("Platform","x64")
            .SetVerbosity(Verbosity.Normal));
    } 
});

Task("Installer")
    .Does(() =>
{
      var settings = new MSBuildSettings();
      settings.SetConfiguration("Release");
	  settings.WithTarget("Build");
      //settings.WithProperty("R2015","Disabled");
      //settings.WithProperty("R2016","Enabled");
      //settings.WithProperty("R2017","Disabled");
      settings.WorkingDirectory = new DirectoryPath(Environment.CurrentDirectory + @"\installer");
      MSBuild(solutionFileWix, settings);  
});


Task("Revit2017")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(FileExists(@"C:\Program Files\Autodesk\Revit 2017\RevitAPI.dll"))
    .Does(() =>
{
    if(IsRunningOnWindows()){
      MSBuild(solutionFile, settings => settings
			.SetConfiguration("Release2017")
			.WithTarget("Clean,Build")
            .WithProperty("Platform","x64")
            .SetVerbosity(Verbosity.Normal));
    } 
});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Revit2015")
    .IsDependentOn("Revit2016")
    .IsDependentOn("Revit2017");
//Task("Default");

RunTarget(target);
