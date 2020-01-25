using System;
using System.IO;

using R5T.Cambridge.Types;
using R5T.Magyar.IO;
using R5T.Magyar.IO.Extensions;
using R5T.Tiros;


namespace R5T.Solutas.Tiros
{
    public class VisualStudioSolutionFileTextSerializer : ITextSerializer<SolutionFile>
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
