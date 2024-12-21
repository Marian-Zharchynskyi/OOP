using Application.Abstraction.Interfaces;
using Infrastructure.ConsoleWrappers;
using Infrastructure.Loggers;
using Infrastructure.LoggersFactory;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddSingleton<IConsoleWrapper, ConsoleWrapper>();

        services.AddSingleton<LoggerFactory>(provider =>
        {
            var consoleWrapper = provider.GetRequiredService<IConsoleWrapper>();
            return new LoggerFactory(configuration, consoleWrapper);
        });

        services.AddSingleton<ILogger>(provider =>
        {
            try
            {
                var factory = provider.GetRequiredService<LoggerFactory>();
                return factory.CreateLogger();
            }
            catch (InvalidOperationException ex)
            {
                var consoleWrapper = provider.GetRequiredService<IConsoleWrapper>();
                consoleWrapper.WriteLine($"Error creating file logger: {ex.Message}");
                return new ConsoleLogger(consoleWrapper);
            }
        });
    }
}