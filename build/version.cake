#load "./parameters.cake"

public class BuildVersion
{
    public GitVersion GitVersion { get; private set; }
    public string Version { get; private set; }
    public string SemVersion { get; private set; }
    public string NuGetVersion { get; private set; }

    public static BuildVersion Calculate(ICakeContext context, BuildParameters parameters, GitVersion gitVersion)
    {
        var versionSuffix = parameters.IsStableBranch ? "" : "-beta";
        var version = $"{gitVersion.MajorMinorPatch}{versionSuffix}";
        var semVersion = gitVersion.MajorMinorPatch;

        return new BuildVersion
        {
            GitVersion = gitVersion,
            Version = version,
            SemVersion = semVersion,
            NuGetVersion = version,
        };
    }
}
