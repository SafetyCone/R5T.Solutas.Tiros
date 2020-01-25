using System;

using R5T.Cambridge.Types;


namespace R5T.Solutas.Tiros
{
    public static class SolutionBuildConfigurationPlatformExtensions
    {
        public static string ToSolutionFileLine(this SolutionBuildConfigurationPlatform solutionBuildConfigurationPlatform)
        {
            var solutionBuildConfigurationToken = solutionBuildConfigurationPlatform.Source.ToSolutionFileToken();
            var mappedSolutionBuildConfigurationToken = solutionBuildConfigurationPlatform.Destination.ToSolutionFileToken();

            var token = $"{solutionBuildConfigurationToken} = {mappedSolutionBuildConfigurationToken}";
            return token;
        }
    }
}
