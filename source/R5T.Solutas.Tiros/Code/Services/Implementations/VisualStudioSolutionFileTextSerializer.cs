using System;
using System.IO;

using R5T.Cambridge.Types;
using R5T.Tiros;using R5T.T0064;


namespace R5T.Solutas.Tiros
{[ServiceImplementationMarker]
    public class VisualStudioSolutionFileTextSerializer : ITextSerializer<SolutionFile>,IServiceImplementation
    {
        public SolutionFile Deserialize(TextReader reader)
        {
            var solutionFile = SolutionFileTextSerializer.Deserialize(reader);
            return solutionFile;
        }

        public void Serialize(TextWriter writer, SolutionFile solutionFile)
        {
            SolutionFileTextSerializer.Serialize(writer, solutionFile);
        }
    }
}
