/*
 * Install addins.
 */
#addin "nuget:?package=Cake.Codecov&version=0.6.0"
#addin "nuget:?package=Cake.Coverlet&version=2.2.1"
#addin "nuget:?package=Cake.Json&version=3.0.1"
#addin "nuget:?package=Newtonsoft.Json&version=11.0.2"
#addin "nuget:?package=Cake.Gitter&version=0.11.0"
#addin "nuget:?package=Cake.Incubator&version=5.0.1"

/*
 * Install tools.
 */
#tool "nuget:?package=GitReleaseNotes&version=0.7.1"
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
#tool "nuget:?package=ReportGenerator&version=4.1.5"
#tool "nuget:?package=xunit.runner.console&version=2.4.1"
#tool "nuget:?package=Codecov&version=1.5.0"
#tool "nuget:?package=NuGet.CommandLine&version=5.0.2"
#tool "nuget:?package=gitreleasemanager&version=0.8.0"

/*
 * Load other scripts.
 */
#load "./build/parameters.cake"
#load "./build/utils.cake"

/*
 * Variables.
 */
bool publishingError = false;

void TaskErrorReporter(
    string information,
    Exception exception,
    bool isPublishing = true) 
{
    Information(information);
    Error(exception.Dump());
    publishingError = isPublishing; 
}

/*
 * Setup
 */
