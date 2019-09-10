#load "./parameters.cake"

public class BuildVersion
{
    public GitVersion GitVersion { get; private set; }
    public string Version { get; private set; }
    public string SemVersion { get; private set; }
    public string NuGetVersion { get; private set; }

    public static BuildVersion Calculate(ICakeContext context, BuildParameters parameters, GitVersion gitVersion)
    {
        var semVersion = $"{gitVersion.Major}.{gitVersion.Minor}.{gitVersion.Patch + gitVersion.PreReleaseNumber}";
        var versionSuffix = parameters.IsStableRelease() ? "" : "-beta";
        var version = $"{semVersion}{versionSuffix}";

        return new BuildVersion
        {
            GitVersion = gitVersion,
            Version = version,
            SemVersion = semVersion,
            NuGetVersion = version,
        };
    }
}
