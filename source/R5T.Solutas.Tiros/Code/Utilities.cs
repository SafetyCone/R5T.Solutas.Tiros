using System;

using R5T.Cambridge.Types;
using R5T.Magyar;


namespace R5T.Solutas.Tiros
{
    public static class Utilities
    {
        public static PreOrPostSolution ToPreOrPostSolution(string value)
        {
            switch (value)
            {
                case "preSolution":
                    return PreOrPostSolution.PreSolution;

                case "postSolution":
                    return PreOrPostSolution.PostSolution;

                default:
                    throw new Exception(EnumerationHelper.UnrecognizedEnumerationValueMessage<PreOrPostSolution>(value));
            }
        }

        public static string ToStringStandard(PreOrPostSolution preOrPostSolution)
        {
            switch (preOrPostSolution)
            {
                case PreOrPostSolution.PreSolution:
                    return "preSolution";

                case PreOrPostSolution.PostSolution:
                    return "postSolution";

                default:
                    throw new Exception(EnumerationHelper.UnexpectedEnumerationValueMessage(preOrPostSolution));
            }
        }

        public static BuildConfiguration ToBuildConfiguration(string value)
        {
            switch (value)
            {
                case Constants.DebugSolutionFileBuildConfigurationToken:
                    return BuildConfiguration.Debug;

                case Constants.ReleaseSolutionFileBuildConfigurationToken:
                    return BuildConfiguration.Release;

                default:
                    throw new Exception(EnumerationHelper.UnrecognizedEnumerationValueMessage<BuildConfiguration>(value));
            }
        }

        public static Platform ToPlatform(string value)
        {
            switch (value)
            {
                case Constants.AnyCpuSolutionFileReleasePlatformToken:
                    return Platform.AnyCPU;

                case Constants.x86SolutionFileReleasePlatformToken:
                    return Platform.x86;

                case Constants.x64SolutionFileReleasePlatformToken:
                    return Platform.x64;

                default:
                    throw new Exception(EnumerationHelper.UnrecognizedEnumerationValueMessage<Platform>(value));
            }
        }

        public static ProjectConfigurationIndicator ToProjectConfigurationIndicator(string value)
        {
            switch (value)
            {
                case Constants.ActiveCfgSolutionFileToken:
                    return ProjectConfigurationIndicator.ActiveCfg;

                case Constants.Build0SolutionFileToken:
                    return ProjectConfigurationIndicator.Build0;

                default:
                    throw new Exception(EnumerationHelper.UnrecognizedEnumerationValueMessage<ProjectConfigurationIndicator>(value));
            }
        }
    }
}
