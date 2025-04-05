using Aspirate.Shared.Helpers;
using HandlebarsDotNet;

namespace Aspirate.Cli;

internal static class ServiceCollectionExtensions
{
    internal static void RegisterAspirateEssential(this IServiceCollection services) =>
        services
            .AddSpectreConsole()
            .AddSecretProtectionStrategies()
            .AddAspirateState()
            .AddAspirateServices()
            .AddAspirateActions()
            .AddAspirateProcessors()
            .AddAspirateSecretProvider()
            .AddPlaceholderTransformation()
            .RegisterHandlebarsHelpers();

    private static IServiceCollection AddSpectreConsole(this IServiceCollection services) =>
        services.AddSingleton(AnsiConsole.Console);
    private static IServiceCollection RegisterHandlebarsHelpers(this IServiceCollection services)
    {
        Handlebars.RegisterHelper("IsProjectName", HandlebarsHelpers.IsProjectNameHelper);
        // Register other helpers here if needed in the future

        return services;
    }
}
