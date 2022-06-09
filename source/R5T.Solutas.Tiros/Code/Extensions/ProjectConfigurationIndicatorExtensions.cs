using System;

using R5T.Cambridge.Types;
using R5T.Magyar;


namespace R5T.Solutas.Tiros
{
    public static class ProjectConfigurationIndicatorExtensions
    {
        public static string ToSolutionFileToken(this ProjectConfigurationIndicator projectConfigurationIndicator)
        {
            switch (projectConfigurationIndicator)
            {
                case ProjectConfigurationIndicator.ActiveCfg:
                    return Constants.ActiveCfgSolutionFileToken;

                case ProjectConfigurationIndicator.Build0:
                    return Constants.Build0SolutionFileToken;

                default:
                    throw new Exception(EnumerationHelper.UnexpectedEnumerationValueMessage(projectConfigurationIndicator));
            }
        }
    }
}
