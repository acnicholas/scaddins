#tool nuget:?package=Tools.InnoSetup&version=6.2.2

using Cake.Common.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

var target = Argument("target", "Default");
var solutionFile = GetFiles("src/*.sln").First();
var innoSetupFile = GetFiles("setup/*.iss").First();
var buildDir = Directory(@"./src/bin");

// METHODS

public MSBuildSettings GetBuildSettings(string config)
{
	var result = new MSBuildSettings()
		.SetConfiguration(config)
		.WithTarget("Clean,Build")
		.WithProperty("Platform","x64")
		.SetVerbosity(Verbosity.Minimal);
	result.WarningsAsError = false;
	return result;
}

public bool APIAvailable(string revitVersion)
{
	return FileExists(@"C:\Program Files\Autodesk\Revit " + revitVersion + @"\RevitAPI.dll");
}

public string GetAssemblyFile()
{
    if (APIAvailable("2024")) return  System.IO.Path.GetFullPath(@"src/bin/Release2024/SCaddins.dll");
	if (APIAvailable("2023")) return  System.IO.Path.GetFullPath(@"src/bin/Release2023/SCaddins.dll");
	if (APIAvailable("2022")) return  System.IO.Path.GetFullPath(@"src/bin/Release2022/SCaddins.dll");
	if (APIAvailable("2021")) return  System.IO.Path.GetFullPath(@"src/bin/Release2021/SCaddins.dll");
	if (APIAvailable("2020")) return  System.IO.Path.GetFullPath(@"src/bin/Release2020/SCaddins.dll");
	return string.Empty;
}

public string GetFullVersionNumber()
{
    var filePath = GetAssemblyFile();
	if (string.IsNullOrEmpty(filePath))
	{
	    return "99.99.99.99";
	}
    var version = FileVersionInfo.GetVersionInfo(filePath);
	return $"{version.FileMajorPart}.{version.FileMinorPart}.{version.FileBuildPart}.{version.FilePrivatePart}";
}

// TASKS

Task("Clean").Does(() => CleanDirectory(buildDir));

Task("Restore-NuGet-Packages").Does(() => NuGetRestore(solutionFile));

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
		});

Task("Revit2020")
.IsDependentOn("Restore-NuGet-Packages")
.WithCriteria(APIAvailable("2020"))
.Does(() => MSBuild(solutionFile, GetBuildSettings("Release2020")));

Task("Revit2021")
.IsDependentOn("Restore-NuGet-Packages")
.WithCriteria(APIAvailable("2021"))
.Does(() => MSBuild(solutionFile, GetBuildSettings("Release2021")));

Task("Revit2022")
.IsDependentOn("Restore-NuGet-Packages")
.WithCriteria(APIAvailable("2022"))
.Does(() => MSBuild(solutionFile, GetBuildSettings("Release2022")));

Task("Revit2023")
.IsDependentOn("Restore-NuGet-Packages")
.WithCriteria(APIAvailable("2023"))
.Does(() => MSBuild(solutionFile, GetBuildSettings("Release2023")));

Task("Revit2024")
.IsDependentOn("Restore-NuGet-Packages")
.WithCriteria(APIAvailable("2024"))
.Does(() => MSBuild(solutionFile, GetBuildSettings("Release2024")));

Task("Installer")
.Does(() =>
		{
		var version = GetFullVersionNumber();		
		Dictionary<string, string> dict =  new Dictionary<string, string>();
		dict.Add("R2020", APIAvailable("2020") ? "Enabled" : "Disabled");
		dict.Add("R2021", APIAvailable("2021") ? "Enabled" : "Disabled");
		dict.Add("R2022", APIAvailable("2022") ? "Enabled" : "Disabled");
		dict.Add("R2023", APIAvailable("2023") ? "Enabled" : "Disabled");
		dict.Add("R2024", APIAvailable("2024") ? "Enabled" : "Disabled");
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
.IsDependentOn("Revit2020")
.IsDependentOn("Revit2021")
.IsDependentOn("Revit2022")
.IsDependentOn("Revit2023")
.IsDependentOn("Revit2024")
.IsDependentOn("CreateAddinManifests");

RunTarget(target);
