#load "./parameters.cake"

public class BuildVersion
{
    public string Version { get; private set; }
    public string SemVersion { get; private set; }
    public string NuGetVersion { get; private set; }

    public static BuildVersion Calculate(ICakeContext context, BuildParameters parameters, string version)
    {
        var versionSuffix = parameters.IsStableBranch ? "" : "beta";
        var semVersion = System.Text.RegularExpressions.Regex.Match(version, @"([\d].[\d].[\d])", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Value;
        var nugetVersion = $"{semVersion}-{versionSuffix}";

        return new BuildVersion
        {
            Version = version,
            SemVersion = semVersion,
            NuGetVersion = nugetVersion
        };
    }
}
