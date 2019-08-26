#load "./paths.cake"
#load "./version.cake"
#load "./utils.cake"
#load "./credentials.cake"

public class BuildParameters
{
    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public string Framework { get; private set; }

    public int CoverageThreshold { get; private set; }
    
    public bool EnabledUnitTests { get; private set; }
    public bool EnabledPublishNuget { get; private set; }

    public bool IsRunningOnUnix { get; private set; }
    public bool IsRunningOnWindows { get; private set; }
    public bool IsRunningOnLinux { get; private set; }
    public bool IsRunningOnMacOS { get; private set; }

    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnAzurePipeline { get; private set; }

    public bool IsMainRepo { get; private set; }
    public bool IsStableBranch { get; private set; }
    public bool IsPreviewBranch { get; private set; }
    public bool IsMainBranch { get; private set; }

    public bool IsTagged { get; private set; }
    public bool IsPullRequest { get; private set; }

    public DotNetCoreMSBuildSettings MSBuildSettings { get; private set; }
    public BuildPaths ArtifactPaths { get; private set; }
    public BuildVersion Version { get; private set; }
    public BuildCredentials Credentials { get; private set; }

    public bool IsStableRelease() => !IsLocalBuild && IsMainRepo && IsStableBranch && !IsPullRequest;
    public bool IsPreviewRelease() => !IsLocalBuild && IsMainRepo && IsPreviewBranch && !IsPullRequest;

    public Dictionary<string, object> ProcessVariables { get; private set; }

    /*
     * Get build parameters.
     */
    public static BuildParameters GetParameters(ICakeContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var buildSystem = context.BuildSystem();

        var coverageThreshold = Environment.GetEnvironmentVariable("COVERAGE_THRESHOLD");

        return new BuildParameters
        {
            Target = context.Argument("target", "Default"),
            Configuration = context.Argument("configuration", "Release"),
            Framework = context.Argument("framework", "netstandard2.0"),

            EnabledUnitTests = IsEnabled(context, "ENABLED_UNIT_TESTS"),
            EnabledPublishNuget = IsEnabled(context, "ENABLED_PUBLISH_NUGET"),

            CoverageThreshold = context.Argument("coverage-threshold", string.IsNullOrWhiteSpace(coverageThreshold) ? 0  : int.Parse(coverageThreshold)),

            IsRunningOnUnix = context.IsRunningOnUnix(),
            IsRunningOnWindows = context.IsRunningOnWindows(),
            IsRunningOnLinux = context.Environment.Platform.Family == PlatformFamily.Linux,
            IsRunningOnMacOS = context.Environment.Platform.Family == PlatformFamily.OSX,

            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnAzurePipeline = buildSystem.IsRunningOnAzurePipelinesHosted,

            IsMainRepo = IsOnMainRepo(context),
            IsMainBranch = IsOnBranch(context, new System.Text.RegularExpressions.Regex("master")),
            IsStableBranch = IsOnBranch(context, new System.Text.RegularExpressions.Regex(@"^stable\/\d[.]\d[.]\d")),
            IsPreviewBranch = IsOnBranch(context, new System.Text.RegularExpressions.Regex(@"^preview\/\d[.]\d[.]\d")),
            IsPullRequest = IsPullRequestBuild(context),
            IsTagged = IsBuildTagged(context),

            ProcessVariables = new Dictionary<string, object>()
        };
    }

    /*
     * Initialize build context.
     */
    public void Setup(
        ICakeContext context,
        GitVersion gitVersion,
        int? lineCoverageThreshold = null)
    {
        Version = BuildVersion.Calculate(context, this, gitVersion);
        CoverageThreshold = lineCoverageThreshold ?? CoverageThreshold;
        MSBuildSettings = GetMsBuildSettings(context, Version);

        ArtifactPaths = BuildPaths.GetPaths(context, Configuration, Version);
        Credentials = BuildCredentials.GetCredentials(context);
    }

    private DotNetCoreMSBuildSettings GetMsBuildSettings(
        ICakeContext context,
        BuildVersion version)
    {
        var msBuildSettings = new DotNetCoreMSBuildSettings()
                                .WithProperty("Version", version.SemVersion)
                                .WithProperty("IsBeta", version.Version.Contains("beta").ToString());

        if (!IsRunningOnWindows)
        {
            var frameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

            // Use FrameworkPathOverride when not running on Windows.
            context.Information("Build will use FrameworkPathOverride={0} since not building on Windows.", frameworkPathOverride);
            msBuildSettings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
        }

        return msBuildSettings;
    }

    private static bool IsOnMainRepo(ICakeContext context)
    {
        var buildSystem = context.BuildSystem();
        string repositoryName = null;

        if (buildSystem.IsRunningOnAzurePipelinesHosted)
            repositoryName = buildSystem.TFBuild.Environment.Repository.RepoName;

        if(!string.IsNullOrWhiteSpace(repositoryName))
            context.Information("Repository Name: {0}", repositoryName);

        return !string.IsNullOrWhiteSpace(repositoryName) && StringComparer.OrdinalIgnoreCase.Equals("cake-contrib/Cake.Board", repositoryName);
    }

    private static bool IsOnBranch(
        ICakeContext context,
        System.Text.RegularExpressions.Regex regex)
    {
        var buildSystem = context.BuildSystem();
        string repositoryBranch = null;

        if (buildSystem.IsRunningOnAzurePipelinesHosted)
            repositoryBranch = buildSystem.TFBuild.Environment.Repository.SourceBranch;

        if(!string.IsNullOrWhiteSpace(repositoryBranch))
            context.Information("Repository Branch: {0}", repositoryBranch);

        return !string.IsNullOrWhiteSpace(repositoryBranch) && regex.Match(repositoryBranch.Replace("refs/heads/", string.Empty)).Success;
    }

    private static bool IsPullRequestBuild(ICakeContext context)
    {
        var buildSystem = context.BuildSystem();

        if (buildSystem.IsRunningOnAzurePipelinesHosted)
            return buildSystem.TFBuild.Environment.PullRequest.IsPullRequest;
        return false;
    }

    private static bool IsBuildTagged(ICakeContext context)
    {
        var gitPath = context.Tools.Resolve(context.IsRunningOnWindows() ? "git.exe" : "git");
        context.StartProcess(gitPath, new ProcessSettings { Arguments = "rev-parse --verify HEAD", RedirectStandardOutput = true }, out var sha);
        context.StartProcess(gitPath, new ProcessSettings { Arguments = "tag --points-at " + sha.Single(), RedirectStandardOutput = true }, out var redirectedOutput);

        return redirectedOutput.Any();
    }

    private static bool IsEnabled(
        ICakeContext context,
        string envVar,
        bool nullOrEmptyAsEnabled = true)
    {
        var value = context.EnvironmentVariable(envVar);
        return string.IsNullOrWhiteSpace(value) ? nullOrEmptyAsEnabled : bool.Parse(value);
    }
}
