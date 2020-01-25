using System;

using R5T.Cambridge.Types;
using R5T.Magyar;


namespace R5T.Solutas.Tiros
{
    public static class BuildConfigurationExtensions
    {
        public static string ToSolutionFileToken(this BuildConfiguration buildConfiguration)
        {
            switch (buildConfiguration)
            {
                case BuildConfiguration.Debug:
                    return Constants.DebugSolutionFileBuildConfigurationToken;

                case BuildConfiguration.Release:
                    return Constants.ReleaseSolutionFileBuildConfigurationToken;

                default:
                    throw new Exception(EnumHelper.UnexpectedEnumerationValueMessage(buildConfiguration));
            }
        }
    }
}
