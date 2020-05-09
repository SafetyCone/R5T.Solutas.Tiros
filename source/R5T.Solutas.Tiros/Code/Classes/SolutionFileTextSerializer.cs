using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using R5T.Cambridge.Types;
using R5T.Magyar;
using R5T.Magyar.Extensions;
using R5T.Magyar.IO;

using SolutionUtilities = R5T.Solutas.Tiros.Utilities;


namespace R5T.Solutas.Tiros
{
    public static class SolutionFileTextSerializer
    {
        public static readonly string ProjectLineRegexPattern = $@"^{Constants.Project}";
        public static readonly string ProjectLineEndRegexPattern = $@"^{Constants.EndProject}";
        public static readonly string GlobalLineRegexPattern = $@"^{Constants.Global}";
        public static readonly string GlobalEndLineRegexPattern = $@"^{Constants.EndGlobal}($|\s)";
        public static readonly string GlobalSectionRegexPattern = $@"^{Constants.GlobalSection}";
        public static readonly string GlobalSectionEndRegexPattern = $@"^{Constants.EndGlobalSection}";

        public const string ProjectLineValuesRegexPattern = @"""([^""]*)"""; // Find matches in quotes, but includes the quotes.
        public const string GlobalSectionLineValuesRegexPattern = @"\(.*\)|(?<== ).*";


