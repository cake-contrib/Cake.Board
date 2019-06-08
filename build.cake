/*
 * Install addins.
 */
#addin "nuget:?package=Cake.Codecov&version=0.6.0"
#addin "nuget:?package=Cake.Coverlet&version=2.2.1"
#addin "nuget:?package=Cake.Json&version=3.0.1"
#addin "nuget:?package=Newtonsoft.Json&version=11.0.2"
#addin "nuget:?package=Cake.Gitter&version=0.11.0"

/*
 * Install tools.
 */
#tool "nuget:?package=GitReleaseNotes&version=0.7.1"
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
#tool "nuget:?package=ReportGenerator&version=4.1.5"
#tool "nuget:?package=xunit.runner.console&version=2.4.1"
#tool "nuget:?package=Codecov&version=1.5.0"
#tool "nuget:?package=NuGet.CommandLine&version=5.0.2"

/*
 * Load other scripts.
 */
#load "./build/parameters.cake"
#load "./build/utils.cake"

/*
 * Variables
 */
bool publishingError = false;

/*
 * Setup
 */
Setup<BuildParameters>(context =>
{
    var parameters = BuildParameters.GetParameters(Context);
    var gitVersion = GetVersion(parameters);
    parameters.Setup(context, gitVersion, 60);

    if (parameters.IsMainBranch && (context.Log.Verbosity != Verbosity.Diagnostic)) {
        Information("Increasing verbosity to diagnostic.");
        context.Log.Verbosity = Verbosity.Diagnostic;
    }

    Information("Building of Cake Board ({0})", parameters.Configuration);

    Information("Build version : Version {0}, SemVersion {1}, NuGetVersion: {2}",
        parameters.Version.Version, parameters.Version.SemVersion, parameters.Version.NuGetVersion);

    Information("Repository info : IsMainRepo {0}, IsMainBranch {1}, IsStableBranch {2}, IsTagged: {3}, IsPullRequest: {4}",
        parameters.IsMainRepo, parameters.IsMainBranch, parameters.IsStableBranch, parameters.IsTagged, parameters.IsPullRequest);

    return parameters;
});

/*
 * Teardown
 */
Teardown<BuildParameters>((context, parameters) =>
{
    if(context.Successful)
    {
        Information("Finished running tasks. Thanks for your patience :D");
    }
    else
    {
        Error("Something wrong! :|");
        Error(context.ThrownException.Message);
    }
});

/*
 * Tasks
 */
Task("Clean")   
    .Does<BuildParameters>((parameters) => 
    {
        CleanDirectories(parameters.ArtifactPaths.Directories.ToClean);

        CleanDirectories($"./**/bin/{parameters.Configuration}");
        CleanDirectories("./**/obj");
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does<BuildParameters>((parameters) =>
    {
        foreach (var project in GetFiles("./src/**/*.csproj"))
        {
            Build(project, parameters.Configuration, parameters.MSBuildSettings);        
        }
    });

Task("Test")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.EnabledUnitTests, "Unit tests were disabled.")
    .IsDependentOn("Build")
    .OnError<BuildParameters>((exception, parameters) => {
        parameters.ProcessVariables.Add("IsTestsFailed", true);
    })
    .Does<BuildParameters>((parameters) => 
    {
        var settings = new DotNetCoreTestSettings 
        {
            Configuration = parameters.Configuration,
            NoBuild = false
        };

        var timestamp = $"{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}";

        var coverletSettings = new CoverletSettings 
        {
            CollectCoverage = true,
            CoverletOutputDirectory = parameters.ArtifactPaths.Directories.TestCoverage,
            CoverletOutputName = $"results.{timestamp}.xml",
            Exclude = new List<string>() { "[xunit.*]*", "[*.Specs?]*" }
        };

        var projects = GetFiles("./test/**/*.csproj");

        if (projects.Count > 1)
            coverletSettings.MergeWithFile = $"{coverletSettings.CoverletOutputDirectory.FullPath}/{coverletSettings.CoverletOutputName}";

        var i = 1;
        foreach (var project in projects)
        {   
            if (i++ == projects.Count)
                coverletSettings.CoverletOutputFormat = CoverletOutputFormat.cobertura;

            var projectName = project.GetFilenameWithoutExtension();
            Information("Run specs for {0}", projectName);

            settings.ArgumentCustomization = args => args
                .Append("--logger")
                .AppendQuoted(
                    $"trx;LogFileName={MakeAbsolute(parameters.ArtifactPaths.Directories.TestCoverage).FullPath}/{projectName}_{timestamp}.trx");

            DotNetCoreTest(project.FullPath, settings, coverletSettings);
        }
    });

Task("Coverage-Report")
    .WithCriteria<BuildParameters>((context, parameters) => 
        GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverage.FullPath}/**/*.xml").Count != 0)
    .Does<BuildParameters>((parameters) => 
    {
        var settings = new ReportGeneratorSettings
        {
            ReportTypes = { ReportGeneratorReportType.HtmlInline }
        };

        if (parameters.IsRunningOnAzurePipeline)
            settings.ReportTypes.Add(ReportGeneratorReportType.HtmlInline_AzurePipelines);

        ReportGenerator(
            GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverage.FullPath}/**/*.xml"),
            parameters.ArtifactPaths.Directories.TestCoverageResults, settings);
    });

