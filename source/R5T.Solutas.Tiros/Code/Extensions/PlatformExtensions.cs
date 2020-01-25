using System;

using R5T.Cambridge.Types;
using R5T.Magyar;


namespace R5T.Solutas.Tiros
{
    public static class PlatformExtensions
    {
        public static string ToSolutionFileToken(this Platform platformTarget)
        {
            switch (platformTarget)
            {
                case Platform.AnyCPU:
                    return Constants.AnyCpuSolutionFileReleasePlatformToken;

                case Platform.x86:
                    return Constants.x86SolutionFileReleasePlatformToken; // x32 is x86.

                case Platform.x64:
                    return Constants.x64SolutionFileReleasePlatformToken;

                default:
                    throw new Exception(EnumHelper.UnexpectedEnumerationValueMessage(platformTarget));
            }
        }
    }
}
