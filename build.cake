#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.1

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Version + "-alpha" : "0-dev";
var releaseBinPath = "./LambdaS3FileZipper/bin/Release";
var artifactsDirectory = "./artifacts";

Task("Setup")
	.Does(() => { 
		CreateDirectory(artifactsDirectory);
	});

Task("Build")
	.Does(() => {
            var settings = new DotNetCoreBuildSettings
            {
                Configuration = configuration,
                OutputDirectory = artifactsDirectory,
				VersionSuffix = version,
            };
            DotNetCoreBuild("./LambdaS3FileZipper.sln", settings);
	});

Task("UnitTest")
	.IsDependentOn("Build")
	.IsDependentOn("Setup")
	.Does(() => {
		var resultsDirectory = artifactsDirectory + "/TestResults";

        DotNetCoreVSTest($"{artifactsDirectory}/LambdaS3FileZipper.Test.dll", new DotNetCoreVSTestSettings
        {
            Logger = "trx",
            ArgumentCustomization = a => a.Append($"--ResultsDirectory:{resultsDirectory}")
        });

		if(AppVeyor.IsRunningOnAppVeyor)
		{
            foreach(var file in GetFiles(resultsDirectory))
            {
			    AppVeyor.UploadTestResults(file, AppVeyorTestResultsType.MSTest);
            }
		}
	});

Task("UploadNugetPackages")
	.Does(() => {
		if(!AppVeyor.IsRunningOnAppVeyor)
		{
			return;
		}

		var files = GetFiles(artifactsDirectory + "/*.nupkg");
		foreach(var file in files)
		{
			AppVeyor.UploadArtifact(file);
		}
	});

Task("Default")
	.IsDependentOn("Build")
	.IsDependentOn("UnitTest")
	.IsDependentOn("UploadNugetPackages");
  
RunTarget(target);