Setup<BuildParameters>(context =>
{
    if(!string.IsNullOrWhiteSpace(EnvironmentVariable("DOTNET_ROOT")))
        context.Tools.RegisterFile(EnvironmentVariable("DOTNET_ROOT"));

    var parameters = BuildParameters.GetParameters(Context);
    var gitVersion = GetVersion(parameters);
    parameters.Setup(context, gitVersion);

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
    if (context.Successful)
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
        foreach (var project in GetFiles("./**/*.csproj"))
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
            NoBuild = true
        };

        var timestamp = $"{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}";

        var exclude = GetFiles("./shared/**/*.csproj")
                .Select(p => $"[{p.GetFilenameWithoutExtension()}]*")
                .ToList();
        exclude.Add("[xunit.*]*");
        exclude.Add("[*.Tests?]*");

        var coverletSettings = new CoverletSettings 
        {
            CollectCoverage = true,
            CoverletOutputDirectory = parameters.ArtifactPaths.Directories.TestCoverage,
            Exclude = exclude,
            Threshold = (uint)parameters.CoverageThreshold,
            ThresholdType = ThresholdType.Line,
            CoverletOutputFormat = CoverletOutputFormat.cobertura
        };

        foreach (var project in GetFiles("./test/**/*.csproj"))
        {   
            var projectName = project.GetFilenameWithoutExtension();
            Information("Run specs for {0}", projectName);

            coverletSettings.CoverletOutputName = $"results.{projectName}.{timestamp}.xml";

            settings.ArgumentCustomization = args => args
                .Append("--logger")
                .AppendQuoted(
                    $"trx;LogFileName={MakeAbsolute(parameters.ArtifactPaths.Directories.TestResult).FullPath}/{projectName}_{timestamp}.trx");

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

Task("Copy-Files")
    .Does<BuildParameters>((parameters) => 
    {
        Information("Copy static files to artifacts"); 
        CopyFile("./LICENSE", parameters.ArtifactPaths.Files.License);
    });

Task("Release-Notes")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnWindows, "Release notes are generated only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Release notes are generated only on release agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsStableRelease() || parameters.IsPreviewRelease(), "Release notes are generated only for releases.")
    .Does<BuildParameters>((parameters) => 
    {
        Information("Generate release notes"); 
        GetReleaseNotes(parameters.ArtifactPaths.Files.ReleaseNotes);

        if (string.IsNullOrEmpty(System.IO.File.ReadAllText(parameters.ArtifactPaths.Files.ReleaseNotes.FullPath)))
            System.IO.File.WriteAllText(parameters.ArtifactPaths.Files.ReleaseNotes.FullPath, "No issues closed since last release");
    });

Task("Pack-Nuget")
    .Does<BuildParameters>((parameters) => 
    {   
        foreach(var package in GetFiles($"./nuspec/*.nuspec"))
        {
            var packageId = package.GetFilenameWithoutExtension();

            Information("Run pack for {0} to {1}", packageId, parameters.ArtifactPaths.Directories.Nuget.Combine($"{packageId}.nupkg")); 
            Pack(
                File($"./nuspec/{packageId}.nuspec"),
                parameters.Configuration,
                Directory($"./src/{packageId}"),
                parameters.ArtifactPaths.Directories.Nuget,
                parameters.Version.NuGetVersion,
                parameters.ArtifactPaths.Files.License);
        }
    });

Task("Publish-Nuget")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.EnabledPublishNuget, "Publish-NuGet was disabled.")
    .WithCriteria<BuildParameters>((context, parameters) =>
        parameters.IsRunningOnWindows, "Publish-NuGet works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Publish-NuGet works only on AzurePipeline.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsStableRelease() || parameters.IsPreviewRelease(), "Publish-NuGet works only for releases.")
    .IsDependentOn("Pack-NuGet")
    .ContinueOnError()
    .ReportError(exception => 
        TaskErrorReporter(
            "Publish-Nuget task failed, but continuing with next Task...",
            exception,
            true
    ))
    .Does<BuildParameters>((parameters) => 
    {
        if (string.IsNullOrWhiteSpace(parameters.Credentials.Nuget.ApiKey))
            throw new InvalidOperationException("Could not resolve NuGet API key.");

        if (string.IsNullOrWhiteSpace(parameters.Credentials.Nuget.ApiUrl))
            throw new InvalidOperationException("Could not resolve NuGet API url.");

        var settings = new NuGetPushSettings
        {
            ApiKey = parameters.Credentials.Nuget.ApiKey,
            Source = parameters.Credentials.Nuget.ApiUrl
        };

        foreach(var package in GetFiles($"{parameters.ArtifactPaths.Directories.Nuget}/*.nupkg"))
        {
            NuGetPush(package, settings);
        }
    });

Task("Publish-GitHub")
    .WithCriteria<BuildParameters>((context, parameters) =>
        parameters.IsRunningOnWindows, "Publish-GitHub works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Publish-GitHub works only on AzurePipeline.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsStableRelease() || parameters.IsPreviewRelease(), "Publish-GitHub works only for releases.")
    .IsDependentOn("Pack-NuGet")
    .ContinueOnError()
    .ReportError(exception => 
        TaskErrorReporter(
            "Publish-GitHub task failed, but continuing with next Task...",
            exception,
            true
    ))
    .Does<BuildParameters>((parameters) => 
    {
        if (string.IsNullOrWhiteSpace(parameters.Credentials.GitHub.Token))
            throw new InvalidOperationException("Could not resolve GitHub token.");

        GitReleaseManagerCreate(
            parameters.Credentials.GitHub.Token,
            "nicolabiancolini",
            "Cake.Board",
            new GitReleaseManagerCreateSettings
            {
                Prerelease = !parameters.IsStableRelease() && parameters.IsPreviewRelease(),
                TargetCommitish = "master"
            });

        foreach(var package in GetFiles($"{parameters.ArtifactPaths.Directories.Nuget}/*.nupkg"))
        {
            Information("Add {0} to release.", package.FullPath);
            GitReleaseManagerAddAssets(
                parameters.Credentials.GitHub.Token,
                "nicolabiancolini",
                "Cake.Board",
                parameters.Version.Version,
                package.FullPath);
        }

        GitReleaseManagerClose(
            parameters.Credentials.GitHub.Token,
            "nicolabiancolini",
            "Cake.Board",
            parameters.Version.Version);
    });


Task("Publish-Test-Results-AzurePipelines-UbuntuAgent")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Test results are generated only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnLinux, "Test results for Ubuntu agent are generated only on Ubuntu agents.")    
    .ContinueOnError()
    .ReportError(exception => 
        TaskErrorReporter(
            "Publish-Test-Results-AzurePipelines-UbuntuAgent task failed, but continuing with next Task...",
            exception,
            true
    ))
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
    .ContinueOnError()
    .ReportError(exception => 
        TaskErrorReporter(
            "Publish-Test-Results-AzurePipelines-WindowsAgent task failed, but continuing with next Task...",
            exception,
            true
    ))
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
    .ContinueOnError()
    .ReportError(exception => 
        TaskErrorReporter(
            "Publish-Coverage-Results-AzurePipelines-UbuntuAgent task failed, but continuing with next Task...",
            exception,
            true
    ))
    .Does<BuildParameters>((parameters) => 
    {
        Information("Publish code coverage results for Ubuntu Agent"); 

        PublishCodeCoverage(
            GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverage}/results.*.xml").First().FullPath,
            parameters.ArtifactPaths.Directories.TestCoverageResults,
            GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverageResults}/**/*").ToArray());
    });

Task("Publish-Coverage-Results-AzurePipelines-WindowsAgent")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Coverage results are generated only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnWindows, "Coverage results for Windows agent are generated only on Windows agents.")
    .ContinueOnError()
    .ReportError(exception => 
        TaskErrorReporter(
            "Publish-Coverage-Results-AzurePipelines-WindowsAgent task failed, but continuing with next Task...",
            exception,
            true
    ))
    .Does<BuildParameters>((parameters) => 
    {
        Information("Publish code coverage results for Windows Agent"); 

        PublishCodeCoverage(
            GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverage}/results.*.xml").First().FullPath,
            parameters.ArtifactPaths.Directories.TestCoverageResults,
            GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverageResults}/**/*").ToArray());
    });

