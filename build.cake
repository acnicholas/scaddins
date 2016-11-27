using Cake.Common.Diagnostics;
using System.IO;

var target = Argument("target", "Default");
var solutionFile = GetFiles("./*.sln").First();
var solutionFileWix = GetFiles("installer/SCaddins.Installer.wixproj").First();
var buildDir = Directory(@"./bin/Release");

// METHODS

public MSBuildSettings GetBuildSettings(string config)
{
    return new MSBuildSettings()
			.SetConfiguration(config)
			.WithTarget("Clean,Build")
            .WithProperty("Platform","x64")
            .SetVerbosity(Verbosity.Normal);
}

public bool APIAvailable(string revitVersion)
{
    return FileExists(@"C:\Program Files\Autodesk\Revit " + revitVersion + @"\RevitAPI.dll");
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
    .WithCriteria(APIAvailable("2015"))
    .Does(() => MSBuild(solutionFile, GetBuildSettings("Release2015")));

Task("Revit2016")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(APIAvailable("2016"))
    .Does(() => MSBuild(solutionFile, GetBuildSettings("Release2016")));

Task("Revit2017")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(APIAvailable("2017"))
    .Does(() => MSBuild(solutionFile, GetBuildSettings("Release2017")));

Task("Dist")
    .IsDependentOn("Default")
    .Does(() =>
{
      Environment.SetEnvironmentVariable("R2015", APIAvailable("2015") ? "Enabled" : "Disabled");
	  Environment.SetEnvironmentVariable("R2016", APIAvailable("2016") ? "Enabled" : "Disabled");
	  Environment.SetEnvironmentVariable("R2017", APIAvailable("2017") ? "Enabled" : "Disabled");
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
