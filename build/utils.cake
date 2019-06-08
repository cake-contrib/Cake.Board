#load "./parameters.cake"
#load "./paths.cake"

FilePath FindToolInPath(string tool)
{
    var pathEnv = EnvironmentVariable("PATH");
    if (string.IsNullOrEmpty(pathEnv) || string.IsNullOrEmpty(tool)) return tool;

    var paths = pathEnv.Split(new []{ IsRunningOnUnix() ? ':' : ';'},  StringSplitOptions.RemoveEmptyEntries);
    return paths.Select(path => new DirectoryPath(path).CombineWithFilePath(tool)).FirstOrDefault(filePath => FileExists(filePath.FullPath));
}

void FixForMono(
    Cake.Core.Tooling.ToolSettings toolSettings,
    string toolExe)
{
    if (IsRunningOnUnix())
    {
        var toolPath = Context.Tools.Resolve(toolExe);
        toolSettings.ToolPath = FindToolInPath("mono");
        toolSettings.ArgumentCustomization = args => toolPath.FullPath + " " + args.Render();
    }
}

DirectoryPath HomePath()
{
    return IsRunningOnWindows()
        ? new DirectoryPath(EnvironmentVariable("HOMEDRIVE") +  EnvironmentVariable("HOMEPATH"))
        : new DirectoryPath(EnvironmentVariable("HOME"));
}

void ReplaceTextInFile(
    FilePath filePath,
    string oldValue,
    string newValue,
    bool encrypt = false)
{
    Information("Replacing {0} with {1} in {2}", oldValue, !encrypt ? newValue : "******", filePath);
    var file = filePath.FullPath.ToString();
    System.IO.File.WriteAllText(file, System.IO.File.ReadAllText(file).Replace(oldValue, newValue));
}

GitVersion GetVersion(BuildParameters parameters)
{
    var settings = new GitVersionSettings
    {
        OutputType = GitVersionOutput.Json    
    };

    if (parameters.IsRunningOnAzurePipeline && !parameters.IsPullRequest)
    {
        settings.UpdateAssemblyInfo = true;
        settings.LogFilePath = "console";
        settings.OutputType = GitVersionOutput.BuildServer;
    }
    return GitVersion(settings);
}

void Build(
    FilePath projectPath,
    string configuration,
    DotNetCoreMSBuildSettings settings = null)
{
    Information("Run restore for {0}", projectPath.GetFilenameWithoutExtension());
    DotNetCoreRestore(projectPath.FullPath);
    Information("Run build for {0}", projectPath.GetFilenameWithoutExtension());
    DotNetCoreBuild(projectPath.FullPath, new DotNetCoreBuildSettings {
        Configuration = configuration,
        Verbosity = DotNetCoreVerbosity.Minimal,
        NoRestore = true,
        MSBuildSettings = settings
    });   
}

void GetReleaseNotes(
    FilePath outputPath,
    DirectoryPath workDir = null,
    string repoToken = null)
{
    var toolPath = Context.Tools.Resolve(Context.IsRunningOnWindows() ? "GitReleaseNotes.exe" : "GitReleaseNotes");

    workDir = workDir ?? ".";
    
    var arguments = new ProcessArgumentBuilder()
        .Append(workDir.ToString())
        .Append("/OutputFile")
        .Append(outputPath.ToString());
    if (repoToken != null)
        arguments.Append("/RepoToken").Append(repoToken);

    StartProcess(toolPath, new ProcessSettings { Arguments = arguments, RedirectStandardOutput = true }, out var redirectedOutput);

    Information(string.Join("\n", redirectedOutput));
}

string GetDotnetVersion()
{
    var toolPath = Context.Tools.Resolve(Context.IsRunningOnWindows() ? "dotnet.exe" : "dotnet");

    var arguments = new ProcessArgumentBuilder()
        .Append("--version");

    using(var process = StartAndReturnProcess(toolPath, new ProcessSettings { Arguments = arguments, RedirectStandardOutput = true }))
    {
        process.WaitForExit();

        return process.GetStandardOutput().LastOrDefault();
    }
}

void PublishCodeCoverage(
    FilePath summary,
    DirectoryPath reportDir,
    FilePath[] additionlFiles) 
{ 
    new TFBuildCommands(Context.Environment, Context.Log)
        .PublishCodeCoverage(new TFBuildPublishCodeCoverageData {
            CodeCoverageTool = TFCodeCoverageToolType.Cobertura,
            SummaryFileLocation = summary,           
            ReportDirectory = reportDir,
            AdditionalCodeCoverageFiles = additionlFiles
        });
}

void PublishArtifacts(
    string folderName,
    string artifactName,
    FilePath[] files)
{
    var command = new TFBuildCommands(Context.Environment, Context.Log);

    foreach(var file in files)
    {
        command.UploadArtifact(folderName, file, artifactName);
    }
}

void PublishTestResults(
    string runTitle,
    FilePath[] testResults,
    string configuration = "Release") 
{
    new TFBuildCommands(Context.Environment, Context.Log)
        .PublishTestResults(new TFBuildPublishTestResultsData {
            TestResultsFiles = testResults,
            MergeTestResults = true,
            Configuration = configuration,
            TestRunTitle = runTitle,
            TestRunner = TFTestRunnerType.VSTest
        });
}

void Pack(
    FilePath nuspecPath,
    string configuration,
    DirectoryPath binDir,
    DirectoryPath outputDir,
    string version,
    DotNetCoreMSBuildSettings msBuildSettings)
{
    NuGetPack(
        nuspecPath,
        new NuGetPackSettings
        {
            Version = version,
            OutputDirectory = outputDir,
            Files = GetFiles(binDir + "/**/*.*")
                .Select(file => 
                    new NuSpecContent { 
                        Source = file.FullPath,
                        Target = file.FullPath.Replace(binDir.FullPath, "") 
                        })
                .ToArray()
        });
}
