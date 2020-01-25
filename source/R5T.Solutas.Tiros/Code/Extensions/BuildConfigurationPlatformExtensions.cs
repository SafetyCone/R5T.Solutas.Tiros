using System;

using R5T.Cambridge.Types;


namespace R5T.Solutas.Tiros
{
    public static class BuildConfigurationPlatformExtensions
    {
        public static string ToSolutionFileToken(this BuildConfigurationPlatform buildConfigurationPlatform)
        {
            var buildConfigurationToken = buildConfigurationPlatform.BuildConfiguration.ToSolutionFileToken();
            var platformTargetToken = buildConfigurationPlatform.Platform.ToSolutionFileToken();

            var token = $"{buildConfigurationToken}{Constants.SolutionBuildConfigurationTokenSeparator}{platformTargetToken}";
            return token;
        }
    }
}
