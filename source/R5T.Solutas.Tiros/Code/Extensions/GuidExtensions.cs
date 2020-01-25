using System;


namespace R5T.Solutas.Tiros
{
    public static class GuidExtensions
    {
        public static string ToStringSolutionFileFormat(this Guid guid)
        {
            var output = guid.ToString("B").ToUpperInvariant();
            return output;
        }
    }
}
