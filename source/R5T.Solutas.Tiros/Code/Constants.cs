using System;


namespace R5T.Solutas.Tiros
{
    class Constants
    {
        public const string DependenciesSolutionFolderName = "_Dependencies";
        public const string DebugSolutionFileBuildConfigurationToken = "Debug";
        public const string ReleaseSolutionFileBuildConfigurationToken = "Release";
        public const string AnyCpuSolutionFileReleasePlatformToken = "Any CPU";
        public const string x86SolutionFileReleasePlatformToken = "x86";
        public const string x64SolutionFileReleasePlatformToken = "x64";
        public const string ActiveCfgSolutionFileToken = "ActiveCfg";
        public const string Build0SolutionFileToken = "Build.0";

        public const string SolutionBuildConfigurationTokenSeparator = "|";
        public const string SolutionProjectConfigurationTokenSeparator = ".";

        public static readonly Guid SolutionFolderProjectTypeGUID = Guid.Parse("2150E333-8FDC-42A3-9474-1A3956D46DE8");

        public static readonly Guid NetStandardLibraryProjectTypeGUID = Guid.Parse("9A19103F-16F7-4668-BE54-9A1E7A4F7556");
    }
}
