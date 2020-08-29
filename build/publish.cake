/*
 * Load other scripts.
 */
#load "./build/parameters.cake"
#load "./build/utils.cake"

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

        Information("Create compressed asset.");
        Zip(parameters.ArtifactPaths.Directories.Nuget, $"{parameters.ArtifactPaths.Directories.Output}/nuget-packages.zip");

        var owner = "nicolabiancolini";
        var repository = "Cake.Board";

        GitReleaseManagerCreate(
            parameters.Credentials.GitHub.Token,
            owner,
            repository,
            new GitReleaseManagerCreateSettings
            {
                Name = parameters.Version.Version,
                Prerelease = !parameters.IsStableRelease() && parameters.IsPreviewRelease(),
                TargetCommitish = "master"
            });

        GitReleaseManagerAddAssets(
            parameters.Credentials.GitHub.Token,
            owner,
            repository,
            parameters.Version.Version,
            $"{parameters.ArtifactPaths.Directories.Output}/nuget-packages.zip");
        GitReleaseManagerAddAssets(
            parameters.Credentials.GitHub.Token,
            owner,
            repository,
            parameters.Version.Version,
            $"{parameters.ArtifactPaths.Directories.Output}/LICENSE.txt");
        GitReleaseManagerClose(
            parameters.Credentials.GitHub.Token,
            owner,
            repository,
            parameters.Version.Version);
    });
