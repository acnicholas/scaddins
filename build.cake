using Cake.Common.Diagnostics;
using System.IO;

// ARGUMENTS

var target = Argument("target", "Default");
var solutionFile = GetFiles("./*.sln").First();
var solutionFileWix = GetFiles("installer/SCaddins.Installer.wixproj").First();

// PREPARATION

// Define directories.
var buildDir = Directory("./bin/Release");

// METHODS

public MSBuildSettings GetBuildSettings(string config)
{
    return new MSBuildSettings()
			.SetConfiguration(config)
			.WithTarget("Clean,Build")
            .WithProperty("Platform","x64")
            .SetVerbosity(Verbosity.Normal);
}

// TASKS

Task("Clean").Does(() => CleanDirectory(buildDir));

Task("Restore-NuGet-Packages").Does(() => NuGetRestore(solutionFile));

Task("CreateAddinManifests")
    .Does(() =>
{
    string text = System.IO.File.ReadAllText("SCaddins.addin");
    System.IO.File.WriteAllText(@"bin\Release\SCaddins2015.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2015"));
    System.IO.File.WriteAllText(@"bin\Release\SCaddins2016.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2016"));
    System.IO.File.WriteAllText(@"bin\Release\SCaddins2017.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2017"));
});


Task("Revit2015")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(FileExists(@"C:\Program Files\Autodesk\Revit 2015\RevitAPI.dll"))
    .Does(() => MSBuild(solutionFile, GetBuildSettings("Release2015")));

Task("Revit2016")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(FileExists(@"C:\Program Files\Autodesk\Revit 2016\RevitAPI.dll"))
    .Does(() => MSBuild(solutionFile, GetBuildSettings("Release2016")));

Task("Revit2017")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(FileExists(@"C:\Program Files\Autodesk\Revit 2017\RevitAPI.dll"))
    .Does(() => MSBuild(solutionFile, GetBuildSettings("Release2017")));

Task("Dist")
    .IsDependentOn("Default")
    .Does(() =>
{
      Environment.SetEnvironmentVariable("R2015", FileExists(@"C:\Program Files\Autodesk\Revit 2015\RevitAPI.dll") ? "Enabled" : "Disabled");
	  Environment.SetEnvironmentVariable("R2016", FileExists(@"C:\Program Files\Autodesk\Revit 2016\RevitAPI.dll") ? "Enabled" : "Disabled");
	  Environment.SetEnvironmentVariable("R2017", FileExists(@"C:\Program Files\Autodesk\Revit 2017\RevitAPI.dll") ? "Enabled" : "Disabled");
      var settings = new MSBuildSettings();
      settings.SetConfiguration("Release");
	  settings.WithTarget("Clean,Build");
      settings.WorkingDirectory = new DirectoryPath(Environment.CurrentDirectory + @"\installer");
      MSBuild(solutionFileWix, settings);  
});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Revit2015")
    .IsDependentOn("Revit2016")
    .IsDependentOn("Revit2017")
	.IsDependentOn("CreateAddinManifests");

RunTarget(target);
