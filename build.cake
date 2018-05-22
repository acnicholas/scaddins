using Cake.Common.Diagnostics;
using System.IO;

var target = Argument("target", "Default");
var solutionFile = GetFiles("src/*.sln").First();
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

Task("Restore-Installer-NuGet-Packages").Does(() => 
{
    var settings = new NuGetRestoreSettings();
    settings.PackagesDirectory = @"installer/packages";
    settings.WorkingDirectory = @"installer";
    NuGetRestore(solutionFileWix, settings);
});

Task("CreateAddinManifests")
    .Does(() =>
{
    string text = System.IO.File.ReadAllText(@"src\SCaddins.addin");
    System.IO.File.WriteAllText(@"src\bin\Release2016\SCaddins2016.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2016"));
    System.IO.File.WriteAllText(@"src\bin\Release2017\SCaddins2017.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2017"));
    System.IO.File.WriteAllText(@"src\bin\Release2018\SCaddins2018.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2018"));
	System.IO.File.WriteAllText(@"src\bin\Release2019\SCaddins2019.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2019"));
});

Task("Revit2016")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(APIAvailable("2016"))
    .Does(() => MSBuild(solutionFile, GetBuildSettings("Release2016")));

Task("Revit2017")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(APIAvailable("2017"))
    .Does(() => MSBuild(solutionFile, GetBuildSettings("Release2017")));
    
Task("Revit2018")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(APIAvailable("2018"))
    .Does(() => MSBuild(solutionFile, GetBuildSettings("Release2018")));
	
Task("Revit2019")
    .IsDependentOn("Restore-NuGet-Packages")
    .WithCriteria(APIAvailable("2019"))
    .Does(() => MSBuild(solutionFile, GetBuildSettings("Release2019")));

Task("Installer")
    .IsDependentOn("Restore-Installer-NuGet-Packages")
    .Does(() =>
{
	  Environment.SetEnvironmentVariable("R2016", APIAvailable("2016") ? "Enabled" : "Disabled");
	  Environment.SetEnvironmentVariable("R2017", APIAvailable("2017") ? "Enabled" : "Disabled");
      Environment.SetEnvironmentVariable("R2018", APIAvailable("2018") ? "Enabled" : "Disabled");
	  Environment.SetEnvironmentVariable("R2019", APIAvailable("2019") ? "Enabled" : "Disabled");
      var settings = new MSBuildSettings();
      settings.SetConfiguration("Release");
	  settings.WithTarget("Clean,Build");
      settings.WorkingDirectory = new DirectoryPath(Environment.CurrentDirectory + @"\installer");
      MSBuild(solutionFileWix, settings);  
});

Task("Dist")
    .IsDependentOn("Default")
    .IsDependentOn("Installer");

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Revit2016")
    .IsDependentOn("Revit2017")
    .IsDependentOn("Revit2018")
    .IsDependentOn("Revit2019")
    .IsDependentOn("CreateAddinManifests");

RunTarget(target);
