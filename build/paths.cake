#load "./version.cake"

public class BuildPaths
{
    public BuildFiles Files { get; private set; }
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(
        ICakeContext context,
        string configuration,
        BuildVersion version)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (string.IsNullOrWhiteSpace(configuration))
            throw new ArgumentNullException(nameof(configuration));
        if (version == null)
            throw new ArgumentNullException(nameof(version));

        var rootDir = (DirectoryPath)(context.Directory("./.artifacts") + context.Directory("v" + version.SemVersion));
        var binDir = rootDir.Combine("bin");
        
        var outputDir = rootDir.Combine("output");

        var licensePath = $"{outputDir.FullPath}/LICENSE.txt";
        var releaseNotesPath = $"{outputDir.FullPath}/release-notes.md";

        var testResultDir = rootDir.Combine("test-results");
        var testCoverageDir = rootDir.Combine("code-coverage");
        var testConverageResultsDir = testCoverageDir.Combine("results");
        
        var nugetDir = outputDir.Combine("nuget");

        // Directories
        var buildDirectories = new BuildDirectories(
            rootDir,
            binDir,
            outputDir,
            testResultDir,
            testCoverageDir,
            testConverageResultsDir,
            nugetDir);

        // Files
        var buildFiles = new BuildFiles(
            context,
            licensePath,
            releaseNotesPath);

        return new BuildPaths
        {
            Files = buildFiles,
            Directories = buildDirectories
        };
    }
}

public class BuildFiles
{
    public FilePath License { get; private set; }
    public FilePath ReleaseNotes { get; private set; }
    
    public BuildFiles(
        ICakeContext context,
        FilePath license,
        FilePath releaseNotes)
    {
        License = license;
        ReleaseNotes = releaseNotes;
    }
}

public class BuildDirectories
{
    public DirectoryPath Root { get; private set; }
    public DirectoryPath Bin { get; private set; }
    public DirectoryPath Output { get; private set; }
    public DirectoryPath TestResult { get; private set; }
    public DirectoryPath TestCoverage { get; private set; }
    public DirectoryPath TestCoverageResults { get; private set; } 
    public DirectoryPath Nuget { get; private set; }
    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath root,
        DirectoryPath bin,
        DirectoryPath output,
        DirectoryPath testResult,
        DirectoryPath testCoverage,
        DirectoryPath testCoverageResults,
        DirectoryPath nuget)
    {
        Root = root;
        Bin = bin;
        Output = output;
        TestResult = testResult;
        TestCoverage = testCoverage;
        TestCoverageResults = testCoverageResults;
        Nuget = nuget;
        ToClean = new[]
        {
            Root,
            Bin,
            Output,
            TestResult,
            TestCoverage,
            TestCoverageResults,
            Nuget
        };
    }
}