Task("Publish-Coverage-Results-CodeCov")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Coverage results are generated only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnWindows, "Coverage results for Windows agent are generated only on Windows agents.")
    .ContinueOnError()
    .ReportError(exception => 
        TaskErrorReporter(
            "Publish-Coverage-Results-CodeCov task failed, but continuing with next Task...",
            exception,
            true
    ))
    .Does<BuildParameters>((parameters) => 
    {
        Information("Publish code coverage results for Coverlet"); 

        if (string.IsNullOrWhiteSpace(parameters.Credentials.CodeCov.Token))
            throw new InvalidOperationException("Could not resolve CodeCov token.");

        Codecov(
            GetFiles($"{parameters.ArtifactPaths.Directories.TestCoverage}/results.*.xml").Select(file => file.FullPath),
            parameters.Credentials.CodeCov.Token);
    });

Task("Publish-Coverage-Results-AzurePipelines")
    .IsDependentOn("Publish-Coverage-Results-AzurePipelines-WindowsAgent") 
    .IsDependentOn("Publish-Coverage-Results-AzurePipelines-UbuntuAgent") 
    .Does(() => 
    {
    });

Task("Publish-Coverage-Results")
    .IsDependentOn("Publish-Coverage-Results-AzurePipelines")
    .IsDependentOn("Publish-Coverage-Results-CodeCov")
    .Does(() =>
    {
    });

Task("Publish-Artifacts-AzurePipelines-UbuntuAgent")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnAzurePipeline, "Artifacts are published only on agents.")
    .WithCriteria<BuildParameters>((context, parameters) => 
        parameters.IsRunningOnLinux, "Artifacts for Ubuntu agent are published only on Ubuntu agents.")    
    .ContinueOnError()
    .ReportError(exception => 
        TaskErrorReporter(
            "Publish-Artifacts-AzurePipelines-UbuntuAgent task failed, but continuing with next Task...",
            exception,
            true
    ))
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
    .ContinueOnError()
    .ReportError(exception => 
        TaskErrorReporter(
            "Publish-Artifacts-AzurePipelines-WindowsAgent task failed, but continuing with next Task...",
            exception,
            true
    ))
    .Does<BuildParameters>((parameters) => 
    {
        Information("Publish artifacts for Windows Agent"); 

        PublishArtifacts(
            "drop",
            "windows-agent",
            GetFiles($"{parameters.ArtifactPaths.Directories.Output}/**/*.nupkg").ToArray());
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
    .IsDependentOn("Copy-Files")   
    .IsDependentOn("Release-Notes")
    .OnError<BuildParameters>((exception, parameters) => {
        throw exception;
    })
    .Does<BuildParameters>((parameters) =>
    {
        if (parameters.ProcessVariables.Contains(new KeyValuePair<string, object>("IsTestsFailed", true)))
            throw new InvalidProgramException("Test failed or code coverage under the minimal threshold.");
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
    .IsDependentOn("Publish-Nuget")
    .IsDependentOn("Publish-GitHub")
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
