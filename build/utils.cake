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

    var version = GitVersion(settings);

    if (parameters.IsRunningOnAzurePipeline)
    {
        var serverBuildVersion = version.SemVer;

        if(!parameters.IsPullRequest && parameters.IsStableRelease())
            serverBuildVersion = $"{serverBuildVersion}-{version.Sha}";

        Console.WriteLine($"##vso[build.updatebuildnumber]{serverBuildVersion}");
    }
        
    return version;
}

void Build(
    FilePath projectPath,
    string configuration,
    DotNetCoreMSBuildSettings settings = null)
{
    Information("Run build for {0}", projectPath.GetFilenameWithoutExtension());
    DotNetCoreBuild(projectPath.FullPath, new DotNetCoreBuildSettings {
        Configuration = configuration,
        Verbosity = DotNetCoreVerbosity.Minimal,
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
    AzurePipelines.Commands
        .PublishCodeCoverage(new AzurePipelinesPublishCodeCoverageData {
            CodeCoverageTool = AzurePipelinesCodeCoverageToolType.Cobertura,
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
    var command = AzurePipelines.Commands;

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
    AzurePipelines.Commands
        .PublishTestResults(new AzurePipelinesPublishTestResultsData {
            TestResultsFiles = testResults,
            MergeTestResults = true,
            Configuration = configuration,
            TestRunTitle = runTitle,
            TestRunner = AzurePipelinesTestRunnerType.VSTest
        });
}

void Pack(
    FilePath nuspecPath,
    string configuration,
    DirectoryPath projectDir,
    DirectoryPath outputDir,
    string version,
    FilePath licensePath)
{
    projectDir = MakeAbsolute(projectDir);
    licensePath = MakeAbsolute(licensePath);

    var files = GetFiles($"{projectDir}/**/bin/{configuration}/**/{nuspecPath.GetFilenameWithoutExtension()}" + ".{dll,pdb,xml}")
        .Select(file => 
            new NuSpecContent 
            { 
                Source = file.FullPath,
                Target = file.FullPath.Replace(projectDir.FullPath, "").Replace($"bin/{configuration}", "lib") 
            })
        .ToList();

    files.Add(new NuSpecContent 
        {
            Source = licensePath.FullPath,
            Target = licensePath.FullPath.Replace(licensePath.FullPath, "")
        });
    
    NuGetPack(
        nuspecPath,
        new NuGetPackSettings
        {
            Version = version,
            OutputDirectory = outputDir,
            Files = files,
            Properties = new Dictionary<string, string> {
                { "title", $"{nuspecPath.GetFilenameWithoutExtension()}" },
                { "authors", "Nicola Biancolini" },
                { "owners", "Nicola Biancolini" },
                { "projectUrl", "https://github.com/cake-contrib/Cake.Board" },
                { "repositoryUrl", "https://github.com/cake-contrib/Cake.Board" },
                { "language", "en-US" },
                { "releaseNotes", $"https://github.com/cake-contrib/Cake.Board/releases/tag/{version}" }
            }
        });
}
