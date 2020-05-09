using System;

using R5T.Angleterria;


namespace R5T.Solutas.Tiros
{
    class Constants
    {
        // Solution file content constants.
        public const string FormatVersion = "Format Version";
        public const string HideSolutionNode = "HideSolutionNode";
        public const string MicrosoftVisualStudioSolutionFile = "Microsoft Visual Studio Solution File";
        public const string MinimumVisualStudioVersion = "MinimumVisualStudioVersion";
        public const string SolutionGuid = "SolutionGuid";
        public const string VisualStudioVersion = "VisualStudioVersion";

        public const string Global = "Global";
        public const string EndGlobal = "EndGlobal";
        public const string GlobalSection = "GlobalSection";
        public const string EndGlobalSection = "EndGlobalSection";
        public const string Project = "Project";
        public const string EndProject = "EndProject";

        public const string DependenciesSolutionFolderName = "_Dependencies";
        public const string DebugSolutionFileBuildConfigurationToken = BuildConfigurationName.Debug;
        public const string ReleaseSolutionFileBuildConfigurationToken = BuildConfigurationName.Release;
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