        private static Regex ProjectLineRegex { get; } = new Regex(SolutionFileTextSerializer.ProjectLineRegexPattern);
        private static Regex ProjectLineEndRegex { get; } = new Regex(SolutionFileTextSerializer.ProjectLineEndRegexPattern);
        private static Regex GlobalLineRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalLineRegexPattern);
        private static Regex GlobalLineEndRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalEndLineRegexPattern);
        private static Regex GlobalSectionRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalSectionRegexPattern);
        private static Regex GlobalSectionEndRegex { get; } = new Regex(SolutionFileTextSerializer.GlobalSectionEndRegexPattern);


        private static void SerializeGlobals(TextWriter textWriter, SolutionFile solutionFile)
        {
            var writer = new TabinatedWriter(textWriter);

            var globalSections = new List<ISolutionFileGlobalSection>(solutionFile.GlobalSections);
            var globalSectionsInOrder = new List<ISolutionFileGlobalSection>();

            // SolutionConfigurationPlatforms.
            var hasSolutionConfigurationPlatforms = globalSections.HasSolutionConfigurationPlatformsGlobalSection(out var solutionConfigurationPlatforms);
            if(hasSolutionConfigurationPlatforms)
            {
                globalSectionsInOrder.Add(solutionConfigurationPlatforms);
                globalSections.Remove(solutionConfigurationPlatforms);
            }

            // ProjectConfigurationPlatforms.
            var hasProjectConfigurationPlatforms = globalSections.HasProjectConfigurationPlatformsGlobalSection(out var projectConfigurationPlatforms); // Can have.
            if (hasProjectConfigurationPlatforms)
            {
                globalSectionsInOrder.Add(projectConfigurationPlatforms);
                globalSections.Remove(projectConfigurationPlatforms);
            }

            // Solution properties.
            var solutionProperties = globalSections.GetSolutionPropertiesGlobalSection(); // Must have.

            globalSectionsInOrder.Add(solutionProperties);
            globalSections.Remove(solutionProperties);

            // Nested projects.
            var hasNestedProjects = globalSections.HasNestedProjectsGlobalSection(out var nestedProjects); // Can have.
            if (hasNestedProjects)
            {
                globalSectionsInOrder.Add(nestedProjects);
                globalSections.Remove(nestedProjects);
            }

            // Extensibility globals.
            var hasExtensibilityGlobals = globalSections.HasGlobalSectionByName<GenericSolutionFileGlobalSection>(ExtensibilityGlobalsGlobalSection.GlobalSectionName, out var extensibilityGlobals); // Can have.
            if (hasExtensibilityGlobals)
            {
                globalSectionsInOrder.Add(extensibilityGlobals);
                globalSections.Remove(extensibilityGlobals);
            }

            // All others that remain.
            globalSectionsInOrder.AddRange(globalSections);

            writer.WriteLine(Constants.Global);

            writer.IncreaseTabination();

            foreach (var globalSection in globalSectionsInOrder)
            {
                SolutionFileTextSerializer.SerializeGlobal(writer, globalSection);
            }

            writer.DecreaseTabination();

            writer.WriteLine(Constants.EndGlobal);
        }

        private static void SerializeGlobal(TabinatedWriter writer, ISolutionFileGlobalSection globalSection)
        {
            var globalSectionLine = $"{Constants.GlobalSection}({globalSection.Name}) = {SolutionUtilities.ToStringStandard(globalSection.PreOrPostSolution)}";
            writer.WriteLine(globalSectionLine);

            writer.IncreaseTabination();

            var lines = SolutionFileTextSerializer.SerializeGlobal(globalSection);
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }            

            writer.DecreaseTabination();

            writer.WriteLine(Constants.EndGlobalSection);
        }

        private static IEnumerable<string> SerializeGlobal(ISolutionFileGlobalSection globalSection)
        {
            IEnumerable<string> lines;
            switch (globalSection)
            {
                case GenericSolutionFileGlobalSection genericGlobalSection:
                    lines = SolutionFileTextSerializer.SerializeGenericGlobalSection(genericGlobalSection);
                    break;

                case NestedProjectsGlobalSection nestedGlobalSection:
                    lines = SolutionFileTextSerializer.SerializeNestedProjectsGlobalSection(nestedGlobalSection);
                    break;

                case ProjectConfigurationPlatformsGlobalSection projectConfigurationPlatformsGlobalSection:
                    lines = SolutionFileTextSerializer.SerializeProjectConfigurationPlatformsGlobalSection(projectConfigurationPlatformsGlobalSection);
                    break;

                case SolutionConfigurationPlatformsGlobalSection solutionConfigurationPlatformsGlobalSection:
                    lines = SolutionFileTextSerializer.SerializeSolutionConfigurationPlatformsGlobalSection(solutionConfigurationPlatformsGlobalSection);
                    break;

                case ExtensibilityGlobalsGlobalSection extensibilityGlobalsGlobalSection:
                    lines = SolutionFileTextSerializer.SerializeExtensibilityGlobalsGlobalSection(extensibilityGlobalsGlobalSection);
                    break;

                case SolutionPropertiesGlobalSection solutionPropertiesGlobalSection:
                    lines = SolutionFileTextSerializer.SerializeSolutionPropertiesGlobalSection(solutionPropertiesGlobalSection);
                    break;

                default:
                    throw new Exception($"Unserializable global section: {globalSection.Name} (Type: {globalSection.GetType()}).");
            }

            return lines;
        }

        private static IEnumerable<string> SerializeSolutionPropertiesGlobalSection(SolutionPropertiesGlobalSection solutionPropertiesGlobalSection)
        {
            var hideSolutionNode = $"{Constants.HideSolutionNode} = {solutionPropertiesGlobalSection.HideSolutionNode.ToStringSolutionFileFormat()}";
            yield return hideSolutionNode;
        }

        private static IEnumerable<string> SerializeExtensibilityGlobalsGlobalSection(ExtensibilityGlobalsGlobalSection extensibilityGlobalsGlobalSection)
        {
            var solutionGuid = $"{Constants.SolutionGuid} = {extensibilityGlobalsGlobalSection.SolutionGuid.ToStringSolutionFileFormat()}";
            yield return solutionGuid;
        }

        private static IEnumerable<string> SerializeSolutionConfigurationPlatformsGlobalSection(SolutionConfigurationPlatformsGlobalSection solutionConfigurationPlatformsGlobalSection)
        {
            foreach (var solutionBuildConfigurationMapping in solutionConfigurationPlatformsGlobalSection.SolutionBuildConfigurationMappings)
            {
                var line = solutionBuildConfigurationMapping.ToSolutionFileLine();
                yield return line;
            }
        }

        private static IEnumerable<string> SerializeProjectConfigurationPlatformsGlobalSection(ProjectConfigurationPlatformsGlobalSection projectConfigurationPlatformsGlobalSection)
        {
            foreach (var projectBuildConfigurationMapping in projectConfigurationPlatformsGlobalSection.ProjectBuildConfigurationMappings)
            {
                var line = projectBuildConfigurationMapping.ToSolutionFileLine();
                yield return line;
            }
        }

        private static IEnumerable<string> SerializeNestedProjectsGlobalSection(NestedProjectsGlobalSection nestedProjectsGlobalSection)
        {
            foreach (var projectNesting in nestedProjectsGlobalSection.ProjectNestings)
            {
                var line = SolutionFileTextSerializer.SerializeProjectNesting(projectNesting);
                yield return line;
            }
        }

        private static IEnumerable<string> SerializeGenericGlobalSection(GenericSolutionFileGlobalSection genericGlobalSection)
        {
            return genericGlobalSection.ContentLines;
        }

        private static void SerializeProjectReferences(TextWriter writer, SolutionFile solutionFile)
        {
            foreach (var solutionProjectFileReference in solutionFile.SolutionFileProjectReferences)
            {
                var projectLine = $@"{Constants.Project}(""{solutionProjectFileReference.ProjectTypeGUID.ToStringSolutionFileFormat()}"") = ""{solutionProjectFileReference.ProjectName}"", ""{solutionProjectFileReference.ProjectFileRelativePathValue}"", ""{solutionProjectFileReference.ProjectGUID.ToStringSolutionFileFormat()}""";
                writer.WriteLine(projectLine);

                writer.WriteLine(Constants.EndProject);
            }
        }

        private static void DeserializeGlobals(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!SolutionFileTextSerializer.GlobalLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Global\".\nFound: {currentLine}");
            }

            currentLine = reader.ReadLine().Trim();

            while (!SolutionFileTextSerializer.GlobalLineEndRegex.IsMatch(currentLine))
            {
                SolutionFileTextSerializer.DeserializeGlobal(reader, ref currentLine, solutionFile);

                currentLine = reader.ReadLine().Trim();
            }
        }

        private static void DeserializeGlobal(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!SolutionFileTextSerializer.GlobalSectionRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"GlobalSection\".\nFound: {currentLine}");
            }

            var globalSectionMatches = Regex.Matches(currentLine, SolutionFileTextSerializer.GlobalSectionLineValuesRegexPattern);

            var sectionName = globalSectionMatches[0].Value.TrimStart('(').TrimEnd(')');
            var preOrPostSolutionStr = globalSectionMatches[1].Value;

            var preOrPostSolution = SolutionUtilities.ToPreOrPostSolution(preOrPostSolutionStr);

            ISolutionFileGlobalSection globalSection;
            switch (sectionName)
            {
                case SolutionConfigurationPlatformsGlobalSection.GlobalSectionName:
                    globalSection = SolutionFileTextSerializer.DeserializeSolutionConfigurationPlatformsGlobalSection(reader, ref currentLine, preOrPostSolution);
                    break;

                case ProjectConfigurationPlatformsGlobalSection.GlobalSectionName:
                    globalSection = SolutionFileTextSerializer.DeserializeProjectConfigurationPlatformsGlobalSection(reader, ref currentLine, preOrPostSolution);
                    break;

                case NestedProjectsGlobalSection.GlobalSectionName:
                    globalSection = SolutionFileTextSerializer.DeserializeNestedProjectsGlobalSection(reader, ref currentLine, preOrPostSolution);
                    break;

                case SolutionPropertiesGlobalSection.GlobalSectionName:
                    globalSection = SolutionFileTextSerializer.DeserializeSolutionPropertiesGlobalSection(reader, ref currentLine, preOrPostSolution);
                    break;

                case ExtensibilityGlobalsGlobalSection.GlobalSectionName:
                    globalSection = SolutionFileTextSerializer.DeserializeExtensibilityGlobalsGlobalSection(reader, ref currentLine, preOrPostSolution);
                    break;

                default:
                    globalSection = SolutionFileTextSerializer.DeserializeGeneralGlobal(reader, ref currentLine, sectionName, preOrPostSolution);
                    break;
            }
            solutionFile.GlobalSections.Add(globalSection);
        }

        private static ProjectConfigurationPlatformsGlobalSection DeserializeProjectConfigurationPlatformsGlobalSection(TextReader reader, ref string currentLine, PreOrPostSolution preOrPostSolution)
        {
            var projectConfigurationPlatformsGlobalSection = new ProjectConfigurationPlatformsGlobalSection
            {
                Name = ProjectConfigurationPlatformsGlobalSection.GlobalSectionName,
                PreOrPostSolution = preOrPostSolution
            };

            currentLine = reader.ReadLine().Trim();

            while (!SolutionFileTextSerializer.GlobalSectionEndRegex.IsMatch(currentLine))
            {
                var assignmentTokens = currentLine.Split("=");

                var targetToken = assignmentTokens[0].Trim();
                var valueToken = assignmentTokens[1].Trim();

                var projectBuildConfigurationTokens = targetToken.Split(new string[] { Constants.SolutionProjectConfigurationTokenSeparator }, 3, StringSplitOptions.None);

                var projectGuidToken = projectBuildConfigurationTokens[0];
                var solutionBuildConfigurationToken = projectBuildConfigurationTokens[1];
                var indicatorToken = projectBuildConfigurationTokens[2];

                var projectGUID = Guid.Parse(projectGuidToken);
                var solutionBuildConfiguration = SolutionFileTextSerializer.DeserializeBuildConfigurationPlatform(solutionBuildConfigurationToken);
                var indicator = SolutionUtilities.ToProjectConfigurationIndicator(indicatorToken);
                var mappedSolutionBuildConfiguration = SolutionFileTextSerializer.DeserializeBuildConfigurationPlatform(valueToken);

                var projectBuildConfigurationMapping = new ProjectBuildConfigurationMapping
                {
                    ProjectGUID = projectGUID,
                    SolutionBuildConfiguration = solutionBuildConfiguration,
                    ProjectConfigurationIndicator = indicator,
                    MappedSolutionBuildConfiguration = mappedSolutionBuildConfiguration,
                };

                projectConfigurationPlatformsGlobalSection.ProjectBuildConfigurationMappings.Add(projectBuildConfigurationMapping);

                currentLine = reader.ReadLine().Trim();
            }

            return projectConfigurationPlatformsGlobalSection;
        }

        private static SolutionConfigurationPlatformsGlobalSection DeserializeSolutionConfigurationPlatformsGlobalSection(TextReader reader, ref string currentLine, PreOrPostSolution preOrPostSolution)
        {
            var solutionConfigurationPlatformsGlobalSection = new SolutionConfigurationPlatformsGlobalSection
            {
                Name = SolutionConfigurationPlatformsGlobalSection.GlobalSectionName,
                PreOrPostSolution = preOrPostSolution
            };

            currentLine = reader.ReadLine().Trim();

            while (!SolutionFileTextSerializer.GlobalSectionEndRegex.IsMatch(currentLine))
            {
                var assignmentTokens = currentLine.Split("=");

                var targetToken = assignmentTokens[0].Trim();
                var valueToken = assignmentTokens[1].Trim();

                var solutionBuildConfiguration = SolutionFileTextSerializer.DeserializeBuildConfigurationPlatform(targetToken);
                var mappedSolutionBuildConfiguration = SolutionFileTextSerializer.DeserializeBuildConfigurationPlatform(valueToken);

                var solutionBuildConfigurationMapping = new SolutionBuildConfigurationPlatform
                {
                    Source = solutionBuildConfiguration,
                    Destination = mappedSolutionBuildConfiguration,
                };

                solutionConfigurationPlatformsGlobalSection.SolutionBuildConfigurationMappings.Add(solutionBuildConfigurationMapping);

                currentLine = reader.ReadLine().Trim();
            }

            return solutionConfigurationPlatformsGlobalSection;
        }

        private static BuildConfigurationPlatform DeserializeBuildConfigurationPlatform(string token)
        {
            var tokens = token.Split(Constants.SolutionBuildConfigurationTokenSeparator);

            var buildConfigurationToken = tokens[0].Trim();
            var platformTargetToken = tokens[1].Trim();

            var buildConfiguration = SolutionUtilities.ToBuildConfiguration(buildConfigurationToken);
            var platform = SolutionUtilities.ToPlatform(platformTargetToken);

            var solutionBuildConfiguration = new BuildConfigurationPlatform
            {
                BuildConfiguration = buildConfiguration,
                Platform = platform,
            };
            return solutionBuildConfiguration;
        }

        private static NestedProjectsGlobalSection DeserializeNestedProjectsGlobalSection(TextReader reader, ref string currentLine, PreOrPostSolution preOrPostSolution)
        {
            var nestedProjectGlobalSection = new NestedProjectsGlobalSection
            {
                Name = NestedProjectsGlobalSection.GlobalSectionName,
                PreOrPostSolution = preOrPostSolution
            };

            currentLine = reader.ReadLine().Trim();

            while (!SolutionFileTextSerializer.GlobalSectionEndRegex.IsMatch(currentLine))
            {
                var projectNesting = SolutionFileTextSerializer.DeserializeProjectNesting(currentLine);
                nestedProjectGlobalSection.ProjectNestings.Add(projectNesting);

                currentLine = reader.ReadLine().Trim();
            }

            return nestedProjectGlobalSection;
        }

        private static SolutionPropertiesGlobalSection DeserializeSolutionPropertiesGlobalSection(TextReader reader, ref string currentLine, PreOrPostSolution preOrPostSolution)
        {
            var solutionPropertiesGlobalSection = new SolutionPropertiesGlobalSection()
            {
                Name = SolutionPropertiesGlobalSection.GlobalSectionName,
                PreOrPostSolution = preOrPostSolution,
            };

            currentLine = reader.ReadLine().Trim();

            var tokens = currentLine.Split(' ');

            var hideSolutionNodeValueToken = tokens[2];

            var hideSolutionNodeValue = Boolean.Parse(hideSolutionNodeValueToken);

            solutionPropertiesGlobalSection.HideSolutionNode = hideSolutionNodeValue;

            currentLine = reader.ReadLine().Trim();
            if(!SolutionFileTextSerializer.GlobalSectionEndRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown data in {SolutionPropertiesGlobalSection.GlobalSectionName} global section:\n{currentLine}");
            }

            return solutionPropertiesGlobalSection;
        }

        private static ExtensibilityGlobalsGlobalSection DeserializeExtensibilityGlobalsGlobalSection(TextReader reader, ref string currentLine, PreOrPostSolution preOrPostSolution)
        {
            var extensibilityGlobalsGlobalSection = new ExtensibilityGlobalsGlobalSection()
            {
                Name = ExtensibilityGlobalsGlobalSection.GlobalSectionName,
                PreOrPostSolution = preOrPostSolution,
            };

            currentLine = reader.ReadLine().Trim();

            var tokens = currentLine.Split(' ');

            var solutionGuidValueToken = tokens[2];

            var solutionGuidValue = Guid.Parse(solutionGuidValueToken);

            extensibilityGlobalsGlobalSection.SolutionGuid = solutionGuidValue;

            currentLine = reader.ReadLine().Trim();
            if (!SolutionFileTextSerializer.GlobalSectionEndRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown data in {ExtensibilityGlobalsGlobalSection.GlobalSectionName} global section:\n{currentLine}");
            }

            return extensibilityGlobalsGlobalSection;
        }

        private static GenericSolutionFileGlobalSection DeserializeGeneralGlobal(TextReader reader, ref string currentLine, string sectionName, PreOrPostSolution preOrPostSolution)
        {
            var globalSection = new GenericSolutionFileGlobalSection
            {
                Name = sectionName,
                PreOrPostSolution = preOrPostSolution,
            };

            currentLine = reader.ReadLine().Trim();

            while (!SolutionFileTextSerializer.GlobalSectionEndRegex.IsMatch(currentLine))
            {
                globalSection.ContentLines.Add(currentLine);

                currentLine = reader.ReadLine().Trim();
            }

            return globalSection;
        }

        private static void DeserializeProjects(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!SolutionFileTextSerializer.ProjectLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Project...\".\nFound: {currentLine}");
            }

            while (!SolutionFileTextSerializer.GlobalLineRegex.IsMatch(currentLine))
            {
                SolutionFileTextSerializer.DeserializeProject(reader, ref currentLine, solutionFile);

                currentLine = reader.ReadLine();
            }
        }

        private static void DeserializeProject(TextReader reader, ref string currentLine, SolutionFile solutionFile)
        {
            if (!SolutionFileTextSerializer.ProjectLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Project...\".\nFound: {currentLine}");
            }

            var matches = Regex.Matches(currentLine, SolutionFileTextSerializer.ProjectLineValuesRegexPattern);

            var projectTypeGUIDStr = matches[0].Value.Trim('"');
            var projectName = matches[1].Value.Trim('"');
            var projectFileRelativePathValue = matches[2].Value.Trim('"');
            var projectGUIDStr = matches[3].Value.Trim('"');

            var projectTypeGUID = Guid.Parse(projectTypeGUIDStr);
            var projectGUID = Guid.Parse(projectGUIDStr);

            var solutionProjectFileReference = new SolutionFileProjectReference
            {
                ProjectTypeGUID = projectTypeGUID,
                ProjectName = projectName,
                ProjectFileRelativePathValue = projectFileRelativePathValue,
                ProjectGUID = projectGUID
            };

            solutionFile.SolutionFileProjectReferences.Add(solutionProjectFileReference);

            currentLine = reader.ReadLine();
            if (!SolutionFileTextSerializer.ProjectLineEndRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"EndProject\".\nFound: {currentLine}");
            }
        }

        public static ProjectNesting DeserializeProjectNesting(string line)
        {
            var matches = Regex.Matches(line, @"{[^}]*}");

            var projectGuidStr = matches[0].Value;
            var projectParentGuidStr = matches[1].Value;

            var projectGUID = Guid.Parse(projectGuidStr);
            var projectParentGuid = Guid.Parse(projectParentGuidStr);

            var projectNesting = new ProjectNesting
            {
                ChildProjectGUID = projectGUID,
                ParentProjectGUID = projectParentGuid
            };
            return projectNesting;
        }

        public static string SerializeProjectNesting(ProjectNesting projectNesting)
        {
            var line = $"{projectNesting.ChildProjectGUID.ToStringSolutionFileFormat()} = {projectNesting.ParentProjectGUID.ToStringSolutionFileFormat()}";
            return line;
        }

        public static SolutionFile Deserialize(TextReader reader)
        {
            var solutionFile = new SolutionFile();

            var blankBeginLine = reader.ReadLine();
            var formatVersionLine = reader.ReadLine();
            var monikerLine = reader.ReadLine();
            var vsVersionLine = reader.ReadLine();
            var vsMinimumVersionLine = reader.ReadLine();

            var formatVersionTokens = formatVersionLine.Split(' ');
            var formatVersionToken = formatVersionTokens.Last();
            solutionFile.FormatVersion = Version.Parse(formatVersionToken);

            solutionFile.VisualStudioMoniker = monikerLine;

            var vsVersionTokens = vsVersionLine.Split(' ');
            var vsVersionToken = vsVersionTokens.Last();
            solutionFile.VisualStudioVersion = Version.Parse(vsVersionToken);

            var vsMinimumVersionTokens = vsMinimumVersionLine.Split(' ');
            var vsMinimumVersionToken = vsMinimumVersionTokens.Last();
            solutionFile.MinimumVisualStudioVersion = Version.Parse(vsMinimumVersionToken);

            var currentLine = reader.ReadLine();

            if (SolutionFileTextSerializer.ProjectLineRegex.IsMatch(currentLine))
            {
                SolutionFileTextSerializer.DeserializeProjects(reader, ref currentLine, solutionFile);
            }

            if (!SolutionFileTextSerializer.GlobalLineRegex.IsMatch(currentLine))
            {
                throw new Exception($"Unknown line.\nExpected: \"Global\".\nFound: {currentLine}");
            }

            SolutionFileTextSerializer.DeserializeGlobals(reader, ref currentLine, solutionFile);

            var blankEndLine = reader.ReadLine();

            if (reader.ReadToEnd() != String.Empty)
            {
                throw new Exception("Reader was not at end.");
            }

            return solutionFile;
        }

        public static void Serialize(TextWriter writer, SolutionFile solutionFile)
        {
            writer.WriteLine(); // Blank first line.

            var formatVersionLine = $"{Constants.MicrosoftVisualStudioSolutionFile}, {Constants.FormatVersion} {solutionFile.FormatVersion.Major}.{solutionFile.FormatVersion.Minor:00}";
            writer.WriteLine(formatVersionLine);
            writer.WriteLine(solutionFile.VisualStudioMoniker);
            var vsVersionLine = $"{Constants.VisualStudioVersion} = {solutionFile.VisualStudioVersion}";
            writer.WriteLine(vsVersionLine);
            var vsMinimumVersionLine = $"{Constants.MinimumVisualStudioVersion} = {solutionFile.MinimumVisualStudioVersion}";
            writer.WriteLine(vsMinimumVersionLine);

            SolutionFileTextSerializer.SerializeProjectReferences(writer, solutionFile);

            SolutionFileTextSerializer.SerializeGlobals(writer, solutionFile);

            // Blank last line for free due to prior WriteLine().
        }
    }
}
