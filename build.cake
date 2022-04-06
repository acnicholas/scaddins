using Cake.Common.Diagnostics;
using System.IO;

var target = Argument("target", "Default");
var solutionFile = GetFiles("src/*.sln").First();
var solutionFileWix = GetFiles("installer/SCaddins.Installer.wixproj").First();
var buildDir = Directory(@"./src/bin");
var testBuildDir = Directory(@"./src/bin");
var testAssemblyDllName = "SCaddins.Tests.dll";
var revitTestFrameworkBin = Argument("RTFBin", @"tools/RevitTestFramework/RevitTestFrameworkConsole.exe");

// METHODS

public MSBuildSettings GetBuildSettings(string config)
{
	var result = new MSBuildSettings()
		.SetConfiguration(config)
		.WithTarget("Clean,Build")
		.WithProperty("Platform","x64")
		.SetVerbosity(Verbosity.Minimal);
	result.WarningsAsError = true;
	return result;
}

public string GetTestAssembly(string revitVersion)
{
	return string.Format(@"tests\bin\x64\Release{0}\{1}",revitVersion, testAssemblyDllName);
}

public bool APIAvailable(string revitVersion)
{
	return FileExists(@"C:\Program Files\Autodesk\Revit " + revitVersion + @"\RevitAPI.dll");
}

public string GetTestArgs(string revitVersion)
{
	var path = string.Format(@"tests\bin\x64\Release{0}",revitVersion);
	string result = string.Format(@"--assembly={0} --dir={1} --results={1}\result.xml --groupByModel --color --continuous", GetTestAssembly(revitVersion), path);
	System.Console.WriteLine(result);
	return result;
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
		if (DirectoryExists(@"src\bin\Release2018"))
		    System.IO.File.WriteAllText(@"src\bin\Release2018\SCaddins2018.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2018"));
		if (DirectoryExists(@"src\bin\Release2019"))
		    System.IO.File.WriteAllText(@"src\bin\Release2019\SCaddins2019.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2019"));
		if (DirectoryExists(@"src\bin\Release2020"))
		    System.IO.File.WriteAllText(@"src\bin\Release2020\SCaddins2020.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2020"));
		if (DirectoryExists(@"src\bin\Release2021"))
		    System.IO.File.WriteAllText(@"src\bin\Release2021\SCaddins2021.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2021"));
		if (DirectoryExists(@"src\bin\Release2022"))
		    System.IO.File.WriteAllText(@"src\bin\Release2022\SCaddins2022.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2022"));
		if (DirectoryExists(@"src\bin\Release2023"))
		    System.IO.File.WriteAllText(@"src\bin\Release2022\SCaddins2023.addin", String.Copy(text).Replace("_REVIT_VERSION_", "2023"));
		});

Task("Revit2018")
.IsDependentOn("Restore-NuGet-Packages")
.WithCriteria(APIAvailable("2018"))
.Does(() => MSBuild(solutionFile, GetBuildSettings("Release2018")));

Task("Revit2019")
.IsDependentOn("Restore-NuGet-Packages")
.WithCriteria(APIAvailable("2019"))
.Does(() => MSBuild(solutionFile, GetBuildSettings("Release2019")));

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

Task("SetUpTests")
.Does(() =>
	{
		if (FileExists(@"tests\bin\x64\Release2019\scaddins_test_model.rvt")) {
			DeleteFile(@"tests\bin\x64\Release2019\scaddins_test_model.rvt");
		}
		CopyFile(@"tests\models\scaddins_test_model.rvt", @"tests\bin\x64\Release2019\scaddins_test_model.rvt");
	});

Task("Test2018")
.IsDependentOn("SetUpTests")
.Does(() => StartProcess(revitTestFrameworkBin, GetTestArgs("2018")));

Task("Test2019")
.IsDependentOn("SetUpTests")
.Does(() => StartProcess(revitTestFrameworkBin, GetTestArgs("2019")));

Task("Test2020")
.IsDependentOn("SetUpTests")
.Does(() => StartProcess(revitTestFrameworkBin, GetTestArgs("2020")));

Task("Test2021")
.IsDependentOn("SetUpTests")
.Does(() => StartProcess(revitTestFrameworkBin, GetTestArgs("2021")));

Task("Test2022")
.IsDependentOn("SetUpTests")
.Does(() => StartProcess(revitTestFrameworkBin, GetTestArgs("2022")));

Task("Test2023")
.IsDependentOn("SetUpTests")
.Does(() => StartProcess(revitTestFrameworkBin, GetTestArgs("2023")));

Task("Installer")
.IsDependentOn("Restore-Installer-NuGet-Packages")
.Does(() =>
		{
		Environment.SetEnvironmentVariable("R2018", APIAvailable("2018") ? "Enabled" : "Disabled");
		Environment.SetEnvironmentVariable("R2019", APIAvailable("2019") ? "Enabled" : "Disabled");
		Environment.SetEnvironmentVariable("R2020", APIAvailable("2020") ? "Enabled" : "Disabled");
		Environment.SetEnvironmentVariable("R2021", APIAvailable("2021") ? "Enabled" : "Disabled");
		Environment.SetEnvironmentVariable("R2022", APIAvailable("2022") ? "Enabled" : "Disabled");
		Environment.SetEnvironmentVariable("R2023", APIAvailable("2023") ? "Enabled" : "Disabled");
		var settings = new MSBuildSettings();
		settings.SetConfiguration("Release");
		settings.WithTarget("Rebuild");
		settings.SetVerbosity(Verbosity.Minimal);
		settings.WorkingDirectory = new DirectoryPath(Environment.CurrentDirectory + @"\installer");
		MSBuild(solutionFileWix, settings);  
		});

Task("Dist")
.IsDependentOn("Default")
.IsDependentOn("Installer");

Task("Default")
.IsDependentOn("Clean")
.IsDependentOn("Revit2018")
.IsDependentOn("Revit2019")
.IsDependentOn("Revit2020")
.IsDependentOn("Revit2021")
.IsDependentOn("Revit2022")
.IsDependentOn("Revit2023")
.IsDependentOn("CreateAddinManifests");

RunTarget(target);
