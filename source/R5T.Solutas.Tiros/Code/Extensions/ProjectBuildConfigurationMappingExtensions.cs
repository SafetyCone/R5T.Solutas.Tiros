using System;

using R5T.Cambridge.Types;


namespace R5T.Solutas.Tiros
{
    public static class ProjectBuildConfigurationMappingExtensions
    {
        public static string ToSolutionFileLine(this ProjectBuildConfigurationMapping projectBuildConfigurationMapping)
        {
            var projectGuidToken = projectBuildConfigurationMapping.ProjectGUID.ToStringSolutionFileFormat();
            var solutionBuildConfigurationToken = projectBuildConfigurationMapping.SolutionBuildConfiguration.ToSolutionFileToken();
            var indicatorToken = projectBuildConfigurationMapping.ProjectConfigurationIndicator.ToSolutionFileToken();

            var targetToken = $"{projectGuidToken}{Constants.SolutionProjectConfigurationTokenSeparator}{solutionBuildConfigurationToken}{Constants.SolutionProjectConfigurationTokenSeparator}{indicatorToken}";

            var mappingToken = projectBuildConfigurationMapping.MappedSolutionBuildConfiguration.ToSolutionFileToken();

            var line = $"{targetToken} = {mappingToken}";
            return line;
        }
    }
}
