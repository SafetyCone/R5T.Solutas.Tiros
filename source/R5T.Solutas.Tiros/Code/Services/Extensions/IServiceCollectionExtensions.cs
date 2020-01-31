using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.Cambridge.Types;
using R5T.Dacia;
using R5T.Tiros;


namespace R5T.Solutas.Tiros
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="VisualStudioSolutionFileTextSerializer"/> implementation of <see cref="ITextSerializer{T}"/>, T: <see cref="SolutionFile"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddVisualStudioSolutionFileTextSerializer(this IServiceCollection services)
        {
            services.AddSingleton<ITextSerializer<SolutionFile>, VisualStudioSolutionFileTextSerializer>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="VisualStudioSolutionFileTextSerializer"/> implementation of <see cref="ITextSerializer{T}"/>, T: <see cref="SolutionFile"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static ServiceAction<ITextSerializer<SolutionFile>> AddVisualStudioSolutionFileTextSerializerAction(this IServiceCollection services)
        {
            var serviceAction = new ServiceAction<ITextSerializer<SolutionFile>>(() => services.AddVisualStudioSolutionFileTextSerializer());
            return serviceAction;
        }
    }
}
