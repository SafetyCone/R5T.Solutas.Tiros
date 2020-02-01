using System;

using R5T.Magyar.Extensions;


namespace R5T.Solutas.Tiros
{
    public static class BooleanExtensions
    {
        public static string ToStringSolutionFileFormat(this bool value)
        {
            var representation = value.ToStringAllCapitals();
            return representation;
        }
    }
}