Task("Copy-Bin")
    .Does<BuildParameters>((parameters) =>
    {
        var settings = new DotNetCorePublishSettings
        {
            Configuration = parameters.Configuration,
            Framework = parameters.Framework,
            NoBuild = true,
            MSBuildSettings = parameters.MSBuildSettings
        };

        foreach (var package in parameters.PackagesBuildMap)
        {
            Information("Run publish for {0} to {1}", package.Key, package.Value); 

            settings.OutputDirectory = package.Value;
            DotNetCorePublish($"./src/**/{package.Key}.csproj", settings);  
        }
    });

Task("Copy-Files")
    .Does<BuildParameters>((parameters) => 
    {
        Information("Copy static files to artifacts"); 
        CopyFileToDirectory("./LICENSE", parameters.ArtifactPaths.Directories.Root);

        foreach (var project in GetFiles("./src/**/*.csproj"))
        {
            var settings = new DotNetCorePackSettings 
            {
                NoBuild = true,
                NoRestore = true,
                Configuration = parameters.Configuration,
                OutputDirectory = parameters.ArtifactPaths.Directories.Root,
                MSBuildSettings = parameters.MSBuildSettings
            };

            Information("Run pack for {0} to {1}", project.GetFilenameWithoutExtension(), settings.OutputDirectory); 
            DotNetCorePack(project.FullPath, settings);
        }
    });

Task("Release-Notes")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnWindows, "Release notes are generated only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Release notes are generated only on release agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsStableRelease(), "Release notes are generated only for stable releases.")
    .Does<BuildParameters>((parameters) => 
    {
        Information("Generate release notes"); 
        GetReleaseNotes(parameters.ArtifactPaths.Files.ReleaseNotes);

        if (string.IsNullOrEmpty(System.IO.File.ReadAllText(parameters.ArtifactPaths.Files.ReleaseNotes.FullPath)))
            System.IO.File.WriteAllText(parameters.ArtifactPaths.Files.ReleaseNotes.FullPath, "No issues closed since last release");
    });



Task("Publish-Test-Results-AzurePipelines-UbuntuAgent")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Test results are generated only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnLinux, "Test results for Ubuntu agent are generated only on Ubuntu agents.")    
    .Does<BuildParameters>((parameters) => 
    {
        Information("Publish test results for Ubuntu Agent"); 

        PublishTestResults(
            "ubuntu-agent",
            GetFiles($"{parameters.ArtifactPaths.Directories.TestResult}/**/*.trx").ToArray());
    });

Task("Publish-Test-Results-AzurePipelines-WindowsAgent")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Test results are generated only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnWindows, "Test results for Windows agent are generated only on Windows agents.")
    .Does<BuildParameters>((parameters) => 
    {
        Information("Publish test results for Windows Agent"); 

        PublishTestResults(
            "windows-agent",
            GetFiles($"{parameters.ArtifactPaths.Directories.TestResult}/**/*.trx").ToArray());
    });

Task("Publish-Test-Results-AzurePipelines")
    .IsDependentOn("Publish-Test-Results-AzurePipelines-WindowsAgent") 
    .IsDependentOn("Publish-Test-Results-AzurePipelines-UbuntuAgent") 
    .Does(() => 
    {
    });

Task("Publish-Test-Results")
    .IsDependentOn("Publish-Test-Results-AzurePipelines")
    .Does(() =>
    {

    });

