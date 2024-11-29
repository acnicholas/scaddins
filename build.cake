#tool nuget:?package=NuGet.CommandLine&version=6.12.1
#tool nuget:?package=Tools.InnoSetup&version=6.2.2

using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

var target = Argument("target", "Default");
var solutionFile = GetFiles("src/*.sln").First();
var testSolutionFile = GetFiles("tests/*.sln").First();
var innoSetupFile = GetFiles("setup/*.iss").First();
var buildDir = Directory(@"./src/bin");
var objDir = Directory(@"./src/obj");

// METHODS

public MSBuildSettings GetBuildSettings(string config)
{
	var result = new MSBuildSettings()
		.SetConfiguration("Release" + config)
		.WithTarget("Clean,Build")
		.WithProperty("Platform","Any CPU")
		.SetVerbosity(Verbosity.Minimal);
	result.WarningsAsError = false;
	result.Restore = true;
	return result;
}

public MSBuildSettings GetTestBuildSettings()
{
	var result = new MSBuildSettings()
		.SetConfiguration("Debug")
		.WithTarget("Clean,Build")
		.WithProperty("Platform","Any CPU")
		.SetVerbosity(Verbosity.Minimal);
	result.WarningsAsError = false;
	result.Restore = true;
	return result;
}

public string GetAssemblyFile()
{
    return  System.IO.Path.GetFullPath(@"src/bin/Release2025/SCaddins.dll");
}

public string GetFullVersionNumber()
{
    var filePath = GetAssemblyFile();
    var version = FileVersionInfo.GetVersionInfo(filePath);
	return $"{version.FileMajorPart}.{version.FileMinorPart}.{version.FileBuildPart}.{version.FilePrivatePart}";
}

Task("Clean").Does(() => CleanDirectory(buildDir));

Task("CleanOBJ").Does(() => CleanDirectory(objDir));

Task("Restore-NuGet-Packages")
.IsDependentOn("DotNetRestore")
.Does(() => NuGetRestore(solutionFile));

Task("DotNetRestore")
.Does(() => DotNetRestore(new DotNetRestoreSettings(){
WorkingDirectory="src/",
Force = true,
PackagesDirectory = "src/packages",
}));

Task("Restore-Test-NuGet-Packages").Does(() => NuGetRestore(testSolutionFile));

Task("CreateAddinManifests")
.Does(() =>
		{
		string text = System.IO.File.ReadAllText(@"src\SCaddins.addin");
		if (DirectoryExists(@"src\bin\Release2020"))
		    System.IO.File.WriteAllText(@"src\bin\Release2020\SCaddins2020.addin", text.Replace("_REVIT_VERSION_", "2020"));
		if (DirectoryExists(@"src\bin\Release2021"))
		    System.IO.File.WriteAllText(@"src\bin\Release2021\SCaddins2021.addin", text.Replace("_REVIT_VERSION_", "2021"));
		if (DirectoryExists(@"src\bin\Release2022"))
		    System.IO.File.WriteAllText(@"src\bin\Release2022\SCaddins2022.addin", text.Replace("_REVIT_VERSION_", "2022"));
		if (DirectoryExists(@"src\bin\Release2023"))
		    System.IO.File.WriteAllText(@"src\bin\Release2023\SCaddins2023.addin", text.Replace("_REVIT_VERSION_", "2023"));
		if (DirectoryExists(@"src\bin\Release2024"))
		    System.IO.File.WriteAllText(@"src\bin\Release2024\SCaddins2024.addin", text.Replace("_REVIT_VERSION_", "2024"));
	    if (DirectoryExists(@"src\bin\Release2025"))
		    System.IO.File.WriteAllText(@"src\bin\Release2025\SCaddins2025.addin", text.Replace("_REVIT_VERSION_", "2025"));
		});

Task("Revit2020")
.IsDependentOn("CleanOBJ")
.Does(() => MSBuild(solutionFile, GetBuildSettings("2020")));

Task("Revit2021")
.IsDependentOn("CleanOBJ")
.Does(() => MSBuild(solutionFile, GetBuildSettings("2021")));

Task("Revit2022")
.IsDependentOn("CleanOBJ")
.Does(() => MSBuild(solutionFile, GetBuildSettings("2022")));

Task("Revit2023")
.IsDependentOn("CleanOBJ")
.Does(() => MSBuild(solutionFile, GetBuildSettings("2023")));

Task("Revit2024")
.IsDependentOn("CleanOBJ")
.Does(() => MSBuild(solutionFile, GetBuildSettings("2024")));

Task("Revit2025")
.IsDependentOn("CleanOBJ")
.Does(() => MSBuild(solutionFile, GetBuildSettings("2025")));

Task("Tests")
.IsDependentOn("Restore-Test-NuGet-Packages")
.Does(() => MSBuild(testSolutionFile, GetTestBuildSettings()));

Task("Installer")
.Does(() =>
		{
		var version = GetFullVersionNumber();		
		Dictionary<string, string> dict =  new Dictionary<string, string>();
		dict.Add("R2020", "Enabled");
		dict.Add("R2021", "Enabled");
		dict.Add("R2022", "Enabled");
		dict.Add("R2023", "Enabled");
		dict.Add("R2024", "Enabled");
		dict.Add("R2025", "Enabled");
		dict.Add("MyAppVersion", version); 
		var settings = new InnoSetupSettings();
		settings.Defines = dict;
		settings.QuietMode = InnoSetupQuietMode.Quiet;
		settings.WorkingDirectory = new DirectoryPath(Environment.CurrentDirectory + @"\setup");
		InnoSetup(innoSetupFile, settings);  
		});

Task("Dist")
.IsDependentOn("Default")
.IsDependentOn("Installer");

Task("Default")
.IsDependentOn("Clean")
.IsDependentOn("Restore-NuGet-Packages")
.IsDependentOn("Revit2020")
.IsDependentOn("Revit2021")
.IsDependentOn("Revit2022")
.IsDependentOn("Revit2023")
.IsDependentOn("Revit2024")
.IsDependentOn("Revit2025")
.IsDependentOn("CreateAddinManifests");

RunTarget(target);