Task("Publish-Coverage-Results-AzurePipelines-UbuntuAgent")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Coverage results are generated only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnLinux, "Coverage results for Ubuntu agent are generated only on Ubuntu agents.")    
    .Does<BuildParameters>((parameters) => 
    {
        Information("Publish code coverage results for Ubuntu Agent"); 

        PublishCodeCoverage(
            GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverage}/results.*.xml").Single().FullPath,
            parameters.ArtifactPaths.Directories.TestCoverageResults,
            GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverageResults}/**/*").ToArray());
    });

Task("Publish-Coverage-Results-AzurePipelines-WindowsAgent")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Coverage results are generated only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnWindows, "Coverage results for Windows agent are generated only on Windows agents.")
    .Does<BuildParameters>((parameters) => 
    {
        Information("Publish code coverage results for Windows Agent"); 

        PublishCodeCoverage(
            GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverage}/results.*.xml").Single().FullPath,
            parameters.ArtifactPaths.Directories.TestCoverageResults,
            GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverageResults}/**/*").ToArray());
    });

Task("Publish-Coverage-Results-AzurePipelines")
    .IsDependentOn("Publish-Coverage-Results-AzurePipelines-WindowsAgent") 
    .IsDependentOn("Publish-Coverage-Results-AzurePipelines-UbuntuAgent") 
    .Does(() => 
    {
    });

Task("Publish-Coverage-Results")
    .IsDependentOn("Publish-Coverage-Results-AzurePipelines")
    .Does(() =>
    {

    });

Task("Publish-Artifacts-AzurePipelines-UbuntuAgent")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Artifacts are published only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnLinux, "Artifacts for Ubuntu agent are published only on Ubuntu agents.")    
    .Does<BuildParameters>((parameters) => 
    {
        Information("Publish artifacts for Ubuntu Agent"); 

        PublishArtifacts(
            "drop",
            "ubuntu-agent",
            GetFiles($"{parameters.ArtifactPaths.Directories.Output}/**/*.nupkg").ToArray());
    });

Task("Publish-Artifacts-AzurePipelines-WindowsAgent")
    .WithCriteria<BuildParameters>((context, parameters) =>
        parameters.IsRunningOnAzurePipeline, "Artifacts are published only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnWindows, "Artifacts for Windows agent are published only on Windows agents.")
    .Does<BuildParameters>((parameters) => 
    {
        Information("Publish artifacts for Windows Agent"); 

        PublishArtifacts(
            "drop",
            "windows-agent",
            GetFiles($"{parameters.ArtifactPaths.Directories.Output}/**/*.nupkg").ToArray());
    });

Task("Pack-Nuget")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Nuget packages are generated only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnWindows, "Nuget packages are generated only on Windows agents.")
    .Does<BuildParameters>((parameters) => 
    {
        //foreach(var package in parameters.Packages.Nuget)
            // Pack(
            //     package.NuspecPath,
            //     parameters.Configuration,
            //     parameters.ArtifactPaths.Directories.)
    });

Task("Publish-Artifacts-AzurePipelines")
    .IsDependentOn("Publish-Artifacts-AzurePipelines-WindowsAgent") 
    .IsDependentOn("Publish-Artifacts-AzurePipelines-UbuntuAgent") 
    .Does(() => 
    {
    });

Task("Publish-Artifacts")
    .IsDependentOn("Publish-Artifacts-AzurePipelines")
    .Does(() =>
    {
    });

Task("Copy")
    .IsDependentOn("Test")
    .IsDependentOn("Coverage-Report")
    .IsDependentOn("Copy-Bin") 
    .IsDependentOn("Copy-Files")   
    .IsDependentOn("Release-Notes")
    .Does(() =>
    {
    });

Task("Pack")
    .IsDependentOn("Copy")
    .IsDependentOn("Pack-Nuget")
    .Does(() =>
    {
    });

Task("Publish")
    .IsDependentOn("Pack")
    .IsDependentOn("Publish-Test-Results")
    .IsDependentOn("Publish-Coverage-Results")
    .IsDependentOn("Publish-Artifacts")
    .Does(()=> 
    {

    });

Task("Default")
    .IsDependentOn("Publish")
    .Does(() =>
    {

    });

/*
 * Execution
 */
var target = Argument("target", "Default");
RunTarget(target);
